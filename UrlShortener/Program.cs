using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Entities;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Console.WriteLine($"Data SOurce: {builder.Configuration.GetConnectionString("DefaultConnection")}");

//builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

//builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLITE_DB")));
builder.Services.AddDbContext<UrlShortener.ApplicationDbContext>(options => options.UseSqlite($"Data Source={AppDomain.CurrentDomain.BaseDirectory}UrlShortenerDb.db"));


builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

   

   
}

app.MapPost("api/shorten", async (ShortenUrlRequest request, UrlShorteningService service, ApplicationDbContext dbContext, HttpContext httpContext) =>
{

    if(!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("Invalid URL");
    }

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

    return Results.Ok(shortenedUrl.ShortUrl);

//Shorten URL
//Save Shortened URL in DB

});

app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) => {
    var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

    if(shortenedUrl is null)
    {
        return Results.NotFound();
    }
    //TODO:
    //Add Accessed URl To Cache to improve performance


    return Results.Redirect(shortenedUrl.LongUrl);
});

app.UseHttpsRedirection();



app.Run();



