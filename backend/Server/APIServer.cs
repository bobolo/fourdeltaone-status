using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace fdocheck.Server
{
    public class APIServer
    {
        CancellationTokenSource cancel = new CancellationTokenSource();
        JsonSerializerSettings SerializationSettings = new JsonSerializerSettings();
        Task listenerTask;
        HttpListener listener = new HttpListener();
        Thread lthr;

        // /api/cache/*
        public Dictionary<string, object> Content = new Dictionary<string, object>();
        // /api/serverlist/*
        public Dictionary<string, object> ServerLists = new Dictionary<string, object>();
        // /api/status/*
        public Dictionary<string, BasicStatusIndicator> StatusIndicators = new Dictionary<string, BasicStatusIndicator>();

        public APIServer(int port = 29001)
        {
            SerializationSettings.Converters.Add(new IsoDateTimeConverter());
            SerializationSettings.Converters.Add(new KeyValuePairConverter());
            SerializationSettings.Converters.Add(new StringEnumConverter());

            lthr = new Thread(new ThreadStart(_lthr));
            lthr.IsBackground = true;

            // Apply ports
            listener.Prefixes.Clear();
            listener.Prefixes.Add("http://*:" + port + "/4d1/");
        }

        public void Start()
        {
            listenerTask = Task.Factory.StartNew(() => _lthr(), cancel.Token);
        }

        public void Stop()
        {
            cancel.Cancel();
            listenerTask.Wait();
        }

        private void _lthr()
        {
            try
            {
                Console.WriteLine("Starting listener...");
                this.listener.Start();

                Console.WriteLine("Started listener.");
                while (true)
                {
                    try
                    {
                        var req = listener.GetContext();
                        Task.Factory.StartNew(() => _http(req), cancel.Token);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("===");
                        Console.Error.WriteLine("Error in listening loop: {0}", e.Message);
                        Console.Error.WriteLine("Inner exception: {0}", e.InnerException != null ? e.InnerException.Message : "<none>");
                        Console.Error.WriteLine("StackTrace:");
                        Console.Error.WriteLine(e.StackTrace);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Shutting down listener...");
                this.listener.Stop();

                Console.WriteLine("Listener has been shut down.");
            }
        }

        private void _http(HttpListenerContext req)
        {
            try
            {
                string path = string.Join("/", req.Request.Url.LocalPath.Split('/').Skip(2));
                Console.WriteLine("[HTTP] " + path);
                string cmd = path.Split('/')[0].ToLower();
                Console.WriteLine("[HTTP] CMD = " + cmd);
                switch (cmd)
                {
                    case "serverlist":
                        {
                            if (path.Split('/').Length == 2)
                            {
                                if (!ServerLists.ContainsKey(cmd))
                                {
                                    req.Response.StatusCode = 404;
                                    req.Response.StatusDescription = "Not Found";
                                    req.Response.Close();
                                    return;
                                }

                                cmd = path.Split('/')[1].ToLower();

                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ServerLists[cmd], Formatting.Indented)),
                                    true
                                );
                            }
                            else if (path.Split('/').Length == 1)
                            {
                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ServerLists, Formatting.Indented)),
                                    true
                                );
                            }
                            else
                            {
                                req.Response.StatusCode = 404;
                                req.Response.StatusDescription = "Not Found";
                                req.Response.Close();
                                return;
                            }
                        }
                        break;
                    case "status":
                        {
                            if (path.Split('/').Length == 2)
                            {
                                if (!StatusIndicators.ContainsKey(cmd))
                                {
                                    req.Response.StatusCode = 404;
                                    req.Response.StatusDescription = "Not Found";
                                    req.Response.Close();
                                    return;
                                }

                                cmd = path.Split('/')[1].ToLower();

                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(StatusIndicators[cmd], Formatting.Indented)),
                                    true
                                );
                            }
                            else if (path.Split('/').Length == 1)
                            {
                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(StatusIndicators, Formatting.Indented)),
                                    true
                                );
                            }
                            else
                            {
                                req.Response.StatusCode = 404;
                                req.Response.StatusDescription = "Not Found";
                                req.Response.Close();
                                return;
                            }
                        }
                        break;
                    case "cache":
                        {
                            if (path.Split('/').Length == 2)
                            {
                                if (!Content.ContainsKey(cmd))
                                {
                                    req.Response.StatusCode = 404;
                                    req.Response.StatusDescription = "Not Found";
                                    req.Response.Close();
                                    return;
                                }

                                cmd = path.Split('/')[1].ToLower();

                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Content[cmd], Formatting.Indented)),
                                    true
                                );
                            }
                            else if (path.Split('/').Length == 1)
                            {
                                req.Response.Close(
                                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Content, Formatting.Indented)),
                                    true
                                );
                            }
                            else
                            {
                                req.Response.StatusCode = 404;
                                req.Response.StatusDescription = "Not Found";
                                req.Response.Close();
                                return;
                            }
                        }
                        break;
                    default:
                        {
                            req.Response.StatusCode = 404;
                            req.Response.StatusDescription = "Not Found";
                            req.Response.Close();
                        }
                        break;
                }
            }
            catch
            {
                try
                {
                    req.Response.StatusCode = 500;
                    req.Response.StatusDescription = "Internal Server Error";
                    req.Response.Close();
                }
                catch
                {
                    { }
                }
            }
        }
    }
}
