using backend.DTOs;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResult?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var normalizedEmail = loginRequestDto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null)
                return null;

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginRequestDto.Password
            );

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return null;

            return await IssueTokensAsync(user, loginRequestDto.RememberMe);
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedException("Refresh token manquant.");

            var tokenHash = HashToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);

            if (storedToken == null || storedToken.RevokedAt.HasValue || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token invalide ou expiré.");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user == null)
                throw new UnauthorizedException("Utilisateur introuvable.");

            var newTokens = await IssueTokensAsync(user, storedToken.IsPersistent);

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByTokenHash = HashToken(newTokens.RefreshToken);
            await _refreshTokenRepository.UpdateAsync(storedToken);

            return newTokens;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var tokenHash = HashToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);

            if (storedToken == null || storedToken.RevokedAt.HasValue)
                return;

            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);
        }

        private async Task<AuthResult> IssueTokensAsync(User user, bool isPersistent)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes());
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays());

            var accessToken = GenerateAccessToken(user, accessTokenExpiry);
            var refreshToken = GenerateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                TokenHash = HashToken(refreshToken),
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiry,
                CreatedAt = DateTime.UtcNow,
                IsPersistent = isPersistent
            });

            return new AuthResult
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiry,
                Email = user.Email,
                Role = user.Role,
                IsPersistent = isPersistent
            };
        }

        private string GenerateAccessToken(User user, DateTime expiresAt)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", string.Empty);
        }

        private static string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        private int GetAccessTokenExpiryMinutes()
        {
            return int.TryParse(_configuration["Jwt:AccessTokenExpiryMinutes"], out var value) ? value : 15;
        }

        private int GetRefreshTokenExpiryDays()
        {
            return int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var value) ? value : 30;
        }
    }
}
