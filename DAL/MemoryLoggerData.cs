using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using RestServer1.DAL.Abstract;
using RestServer1.Domain;
using RestServer1.DAL.Model;
using RestServer1.DAL.Enum;
using log4net;


namespace RestServer1.DAL
{
    public class MemoryLoggerData : ILoggerData
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ConcurrentDictionary<string, LoggerEvent> events = new ConcurrentDictionary<string, LoggerEvent>();
        private readonly SortedList<DateTime, LoggerEvent> sortedEvents = new SortedList<DateTime, LoggerEvent>();

        public MemoryLoggerData()
        {
            log.Debug("constructor");
        }

        public Task CreateAsync(LoggerEvent loggerEvent)
        {
            log.Debug("CreateAsync: ");

            if (this.events.TryAdd(loggerEvent.Id, loggerEvent))
            {
                this.sortedEvents.TryAdd(loggerEvent.EventTime, loggerEvent);
            }

            return Task.CompletedTask;
        }

        public Task CreateAsync(IEnumerable<LoggerEvent> loggerEvents)
        {
            log.Debug("CreateAsync(bulk): ");

            loggerEvents.Select((evt) =>
            {
                if (this.events.TryAdd(evt.Id, evt))
                {
                    this.sortedEvents.TryAdd(evt.EventTime, evt);
                }

                return true;
            });

            return Task.CompletedTask;
        }

        public Task<IEnumerable<LoggerEvent>> ReadAsync()
        {
            log.Debug("ReadAsync");

            return Task.FromResult<IEnumerable<LoggerEvent>>(this.sortedEvents.Values);
        }

        public Task<LoggerEvent> ReadByIdAsync(Guid id)
        {
            log.Debug("ReadByIdAsync: " + id);

            if (this.events.TryGetValue(id.ToString(), out LoggerEvent val))
                return Task.FromResult(val);

            throw new KeyNotFoundException("The event was not found !");
        }

        public Task<IEnumerable<LoggerEvent>> ReadByTimeAndLevelAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            log.Debug("ReadByTimeAndLevelAsync: "
                      + (start.HasValue ? start.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (end.HasValue ? end.Value.ToString(CultureInfo.InvariantCulture) : "") + ", "
                      + (level.HasValue ? level.Value.ToString() : ""));

            return Task.FromResult<IEnumerable<LoggerEvent>>(this.sortedEvents.Values.Where((evt) => (!start.HasValue || evt.EventTime > start.Value)
                                                && (!end.HasValue || evt.EventTime < end.Value)
                                                && (!level.HasValue || evt.Level == level)
                                                                                ));
        }


        public Task DeleteAllAsync()
        {
            log.Debug("DeleteAllAsync: ");

            events.Clear();

            return Task.CompletedTask;
        }
    }
}
