using Microsoft.Extensions.Options;
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
            IOptionsMonitor<AppSettings> appSettings
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _passwordResetTokenService = passwordResetTokenService;
            _emailSender = emailSender;
            _appSettings = appSettings.CurrentValue;
        }

        public async Task<Result<bool>> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                    return Result<bool>.Fail("El email ya está registrado.", TypeMessage.Warning);

                var existingUserName = await _userRepository.GetByUserNameAsync(dto.UserName);
                if (existingUserName != null)
                    return Result<bool>.Fail("El username ya existe.", TypeMessage.Warning);

                var hash = _passwordHasher.HashPassword(dto.Password);

                var user = new User(
                    dto.Email,
                    dto.UserName,
                    hash,
                    dto.FirstName,
                    dto.LastName
                );

                await _userRepository.AddAsync(user);

                return Result<bool>.Ok(true, "Usuario registrado correctamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    $"Error registrando usuario: {ex.Message}",
                    TypeMessage.Error
                );
            }
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetByUserNameAsync(dto.UserName);

                if (user == null)
                    return Result<UserDto>.Fail("Usuario o contraseña incorrectos.", TypeMessage.Warning);

                if (!user.IsActive)
                    return Result<UserDto>.Fail("El usuario está deshabilitado.", TypeMessage.Warning);

                var isValid = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);

                if (!isValid)
                    return Result<UserDto>.Fail("Usuario o contraseña incorrectos.", TypeMessage.Warning);

                user.UpdateLastLogin();
                await _userRepository.UpdateAsync(user);

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

                return Result<UserDto>.Ok(dtoUser, "Login exitoso.");
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Fail(
                    $"Error durante el login: {ex.Message}",
                    TypeMessage.Error
                );
            }
        }

        public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);

                if (user == null)
                    return Result<bool>.Ok(true, "Si el correo existe se enviará un enlace de recuperación.");

                var tokenResult = await _passwordResetTokenService.CreateTokenAsync(user.Id);

                if (!tokenResult.Success)
                    return Result<bool>.Fail("No se pudo generar el token.", TypeMessage.Error);

                var token = tokenResult.Data!.Token;

                var resetLink = $"{_appSettings.BaseUrl}/Authentication/ResetPassword?token={token}";

                string html;

                try
                {
                    html = LoadEmailTemplate("RecoverPasswordEmail.html");
                }
                catch (Exception ex)
                {
                    return Result<bool>.Fail(
                        $"Error cargando template de correo: {ex.Message}",
                        TypeMessage.Error
                    );
                }

                html = html.Replace("{{USER_NAME}}", user.FirstName);
                html = html.Replace("{{RESET_LINK}}", resetLink);
                html = html.Replace("{{EXPIRES_AT}}", tokenResult.Data.ExpiresAt.ToString());

                try
                {
                    await _emailSender.SendAsync(
                        user.Email,
                        "Reset your password",
                        html
                    );
                }
                catch (Exception ex)
                {
                    return Result<bool>.Fail(
                        $"Error enviando correo: {ex.Message}",
                        TypeMessage.Error
                    );
                }

                return Result<bool>.Ok(true, "Correo de recuperación enviado.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    $"Error en recuperación de contraseña: {ex.Message}",
                    TypeMessage.Error
                );
            }
        }

        public async Task<Result<bool>> ResetPasswordAsync(PasswordResetTokenDto dto)
        {
            try
            {
                var tokenResult = await _passwordResetTokenService.ValidateTokenAsync(dto.Token);

                if (!tokenResult.Success)
                    return Result<bool>.Fail(tokenResult.Error!, tokenResult.TypeMessage);

                var token = tokenResult.Data!;

                var user = await _userRepository.GetByIdAsync(token.UserId);

                if (user == null)
                    return Result<bool>.Fail("Usuario no encontrado.", TypeMessage.Error);

                var newHash = _passwordHasher.HashPassword(dto.Password);

                user.ChangePassword(newHash);

                await _userRepository.UpdateAsync(user);

                await _passwordResetTokenService.MarkTokenAsUsedAsync(token);

                return Result<bool>.Ok(true, "Contraseña actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    $"Error restableciendo contraseña: {ex.Message}",
                    TypeMessage.Error
                );
            }
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

            if (!File.Exists(path))
                throw new FileNotFoundException($"Template no encontrado: {templateName}");

            return File.ReadAllText(path);
        }

        #endregion
    }
}