using System;
using RestServer1.DAL.Enum;

namespace RestServer1.DAL.Model
{
    public class LoggerEvent
    {
        private const string ToStringSeparator  = ", ";
        private const string ToStringHeader     = "{";
        private const string ToStringFooter     = "}";

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
            return  ToStringHeader
                        + "GUID=" + this.Id + ToStringSeparator
                        + "create_time=" + this.CreateTime + ToStringSeparator
                        + "event_time=" + this.EventTime + ToStringSeparator
                        + "namespace=" + this.Namespace + ToStringSeparator
                        + "name=" + this.Name + ToStringSeparator
                        + "level=" + this.Level.ToString("G") + ToStringSeparator
                        + "id=" + this.CorrelationId + ToStringSeparator
                        + "message=" + this.Message +
                    ToStringFooter;
        }
    }
}
