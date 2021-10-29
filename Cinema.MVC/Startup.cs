using Cinema.BLL.Helpers.Mapping;
using Cinema.BLL.Helpers.UserRoles;
using Cinema.DAL.DbContext;
using Cinema.DAL.Models;
using Cinema.MVC.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cinema.COMMON.Constants;
using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Cinema.BLL.Helpers.DI;

namespace Cinema.MVC
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var serviceProvider = new ServiceCollection()
                                                .AddLogging()
                                                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            _logger = factory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq(Configuration.GetSection("Seq"));
            });

            services.AddDbContext<CinemaDbContext>(options => options.UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.GetConnectionString("DefaultDatabase"), o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<CinemaDbContext>()
                    .AddDefaultTokenProviders();

            services.AddHttpClient();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie()
                        .AddGoogle(googleOptions =>
                        {
                            googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                            googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                        });

            services.AddMvc();
            services.AddSignalR();

            var resultOfDIHelper = DIHelper.Register(services);

            if (!resultOfDIHelper.Succeeded)
            {
                _logger.LogError(resultOfDIHelper.Ex.Message, resultOfDIHelper.Ex);
            }

        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider)
        {
            var resultOfAutoMapper = MappingHelper.AutoMapperInit();

            if (!resultOfAutoMapper.Succeeded)
            {
                _logger.LogError(resultOfAutoMapper.Ex.Message, resultOfAutoMapper.Ex);
            }

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePagesWithReExecute("/Home/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/" + Routes.Hubs.CHAT);
            });

            var resultOfUserRolesSeeding = UserRolesHelper.CreateRoles(serviceProvider);
            if (!resultOfUserRolesSeeding.Succeeded)
            {
                _logger.LogError(resultOfUserRolesSeeding.Ex.Message, resultOfUserRolesSeeding.Ex);
            }
        }
    }
}
