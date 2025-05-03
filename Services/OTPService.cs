using auth_server.Data;
using auth_server.Models;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using System.Net;
using System.Net.Mail;

namespace auth_server.Services
{
    public class OTPService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        [FromServices] AppDbContext dbContext
    )
    {
        public async Task SendOTP(User user)
        {
            byte[] secretKey = Base32Encoding.ToBytes(configuration.GetValue<string>("OtpSecret"));
            string otpCode = new Totp(secretKey, mode: OtpHashMode.Sha256).ComputeTotp();

            Models.Otp? otp = dbContext.Otps.Find(user.peopleId);
            if (otp == null)
                dbContext.Otps.Add(new Models.Otp
                {
                    peopleId = user.peopleId,
                    otp = otpCode,
                    expiresAt = DateTime.Now.AddMinutes(5)
                });
            else
            {
                otp.otp = otpCode;
                otp.expiresAt = DateTime.Now.AddMinutes(5);
            }
            await dbContext.SaveChangesAsync();

            try
            {
                if (environment.IsProduction()) 
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("pgennari@gmail.com", "Acordemus"),
                        Subject = "Acordemus - Login",
                        Body = $"<h3>Olá, {user.socialName}!</h3><p>Informe o código abaixo para realizar o login:</p><p><h1>{otpCode}</p><br/><br/><h5>Esse código expira em 5 minutos.</h5>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(new MailAddress(user.email, user.socialName));

                    var smtpAddress = configuration.GetSection("Brevo").GetValue<string>("Address");
                    var smtpPort = configuration.GetSection("Brevo").GetValue<int>("Port");
                    var smtpUsername = configuration.GetSection("Brevo").GetValue<string>("Username");
                    var smtpPassword = configuration.GetSection("Brevo").GetValue<string>("Password");
                    var smtpClient = new SmtpClient(smtpAddress)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = true,
                    };
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("E-mail enviado com sucesso!");
                }
                else
                {
                    Console.WriteLine(otpCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar e-mail: " + ex.Message);
            }
        }
    }
}