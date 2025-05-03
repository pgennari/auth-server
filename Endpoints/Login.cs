using auth_server.Data;
using auth_server.DTO;
using auth_server.Models;
using auth_server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace auth_server.Endpoints
{
    public static class Login
    {
        public static async Task<IResult> Handler(
            [FromBody] User user,
            [FromServices] AppDbContext dbContext,
            [FromServices] OTPService otp)
        {
            var existingUser = dbContext.Users.Find(user.peopleId);
            if (existingUser == null)
                return Results.BadRequest();

            await otp.SendOTP(existingUser);

            return Results.Created();
        }

        public static async Task<IResult> VerifyOtp(
            [FromBody] LoginVerify loginVerify,
            [FromServices] AppDbContext dbContext,
            [FromServices] DevKeys devKeys,
            [FromServices] IConfiguration configuration)
        {
            Otp? otp = dbContext.Otps.Find(loginVerify.peopleId);
            if (otp == null)
                return Results.BadRequest();

            if (otp.otp != loginVerify.otp)
                return Results.BadRequest();

            if (otp.expiresAt < DateTimeOffset.Now)
                return Results.BadRequest();

            dbContext.Otps.Remove(otp);
            await dbContext.SaveChangesAsync();

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(new[] {
                    new Claim("sub", loginVerify.peopleId),
                }),
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(devKeys.RsaSecurityKey, SecurityAlgorithms.RsaSha256)

            });


            return Results.Ok(new
            {
                data = new
                {
                    access_token = token,
                    token_type = "Bearer"
                }
            });
        }
    }
}
