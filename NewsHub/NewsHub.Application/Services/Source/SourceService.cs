using NewsHub.Application.Common;
using NewsHub.Application.DTOs.Source;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Services;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Source
{
    public class SourceService : ISourceService
    {
        private readonly ISourceRepository _sourceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SourceService(ISourceRepository sourceRepository, IUnitOfWork unitOfWork)
        {
            _sourceRepository = sourceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SourceDto>> CreateAsync(SourceDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var exists = await _sourceRepository.ExistsAsync(s => s.Url == dto.Url);
                if (exists)
                    return Result<SourceDto>.Fail("La fuente ya existe.", TypeMessage.Warning);

                var entity = new SourceEnt(dto.Url, dto.Name, dto.ComponentType, dto.RequiresSecret, dto.Description);

                var added = await _sourceRepository.AddAsync(entity);

                if (added == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<SourceDto>.Fail("No se pudo crear la fuente.", TypeMessage.Error);
                }

                await _unitOfWork.CommitAsync();

                var resultDto = new SourceDto
                {
                    Id = added.Id,
                    Url = added.Url,
                    Name = added.Name,
                    ComponentType = added.ComponentType,
                    RequiresSecret = added.RequiresSecret,
                    Description = added.Description
                };

                return Result<SourceDto>.Ok(resultDto, "Fuente creada.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result<SourceDto>.Fail($"Error creando fuente: {ex.Message}", TypeMessage.Error);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _sourceRepository.DeleteAsync(id);

                if (!deleted)
                    return Result<bool>.Fail("No se pudo eliminar la fuente.", TypeMessage.Warning);

                return Result<bool>.Ok(true, "Fuente eliminada.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error eliminando fuente: {ex.Message}", TypeMessage.Error);
            }
        }

        public async Task<Result<List<SourceDto>>> GetAllAsync()
        {
            try
            {
                var list = await _sourceRepository.GetAllAsync();

                var dtos = list.Select(s => new SourceDto
                {
                    Id = s.Id,
                    Url = s.Url,
                    Name = s.Name,
                    ComponentType = s.ComponentType,
                    RequiresSecret = s.RequiresSecret,
                    Description = s.Description
                }).ToList();

                return Result<List<SourceDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<SourceDto>>.Fail($"Error obteniendo fuentes: {ex.Message}", TypeMessage.Error);
            }
        }

        public async Task<Result<SourceDto>> GetByIdAsync(int id)
        {
            try
            {
                var s = await _sourceRepository.GetByIdAsync(id);

                if (s == null)
                    return Result<SourceDto>.Fail("Fuente no encontrada.", TypeMessage.Warning);

                var dto = new SourceDto
                {
                    Id = s.Id,
                    Url = s.Url,
                    Name = s.Name,
                    ComponentType = s.ComponentType,
                    RequiresSecret = s.RequiresSecret,
                    Description = s.Description
                };

                return Result<SourceDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                return Result<SourceDto>.Fail($"Error obteniendo fuente: {ex.Message}", TypeMessage.Error);
            }
        }

        public async Task<Result<SourceDto>> UpdateAsync(int id, SourceDto dto)
        {
            try
            {
                var entity = await _sourceRepository.GetByIdAsync(id, asNoTracking: false);

                if (entity == null)
                    return Result<SourceDto>.Fail("Fuente no encontrada.", TypeMessage.Warning);

                entity.Update(dto.Url, dto.Name, dto.ComponentType, dto.RequiresSecret, dto.Description);

                var updated = await _sourceRepository.UpdateAsync(entity);

                if (updated == null)
                    return Result<SourceDto>.Fail("No se pudo actualizar la fuente.", TypeMessage.Error);

                var resultDto = new SourceDto
                {
                    Id = updated.Id,
                    Url = updated.Url,
                    Name = updated.Name,
                    ComponentType = updated.ComponentType,
                    RequiresSecret = updated.RequiresSecret,
                    Description = updated.Description
                };

                return Result<SourceDto>.Ok(resultDto, "Fuente actualizada.");
            }
            catch (Exception ex)
            {
                return Result<SourceDto>.Fail($"Error actualizando fuente: {ex.Message}", TypeMessage.Error);
            }
        }
    }
}
