using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fdocheck.Server;
using fdocheck.Checks;
using log4net;
using log4net.Config;
using System.Xml;

namespace fdocheck
{
    static class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static APIServer api;
        static FDOAuthServerCheck auth;
        static Iw4mCheck iw4m;
        static Iw5mCheck iw5m;

        static XmlDocument config;
        
        static void Main(string[] args)
        {
            Console.WriteLine("API server initializing...");

            config = new XmlDocument();
            config.Load("config.xml");

            var cancel = new CancellationTokenSource();

            var appender = new log4net.Appender.ManagedColoredConsoleAppender();
            appender.Threshold = log4net.Core.Level.All;
            XmlConfigurator.Configure(config.SelectSingleNode("/log4net") as XmlElement);

            api = new APIServer();
            auth = new FDOAuthServerCheck();
            iw4m = new Iw4mCheck(auth);
            iw5m = new Iw5mCheck(auth);

            auth.TestUsername = config.SelectSingleNode("/test-username").InnerText;
            auth.TestPassword = config.SelectSingleNode("/test-password").InnerText;

            api.Content.Add("login", auth);
            api.Content.Add("iw4m", iw4m);
            api.Content.Add("iw5m", iw5m);
            api.Content.Add("forum", new WebCheck("http://fourdeltaone.net/index.php", 60));
            api.Content.Add("kmshost", new KmshostCheck());
            api.Start();

            Console.WriteLine("API server starting, regular checks are now enabled.");

            while (true)
            {
                var task = Task.Factory.StartNew(() => CheckNow(), cancel.Token);
                Thread.Sleep(30 * 1000);
                task.Wait();
                task.Dispose();
            }
        }

        static void CheckNow()
        {
            try
            {
                // I could have done it in an even more gay way, but nvm.
                ((FDOAuthServerCheck)api.Content["login"]).CheckAuth();
                ((FDOAuthServerCheck)api.Content["login"]).CheckAuthInternal();
                ((Iw4mCheck)api.Content["iw4m"]).CheckNP();
                ((Iw4mCheck)api.Content["iw4m"]).CheckMaster();
                ((Iw5mCheck)api.Content["iw5m"]).CheckNP();
                ((Iw5mCheck)api.Content["iw5m"]).CheckMaster();
                ((WebCheck)api.Content["forum"]).Check();
                ((KmshostCheck)api.Content["kmshost"]).CheckKmshost();

            }
            catch (OperationCanceledException)
            {
                log.Error("Checks aborted by main thread.");
            }
            catch (Exception err)
            {
                log.Fatal("Regular checks failed", err);
            }
        }
    }
}
