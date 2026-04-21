using NewsHub.Domain.Entities.CatchError;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewsHub.Infrastructure.Repositories.CatchError
{
    public class LogErrorRepository : ILogErrorRepository
    {
        private readonly ApplicationDbContext _context;

        public LogErrorRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddLogErrorAsync(string origin, Exception exception)
        {
            if (string.IsNullOrWhiteSpace(origin))
                origin = "Origen desconocido";

            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            try
            {
                var logEntry = new LogErrorEnt
                (
                    origin,
                    exception.Message ?? string.Empty,
                    exception.InnerException?.Message ?? string.Empty,
                    exception.StackTrace ?? string.Empty
                );

                await _context.LogErrors.AddAsync(logEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception exGuardar)
            {
                await WriteToFileAsync(origin, exception, exGuardar);
            }
        }

        private static async Task WriteToFileAsync(string origin, Exception exception, Exception exGuardar)
        {
            try
            {
                var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(logDirectory);

                var logFilePath = Path.Combine(
                    logDirectory,
                    $"errores-{DateTime.UtcNow:yyyy-MM-dd}.txt"
                );

                var logTexto =
                    "=========================================\n" +
                    $"🕒 Fecha: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n" +
                    $"📍 Origen: {origin}\n" +
                    $"💬 Mensaje: {exception.Message ?? "Sin mensaje"}\n" +
                    $"🔁 Excepción interna: {exception.InnerException?.Message ?? "Sin excepción interna"}\n" +
                    $"📄 Traza: {exception.StackTrace ?? "Sin traza"}\n" +
                    $"[⚠️ Error al guardar en BD]: {exGuardar.Message ?? "Sin mensaje"}\n" +
                    "=========================================\n\n";

                await File.AppendAllTextAsync(logFilePath, logTexto);
            }
            catch (Exception exArchivo)
            {
                Console.WriteLine($"[FATAL] Error al registrar en archivo: {exArchivo.Message}");
            }
        }
    }
}