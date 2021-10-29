using Cinema.BLL.Helpers.ChatPresence;
using Cinema.BLL.Helpers.Genre;
using Cinema.BLL.Helpers.YouTubeLink;
using Cinema.BLL.Services.Emailing;
using Cinema.BLL.Services.Movies;
using Cinema.BLL.Services.Storage;
using Cinema.BLL.Services.Users;
using Cinema.COMMON.Responses;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cinema.BLL.Helpers.DI
{
    public static class DIHelper
    {
        public static BaseResponse Register(IServiceCollection services)
        {
            BaseResponse response = new();

            try
            {
                services.AddTransient<IMovieService, MovieService>();
                services.AddTransient<IUserService, UserService>();
                services.AddTransient<IMovieContentHelper, MovieContentHelper>();
                services.AddTransient<IGenresHelper, GenresHelper>();
                services.AddTransient<IGoogleCloudService, GoogleCloudService>();
                services.AddTransient<IPresenceTracker, PresenceTracker>();
                services.AddTransient<IEmailSender, SendGridMailer>();
                //services.AddTransient<IEmailSender, EmailSender>();
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;
        }
    }
}
