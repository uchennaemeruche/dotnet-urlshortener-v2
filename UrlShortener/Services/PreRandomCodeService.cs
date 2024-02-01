using System;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;

namespace UrlShortener.Services
{
    public class PreRandomCodeService : BackgroundService
    {
        //private readonly IServiceProvider _serviceProvider;

        //private readonly IUrlShorteningService _urlShorteningService;

        private readonly ApplicationDbContext _dbContext;

        public PreRandomCodeService( IServiceProvider serviceProvider )
        {
            //_serviceProvider = serviceProvider;
            //_urlShorteningService = urlShorteningService;
            _dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int min = 1;
                int hrs = 0;
                try
                {
                    //using var scope = _serviceProvider.CreateScope();

                    //ApplicationDbContext _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                   string code = "23kwerk";

                    var url_code = new UrlCode
                    {
                        Code = code,
                    };

                    _dbContext.UrlCodes.Add(url_code);

                    await _dbContext.SaveChangesAsync();

                }
                catch(Exception e)
                {
                    Console.WriteLine("An error occured here: ", e);

                }
                finally
                {

                }
            }
        }
    }
}

