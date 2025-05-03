﻿using auth_server.Data;
using auth_server.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace auth_server.Endpoints
{
    public static class Auth
    {
        public static async Task<IResult> Validate(
            [FromBody] TokenRequest request,
            [FromServices] AppDbContext dbContext,
            [FromServices] DevKeys devKeys,
            [FromServices] IConfiguration configuration)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(
                request.Token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = devKeys.RsaSecurityKey,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"]
                }
            );

            if (tokenValidationResult.Exception != null)
                return Results.BadRequest(new { valid = false, error = tokenValidationResult.Exception.Message });

            if (tokenValidationResult.Claims == null)
                return Results.BadRequest(new { valid = false });

            return Results.Ok(new { valid = true , userId = tokenValidationResult.Claims["sub"] });
        }
    }
}
