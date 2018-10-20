using System;
using RestServer1.DAL.Enum;

namespace RestServer1.DAL.Model
{
    public class LoggerEvent
    {
        static readonly string separator = ", ";
        static readonly string object_start = "{ ";
        static readonly string object_end = " }";

        public LoggerEvent(DateTime eventTime,string nameSpace, string name, LoggerEventLevel level, int correlationId, string message)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateTime = DateTime.UtcNow;
            this.EventTime = eventTime;
            this.Namespace = nameSpace;
            this.Name = name;
            this.Level = level;
            this.CorrelationId = correlationId;
            this.Message = message;
        }

        public string Id { get; set;  }

        public DateTime CreateTime { get; set; }

        public DateTime EventTime { get; set; }

        public string Namespace { get; set; }

        public string Name { get; set; }

        public LoggerEventLevel Level { get; set; }

        public int CorrelationId { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return  object_start
                        + "GUID=" + this.Id + separator
                        + "create_time=" + this.CreateTime + separator
                        + "event_time=" + this.CreateTime + separator
                        + "namespace=" + this.Namespace + separator
                        + "name=" + this.Name + separator
                        + "level=" + this.Level.ToString("G") + separator
                        + "id=" + this.CorrelationId + separator
                        + "message=" + this.Message +
                    object_end;
        }
    }
}
