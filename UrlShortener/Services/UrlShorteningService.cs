using System;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Services
{
	public class UrlShorteningService: IUrlShorteningService
	{
        public const int ShortLinkLength = 7;
        public const string AllowedCodeCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly List<string> preGeneneratedCodes = new();

        private readonly Random _random = new();
        private readonly IServiceProvider _serviceProvider;

        public UrlShorteningService(IServiceProvider serviceProvider)
        {

            Console.WriteLine($"Length before initialization is {preGeneneratedCodes.Count} items");

            _serviceProvider = serviceProvider;

            this.PreGenerateCodes();


            Console.WriteLine($"Initialized list with {preGeneneratedCodes.Count} items");
        }


        private async void PreGenerateCodes()
        {
            for(int i =0; i < 100; i++)
            {
                string code = await this.GetRandomCode();
                preGeneneratedCodes.Add(code);
            }
        }


        public async Task<string> GenerateUniqueCode()
        {
            if(preGeneneratedCodes.Count > 0)
            {
                Console.WriteLine( preGeneneratedCodes.Count + " Codes Pregenerated");
                var code = preGeneneratedCodes[0];
                preGeneneratedCodes.RemoveAt(0);

                Console.WriteLine(preGeneneratedCodes.Count + " After taking 1");
                return code;
            }

            Console.WriteLine("Generating new one...");

            return  await GetRandomCode();
          
        }

        private async Task<string> GetRandomCode()
        {
            string uniqueCode;

            do
            {
                uniqueCode = this.GenerateRandomCode();

            } while (preGeneneratedCodes.Contains(uniqueCode) || await IsCodeAlreadyInUse(uniqueCode)  );
            //while (preGeneneratedCodes.Contains(uniqueCode) || await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == uniqueCode)) ;

            this.preGeneneratedCodes.Remove(uniqueCode);

            return uniqueCode;
        }

        private string GenerateRandomCode()
        {
            StringBuilder codeBuilder = new StringBuilder();
            while (codeBuilder.Length < ShortLinkLength)
            {
                int index = _random.Next(AllowedCodeCharacters.Length);
                codeBuilder.Append(AllowedCodeCharacters[index]);
            }
           return codeBuilder.ToString();
        }

        private Task<bool> IsCodeAlreadyInUse(string code)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code);
            }

        }

    }
}

