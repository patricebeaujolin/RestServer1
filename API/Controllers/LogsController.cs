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
        static readonly NotImplementedException notImplementedException = new NotImplementedException("This REST method is not implemented.");
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly IEventLogger logger;

        public LogsController(IEventLogger loggerInstance)
        {
            log.Debug("Logs controller created.");
                
            this.logger = loggerInstance;
        }

        // GET api/logs
        [HttpGet]
        public ActionResult<IEnumerable<LoggerEvent>> Get()
        {
            return new ActionResult<IEnumerable<LoggerEvent>>(logger.GetAllEventsAsync().Result);
        }

        // GET api/logs/1b45c80f-21f6-4569-8500-636efd1e0c02
        [HttpGet("{id:guid}")]
        public ActionResult<LoggerEvent> Get(string id)
        {
            var guid = Guid.Parse(id);
            return new ActionResult<LoggerEvent>(logger.GetEventAsync(guid).Result);
        }

        // GET api/logs/2
        // GET api/logs/2018-10-17T16:21
        // GET api/logs/2018-10-17T16:21/2
        // GET api/logs/2018-10-17T16:21/2018-10-17T16:24
        // GET api/logs/2018-10-17T16:21/2018-10-17T16:24/2
        [HttpGet("{level:int}")]
        [HttpGet("{start:datetime}/{level:int?}")]
        [HttpGet("{start:datetime}/{end:datetime}/{level:int?}")]
        public ActionResult<IEnumerable<LoggerEvent>> Get(string start, string end, string level)
        {
            DateTime? startUtc = null;
            if (!string.IsNullOrEmpty(start)) startUtc = DateTime.Parse(start);

            DateTime? endUtc = null;
            if (!string.IsNullOrEmpty(end)) endUtc = DateTime.Parse(end);

            LoggerEventLevel? logLevel = null;
            if (!string.IsNullOrEmpty(level)) logLevel = Enum.Parse<LoggerEventLevel>(level);

            return new ActionResult<IEnumerable<LoggerEvent>>(logger.GetEventsAsync(startUtc, endUtc, logLevel).Result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] LoggerEvent value)
        {
            value.Id = new Guid().ToString();
            logger.AddEventAsync(value).RunSynchronously();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            throw notImplementedException;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw notImplementedException;
        }

        // GET api/admin/flush
        [HttpGet("admin/flush")]
        public ActionResult<bool> AdminFlush()
        {
            return new ActionResult<bool>(logger.FlushAllAsync().Result);
        }

        // GET api/admin/seed
        [HttpGet("admin/seed")]
        public ActionResult<bool> AdminSeed()
        {
            return new ActionResult<bool>(logger.SeedAsync().Result);
        }

        // GET api/admin/reset
        [HttpGet("admin/reset")]
        public ActionResult<bool> AdminReset()
        {
            return new ActionResult<bool>(logger.ResetAsync().Result);
        }
    }
}
