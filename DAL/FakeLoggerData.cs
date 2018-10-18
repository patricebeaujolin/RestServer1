using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using RestServer1.DAL.Abstract;
using RestServer1.Domain;
using RestServer1.Domain.Model;
using RestServer1.Domain.Enum;

using log4net;

namespace RestServer1.DAL
{
    public class FakeLoggerData : ILoggerData
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly IEnumerable<LoggerEvent> list = new List<LoggerEvent>
            {
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:20", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 1, "this is my trace message 1 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:21", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 2 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:22", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.DEBUG, 1, "this is my trace message 3 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:23", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 4 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:24", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.ERROR, 1, "this is my trace message 5 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:25", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.WARN, 2, "this is my trace message 6 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.ERROR, 2, "this is my trace message 7 !"),
            };

        static readonly ConcurrentDictionary<Guid, LoggerEvent> events = new ConcurrentDictionary<Guid, LoggerEvent>();

        public FakeLoggerData()
        {
            log.Debug("constructor");

            events.Clear();
            foreach (var e in list) { events.TryAdd(e.Id, e); }
        }

        public void SetSettings(ServiceSettings serviceSettings)
        {
        }

        public void Start()
        {
        }

        public Task CreateAsync(LoggerEvent loggerEvent)
        {
            log.Debug("Create: ");

            events.TryAdd(loggerEvent.Id, loggerEvent);

            return Task.CompletedTask;
        }

        public Task<LoggerEvent> ReadByIdAsync(Guid id)
        {
            log.Debug("ReadById: " + id);

            if (events.TryGetValue(id, out LoggerEvent val))
                return Task.FromResult(val);

            throw new KeyNotFoundException("The event was not found !");
        }

        public Task<IEnumerable<LoggerEvent>> ReadByTimeAndLevelAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            log.Debug("ReadByTimeAndLevel: "
                      + (start.HasValue ? start.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (end.HasValue ? end.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (level.HasValue ? level.Value.ToString() : ""));

            return Task.FromResult<IEnumerable<LoggerEvent>>(events.Values.Where((arg) => (!start.HasValue || arg.EventTime > start.Value)
                                                && (!end.HasValue || arg.EventTime < end.Value)
                                                && (!level.HasValue || arg.Level == level)
                                                                                ));
        }

        public Task<IEnumerable<LoggerEvent>> ReadAsync()
        {
            log.Debug("Read");

            return Task.FromResult<IEnumerable<LoggerEvent>>(events.Values);
        }

    }
}
