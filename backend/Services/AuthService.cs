using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt; // création du token JWT
using System.Security.Claims; // permet d'ajouter des infos dans le token
using System.Text; // transforme la clé en bytes
using Microsoft.IdentityModel.Tokens; // outils de signature JWT

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        private AuthResponseDto MapToAuthResponseDto(User user, string token)
        {
            return new AuthResponseDto
            {
                Token = token, // JWT renvoyé au frontend
                Email = user.Email, // email de l'utilisateur connecté
                Role = user.Role // rôle de l'utilisateur
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim> // infos qu'on veut stocker dans le token
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // id utilisateur
                new Claim(ClaimTypes.Email, user.Email), // email
                new Claim(ClaimTypes.Role, user.Role) // rôle
            };

            var key = new SymmetricSecurityKey(// clé de signature du token
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)// clé secrète lue depuis appsettings.json
            ); // clé secrète lue depuis appsettings.json

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            ); // signature du token

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // qui crée le token
                audience: _configuration["Jwt:Audience"], // à qui il est destiné
                claims: claims, // infos stockées dans le token
                expires: DateTime.UtcNow.AddHours(2), // durée de validité
                signingCredentials: credentials // signature
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // transforme en string
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var users = await _userRepository.GetAllAsync(); // récupère les utilisateurs

            var user = users.FirstOrDefault(u => u.Email == loginRequestDto.Email); // cherche par email

            if (user == null)
                return null; // email introuvable

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword( // compare le mot de passe saisi avec le hash stocké
                user,
                user.PasswordHash,
                loginRequestDto.Password
            ); // compare le mot de passe saisi avec le hash stocké

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return null; // mot de passe incorrect

            var token = GenerateJwtToken(user); // génère le JWT

            return MapToAuthResponseDto(user, token); // construit la réponse
        }
    }
}