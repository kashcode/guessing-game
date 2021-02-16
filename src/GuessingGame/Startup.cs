using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GuessingGame
{
    public class Startup
    {
        private SqliteConnection _connection;

        public Startup(IConfiguration configuration)
        {            
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddFacebook(opt => {
                opt.AppId = Configuration["Facebook:AppId"];
                opt.AppSecret = Configuration["Facebook:AppSecret"];
            }).AddCookie(opt => {
                opt.LoginPath = "/auth/signin";
            });

            // The in-memory database only persists while a connection is open to it. To manage
            // its lifetime, keep one open connection around for as long as you need it.
            _connection = new SqliteConnection(Configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();

            services.Configure<DatabaseSettings>(Configuration.GetSection("ConnectionStrings"));
            services.AddScoped<IDatabase, Database>();
            services.AddScoped<IGameService, GameService>();

            services.AddAntiforgery(opt => {
                opt.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddControllersWithViews(opt => {
                opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });            
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopped.Register(OnStopped);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }        

        private void OnStarted()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                var cmd = _connection.CreateCommand();
                cmd.CommandText = @"
                drop table if exists games;
                create table games (
                    id integer primary key,
                    uuid text not null,
                    secret text not null,
                    user_id text not null,                    
                    guesses_count number not null default 0,
                    secret_guessed not null default 0
                );
                drop table if exists players;
                create table players (
                    id text primary key,
                    name text not null,
                    games_played number not null default 0,
                    correct_guesses not null default 0,
                    total_guesses not null default 0
                );
                drop table if exists guesses;
                create table guesses (
                    id integer primary key,
                    game_uuid text not null,
                    input text not null,
                    m number not null,
                    p number not null
                )";
                cmd.ExecuteNonQuery();
            }
        }

        private void OnStopped()
        {
            _connection.Close();
        }
    }
}
