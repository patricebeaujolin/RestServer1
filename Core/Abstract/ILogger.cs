using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.Domain;
using RestServer1.Domain.Enum;
using RestServer1.Domain.Model;
 
namespace RestServer1.Core.Abstract
{
    public interface ILogger
    {
        void SetSettings(ServiceSettings serviceSettings);

        void Start();

        Task AddEventAsync(LoggerEvent loggerEvent);

        Task<LoggerEvent> GetEventAsync(Guid id);

        Task<IEnumerable<LoggerEvent>> GetEventsAsync(DateTime? start, DateTime? end, LoggerEventLevel? level);

        Task<IEnumerable<LoggerEvent>> GetAllEventsAsync();
    }
}
