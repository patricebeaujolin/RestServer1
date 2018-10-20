using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.DAL.Model;
using RestServer1.DAL.Enum;

namespace RestServer1.DAL.Abstract
{
    public interface ILoggerData
    {
        Task CreateAsync(LoggerEvent loggerEvent);

        Task<LoggerEvent> ReadByIdAsync(Guid id);

        Task<IEnumerable<LoggerEvent>> ReadByTimeAndLevelAsync(DateTime? start, DateTime? end, LoggerEventLevel? level);

        Task<IEnumerable<LoggerEvent>> ReadAsync();

        Task DeleteAllAsync();

        Task SeedAsync();
    }
}
