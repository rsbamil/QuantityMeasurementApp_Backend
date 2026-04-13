using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppBusinessLayer.Exception;
using QuantityMeasurementAppRepositoryLayer.Interface;
using QuantityMeasurementAppRepositoryLayer.Database;
using QuantityMeasurementAppModelLayer.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using QuantityMeasurementAppModelLayer.Utils;
namespace QuantityMeasurementAppBusinessLayer.Service
{
    public class QuantityMeasurementAuthService : IAuthService
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly JwtSettings _jwtSettings;
        public QuantityMeasurementAuthService(IQuantityMeasurementRepository _repository, JwtSettings jwtSettings)
        {
            this._repository = _repository;
            _jwtSettings = jwtSettings;
        }
        private readonly PasswordHasher<UserEntity> _passwordHasher = new();
        public void SaveUsers(RegisterDTO user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Phone))
            {
                throw new QuantityMeasurementException("Username,Email,password and Phone Number cannot be empty.");
            }

            var userEntity = new UserEntity
            {
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
            };
            userEntity.Password = _passwordHasher.HashPassword(userEntity, user.Password);
            _repository.SaveUser(userEntity);
        }
        public string VerifyUser(LoginDTO login)
        {
            var hasher = new PasswordHasher<UserEntity>();

            var user = _repository.GetUserByEmail(login.Email);
            // Console.WriteLine("Username entered: [" + login.Username + "]");
            if (user == null)
            {
                Console.WriteLine("User NOT found in DB");
                return null;
            }

            // Console.WriteLine("User found: " + user.Username);
            // Console.WriteLine("Stored Hash: " + user.Password);
            // Console.WriteLine("Hash Length: " + user.Password.Length);

            var result = hasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            // Console.WriteLine("Entered Password: [" + login.Password + "]");
            // Console.WriteLine("Verification Result: " + result);
            var token = GenerateJwtToken(user);
            // return Ok(new { message = "Login successful", token });
            return token;

        }
        public string GenerateJwtToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim("UserId",user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}