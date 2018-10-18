using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using RestServer1.DAL.Abstract;
using RestServer1.Domain;
using RestServer1.Domain.Enum;
using RestServer1.Domain.Model;
using MongoDB.Driver;
using log4net;

namespace RestServer1.DAL
{
    public class MongoDbLoggerData : ILoggerData
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MongoClient mongo = null;
        private IMongoDatabase database = null;
        private IMongoCollection<LoggerEvent> events = null;
        private ServiceSettings settings;
        private bool started = false;

        public MongoDbLoggerData()
        {
        }

        public void SetSettings(ServiceSettings serviceSettings)
        {
            if (this.started) return;

            this.settings = serviceSettings;
        }

        public void Start()
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

            // Add some values in MongoDb
            /*
            IEnumerable<LoggerEvent> list = new List<LoggerEvent>
            {
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:20", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 1, "this is my trace message 1 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:21", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 2 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:22", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.DEBUG, 1, "this is my trace message 3 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:23", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.INFO, 2, "this is my trace message 4 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:24", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.ERROR, 1, "this is my trace message 5 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:25", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.WARN, 2, "this is my trace message 6 !"),
                new LoggerEvent(DateTime.ParseExact("2018-10-17 16:26", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), "RestServer1.Core", "FakeLoggerData", LoggerEventLevel.ERROR, 2, "this is my trace message 7 !"),
            };

            this.events.InsertMany(list);
            */

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

            var filter = Builders<LoggerEvent>.Filter.Eq("Id", id);

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

            var filter = Builders<LoggerEvent>.Filter.Where((arg) => (!start.HasValue || arg.EventTime > start.Value)
                                                && (!end.HasValue || arg.EventTime < end.Value)
                                                && (!level.HasValue || arg.Level == level)); 
                                                                      
            try
            {
                return await this.events.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Cannot read logger event with Start={0} End={2} Level={3} exception={4}!", start.Value, end.Value, level.Value, ex);
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
    }
}
