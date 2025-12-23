using Microsoft.EntityFrameworkCore;
using autocare_api.Data;
using autocare_api.Services;
using Amazon.SimpleNotificationService;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// AWS config from appsettings.json
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// Register SNS
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

// Your existing services
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<InvoiceNumberGeneratorService>();
builder.Services.AddScoped<InvoiceCalculatorService>();
builder.Services.AddScoped<InvoicePdfService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddHttpClient<GeocodingService>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAuthorization();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
