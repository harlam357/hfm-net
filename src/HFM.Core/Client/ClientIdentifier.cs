﻿
using System;
using System.Globalization;
using System.Text.RegularExpressions;

using HFM.Core.Net;

namespace HFM.Core.Client
{
    public struct ClientIdentifier : IEquatable<ClientIdentifier>, IComparable<ClientIdentifier>, IComparable
    {
        internal const int NoPort = 0;

        public static ClientIdentifier None => new ClientIdentifier(null, null, NoPort, Guid.Empty);

        public ClientIdentifier(string name, string server, int port, Guid guid)
        {
            Name = name;
            Server = server;
            Port = port;
            Guid = guid;
        }

        public string Name { get; }

        public string Server { get; }

        public int Port { get; }

        public Guid Guid { get; }

        public bool HasGuid => Guid != Guid.Empty;

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Server)) return Name;
            return TcpPort.Validate(Port) 
                ? String.Format(CultureInfo.InvariantCulture, "{0} ({1}:{2})", Name, Server, Port) 
                : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, Server);
        }

        public bool Equals(ClientIdentifier other)
        {
            if (HasGuid && other.HasGuid)
            {
                return Guid.Equals(other.Guid);
            }
            return Name == other.Name && Server == other.Server && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            return obj is ClientIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (HasGuid)
                {
                    return Guid.GetHashCode();
                }
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Server != null ? Server.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Port;
                return hashCode;
            }
        }

        public static bool operator ==(ClientIdentifier left, ClientIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ClientIdentifier left, ClientIdentifier right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ClientIdentifier other)
        {
            var nameComparison = String.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameComparison != 0) return nameComparison;
            var serverComparison = String.Compare(Server, other.Server, StringComparison.Ordinal);
            if (serverComparison != 0) return serverComparison;
            var portComparison = Port.CompareTo(other.Port);
            if (portComparison != 0) return portComparison;
            return Guid.CompareTo(other.Guid);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is ClientIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ClientIdentifier)}");
        }

        public static bool operator <(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) >= 0;
        }

        // TODO: make ToPath() internal
        // for persistence that combines Server and Port as Legacy client "Path"
        public string ToPath()
        {
            return TcpPort.Validate(Port) 
                ? String.Format(CultureInfo.InvariantCulture, "{0}:{1}", Server, Port) 
                : Server;
        }

        internal static readonly Regex ServerPortRegex = new Regex(@"(?<Server>.+)[-:](?<Port>\d+)$", RegexOptions.ExplicitCapture);

        internal static ClientIdentifier FromPath(string name, string path)
        {
            return FromPath(name, path, Guid.Empty);
        }

        internal static ClientIdentifier FromPath(string name, string path, Guid guid)
        {
            var match = path is null ? null : ServerPortRegex.Match(path);
            return match != null && match.Success 
                ? new ClientIdentifier(name, match.Groups["Server"].Value, Convert.ToInt32(match.Groups["Port"].Value), guid) 
                : new ClientIdentifier(name, path, NoPort, guid);
        }
    }
}