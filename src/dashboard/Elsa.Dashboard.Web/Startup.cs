using Elsa.Activities.Email.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Elsa.Dashboard.Extensions;
using Elsa.Activities.UserTask.Extensions;

namespace Elsa.Dashboard.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var elsaSection = Configuration.GetSection("Elsa");

            // this is enables us to edit the pages without having to stop debugging
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            // uncomment this to enable Sqlite and In-Memory default lock provider
            services
                .AddElsa(x => x.UseEntityFrameworkWorkflowStores(x => x.UseSqlite(Configuration.GetConnectionString("Sqlite"))));

            // uncomment this to enable PostgreSql and Postgres distributed lock provider
            //services
            //.AddElsa(x => x.UseEntityFrameworkWorkflowStores(x => x.UseNpgsql(Configuration.GetConnectionString("PostgreSQL")))
            //.AddPostgreSqlLockProvider(Configuration.GetConnectionString("PostgreSQL")));

            services
                .AddHttp(options => options.Bind(elsaSection.GetSection("Http")))
                .AddEmail(options => options.Bind(elsaSection.GetSection("Smtp")))
                .AddUserTaskActivities()
                .AddElsaDashboard();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app
                .UseStaticFiles()
                // Commented out until we decide if it should be used with UserTask
                //.UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseWelcomePage();
        }
    }
}