using System;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;
using UrlShortener.Services;

namespace UrlShortener
{
	public class ApplicationDbContext:DbContext
	{
		public ApplicationDbContext(DbContextOptions options):base(options)
		{
		}

		public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<ShortenedUrl>(builder =>
			{
				builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.ShortLinkLength);
				builder.HasIndex(s => s.Code).IsUnique();
			});

        }

   //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   //     {
   //         optionsBuilder.UseSqlite($"Data Source={AppDomain.CurrentDomain.BaseDirectory}UrlShortenerDb.db");
			//base.OnConfiguring(optionsBuilder);
   //     }
    }
}

