using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.Core.Abstract;
using RestServer1.DAL.Model;
using RestServer1.DAL.Enum;
using RestServer1.DAL.Abstract;
using log4net;

namespace RestServer1.Core
{
    public class EventLogger : IEventLogger
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly string separator = ", ";

        readonly ILoggerData loggerData;

        public EventLogger(ILoggerData loggerData)
        {
            this.loggerData = loggerData;
        }

        public async Task<bool> AddEventAsync(LoggerEvent loggerEvent)
        {
            log.Debug("AddEventAsync: " + loggerEvent);

            await loggerData.CreateAsync(loggerEvent);

            return true;
        }

        public async Task<LoggerEvent> GetEventAsync(Guid id)
        {
            log.Debug("GetEventAsync: " + id);

            return await loggerData.ReadByIdAsync(id);
        }

        public async Task<IEnumerable<LoggerEvent>> GetAllEventsAsync()
        {
            log.Debug("GetAllEventsAsync: ");

            return await loggerData.ReadAsync();
        }

        public async Task<IEnumerable<LoggerEvent>> GetEventsAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            log.Debug("GetEventsAsync: " + (start.HasValue ? start.Value.ToString(CultureInfo.InvariantCulture) : string.Empty) + separator
                      + (end.HasValue ? end.Value.ToString(CultureInfo.InvariantCulture) : string.Empty) +separator
                      + (level.HasValue ? level.Value.ToString() : string.Empty));

            return await loggerData.ReadByTimeAndLevelAsync(start, end, level);
        }

        public async Task<bool> FlushAllAsync()
        {
            log.Debug("GetAllEvents: ");

            await loggerData.DeleteAllAsync();

            return true;
        }

        public async Task<bool> SeedAsync()
        {
            log.Debug("SeedAsync: ");

            await loggerData.DeleteAllAsync();

            return true;
        }

        public async Task<bool> ResetAsync()
        {
            log.Debug("ResetAsync: ");

            await loggerData.DeleteAllAsync();

            await loggerData.SeedAsync();

            return true;
        }
    }
}
