using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace _4d1StatusBot
{
    [XmlRoot("cod5")]
    public class Q3ApiReply
    {
#if Q3WEB
        public static Q3ApiReply GetReply(XmlReader r)
        {
            XmlSerializer s = new XmlSerializer(typeof(Q3ApiReply));
            if (s.CanDeserialize(r))
                return s.Deserialize(r) as Q3ApiReply;
            else
                return null;
        }
#endif

        public string GetRule(string name)
        {
            try
            {
                foreach (Q3ApiRule r in Rules)
                    if (r.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return r.Value;
                return null;
            }
            catch
            {
                return null;
            }
        }

        /*
         * http://services.eetara.com/cod4/isnipe2.dreamwar.tk:28094
         */

        [XmlArray("rules")]
        [XmlArrayItem("rule")]
        public List<Q3ApiRule> Rules = new List<Q3ApiRule>();

        [XmlElement("ping")]
        public int Ping = -1;

        [XmlElement("maxspectators")]
        public int MaxSpectators = -1;
        [XmlElement("numspectators")]
        public int CurrentSpectators = -1;

        [XmlElement("numplayers")]
        public int CurrentPlayers = -1;
        [XmlElement("maxplayers")]
        public int MaxPlayers = -1;

        [XmlElement("map")]
        public string Map = "";

        [XmlElement("gametype")]
        public string Gametype = "";

        [XmlElement("name")]
        public string Name = "";

        [XmlElement("retries")]
        public int Retries = -1;

        [XmlElement("hostname")]
        public string Hostname = "";

        [XmlAttribute("address")]
        public string Address = "";

        [XmlArray("players")]
        [XmlArrayItem("player")]
        public List<Q3ApiPlayer> Players = new List<Q3ApiPlayer>();


    }

    [XmlType("Q3ApiRule")]
    public class Q3ApiRule
    {
        public Q3ApiRule() { }
        public Q3ApiRule(string rulename, string rulevalue)
        {
            Value = rulevalue;
            Name = rulename;
        }

        [XmlText()]
        public string Value { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    [XmlType("Q3ApiPlayer")]
    public class Q3ApiPlayer
    {
        public Q3ApiPlayer() { }
        public Q3ApiPlayer(string playername)
        {
            Name = playername;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
