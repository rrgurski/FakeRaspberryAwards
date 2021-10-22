using FakeRaspberryAwards.Application.Services.Movies;
using FakeRaspberryAwards.Infrastructure.Database;
using FakeRaspberryAwards.Properties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FakeRaspberryAwards.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FakeRaspberryAwards.WebApi", Version = "v1" });
            });

            services.AddDbContext<DatabaseContext>();
            services.AddScoped<IMovieService, MovieService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostApplicationLifetime hostApplicationLifetime,
            IWebHostEnvironment env,
            DatabaseContext databaseContext,
            IMovieService movieService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FakeRaspberryAwards.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            hostApplicationLifetime.ApplicationStopping.Register(OnShutdown);

            databaseContext.Database.EnsureCreated();

            movieService.ImportFromCsv(Resources.movielist);
        }

        private void OnShutdown()
        {
            DatabaseContext.CloseConnection();
        }
    }
}
