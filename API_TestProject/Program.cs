using API_TestProject.Core;
using API_TestProject.DataBase;
using API_TestProject.Swagger;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to the builder
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add logger to the builder
Log.Logger = new LoggerConfiguration().
    ReadFrom.Configuration(builder.Configuration).
    WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Exception?.GetType() == typeof(SecureException))
        .WriteTo.File("logs/secure_exceptions_log.txt", rollingInterval: RollingInterval.Day, outputTemplate: builder.Configuration.GetValue<string>("SecureExceptionFormat"))).
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
builder.Services.AddScoped<JournalService>();
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
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});
app.UseExceptionHandler(error => error.Run(async context =>
{
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    context.Response.ContentType = "application/json";

    using var scope = app.Services.CreateScope();
    var apiContext = scope.ServiceProvider.GetRequiredService<APIContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ExceptionHandler>>();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    var exceptionHandler = new ExceptionHandler(apiContext, logger, mapper);

    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var exceptionMessage = await exceptionHandler.HandleExceptionAsync(context, exception);
    await context.Response.WriteAsync(exceptionMessage);
}));

app.Run();

