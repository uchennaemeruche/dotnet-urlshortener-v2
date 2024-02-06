using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using UrlShortener;
using UrlShortener.Cache;
using UrlShortener.Entities;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IUrlShorteningService, UrlShorteningService>();

builder.Services.AddScoped<ICacheService,CacheService>();

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
//    options.InstanceName = "BriefUrl_";
//});

//builder.Services.Add(ServiceDescriptor.Singleton<ICacheService, RedisCache>());

//Console.WriteLine($"Data SOurce: {builder.Configuration.GetConnectionString("DefaultConnection")}");

//builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

//builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLITE_DB")));


//builder.Services.AddHostedService<PreRandomCodeService>();



builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlite($"Data Source={AppDomain.CurrentDomain.BaseDirectory}UrlShortenerDb.db"));


builder.Services.AddHangfire(config => config
.UseSimpleAssemblyNameTypeSerializer()
.UseRecommendedSerializerSettings()
.UseSQLiteStorage($"{AppDomain.CurrentDomain.BaseDirectory}UrlShortenerDb.db")
);

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IServiceManagement, ServiceManagement>();


//builder.Services.AddScoped<UrlShorteningService>();

//builder.Services.AddSingleton<IUrlShorteningService, UrlShorteningService>();



var app = builder.Build();

app.Services.GetService<IRecurringJobManager>().AddOrUpdate<IServiceManagement>("syncJob", x=>x.GenerateUrlCode(), "0 */1 * ? * *");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("api/shorten", async (ShortenUrlRequest request, IUrlShorteningService service, ApplicationDbContext dbContext, HttpContext httpContext) =>
{

    if(!Uri.TryCreate(request.Url, UriKind.Absolute, out _)) return Results.BadRequest("Invalid URL");

    var code = await service.GenerateUniqueCode();

    var shortenedUrl = new ShortenedUrl
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatedAt = DateTime.Now

    };

    dbContext.ShortenedUrls.Add(shortenedUrl);

    await dbContext.SaveChangesAsync();

    var jobId = BackgroundJob.Enqueue<IServiceManagement>(x => x.SendEmail());

    Console.WriteLine($"Job id: {jobId}");

    return Results.Ok(shortenedUrl.ShortUrl);


});

app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext, ICacheService cacheService) => {

    string cacheKey = code;

    var cachedData = cacheService.GetData<ShortenedUrl>(cacheKey);
    if (cachedData != null)
    {
        Console.WriteLine("Item was found in Cache");

        //shortenedUrl = cachedData.Where(x => x.Code == code).FirstOrDefault();
        //shortenedUrl = cachedData;

        return Results.Redirect(cachedData.LongUrl);
    }

    Console.WriteLine("Cache is NUll, check DB...");

    var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

    if (shortenedUrl is null) return Results.NotFound();
    

    var expirationTime = DateTimeOffset.Now.AddHours(48.0);

    cacheService.SetData(cacheKey, shortenedUrl, expirationTime);

    var jobId = BackgroundJob.Schedule<IServiceManagement>(x => x.UpdateDatabase(), TimeSpan.FromSeconds(20));
    Console.WriteLine($"Job id: {jobId}");

    return Results.Redirect(shortenedUrl.LongUrl);
});

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.MapHangfireDashboard();



app.Run();



