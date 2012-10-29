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

        public Dictionary<string, object> Content = new Dictionary<string, object>();

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
                string cmd = req.Request.Url.LocalPath.Split('/').Last().ToLower();
                switch (cmd)
                {
                    case "cache":
                        {
                            Task<string> json = JsonConvert.SerializeObjectAsync(this.Content, Formatting.Indented);
                            req.Response.KeepAlive = false;
                            req.Response.ProtocolVersion = req.Request.ProtocolVersion;
                            if (!json.Wait(3000))
                            {
                                req.Response.StatusCode = 503;
                                req.Response.StatusDescription = "Service Unavailable";
                                req.Response.Close();
                                return;
                            }
                            req.Response.Close(
                                Encoding.UTF8.GetBytes(json.Result),
                                true
                            );
                        }
                        break;
                    default:
                        {
                            if (!Content.ContainsKey(cmd))
                            {
                                req.Response.StatusCode = 404;
                                req.Response.StatusDescription = "Not Found";
                                req.Response.Close();
                                return;
                            }
                            Task<string> json = JsonConvert.SerializeObjectAsync(this.Content[cmd], Formatting.Indented);
                            req.Response.KeepAlive = false;
                            req.Response.ProtocolVersion = req.Request.ProtocolVersion;
                            if (!json.Wait(3000, cancel.Token))
                            {
                                req.Response.StatusCode = 503;
                                req.Response.StatusDescription = "Service Unavailable";
                                req.Response.Close();
                                return;
                            }
                            req.Response.Close(
                                Encoding.UTF8.GetBytes(json.Result),
                                true
                            );
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
