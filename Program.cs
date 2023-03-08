using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Text.Json.Serialization;

using TCIG.MhuSigtasAPI.Helpers;
using TCIG.MHUSIGTASAPI.Authorization;
using TCIG.MHUSIGTASAPI.Helpers;
using TCIG.MHUSIGTASAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
    var configuration = builder.Configuration;

    services.AddDbContext<DataContext>();
    services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(
                    configuration.GetConnectionString("OracleConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext)
                    .Assembly.FullName)
                    //.UseOracleSQLCompatibility("12")
                    ));
    services.AddCors();
    services.AddControllers().AddJsonOptions(x => 
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.AddSwaggerGen();

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IEmailService, EmailService>();
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();    
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // generated swagger json and swagger ui middleware
    app.UseSwagger();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "MHU - SIGTAS API"));

    // global cors policy
    app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run("http://localhost:4000");