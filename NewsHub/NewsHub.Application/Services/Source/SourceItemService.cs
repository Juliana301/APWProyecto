using NewsHub.Application.Common;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Services;
using NewsHub.Application.Interfaces.Services.Source;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Source
{
    public class SourceItemService : ISourceItemService
    {
        private readonly ISourceItemRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SourceItemService(
            ISourceItemRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Guardar Item del Feed
        /// </summary>
        /// <param name="sourceId">El ID de la fuente del item</param>
        /// <param name="json">El contenido del item en formato JSON</param>
        /// <returns>Un valor booleano que indica si el item fue guardado exitosamente</returns>
        public async Task<Result<bool>> SaveAsync(
            int sourceId,
            string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Result<bool>.Fail("El contenido del item está vacío.", TypeMessage.Warning);

            try
            {
                // Evitar duplicados
                var exists =
                    await _repository
                        .ExistsByJsonAsync(json);

                if (exists)
                    return Result<bool>.Fail("El item ya existe.", TypeMessage.Warning);

                var item =
                    new SourceItemEnt(
                        sourceId,
                        json
                    );

                await _repository.AddAsync(item);

                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Ok(true, "Item guardado exitosamente.");
            }
            catch
            {
                return Result<bool>.Fail("Ocurrió un error al guardar el item.", TypeMessage.Error);
            }
        }

        /// <summary>
        /// Obtener los últimos 100 items guardados (puede ser modificado para paginación)
        /// </summary>
        /// <returns>Lista de items guardados</returns>
        public async Task<Result<List<SourceItemEnt>>> GetSavedAsync()
        {
            try
            {
                var sourcesSaved = await _repository.GetLatestAsync(100);

                if (sourcesSaved == null)
                {
                    return Result<List<SourceItemEnt>>.Fail("No se encontraron items guardados.", TypeMessage.Warning);
                }

                return Result<List<SourceItemEnt>>.Ok(sourcesSaved, "Items guardados obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return Result<List<SourceItemEnt>>.Fail("Ocurrió un error al obtener los items guardados.", TypeMessage.Error);
            }
        }


        /// <summary>
        /// Importar una lista de items en formato JSON, evitando duplicados. 
        /// Retorna la cantidad de items insertados exitosamente.
        /// </summary>
        /// <param name="sourceId">El ID de la fuente de los items</param>
        /// <param name="jsonItems">Lista de items en formato JSON</param>
        /// <returns>La cantidad de items insertados exitosamente</returns>
        public async Task<int> ImportAsync(
            int sourceId,
            List<string> jsonItems)
        {
            int inserted = 0;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (var json in jsonItems)
                {
                    var exists =
                        await _repository
                            .ExistsByJsonAsync(json);

                    if (exists)
                        continue;

                    var item =
                        new SourceItemEnt(
                            sourceId,
                            json
                        );

                    await _repository.AddAsync(item);

                    inserted++;
                }

                await _unitOfWork.SaveChangesAsync();

                return inserted;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return 0;
            }
        }
    }
}