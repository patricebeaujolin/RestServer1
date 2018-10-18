using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.Core.Abstract;
using RestServer1.Domain;
using RestServer1.Domain.Model;
using RestServer1.Domain.Enum;
using RestServer1.DAL;
using RestServer1.DAL.Abstract;
using log4net;

namespace RestServer1.Core
{
    public class Logger : ILogger
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly ILoggerData loggerData;

        public Logger(ILoggerData loggerData)
        {
            this.loggerData = loggerData;
        }

        public void SetSettings(ServiceSettings serviceSettings)
        {
            this.loggerData.SetSettings(serviceSettings);
        }

        public void Start()
        {
            this.loggerData.Start();
        }

        public async Task AddEventAsync(LoggerEvent loggerEvent)
        {
            log.Debug("AddEvent: " + loggerEvent.ToString());

            await loggerData.CreateAsync(loggerEvent);
        }

        public async Task<LoggerEvent> GetEventAsync(Guid id)
        {
            log.Debug("GetEvent: " + id);

            return await loggerData.ReadByIdAsync(id);
        }

        public async Task<IEnumerable<LoggerEvent>> GetAllEventsAsync()
        {
            log.Debug("GetAllEvents: ");

            return await loggerData.ReadAsync();
        }

        public async Task<IEnumerable<LoggerEvent>> GetEventsAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            log.Debug("GetEvents: " + (start.HasValue ? start.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (end.HasValue ? end.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (level.HasValue ? level.Value.ToString() : ""));

            return await loggerData.ReadByTimeAndLevelAsync(start, end, level);
        }
    }
}
