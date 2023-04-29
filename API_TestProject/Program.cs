using API_TestProject.Core;
using API_TestProject.DataBase;
using API_TestProject.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to the builder
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add logger to the builder
Log.Logger = new LoggerConfiguration().
    ReadFrom.Configuration(builder.Configuration).
    WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day).
    CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddLogging(log => { log.ClearProviders(); log.AddSerilog(); });
builder.Services.AddDbContext<APIContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));
builder.Services.AddScoped<TreeService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSwaggerGen(s => {
    s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "API_TestProject.xml"));
    s.OperationFilter<SummaryToDescriptionFilter>();
});   

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
