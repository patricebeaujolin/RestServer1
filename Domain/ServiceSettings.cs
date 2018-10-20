using System;
using RestServer1.Domain.Abstract;

namespace RestServer1.Domain
{
    public class ServiceSettings : IServiceSettings
    {
        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
        public string MongoCollection { get; set; }
        public bool MongoResetEvents { get; set; }

        public ServiceSettings GetServiceSettings() => this;
    }
}
