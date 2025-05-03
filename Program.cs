using auth_server;
using auth_server.Data;
using auth_server.Endpoints;
using auth_server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DevKeys>();
builder.Services.AddScoped<OTPService>();
builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new DevKeys(builder.Environment).RsaSecurityKey,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"]
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/healthcheck", () => Results.Ok("You should not pass!"));
app.MapPost("/login", Login.Handler);
app.MapPost("/login/verified", Login.VerifyOtp);
app.MapPost("/users", Users.Handler);
app.MapGet("/users", Users.Get).RequireAuthorization();
app.MapPost("/auth/validate", Auth.Validate);
app.Run();

