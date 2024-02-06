using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Entities;

namespace UrlShortener.Services
{
    public class ServiceManagement : IServiceManagement
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IUrlShorteningService _urlShorteningService;

        public ServiceManagement(IUrlShorteningService urlShorteningService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _urlShorteningService = urlShorteningService;
        }


        public async void GenerateUrlCode()
        {

           
            using var scope = _serviceProvider.CreateScope();

            ApplicationDbContext _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var counter = _dbContext.UrlCodes.Where(x => x.IsUsed == false ).Count();

            if(counter < 10)
            {
                string code = await _urlShorteningService.GenerateUniqueCode();

                var url_code = new UrlCode
                {
                    Code = code,
                };

                _dbContext.UrlCodes.Add(url_code);

                await _dbContext.SaveChangesAsync();

            }

            Console.WriteLine($"Generate Url Code: long running service at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void SendEmail()
        {
            Console.WriteLine($"Send Email: delayed execution service at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void SyncRecords()
        {
            Console.WriteLine($"Sync Records: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        public void UpdateDatabase()
        {
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}

