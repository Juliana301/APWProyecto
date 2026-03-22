using NewsHub.Application.Interfaces;
using NewsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.External
{
    public class SourceReaderFactory
    {
        private readonly IEnumerable<ISourceReader> _readers;

        public SourceReaderFactory(
            IEnumerable<ISourceReader> readers)
        {
            _readers = readers;
        }

        public ISourceReader GetReader(
            SourceType type)
        {
            var reader =
                _readers.FirstOrDefault(
                    r => r.Type == type);

            if (reader == null)
            {
                throw new NotImplementedException(
                    $"No existe reader para el tipo {type}");
            }

            return reader;
        }
    }
}
