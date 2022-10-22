using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /*msdn has complete list of DBproviders that ef core supports
             AddDbContextPool provides DBcontext pooling means everytime AppDBcontext is requested instead
            of creating brand new instance it checks for instance available in pool*/
            services.AddDbContextPool<AppDbContext>(
            options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            /*AddMVCCore >> as the name impliesit will only add core services
             Addmvc >> will call all required mvc services & it will internally call the AddMvcCore()*/
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            //services.AddMvcCore();
            //Instance alive for entire scope
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role"));
                options.AddPolicy("EditRolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                options.AddPolicy("AdminRolePolicy", 
                    policy => policy.RequireRole("Admin"));
                options.AddPolicy("AllowedCountryPolicy",
                    policy => policy.RequireClaim("Country", "USA", "India", "UK"));

                options.InvokeHandlersAfterFailure = false;
            });

            
            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "xxxxxx";
                options.ClientSecret = "yyyyy";
            })
            .AddFacebook(options =>
            {
                options.AppId = "nnnnnn";
                options.AppSecret = "yyyy";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });


            // Register the first handler
            services.AddSingleton<IAuthorizationHandler,
                CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            // Register the second handler
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            /*run has requestdelegate which has param context, so we are passing context
            as an anonymous method.the context has both request and responce*/
            //app.Run(async (context) =>
            //{
            //    logger.LogInformation("mw1 request");
            //    //await context.Response.WriteAsync(Process.GetCurrentProcess().ProcessName);
            //    //await context.Response.WriteAsync(System.Reflection.Assembly.GetEntryAssembly().FullName);
            //    logger.LogInformation("mw1 responce");
            //});

            //app.Use(async (context, next) =>
            //{
            //    logger.LogInformation("mw1 request");
            //    await next();
            //    logger.LogInformation("mw1 responce");
            //});

           // app.UseDefaultFiles(); will change the path to default files -- we need to go to controllers so we are not using this
            app.UseStaticFiles(); // will process the static files
            //app.UseFileServer(); // will support both static files and default files
            app.UseAuthentication();
            /*here we delibrately keep usestaticfiles above usemvc, as static file request occurs it will return the response
             there by we are avoiding unnecessary processing*/
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
