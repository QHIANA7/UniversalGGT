using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace GGTBackgroundWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            #region Default Created

            //var config = new JobHostConfiguration();

            //if (config.IsDevelopment)
            //{
            //    config.UseDevelopmentSettings();
            //}

            //var host = new JobHost(config);
            //// The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            #endregion

            ThreadPool.SetMaxThreads(10, 10);

            IPAddress cur_ip = IPAddress.Any;

            IPHostEntry host = new IPHostEntry();
            

            Console.WriteLine(cur_ip);

            TcpListener listener = new TcpListener(cur_ip, 63000);
            listener.Start();

            Console.WriteLine("Waiting for clients...");
            while (true)
            {
                while (!listener.Pending())
                {
                    Thread.Sleep(1000);
                }
                ConnectionThread newconnection = new ConnectionThread(listener);
            }
        }



        private async void StartServer()
        {

        }
    }

    class ConnectionThread
    {
        public TcpListener threadListener;

        public ConnectionThread(TcpListener lis)
        {
            threadListener = lis;
            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleConnection));

        }

        public void HandleConnection(object state)
        {
            using (TcpClient client = threadListener.AcceptTcpClient())
            {
                using (NetworkStream ns = client.GetStream())
                {
                    string buf = String.Empty;

                    using (StreamReader streamReader = new StreamReader(ns))
                    {
                        buf = streamReader.ReadLine();
                    }

                    using (StreamWriter streamWriter = new StreamWriter(ns))
                    {
                        streamWriter.WriteLine(buf);
                        streamWriter.Flush();
                    }

                    ns.Close();
                }
                client.Close();
            }
        }
    }
}