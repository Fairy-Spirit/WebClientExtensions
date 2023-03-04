using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebClientExtensions
{
    public static class WebClientExtensions
    {
        public static async Task<byte[]> DownloadDataTaskAsync(this MyWebClient webClient, string address, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (cancellationToken.Register(webClient.CancelAsync))
            {
                return await webClient.DownloadDataTaskAsync(address);
            }
        }

        public static async Task<string> DownloadStringAsync(this MyWebClient client, string url, CancellationToken cancellationToken, Encoding Encoding)
        {
            var bytes = await client.DownloadBytesAsync(url, cancellationToken);
            if (bytes == null) return "";
            return Encoding.GetString(bytes);
        }
        public static async Task<byte[]> DownloadBytesAsync(this MyWebClient client, string url, CancellationToken cancellationToken)
        {
            byte[] bytes = null;
            int time = 0;
            int times = client.TryTimes;
            Task task = null;
            await Task.Run(() =>
            {
                while (true)
                {
                    if (task == null)
                    {
                        task = Task.Run(async () =>
                        {
                            bytes = await client.DownloadDataTaskAsync(url, cancellationToken);
                        });
                    }
                    if (!task.IsCompleted)
                    {
                        if (time >= client.Timeout)
                        {
                            if (times > 0)
                            {
                                Console.WriteLine("Retry");
                                client.CancelAsync();
                                Thread.Sleep(100);
                                task = Task.Run(async () =>
                                {
                                    bytes = await client.DownloadDataTaskAsync(url, cancellationToken);
                                });
                                time = 0;
                                times--;
                            }
                            else { break; }
                        }
                        else
                        {
                            Thread.Sleep(100);
                            time += 100;
                        }
                    }
                    else if (task.IsCompleted)
                    {
                        Console.WriteLine("complete");
                        break;
                    }
                }
            });
            return bytes;
        }
    }
    public class MyWebClient : WebClient
    {
        public CookieContainer Cookies { get; set; }
        public int Timeout { get; set; }
        public int TryTimes { get; set; }
        public string UserAgent { get; set; }
        public int MaximumAutomaticRedirections = 50;
        public string Accept = null;
        public bool AllowAutoRedirect = true;
        public MyWebClient(int timeout = 2000, int trytimes = 5)
        {
            Timeout = timeout;
            TryTimes = trytimes;
        }
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var req = ((HttpWebRequest)request);
            req.Timeout = Timeout;
            req.ReadWriteTimeout = Timeout;
            req.MaximumAutomaticRedirections = MaximumAutomaticRedirections;
            req.CookieContainer = Cookies;
            req.Accept = Accept;
            req.AllowAutoRedirect = AllowAutoRedirect;
            req.UserAgent = UserAgent;
            try
            {
                return req.GetResponse();
            }
            catch { return null; }
        }
    }
}
