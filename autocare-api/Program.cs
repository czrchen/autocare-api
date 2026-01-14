using Microsoft.EntityFrameworkCore;
using autocare_api.Data;
using autocare_api.Services;
using Amazon.SimpleNotificationService;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// AWS configuration
// --------------------
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

// --------------------
// Application services
// --------------------
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

// --------------------
// CORS configuration
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
               "http://54.166.209.34:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// --------------------
// Database
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// --------------------
// Controllers + JSON
// --------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// --------------------
// Swagger
// --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Other services
// --------------------
builder.Services.AddScoped<InvoiceNumberGeneratorService>();
builder.Services.AddScoped<InvoiceCalculatorService>();
builder.Services.AddScoped<InvoicePdfService>();

var app = builder.Build();

// --------------------
// HTTP request pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you want HTTPS later, enable it with a reverse proxy
// app.UseHttpsRedirection();

app.UseStaticFiles();

// ✅ CORS MUST come before authorization
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
