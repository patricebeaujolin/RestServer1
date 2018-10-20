using System;

namespace RestServer1.Domain
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
