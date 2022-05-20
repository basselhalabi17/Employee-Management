 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _configuration;

        // Notice we are using Dependency Injection here
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("EmployeeDBConnection")));
            services.AddMvc(option => option.EnableEndpointRouting = false ).AddXmlDataContractSerializerFormatters();
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddScoped<IEmployeeRepository,SQLEmployeeRepository>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {  options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role")
                                    .RequireClaim("Create Role")
                    );
                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
                                                    context.User.IsInRole("Admin") &&
                                                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                                                    context.User.IsInRole("Super Admin")
    ));


            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "133338519799-saffiil3ldgmpdt3qi4qnrnd1808a549.apps.googleusercontent.com";
                options.ClientSecret = "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";
            }).AddFacebook(options =>
            {
                options.AppId = "255738426769074";
                options.AppSecret = "e335c9250b13a6e7d571a0294fd898c0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            
            }
         
            app.UseStaticFiles();
            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.
            //         WriteAsync("Hello World");
            //    });
            //});
            //app.UseStaticFiles();
            //conventional routing
            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); 
                });
            //app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    //throw new Exception("fesyat");
            //    await context.Response.WriteAsync("Hello World!");
            //            });
        }
    }
}
