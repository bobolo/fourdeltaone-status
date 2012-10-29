using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace _4d1StatusBot
{
    /// <summary>
    /// Provides accessibility to Q3 and similar RCON interfaces.
    /// </summary>
    public class Q3RCON
    {
        private Socket _sock;

        public byte[] CommandPrefix = { 0xff, 0xff, 0xff, 0xff };
        public byte[] CommandSuffix = { 0x0a };

        public string Password { get; set; }
        public int Ping { get; set; }
        private DateTime lastPingStart = DateTime.MinValue;

        public bool Connect(string ip, int port)
        {
            return Connect(ip, port, null);
        }

        public bool Connect(string host, int port, string password)
        {
            AddressFamily[] afs = {
                                          AddressFamily.InterNetworkV6,
                                          AddressFamily.InterNetwork
                                  };
            foreach (AddressFamily af in afs)
            {
                try
                {
                    Console.WriteLine("[Q3Rcon] Connecting via " + af + " to " + host + ":" + port + "...");
                    _sock = new Socket(af, SocketType.Dgram, ProtocolType.Udp);
                    _sock.ReceiveTimeout = 1000;
                    _sock.SendTimeout = 1000;
                    _sock.Connect(host, port);
                    Password = password;
                    Console.WriteLine("[Q3Rcon] Testing SendNoPass...");
                    string[] hm = SendNoPass("getinfo");
                    if (hm.Contains("Invalid password"))
                        return false;
                    Console.WriteLine("[Q3Rcon] => Connection succeeded!");
                    return true;
                }
                catch (SocketException)
                {
                    Console.WriteLine("[Q3Rcon] => Socket exception");
                    continue;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("[Q3Rcon] => Argument exception");
                    continue;
                }
            }

            return false;
        }

        public void DisconnectAsync()
        {
            if (!_sock.Connected) return;
            Console.WriteLine("[Q3Rcon] Asynchronous disconnecting...");
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            _sock.DisconnectAsync(saea);
        }

        public void Disconnect()
        {
            if (!_sock.Connected) return;
            Console.WriteLine("[Q3Rcon] Disconnecting...");
            ((IDisposable)_sock).Dispose();
        }

        public string[] SendNoPass(string cmd)
        {
            return SendNoPass(cmd, new string[] { });
        }

        public string[] SendNoPass(string cmd, params string[] arguments)
        {

            Console.WriteLine("[Q3Rcon] Command: " + cmd);

            if (_sock == null)
                throw new InvalidOperationException("Not connected.");

            foreach (string argument in arguments)
            {
                cmd += " "
                    + (arguments.Contains(" ") ? "\"" + argument.Replace("\"", "\\\"") + "\"" : argument)
                    ;
            }

            //_sw.Write(cmd);
            //_sw.Flush();
            byte[] buffer = CommandPrefix.Concat(Encoding.ASCII.GetBytes(cmd)).Concat(CommandSuffix).ToArray();
            _sock.Send(buffer);
            lastPingStart = DateTime.Now;
            Console.WriteLine("[Q3Rcon] Sent command, awaiting reply...");
            return GetResponseString();
        }

        public string[] SendRcon(string cmd)
        {
            return SendRcon(cmd, new string[] { });
        }

        public string[] SendRcon(string cmd, params string[] arguments)
        {
            return SendNoPass("rcon " + this.Password + " " + cmd, arguments);
        }

        public Dictionary<string, string> GetStatus()
        {
            string[] info = SendNoPass("getstatus");
            if (info.Length == 0) throw new Exception();
            Dictionary<string, string> ret = new Dictionary<string, string>();
            info = info[1].Substring(1).Split('\\');
            for (int i = 0; i < info.Length; i++)
                ret[info[i]] = info[++i];
            return ret;
        }
        public string[] GetPlayers()
        {
            string[] info = SendNoPass("getstatus");
            if (info.Length == 0) throw new Exception();
            List<string> ret = new List<string>();
            info = info.Skip(2).ToArray();
            foreach (string pline in info)
            {
                if (pline.Trim() == "")
                    continue;
                Console.WriteLine("[Q3Rcon] PLINE " + pline);
                string[] psplit = pline.Split('"');
                string[] psplit2 = psplit[0].Split(' ');
                string playername = psplit.Length > 1 ? psplit[1] : psplit2.Last();
                string info1 = psplit2[0];
                string info2 = psplit2[1];

                ret.Add(playername);
            }

            return ret.ToArray();
        }
        public Dictionary<string, string> GetInfo()
        {
            string[] info = SendNoPass("getinfo");
            if (info.Length == 0) throw new Exception();
            Dictionary<string, string> ret = new Dictionary<string, string>();
            info = info[1].Substring(1).Split('\\');
            for (int i = 0; i < info.Length; i++)
                ret[info[i]] = info[++i];
            return ret;
        }

        public int DoPing()
        {
            try
            {
                DateTime start = DateTime.Now;
                SendNoPass("getstatus");
                return Ping;
            }
            catch { return Ping = -1; }
        }

        private string[] GetResponseString()
        {
            List<string> response = new List<string>();
            List<byte> packets = new List<byte>();
            bool success = true;

            do
            {
                try
                {
                    //string line = _sr.ReadLine();
                    byte[] buffer = new byte[1500];
                    int x = _sock.Receive(buffer, SocketFlags.None);
                    if (lastPingStart != DateTime.MinValue)
                    {
                        Ping = (int)Math.Round((DateTime.Now - lastPingStart).TotalMilliseconds, 0);
                        lastPingStart = DateTime.MinValue;
                    }
                    if (success = x > 0)
                    {
                        Array.Resize(ref buffer, x);
                        packets.AddRange(buffer);
                        Console.WriteLine("[Q3Rcon] Received " + x + " Bytes for 1500 Bytes buffering.");
                        success = x == 1500;
                    }
                }
                catch
                {
                    success = false;
                }
            } while (success);

            if (packets.Count == 0)
                return new string[] { };

            try
            {

                byte[] par = packets.ToArray();
                int header = BitConverter.ToInt32(par, 0);
                string line;

                response.AddRange(
                    (line = Encoding.ASCII.GetString(par, 4, par.Length - 5)).Trim()
                    .Split((char)0x0A)
                    );

                Console.WriteLine("[Q3Rcon] Debug response line: " + line);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Q3Rcon] Exception while decoding packets: " + e.ToString());
            }
            
            return response.ToArray();
        }
    }
}
