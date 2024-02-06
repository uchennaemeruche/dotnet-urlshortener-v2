using System;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;

namespace UrlShortener.Services
{
    public class PreRandomCodeService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IUrlShorteningService _urlShorteningService;

        

        public PreRandomCodeService( IUrlShorteningService urlShorteningService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _urlShorteningService = urlShorteningService;
            //_dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
              
                try
                {

                    var currentTime = DateTime.UtcNow;
                    using var scope = _serviceProvider.CreateScope();

                    ApplicationDbContext _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    //IUrlShorteningService service = scope.ServiceProvider.GetRequiredService<UrlShorteningService>();
                    //Run background service at 10:45am
                    if (currentTime.Hour == 10 && currentTime.Minute == 45 && currentTime.Second == 0)
                    {
                        string code = await _urlShorteningService.GenerateUniqueCode();

                        var url_code = new UrlCode
                        {
                            Code = code,
                        };

                        _dbContext.UrlCodes.Add(url_code);

                        await _dbContext.SaveChangesAsync();

                         
                    }

                  

                }
                catch(Exception e)
                {
                    Console.WriteLine($"An error occured here: {e}");



                }
                finally
                {
                 
                }
                
            }

          
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}

