using PosSelection.Services;
using PosSelection.Services.Background;
using PosSelection.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
        options.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IRatioService, RatioService>();

builder.Services.AddScoped<IPosSelectionService, PosSelectionService>();
builder.Services.AddScoped<IRequestValidationService, RequestValidationService>();
builder.Services.AddHostedService<RatioRefreshBackgroundService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var ratioService = scope.ServiceProvider.GetRequiredService<IRatioService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await ratioService.RefreshRatiosAsync();
        logger.LogInformation("Oranlar kaydedildi.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Oranlar yüklenirken hata oluştu. Uygulama yine de başlatılıyor.");
    }
}

await app.RunAsync();
