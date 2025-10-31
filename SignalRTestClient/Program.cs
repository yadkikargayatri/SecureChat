
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SecureRTestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build the SignalR connection to the chat hub
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5233/chathub") // Change to your hub URL if needed
                .WithAutomaticReconnect()
                .Build();

            // Listen for incoming messages from the hub
            connection.On<string, string, string>("ReceiveMessage", (sender, receiver, message) =>
            {
                Console.WriteLine($"{sender} to {receiver}: {message}");
            });

            try
            {
                await connection.StartAsync();
                Console.WriteLine("Connected to the chat hub!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to hub: {ex.Message}");
                return;
            }

            // Interactive loop to send messages
            while (true)
            {
                Console.Write("Enter receiver: ");
                var receiver = Console.ReadLine();

                Console.Write("Enter message: ");
                var message = Console.ReadLine();

                if (!string.IsNullOrEmpty(receiver) && !string.IsNullOrEmpty(message))
                {
                    try
                    {
                        await connection.InvokeAsync("SendMessage", "Me", receiver, message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message: {ex.Message}");
                    }
                }
            }
        }
    }
}
