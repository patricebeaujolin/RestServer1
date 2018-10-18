using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.Domain;
using RestServer1.Domain.Model;
using RestServer1.Domain.Enum;

namespace RestServer1.DAL.Abstract
{
    public interface ILoggerData
    {
        void SetSettings(ServiceSettings serviceSettings);

        void Start();

        Task CreateAsync(LoggerEvent loggerEvent);

        Task<LoggerEvent> ReadByIdAsync(Guid id);

        Task<IEnumerable<LoggerEvent>> ReadByTimeAndLevelAsync(DateTime? start, DateTime? end, LoggerEventLevel? level);

        Task<IEnumerable<LoggerEvent>> ReadAsync();
    }
}
