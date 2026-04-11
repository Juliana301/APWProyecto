using NewsHub.Application.Common;
using NewsHub.Application.Common.Models;
using NewsHub.Application.DTOs.Source;
using NewsHub.Application.External;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Services.Source;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using System.ComponentModel;

namespace NewsHub.Application.Services.Source
{
    public class SourceService : ISourceService
    {
        private readonly ISourceRepository _sourceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SourceReaderFactory _readerFactory;


        public SourceService(
            ISourceRepository sourceRepository,
            IUnitOfWork unitOfWork,
            SourceReaderFactory readerFactory)
        {
            _sourceRepository = sourceRepository;
            _unitOfWork = unitOfWork;
            _readerFactory = readerFactory;
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

        public async Task<Result<bool>> DeleteAsync(int id, string sourceName)
        {
            try
            {
                // Validamos que exista la fuente con su nombre, para evitar eliminar por error si el id no coincide
                var exists = await _sourceRepository.FindAsync(s => s.Id == id && s.Name == sourceName);

                if (!exists.Any())
                    return Result<bool>.Fail("Fuente no encontrada.", TypeMessage.Warning);

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
                    Description = s.Description,
                    ComponentType = s.ComponentType,
                    RequiresSecret = s.RequiresSecret,
                    ApiConfigJson = s.ApiConfigJson
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
                var entity = await _sourceRepository
                    .GetByIdAsync(id, asNoTracking: false);

                if (entity == null)
                    return Result<SourceDto>.Fail(
                        "Fuente no encontrada.",
                        TypeMessage.Warning);

                entity.Update(
                    dto.Url,
                    dto.Name,
                    dto.ComponentType,
                    dto.RequiresSecret,
                    dto.Description
                );

                if (dto.ComponentType != Domain.Enums.SourceType.Api)
                {
                    dto.ApiConfigJson = null;
                }

                entity.SetApiConfig(dto.ApiConfigJson);

                var updated =
                    await _sourceRepository.UpdateAsync(entity);

                if (updated == null)
                    return Result<SourceDto>.Fail(
                        "No se pudo actualizar la fuente.",
                        TypeMessage.Error);

                var resultDto = new SourceDto
                {
                    Id = updated.Id,
                    Url = updated.Url,
                    Name = updated.Name,
                    ComponentType = updated.ComponentType,
                    RequiresSecret = updated.RequiresSecret,
                    Description = updated.Description,
                    ApiConfigJson = updated.ApiConfigJson
                };

                return Result<SourceDto>.Ok(
                    resultDto,
                    "Fuente actualizada."
                );
            }
            catch (ArgumentException ex)
            {
                // errores de validación JSON
                return Result<SourceDto>.Fail(
                    ex.Message,
                    TypeMessage.Warning);
            }
            catch (Exception ex)
            {
                return Result<SourceDto>.Fail(
                    $"Error actualizando fuente: {ex.Message}",
                    TypeMessage.Error);
            }
        }

        public async Task<Result<List<SourceItem>>> ReadAllFeedsAsync()
        {
            try
            {
                var sources =
                    await _sourceRepository.GetAllAsync();

                if (sources == null || !sources.Any())
                {
                    return Result<List<SourceItem>>
                        .Ok(new List<SourceItem>());
                }

                var allItems =
                    new List<SourceItem>();

                foreach (var source in sources)
                {
                    try
                    {
                        // Obtener reader según tipo
                        var reader =
                            _readerFactory.GetReader(
                                source.ComponentType);

                        // Leer noticias
                        var items =
                            await reader.ReadAsync(source);

                        if (items != null)
                            allItems.AddRange(items);
                    }
                    catch
                    {
                        // Si una fuente falla, seguimos con las demás
                    }
                }

                return Result<List<SourceItem>>
                    .Ok(allItems);
            }
            catch (Exception ex)
            {
                return Result<List<SourceItem>>
                    .Fail(
                        $"Error leyendo feeds: {ex.Message}",
                        TypeMessage.Error);
            }
        }
    }
}
