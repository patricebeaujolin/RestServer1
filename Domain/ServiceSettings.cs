using System;

namespace RestServer1.Domain
{
    public class ServiceSettings
    {
        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
        public string MongoCollection { get; set; }
    }
}
