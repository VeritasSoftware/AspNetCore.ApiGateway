using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hub.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var conn = new HubConnectionBuilder()
                            .WithUrl("https://localhost:5001/GatewayHub")
                            .AddNewtonsoftJsonProtocol()
                            .Build();

            conn.On("ReceiveMessage", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
            {
                return WriteToConsole(arg1);
            }, new object());

            conn.On("ReceiveMyStreamEvent", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(arg1[0].ToString());
                var evt = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                Console.WriteLine(evt);
                return Task.CompletedTask;
            }, new object());

            await conn.StartAsync();

            //await conn.InvokeAsync("SubscribeToRoute", new
            //{
            //    Api = "chatservice",
            //    Key = "room",
            //    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
            //    UserId = "JohnD"
            //});

            //await conn.InvokeAsync("SubscribeToGroup", new
            //{
            //    Api = "chatservice",
            //    Key = "room",
            //    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
            //    ReceiveGroup = "ChatGroup"
            //});

            //await conn.InvokeAsync("InvokeDownstreamHub", new
            //{
            //    Api = "chatservice",
            //    Key = "room",
            //    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
            //    Data = new
            //    {
            //        Name = "John",
            //        Message = "Hello!"
            //    }
            //});

            //await conn.InvokeAsync("InvokeDownstreamHub", new
            //{
            //    Api = "chatservice",
            //    Key = "room",
            //    ReceiveKey = "2f85e3c6-66d2-48a3-8ff7-31a65073558b",
            //    DataArray = new[]
            //    {
            //        new {
            //            Name = "John",
            //            Message = "Hello!"
            //        }
            //    }
            //});

            //await conn.InvokeAsync("SubscribeToEventStoreStream", new
            //{
            //    Api = "eventsourceservice",
            //    Key = "mystream",
            //    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3"
            //});

            //await conn.InvokeAsync("PublishToEventStoreStream", new
            //{
            //    Api = "eventsourceservice",
            //    Key = "mystream",
            //    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3",
            //    Events = new[]
            //    {
            //        new {
            //            EventId = Guid.NewGuid(),
            //            Type = "MyEvent",
            //            Data = Encoding.UTF8.GetBytes("{\"a\":\"15\"}"),
            //            MetaData = Encoding.UTF8.GetBytes("{}"),
            //            IsJson = false
            //        }
            //    }
            //});

            //Thread.Sleep(500);

            //await conn.InvokeAsync("UnsubscribeFromEventStoreStream", new
            //{
            //    Api = "eventsourceservice",
            //    Key = "mystream",
            //    RouteKey = "281802b8-6f19-4b9d-820c-9ed29ee127f3"
            //});

            Console.ReadLine();
        }

        static Task WriteToConsole(object[] arg1)
        {
            var jArray = JArray.Parse(arg1[0].ToString());

            Console.WriteLine($"{jArray[0]} says {jArray[1]}");
            return Task.CompletedTask;
        }

    }
}
