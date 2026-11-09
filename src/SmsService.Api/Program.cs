using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SmsService.Api.Data;
using SmsService.Api.Providers;
using SmsService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SmsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SmsDatabase")));

builder.Services.Configure<SmsProviderOptions>(
    builder.Configuration.GetSection(SmsProviderOptions.SectionName));

var useMockProvider = builder.Configuration.GetValue<bool>("SmsProvider:UseMockProvider");
if (useMockProvider)
{
    builder.Services.AddScoped<ISmsProvider, MockSmsProvider>();
}
else
{
    builder.Services.AddHttpClient<ISmsProvider, ExternalSmsProvider>((services, client) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["SmsProvider:BaseUrl"]
            ?? throw new InvalidOperationException("SmsProvider:BaseUrl is missing.");
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });
}

builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<SmsDbContext>();
    database.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { }
