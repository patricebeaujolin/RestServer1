using System;
using RestServer1.Domain.Abstract;

namespace RestServer1.Domain
{
    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings Settings => this;
        public ServiceSettings Logger { get; set; }
        public MongoSettings Mongo { get; set; }
    }
}
