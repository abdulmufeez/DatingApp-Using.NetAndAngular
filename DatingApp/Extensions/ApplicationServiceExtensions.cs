using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Services;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    //static class means there is only one initiliazing of these class
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, 
            IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            //Connecting Interface to its implementation
            //ITokenService Interface => TokenService Class 
            //Only created when its called and terminated afterward
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
           
           // this is uncommented because of the presence hub
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            // services.AddScoped<ILikesRepository, LikeRepository>();
            // services.AddScoped<IMessageRepository, MessageRepository>();
            // because now we implement IUnitOFWork which will handle all this
            services.AddScoped<IUnitOfWork, UnitOfWork>();        
            services.AddScoped<LogUserActivity>();

            services.AddSingleton<PresenceTracker>();            

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            //Adding database configurations
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}