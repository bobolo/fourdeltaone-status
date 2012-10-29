using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace fdocheck.Checks
{

    public class SSMinecraftCheck : BasicCheck
    {
        public SSMinecraftCheck()
        {
            MinecraftHost = "server.secretschemes.net";
            MinecraftPort = 30000;
        }

        public string MinecraftHost { get; set; }
        public int MinecraftPort { get; set; }

        public bool MinecraftOnline { get; private set; }
        public ulong MinecraftCurPlayers { get; private set; }
        public ulong MinecraftMaxPlayers { get; private set; }
        public string MinecraftSvrDescription { get; private set; }

        public void CheckMinecraft()
        {
            if (!NeedToUpdate("minecraft", 30))
                return;

            TcpClient tcp = new TcpClient();
            try
            {
                tcp.SendTimeout = 2000;
                tcp.ReceiveTimeout = 2000;
                tcp.NoDelay = true;
                tcp.Client.ReceiveTimeout = 2000;
                tcp.Client.SendTimeout = 2000;

                var async = tcp.BeginConnect(MinecraftHost, MinecraftPort, null, null);
                DateTime dt = DateTime.Now;
                while ((DateTime.Now - dt).TotalSeconds < 3 && !async.IsCompleted)
                    System.Threading.Thread.Sleep(40);
                if (!async.IsCompleted)
                {
                    try
                    {
                        tcp.Close();
                    }
                    catch { { } }
                    MinecraftOnline = false;
                    return;
                }

                if (!tcp.Connected)
                {
                    log.Fatal("Minecraft server is offline.");
                    MinecraftOnline = false;
                    return;
                }

                var ns = tcp.GetStream();
                var sw = new StreamWriter(ns);
                var sr = new StreamReader(ns);

                ns.WriteByte(0xFE);

                if (ns.ReadByte() != 0xFF)
                    throw new Exception("Invalid data");

                short strlen = BitConverter.ToInt16(ns.ReadBytes(2), 0);
                string strtxt = Encoding.BigEndianUnicode.GetString(ns.ReadBytes(2 * strlen));

                string[] strdat = strtxt.Split('§'); // Description§Players§Slots[§]
                MinecraftOnline = true;
                MinecraftSvrDescription = strdat[0];
                MinecraftCurPlayers = ulong.Parse(strdat[1]);
                MinecraftMaxPlayers = ulong.Parse(strdat[2]);
            }
            catch (Exception n)
            {
                log.Fatal("Minecraft server check error: " + n);
                MinecraftOnline = false;
            }
        }
    }
}
