using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestServer1.Domain;
using RestServer1.Domain.Enum;
using RestServer1.Domain.Model;
using RestServer1.Core;
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

        readonly RestServer1.Core.Abstract.ILogger logger;

        public LogsController(RestServer1.Core.Abstract.ILogger loggerInstance, IOptions<ServiceSettings> serviceSettingsOptions)
        {
            this.logger = loggerInstance;
            this.logger.SetSettings(serviceSettingsOptions.Value);
            this.logger.Start();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<LoggerEvent>> Get()
        {
            return new ActionResult<IEnumerable<LoggerEvent>>(logger.GetAllEventsAsync().Result);
        }

        // GET api/values/1b45c80f-21f6-4569-8500-636efd1e0c02
        [HttpGet("{id:guid}")]
        public ActionResult<LoggerEvent> Get(string id)
        {
            var guid = Guid.Parse(id);
            return new ActionResult<LoggerEvent>(logger.GetEventAsync(guid).Result);
        }

        // GET api/values/2
        // GET api/values/2018-10-17T16:21
        // GET api/values/2018-10-17T16:21/2
        // GET api/values/2018-10-17T16:21/2018-10-17T16:24
        // GET api/values/2018-10-17T16:21/2018-10-17T16:24/2
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
            logger.AddEventAsync(value);
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
    }
}
