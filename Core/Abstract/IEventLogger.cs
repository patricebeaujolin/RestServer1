﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestServer1.DAL.Enum;
using RestServer1.DAL.Model;
 
namespace RestServer1.Core.Abstract
{
    public interface IEventLogger
    {
        Task<LoggerEvent> AddEventAsync(LoggerEvent loggerEvent);

        Task<LoggerEvent> GetEventAsync(Guid id);

        Task<IEnumerable<LoggerEvent>> GetEventsAsync(DateTime? start, DateTime? end, LoggerEventLevel? level);

        Task<IEnumerable<LoggerEvent>> GetAllEventsAsync();

        Task FlushAsync();

        Task SeedAsync();

        Task ReseedAsync();
    }
}
 