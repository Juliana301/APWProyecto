using NewsHub.Domain.Entities.CatchError;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories.CatchError
{
    public class LogErrorRepository : ILogErrorRepository
    {
        private readonly ApplicationDbContext _context;

        public LogErrorRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        public async Task AddLogErrorAsync(string origin, Exception exception)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(origin))
                    origin = "Origen desconocido";

                if (exception == null)
                    throw new ArgumentNullException(nameof(exception));

                var logEntry = new LogError
                (
                    origin,
                    exception.Message,
                    exception.InnerException?.Message ?? string.Empty,
                    exception.StackTrace ?? string.Empty
                );

                await _context.LogErrors.AddAsync(logEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception exGuardar)
            {
                try
                {
                    Directory.CreateDirectory("logs");
                    var logFilePath = Path.Combine("logs", $"errores-{DateTime.UtcNow:yyyy-MM-dd}.txt");

                    var logTexto = $@"
                        =========================================
                        🕒 Fecha: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}
                        📍 Origen: {origin}
                        💬 Mensaje: {exception.Message}
                        🔁 Excepción interna: {exception.InnerException?.Message}
                        📄 Traza: {exception.StackTrace}

                        [⚠️ Error al guardar en BD]: {exGuardar.Message}
                        =========================================
                        ";

                    await File.AppendAllTextAsync(logFilePath, logTexto);
                }
                catch (Exception exArchivo)
                {
                    // Último recurso: salida en consola
                    Console.WriteLine($"[FATAL] Error al registrar en archivo: {exArchivo.Message}");
                }
            }
        }
    }
}
