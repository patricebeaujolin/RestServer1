using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.Core.Abstract;
using RestServer1.DAL.Model;
using RestServer1.DAL.Enum;
using RestServer1.DAL.Abstract;
using log4net;
using RestServer1.Domain;
using RestServer1.Domain.Abstract;

namespace RestServer1.Core
{
    public class EventLogger : IEventLogger
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string separator = ", ";

        private readonly ServiceSettings settings;
        private readonly ILoggerData loggerData;

        public EventLogger(ILoggerData loggerData, IApplicationSettings applicationSettings)
        {
            this.loggerData = loggerData;
            this.settings = applicationSettings.Settings.Logger;

            switch (this.settings.OnStartup)
            {
                case "flush":
                    this.FlushAsync().Wait();
                    break;

                case "seed":
                    this.ReseedAsync().Wait();
                    break;
            }
        }

        // IEventLogger implementation
        public async Task<LoggerEvent> AddEventAsync(LoggerEvent loggerEvent)
        {
            log.Debug("AddEventAsync: " + loggerEvent);

            await this.loggerData.CreateAsync(loggerEvent);

            return loggerEvent;
        }

        public async Task<LoggerEvent> GetEventAsync(Guid id)
        {
            log.Debug("GetEventAsync: " + id);

            return await this.loggerData.ReadByIdAsync(id);
        }

        public async Task<IEnumerable<LoggerEvent>> GetAllEventsAsync()
        {
            log.Debug("GetAllEventsAsync: ");

            return await this.loggerData.ReadAsync();
        }

        public async Task<IEnumerable<LoggerEvent>> GetEventsAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            log.Debug("GetEventsAsync: " + (start.HasValue ? start.Value.ToString(CultureInfo.InvariantCulture) : string.Empty) + separator
                      + (end.HasValue ? end.Value.ToString(CultureInfo.InvariantCulture) : string.Empty) +separator
                      + (level.HasValue ? level.Value.ToString() : string.Empty));

            return await this.loggerData.ReadByTimeAndLevelAsync(start, end, level);
        }

        // Admin implementation
        public async Task FlushAsync()
        {
            log.Debug("FlushAsync: ");

            await this.loggerData.DeleteAllAsync();
        }

        public async Task SeedAsync()
        {
            log.Debug("SeedAsync: ");

            IEnumerable<LoggerEvent> seed  = new List<LoggerEvent>
            {
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:20", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.INFO, 1, "this is my seed trace message 1 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:21", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.INFO, 2, "this is my seed trace message 2 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:22", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.DEBUG, 1, "this is my seed trace message 3 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:23", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.INFO, 2, "this is my seed trace message 4 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:24", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.ERROR, 1, "this is my seed trace message 5 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:25", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.WARN, 2, "this is my seed trace message 6 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "Logger", LoggerEventLevel.ERROR, 2, "this is my seed trace message 7 !"),
            };


        await this.loggerData.CreateAsync(seed);

        }

        public async Task ReseedAsync()
        {
            log.Debug("ReseedAsync: ");

            await this.FlushAsync();

            await this.SeedAsync();
        }
    }
}
