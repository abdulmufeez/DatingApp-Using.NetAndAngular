using DatingApp.Data;
using DatingApp.Extensions;
using DatingApp.Middlewares;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//Application related services extension
builder.Services.AddApplicationService(builder.Configuration);

builder.Services.AddControllers();

//enabling project to get response from the external application call
//Cross Origin Resource Sharing, used when the backend is different from the frontend
builder.Services.AddCors();

//Identity related service extension
builder.Services.AddIdentityService(builder.Configuration);

builder.Services.AddSignalR(opt => {
    opt.EnableDetailedErrors = true;    
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.


// ====================================================================================
// this section is used for data seeding 
//
// creating a service container which call any service running already in application
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    // calling datacontect service
    var context = services.GetRequiredService<DataContext>();
    // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    // var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    //seeding data 
    //await Seed.SeedAppUsers(userManager, roleManager);
    //await Seed.SeedUserProfiles(context);

    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    // calling logger service
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, ex.Message);
}
// ==========================================================================================

// using custom middleware for exceptions
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

//Reading data from appsetting.json file
var externalUrl = builder.Configuration.GetValue<string>("angularApplicationUrl");
//assigning policy for Cross Origin Response
app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    // for SignalR 
    .AllowCredentials()
    .WithOrigins(externalUrl));

app.UseAuthentication();

app.UseAuthorization();

// app.UseDefaultFiles();
// app.UseStaticFiles();

app.MapControllers();

// this where all hub register
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

//app.MapFallbackToController("Index","Fallback"); 

app.Run();
