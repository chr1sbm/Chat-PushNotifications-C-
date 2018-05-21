using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Management;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoPushNot.Models;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;


namespace DemoPushNot.Controllers
{
    public class ChatController : ApiController
    {
        private static ConcurrentBag<StreamWriter> clients;

        static ChatController() {
            clients = new ConcurrentBag<StreamWriter>();
        }

        public async Task PostAsync(ChatMessage m) {

            m.dt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            await ChatCallbackMsg(m);

        }

        private async Task ChatCallbackMsg(ChatMessage m)
        {

            foreach (var client in clients)
            {
                try
                {
                    var data = string.Format("data:{0}|{1}|{2}\n\n", m.username, m.text, m.dt);
                    await client.WriteAsync(data);
                    await client.FlushAsync();
                    client.Dispose();
                }
                catch (Exception)
                {
                    StreamWriter ignore;
                    clients.TryTake(out ignore);
                }

            }
        }

        [HttpGet]

        public HttpResponseMessage Suscribe(HttpRequestMessage request)
        {
            var response = request.CreateResponse();
            response.Content = new PushStreamContent((a, b, c) => { OnStreamAvailable(a, b, c); }, "text/event-stream");
            return response;
        }

        private void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context)
        {
            
            var client = new StreamWriter(stream);
            clients.Add(client);
        }







    } 
}
