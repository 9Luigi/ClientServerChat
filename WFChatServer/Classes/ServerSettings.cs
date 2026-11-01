using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace WFChatServer
{
    /// <summary>Address and port the server listens on, stored in settings.xml next to the program.</summary>
    [XmlRoot("Settings")] //root and element names match settings.xml files written by earlier versions
    public class ServerSettings
    {
        public const string FileName = "settings.xml";
        public const int DefaultPort = 8888;

        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ServerSettings));

        [XmlElement("ipaddr")]
        public string IpAddress { get; set; }

        [XmlElement("port")]
        public int Port { get; set; }

        /// <summary>Listen on all network interfaces of this machine.</summary>
        public static ServerSettings CreateDefault()
        {
            return new ServerSettings { IpAddress = IPAddress.Any.ToString(), Port = DefaultPort };
        }

        /// <summary>Returns null if the file is missing, unreadable or contains an invalid address or port.</summary>
        public static ServerSettings Load(string path)
        {
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    var settings = (ServerSettings)Serializer.Deserialize(stream);
                    return settings != null && IsValid(settings.IpAddress, settings.Port) ? settings : null;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is InvalidOperationException) //InvalidOperationException: broken XML
            {
                return null;
            }
        }

        public void Save(string path)
        {
            using (FileStream stream = File.Create(path)) //Create truncates; OpenOrCreate left the tail of a longer old file
            {
                Serializer.Serialize(stream, this);
            }
        }

        public static bool IsValid(string ipAddress, int port)
        {
            return IPAddress.TryParse(ipAddress, out _) && port > 0 && port <= IPEndPoint.MaxPort;
        }
    }
}
