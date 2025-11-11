using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Text.Json;

// Simple DTOs for login/register
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
}

partial class Program
{
    static async Task Main()
    {
        var apiBaseUrl = "http://localhost:5234/api/auth";

        using var httpClient = new HttpClient();

        Console.WriteLine("Enter username:");
        var username = Console.ReadLine() ?? "";
        Console.WriteLine("Enter password:");
        var password = Console.ReadLine() ?? "";

        // Attempt login
        var loginResponse = await httpClient.PostAsJsonAsync($"{apiBaseUrl}/login", new LoginDto
        {
            Username = username,
            Password = password
        });

        string token = "";

        if (loginResponse.IsSuccessStatusCode)
        {
            var tokenData = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
            token = tokenData?.Token ?? "";
            Console.WriteLine("✅ Logged in successfully!");
        }
        else
        {
            Console.WriteLine("⚠️ Login failed. Attempting registration...");
            var registerResponse = await httpClient.PostAsJsonAsync($"{apiBaseUrl}/register", new
            {
                Username = username,
                Email = $"{username}@example.com",
                Password = password
            });

            if (registerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ Registration successful! Logging in again...");
                loginResponse = await httpClient.PostAsJsonAsync($"{apiBaseUrl}/login", new LoginDto
                {
                    Username = username,
                    Password = password
                });
                var tokenData = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
                token = tokenData?.Token ?? "";
            }
            else
            {
                Console.WriteLine("❌ Registration failed. Exiting...");
                return;
            }
        }

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("❌ Could not obtain JWT token.");
            return;
        }

        // Connect to SignalR hub
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5234/chatHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        connection.On<string, string>("ReceiveMessage", (senderName, message) =>
        {
            Console.WriteLine($"📩 Message from {senderName}: {message}");
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine("✅ Connected to chat hub!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error connecting to hub: {ex.Message}");
            return;
        }

        while (true)
        {
            Console.Write("Enter receiver ID: ");
            var receiverId = Console.ReadLine() ?? "";

            Console.Write("Enter message: ");
            var msg = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(receiverId) || string.IsNullOrWhiteSpace(msg))
                continue;

            try
            {
                await connection.InvokeAsync("SendMessage", receiverId, msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error sending message: {ex.Message}");
            }
        }
    }
}
