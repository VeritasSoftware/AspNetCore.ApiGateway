using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Hub.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var conn = new HubConnectionBuilder()
                            .WithUrl("https://localhost:44360/GatewayHub")
                            .AddNewtonsoftJsonProtocol()
                            .Build();

            conn.StartAsync().ConfigureAwait(false);

            conn.On("ReceiveMessage", new Type[] { typeof(object), typeof(object) }, (arg1, arg2) =>
            {
                return WriteToConsole(arg1);
            }, new object());

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
