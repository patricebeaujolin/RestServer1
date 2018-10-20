using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using RestServer1.DAL.Abstract;
using RestServer1.DAL.Model;
using RestServer1.DAL.Enum;
using RestServer1.Domain;
using RestServer1.Domain.Abstract;
using MongoDB.Driver;
using log4net;
using System.Net;

namespace RestServer1.DAL
{
    public class MongoDbLoggerData : ILoggerData
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly ServiceSettings settings;

        MongoClient mongo = null;
        IMongoDatabase database = null;
        IMongoCollection<LoggerEvent> events = null;
        bool started = false;

        private static IEnumerable<LoggerEvent> List { get; } = new List<LoggerEvent>
        {
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:20", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.INFO, 1, "this is my trace message 1 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:21", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 2 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:22", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.DEBUG, 1, "this is my trace message 3 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:23", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 4 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:24", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.ERROR, 1, "this is my trace message 5 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:25", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.WARN, 2, "this is my trace message 6 !"),
            new LoggerEvent(DateTime.ParseExact("2018-10-17 16:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "MongoLoggerData", LoggerEventLevel.ERROR, 2, "this is my trace message 7 !"),
        };

        public MongoDbLoggerData(IServiceSettings serviceSettings)
        {
            this.settings = serviceSettings.GetServiceSettings();

            this.Start();

            if (this.settings.MongoResetEvents)
            {
                this.ResetEvents();
            }
        }

        private void Start()
        {
            if (this.started) return;

            if (this.mongo == null)
            {
                this.mongo = new MongoClient(this.settings.MongoConnectionString);
            }

            if (this.mongo != null && this.database == null)
            {
                this.database = this.mongo.GetDatabase(this.settings.MongoDatabase);
            }

            if (this.database != null && this.events == null)
            {
                this.events = this.database.GetCollection<LoggerEvent>("LoggerEvent");
                this.started = true;
            }
        }

        private void ResetEvents()
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            try
            {
                this.DeleteAllAsync().RunSynchronously();
                this.SeedAsync().RunSynchronously();
            }
            catch (Exception ex)
            {
                log.Error("Cannot delete all events !", ex);
                throw ex;
            }
        }

        public async Task CreateAsync(LoggerEvent loggerEvent)
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            try
            {
                await this.events.InsertOneAsync(loggerEvent);
            }
            catch (Exception ex)
            {
                log.Error("Cannot add a logger event !", ex);
                throw ex;
            }
        }

        public async Task<LoggerEvent> ReadByIdAsync(Guid id)
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            var filter = Builders<LoggerEvent>.Filter.Eq("Id", id.ToString());

            try
            {
                return await this.events.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Cannot read logger event with Id={0} exception={1}!", id, ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<LoggerEvent>> ReadByTimeAndLevelAsync(DateTime? start, DateTime? end, LoggerEventLevel? level)
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            var filters = new List<FilterDefinition<LoggerEvent>>();

            if (level.HasValue)
                filters.Add(Builders<LoggerEvent>.Filter.Where((arg) => (arg.Level == level.Value)));

            if (start.HasValue)
                filters.Add(Builders<LoggerEvent>.Filter.Where((arg) => (arg.EventTime > start.Value)));

            if (end.HasValue)
                filters.Add(Builders<LoggerEvent>.Filter.Where((arg) => (arg.EventTime < end.Value)));

            var filter = Builders<LoggerEvent>.Filter.And(filters);

            try
            {
                return await this.events.Find(filter).ToListAsync(); ;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Cannot read logger event with Start={0} End={1} Level={2} ! exception=source:{3} message:{4} stack:{5}", 
                                start.HasValue ? start.Value.ToString() : string.Empty,
                                end.HasValue ? end.Value.ToString() : string.Empty,
                                level.HasValue ? level.Value.ToString() : string.Empty,
                                ex.Source,
                                ex.Message,
                                ex.StackTrace);
                throw ex;
            }
        }

        public async Task<IEnumerable<LoggerEvent>> ReadAsync()
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            try
            {
                return await this.events.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                log.Error("Cannot read all logger events !", ex);
                throw ex;
            }
        }

        public async Task DeleteAllAsync()
        {
            if (!this.started) throw new ApplicationException("logger data not started !");

            var filter = Builders<LoggerEvent>.Filter.Where(_ => true);

            try
            {
                await this.events.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                log.Error("Cannot delete all events !", ex);
                throw ex;
            }
        }

        public async Task SeedAsync()
        {
            // Add some values in MongoDb if empty
            IEnumerable<LoggerEvent> items = await this.ReadAsync();

            if (!items.Any())
                await this.events.InsertManyAsync(List);
        }
    }
}
