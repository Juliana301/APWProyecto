using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Authentication;
using NewsHub.Application.Interfaces.Security;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<bool>> RegisterAsync(RegisterDto dto)
        {
            // Verificar email
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<bool>.Fail("El email ya está registrado.");

            // Verificar username
            var existingUserName = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (existingUserName != null)
                return Result<bool>.Fail("El username ya existe.");

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
                return Result<UserDto>.Fail("Usuario o contraseña incorrectos.");

            if (!user.IsActive)
                return Result<UserDto>.Fail("El usuario está deshabilitado.");

            // Verificar password
            var isValid = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);
            if (!isValid)
                return Result<UserDto>.Fail("Usuario o contraseña incorrectos.");

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
    }
}