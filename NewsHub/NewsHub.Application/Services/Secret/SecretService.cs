using System;
using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Secret;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Services.Secret;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Secret
{
    public class SecretService : ISecretService
    {
        private readonly ISecretRepository _repo;
        private readonly ISourceRepository _sourceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SecretService(
            ISecretRepository repo,
            ISourceRepository sourceRepository,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _sourceRepository = sourceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> CreateAsync(CreateSecretDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Key))
                {
                    return Result<bool>.Fail(
                        "La clave es obligatoria.",
                        TypeMessage.Warning);
                }

                if (string.IsNullOrWhiteSpace(dto.Value))
                {
                    return Result<bool>.Fail(
                        "El valor es obligatorio.",
                        TypeMessage.Warning);
                }

                if (dto.SourceId <= 0)
                {
                    return Result<bool>.Fail(
                        "Debe seleccionar una fuente válida.",
                        TypeMessage.Warning);
                }

                var source = await _sourceRepository.GetByIdAsync(dto.SourceId);

                if (source == null)
                {
                    return Result<bool>.Fail(
                        "La fuente seleccionada no existe.",
                        TypeMessage.Warning);
                }

                var key = dto.Key.Trim();
                var value = dto.Value.Trim();
                var normalizedKey = key.ToLower();

                var exists = await _repo.ExistsAsync(s =>
                    s.Key.ToLower() == normalizedKey &&
                    s.SourceId == dto.SourceId);

                if (exists)
                {
                    return Result<bool>.Fail(
                        "Ya existe un secret con esa clave para esta fuente.",
                        TypeMessage.Warning);
                }

                var secret = new SecretEnt(
                    key,
                    value,
                    dto.IsEncrypted,
                    dto.SourceId
                );

                await _repo.AddAsync(secret);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Ok(true, "Secret creado correctamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    $"Error creando secret: {ex.Message}",
                    TypeMessage.Error);
            }
        }

        public async Task<Result<List<SecretEnt>>> GetAllAsync()
        {
            try
            {
                var data = await _repo.GetAllWithSourceAsync();
                return Result<List<SecretEnt>>.Ok(data);
            }
            catch (Exception ex)
            {
                return Result<List<SecretEnt>>.Fail(
                    $"Error obteniendo secrets: {ex.Message}",
                    TypeMessage.Error);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Result<bool>.Fail(
                        "Id de secret inválido.",
                        TypeMessage.Warning);
                }

                var exists = await _repo.GetByIdAsync(id, asNoTracking: false);

                if (exists == null)
                {
                    return Result<bool>.Fail(
                        "Secret no encontrado.",
                        TypeMessage.Warning);
                }

                var deleted = await _repo.DeleteAsync(id);

                if (!deleted)
                {
                    return Result<bool>.Fail(
                        "No se pudo eliminar el secret.",
                        TypeMessage.Error);
                }

                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Ok(true, "Secret eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    $"Error eliminando secret: {ex.Message}",
                    TypeMessage.Error);
            }
        }
    }
}