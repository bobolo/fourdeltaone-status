using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace _4d1StatusBot
{
    class Q3ServerCheck
    {
        public Q3ApiReply Info = new Q3ApiReply();

        public void Check(string host, int port)
        {
#if Q3WEB
            Info = Q3ApiReply.GetReply(System.Xml.XmlReader.Create("http://services.eetara.com/cod4/" + host + ":" + port.ToString()));
#else
            Info = new Q3ApiReply();
            Info.Address = host + ":" + port;

            // DNS resolve
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(host);
                Info.Hostname = host + ":" + port;
            }
            catch
            {
                Info.Hostname = host + ":" + port;
            }

            // RCON connection
            Q3RCON rcon = new Q3RCON();
            if (!rcon.Connect(host, port))
                return; // offline

            Info.Ping = rcon.Ping;

            if (Info.Ping == -1)
                return;

            try
            {
                Info.Retries = 0;
                var infos = rcon.GetInfo();
                foreach (string infoname in infos.Keys)
                {
                    Console.WriteLine("[Q3ServerCheck] getstatus: " + infoname + " = " + infos[infoname]);
                    Info.Rules.Add(new Q3ApiRule(infoname, infos[infoname]));
                }
                infos = rcon.GetStatus();
                foreach (string infoname in infos.Keys)
                    if (Info.GetRule(infoname) == null)
                    {
                        Console.WriteLine("[Q3ServerCheck] getinfo: " + infoname + " = " + infos[infoname]);
                        Info.Rules.Add(new Q3ApiRule(infoname, infos[infoname]));
                    }
            }
            catch
            {
                // RCON connection unstable, aborting connection
                rcon.Disconnect();
                this.Info.Ping = -1;
                return;
            }

            Info.Map = Info.GetRule("mapname");
            Info.CurrentPlayers = int.Parse(Info.GetRule("clients"));
            Info.MaxPlayers = int.Parse(Info.GetRule("sv_maxclients"));
            Info.Name = Info.GetRule("sv_hostname");

            foreach (string player in rcon.GetPlayers())
                Info.Players.Add(new Q3ApiPlayer(player));
#endif

            rcon.Disconnect();
        }
    }
}
