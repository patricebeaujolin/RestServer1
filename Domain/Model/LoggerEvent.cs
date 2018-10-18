using System;
using RestServer1.Domain.Enum;

namespace RestServer1.Domain.Model
{
    public class LoggerEvent
    {
        public LoggerEvent(DateTime eventTime,string nameSpace, string name, LoggerEventLevel level, int correlationId, string message)
        {
            this.Id = Guid.NewGuid();
            this.CreateTime = DateTime.UtcNow;
            this.EventTime = eventTime;
            this.Namespace = nameSpace;
            this.Name = name;
            this.Level = level;
            this.CorrelationId = correlationId;
            this.Message = message;
        }

        public Guid Id { get; }

        public DateTime CreateTime { get; }

        public DateTime EventTime { get; set; }

        public string Namespace { get; }

        public string Name { get; }

        public LoggerEventLevel Level { get; }

        public int CorrelationId { get; }

        public string Message { get; }

        public override string ToString()
        {
            return "{" 
                        + " GUID=" + this.Id 
                        + ",create_time=" + this.CreateTime
                        + ",event_time=" + this.CreateTime
                        + ",namespace=" + this.Namespace 
                        + ",name=" + this.Name 
                        + ",level=" + this.Level.ToString("G") 
                        + ",id=" + this.CorrelationId
                        + ",message=" + this.Message
                 + "}";
        }
    }
}
