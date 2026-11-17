using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace WFChatClient
{
    /// <summary>Address and port of the server to connect to, stored in settings.xml next to the program.</summary>
    [XmlRoot("Settings")] //root and element names match settings.xml files written by earlier versions
    public class ClientSettings
    {
        public const string FileName = "settings.xml";

        /// <summary>settings.xml in the program folder, wherever the program was started from.</summary>
        public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, FileName);

        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ClientSettings));

        [XmlElement("port")]
        public int Port { get; set; }

        [XmlElement("ipAdd")]
        public string IpAddress { get; set; }

        public static ClientSettings CreateDefault()
        {
            return new ClientSettings { IpAddress = IPAddress.Loopback.ToString(), Port = 8888 };
        }

        /// <summary>Returns null if the file is missing, unreadable or contains an invalid address or port.</summary>
        public static ClientSettings Load(string path)
        {
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    var settings = (ClientSettings)Serializer.Deserialize(stream);
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
