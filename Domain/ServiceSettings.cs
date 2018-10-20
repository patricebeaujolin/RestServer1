using System;
using RestServer1.Domain.Abstract;

namespace RestServer1.Domain
{
    public class ServiceSettings
    {
        public string DataImplementation { get; set; }
        public string OnStartup { get; set; }
    }
}
