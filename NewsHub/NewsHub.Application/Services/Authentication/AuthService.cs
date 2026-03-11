using NewsHub.Application.Common;
using NewsHub.Application.Configuration;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Authentication;
using NewsHub.Application.Interfaces.Notifications;
using NewsHub.Application.Interfaces.Security;
using NewsHub.Application.Interfaces.Tokens;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailSender _emailSender;
        private readonly AppSettings _appSettings;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IPasswordResetTokenService passwordResetTokenService,
            IEmailSender emailSender,
            AppSettings appSettings
           )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _passwordResetTokenService = passwordResetTokenService;
            _emailSender = emailSender;
            _appSettings = appSettings;
        }

        public async Task<Result<bool>> RegisterAsync(RegisterDto dto)
        {
            // Verificar email
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<bool>.Fail("El email ya está registrado.", TypeMessage.Warning);

            // Verificar username
            var existingUserName = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (existingUserName != null)
                return Result<bool>.Fail("El username ya existe.", TypeMessage.Warning);

            // Hash password
            var hash = _passwordHasher.HashPassword(dto.Password);

            // Crear usuario
            var user = new User(
                dto.Email,
                dto.UserName,
                hash,
                dto.FirstName,
                dto.LastName
            );

            await _userRepository.AddAsync(user);

            return Result<bool>.Ok(true);
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto dto)
        {
            // Buscar usuario
            var user = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (user == null)
                return Result<UserDto>.Fail("Usuario o contraseña incorrectos.", TypeMessage.Warning);

            if (!user.IsActive)
                return Result<UserDto>.Fail("El usuario está deshabilitado.", TypeMessage.Warning);

            // Verificar password
            var isValid = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);
            if (!isValid)
                return Result<UserDto>.Fail("Usuario o contraseña incorrectos.", TypeMessage.Warning);

            // Actualizar último login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Mapear DTO
            var dtoUser = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList()
            };

            return Result<UserDto>.Ok(dtoUser);
        }

        public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                return Result<bool>.Ok(true);

            // Crear token
            var tokenResult = await _passwordResetTokenService.CreateTokenAsync(user.Id);

            if (!tokenResult.Success)
                return Result<bool>.Fail("No se pudo generar el token.", TypeMessage.Error);

            var token = tokenResult.Data!.Token;

            var resetLink = $"{_appSettings.BaseUrl}/Authentication/ResetPassword?token={token}";

            // Cargar template
            var html = LoadEmailTemplate("RecoverPasswordEmail.html");

            html = html.Replace("{{USER_NAME}}", user.FirstName);
            html = html.Replace("{{RESET_LINK}}", resetLink);
            html = html.Replace("{{EXPIRES_AT}}", tokenResult.Data.ExpiresAt.ToString());

            await _emailSender.SendAsync(
                user.Email,
                "Reset your password",
                html
            );

            return Result<bool>.Ok(true);
        }

        #region Private methods
        private string LoadEmailTemplate(string templateName)
        {
            var basePath = AppContext.BaseDirectory;

            var path = Path.Combine(
                basePath,
                "Email",
                "Templates",
                templateName
            );

            return File.ReadAllText(path);
        }
        #endregion
    }
}