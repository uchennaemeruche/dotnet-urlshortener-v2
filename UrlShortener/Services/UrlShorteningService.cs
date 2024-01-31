using System;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Services
{
	public class UrlShorteningService
	{
        public const int ShortLinkLength = 7;
        public const string AllowedCodeCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new();
        private readonly ApplicationDbContext _dbContext;

        public UrlShorteningService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }


        public async Task<string> GenerateUniqueCode()
        {

            char[] charCodes = new char[ShortLinkLength];


            while (true)
            {
                for (int i = 0; i < ShortLinkLength; i++)
                {
                    int randomIndex = _random.Next(AllowedCodeCharacters.Length - 1);
                    charCodes[i] = AllowedCodeCharacters[randomIndex];
                }

                string code = new string(charCodes);

                if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code)){
                    return code;
                }
            }

           

          
        }
    }
}

