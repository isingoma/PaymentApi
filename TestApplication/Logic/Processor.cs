using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TestApplication.Logic
{
    public class Processor
    {
        public async Task<string> GenerateTokenAsync(string username)
        {
            string tokenString = string.Empty;

            try
            {
                // Validate secret key and issuer
                string secretKey = "NSSFTests";

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new Exception("Secret key is not provided.");
                }

                string issuer = "NSSFIssuer";

                if (string.IsNullOrEmpty(issuer))
                {
                    throw new Exception("Issuer is not provided.");
                }

                string audience = username;
                int expirationMinutes = 3; // Token expiration time in minutes

                // Create claims for the token
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, "subject"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

                // Create the token
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Convert.FromBase64String(secretKey)),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                );

                // Generate the token string
                tokenString = await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error generating token: {ex.Message}");
            }

            return tokenString;
        }
        public async Task<bool> IsValidAccessToken(string AccessToken, string username)
        {
            bool isAccessToken = false;
            try
            {

                // Validate and parse the received token
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "NSSFIssuer", // Use the loaded issuer
                    ValidAudience = username, // Use the provided username as audience
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("NSSFTests")), // Use the loaded secret key
                };

                try
                {
                    // Validate the token
                    var principal = tokenHandler.ValidateToken(AccessToken, validationParameters, out var securityToken);

                    // If validation is successful, you can access the claims
                    Console.WriteLine("Token Validation Successful!");

                    // Access claims
                    foreach (var claim in principal.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }

                    isAccessToken = true;
                }
                catch (SecurityTokenException ex)
                {
                    // Token validation failed
                    Console.WriteLine($"Token Validation Failed: {ex.Message}");
                    isAccessToken = false;
                }
            }
            catch (Exception ex)
            {
                isAccessToken = false;
            }
            return isAccessToken;
        }
    }
}
