using ChallengeRooftopAcademy.Service;
using ChallengeRooftopAcademy.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddMemoryCache();
                    services.AddScoped<IHttpService, RooftopService>();
                    services.AddScoped<IServiceCache, CacheService>();
                    services.AddScoped<IServiceValidateOrderBlock, ServiceValidateOrderBlock>();
                }).UseConsoleLifetime();

            var host = builder.Build();

            await RunOrderingBlocks(host);
        }

        static async Task RunOrderingBlocks(IHost host) {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                bool activate = true;

                while (activate) {
                    activate = false;

                    try
                    {
                        Console.WriteLine("Por favor ingrese un correo:");
                        Console.WriteLine("");
                        var email = Console.ReadLine();
                        Console.WriteLine("");
                        var service = services.GetRequiredService<IServiceValidateOrderBlock>();
                        await service.executeCheck(email);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        activate = true;
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("Fin");
            }
        }
    }
}
