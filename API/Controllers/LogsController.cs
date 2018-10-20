using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestServer1.DAL.Enum;
using RestServer1.DAL.Model;
using RestServer1.Core.Abstract;
using log4net;

namespace RestServer1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private static readonly NotImplementedException notImplementedException = new NotImplementedException("This REST method is not implemented.");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEventLogger logger;

        public LogsController(IEventLogger loggerInstance)
        {
            log.Info("Logs controller created.");
                
            this.logger = loggerInstance;
        }

        //----------------------------------------------------------
        // RESTful
        //----------------------------------------------------------
        // GET api/logs
        [HttpGet]
        public async Task<IEnumerable<LoggerEvent>> Get()
        {
            return await this.logger.GetAllEventsAsync();
        }

        // GET api/logs/1b45c80f-21f6-4569-8500-636efd1e0c02
        [HttpGet("{id:guid}")]
        public async Task<LoggerEvent> Get(string id)
        {
            var guid = Guid.Parse(id);
            return await this.logger.GetEventAsync(guid);
        }

        // POST api/values
        [HttpPost]
        public async Task<LoggerEvent> Post([FromBody] LoggerEvent value)
        {
            return await this.logger.AddEventAsync(value);
        }

        /*
        // PUT api/values/1b45c80f-21f6-4569-8500-636efd1e0c02
        [HttpPut("{id}")]
        public Task<LoggerEvent> Put(string id, [FromBody] string value)
        {
        }
        */

        /*
        // DELETE api/values/1b45c80f-21f6-4569-8500-636efd1e0c02
        [HttpDelete("{id}")]
        public Task<LoggerEvent> Delete(string id)
        {
        }
        */

        //----------------------------------------------------------
        // SEARCH
        //----------------------------------------------------------
        // GET api/logs/search/2
        // GET api/logs/search/2018-10-17T16:21
        // GET api/logs/search/2018-10-17T16:21/2
        // GET api/logs/search/2018-10-17T16:21/2018-10-17T16:24
        // GET api/logs/search/2018-10-17T16:21/2018-10-17T16:24/2
        [HttpGet("search/{level:int}")]
        [HttpGet("search/{start:datetime}/{level:int?}")]
        [HttpGet("search/{start:datetime}/{end:datetime}/{level:int?}")]
        public async Task<IEnumerable<LoggerEvent>> SearchGet(string start, string end, string level)
        {
            DateTime? startUtc = null;
            if (!string.IsNullOrEmpty(start)) 
                startUtc = DateTime.Parse(start);

            DateTime? endUtc = null;
            if (!string.IsNullOrEmpty(end)) 
                endUtc = DateTime.Parse(end);

            LoggerEventLevel? logLevel = null;
            if (!string.IsNullOrEmpty(level)) 
                logLevel = Enum.Parse<LoggerEventLevel>(level);

            return await this.logger.GetEventsAsync(startUtc, endUtc, logLevel);
        }

        //----------------------------------------------------------
        // ADMIN
        //----------------------------------------------------------
        // GET api/admin/flush
        [HttpGet("admin/flush")]
        public async Task<IEnumerable<LoggerEvent>> AdminFlush()
        {
            await this.logger.FlushAsync();
            return await this.logger.GetAllEventsAsync();
        }   

        // GET api/admin/seed
        [HttpGet("admin/seed")]
        public async Task<IEnumerable<LoggerEvent>> AdminSeed()
        {
            await this.logger.SeedAsync();
            return await this.logger.GetAllEventsAsync();
        }

        // GET api/admin/reset
        [HttpGet("admin/reseed")]
        public async Task<IEnumerable<LoggerEvent>> AdminReset()
        {
            await this.logger.ReseedAsync();
            return await this.logger.GetAllEventsAsync();
        }
    }
}
