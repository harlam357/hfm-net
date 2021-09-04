using System;
using System.Runtime.Serialization;

namespace HFM.Preferences.Data
{
    [DataContract(Namespace = "")]
    public class ClientRetrievalTask : IEquatable<ClientRetrievalTask>
    {
        public ClientRetrievalTask()
        {
            Enabled = true;
            Interval = 15;
            //ProcessingMode = null;
        }

        public ClientRetrievalTask(ClientRetrievalTask other)
        {
            Enabled = other.Enabled;
            Interval = other.Interval;
            ProcessingMode = other.ProcessingMode;
        }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public int Interval { get; set; }

        [DataMember]
        public string ProcessingMode { get; set; }

        public bool Equals(ClientRetrievalTask other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Enabled == other.Enabled && Interval == other.Interval && String.Equals(ProcessingMode, other.ProcessingMode, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ClientRetrievalTask)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Enabled.GetHashCode();
                hashCode = (hashCode * 397) ^ Interval;
                hashCode = (hashCode * 397) ^ (ProcessingMode != null ? ProcessingMode.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [DataContract(Namespace = "")]
    public class WebGenerationTask : IEquatable<WebGenerationTask>
    {
        public WebGenerationTask()
        {
            //Enabled = false;
            Interval = 15;
            //AfterClientRetrieval = false;
        }

        public WebGenerationTask(WebGenerationTask other)
        {
            Enabled = other.Enabled;
            Interval = other.Interval;
            AfterClientRetrieval = other.AfterClientRetrieval;
        }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public int Interval { get; set; }

        [DataMember]
        public bool AfterClientRetrieval { get; set; }

        public bool Equals(WebGenerationTask other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Enabled.Equals(other.Enabled) && Interval == other.Interval && AfterClientRetrieval.Equals(other.AfterClientRetrieval);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((WebGenerationTask)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Enabled.GetHashCode();
                hashCode = (hashCode * 397) ^ Interval;
                hashCode = (hashCode * 397) ^ AfterClientRetrieval.GetHashCode();
                return hashCode;
            }
        }
    }
}
