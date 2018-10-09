using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using DatingApp_IdentityRoles.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //everything as a service get injected into the app
            services.AddDbContext<DataContext>(x => 
                                                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                                                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));

            //'services.AddIdentity' method is use for MVC with razor template engine based on cookies
            //here we use SPA so we will stick to token config that why we use AddIdentityCore and keep
            // adding the feature needed for the App. 
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>{
                //for DEV purposes we will keep a simple PASS strategy, in PRD however we will enable them    
                opt.Password.RequireDigit = false; 
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            /*
                [This will be automatically DONE if we have used 'services.AddIdentity']
                to be able to query user and pull back all their roles at the same time, we need to do the following:
                we passing builder.UserType which a type of user and type of Role which we haven't build yet, and also
                we pass the services (Iservices collection ) we attached to IdentityUser.
                to the builderWrapper we add on the EF classes that it will provide the functionality we've got to use in 
                identity.
            */
            IdentityBuilder builderWrapper = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            /*
                [This will be automatically DONE if we have used 'services.AddIdentity']
                AddEntityFrameworkStores means that we're telling to our identity that we want to use EF as our store
                so that EF will add all our user classes into the DB. we will see it when we do the migrations.                
            */
            builderWrapper.AddEntityFrameworkStores<DataContext>();

            /*
                We need also to add services: RoleValidator (role check), RoleManager( so we can create/remove roles)
                and SigninManager(so we can log in the users when they provide a login and pass)
             */

            // [This will be automatically ADDED if we have used 'services.AddIdentity']
            builderWrapper.AddRoleValidator<RoleValidator<Role>>(); 
            builderWrapper.AddRoleManager<RoleManager<Role>>();
            builderWrapper.AddSignInManager<SignInManager<User>>();

            //instead of using no flexible policy such as [Authorize(Roles="Admin, Moderator")]
            //we will use a more advanced and dynamic policies method
            services.AddAuthorization(options => {
                options.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Moderator"));
                options.AddPolicy("VipOnly", policy => policy.RequireRole("VIP"));
            });


            //add Authorization service : We reorganize all authentications!!! order not important
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false /*So far we use localhost*/,
                    ValidateAudience = false /*So far we use localhost*/
                };
            });

            /*  options!!!: rather than using the [Authorize] attribute on the controller we apply the filter directly
                here!!!, so every request is required to be authenticated rather than use the attribute in
                each controller.
            */
            services.AddMvc(options => {
                var policyBuilder = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                options.Filters.Add(new AuthorizeFilter(policyBuilder));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            /*
                Seed injections 
            */
            services.AddTransient<Seed>();
            Mapper.Reset();
            services.AddAutoMapper();



            //services.AddSingleton(IAuthRepository); //not good for concurrent request
            //services.AddTransient(IAuthRepository); //an object per request is created and lighw ight for services


             /*
               No need to inject AuthRepository, IdentityUser will take over!!!
             */    
            //services.AddScoped<IAuthRepository, AuthResposity>();//created per request within the scope, a singleton within a scope itself
            /*Adding Dating repo as a DI service*/
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<IManageRoleRepository, DatingRepository>();

            

            //Add ActionFilter/ServiceFilter service to change the last active date : create an instance per request
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //Order matter here and not in ConfigureServices
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //pipeline that handles exceptions globally: 

                app.UseExceptionHandler(buidler =>
                {
                    buidler.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            //adding an extension method that adds headers before sending the exception error:
                            //e.g: app error & allow cross origin so the real error could reach the angular client
                            //context.Response.AddApplicationError(error.Error.Message);
                            await context
                            .Response
                            .AddApplicationError(error.Error.Message)
                            .WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();

            /* I N F O : we define a Cors policy because of cross call!!!
            Failed to load http://localhost:5000/api/values: 
            No 'Access-Control-Allow-Origin' header is present on the requested resource.
            Origin 'http://localhost:4200' is therefore not allowed access.
            */
            seeder.SeedUser();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();

            //Order matters for serving SPA (static files)!!!
            //look for html index (or asp, php, ....)
            app.UseDefaultFiles();

            //serve static file from wwwroot folder.            
            app.UseStaticFiles();


            //Middleware: route the request the appropriate controller 
            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                     name: "spa-fallback",
                     defaults: new
                     {
                         controller = "Fallback", //Controller name to use
                        action = "Index" //action method to call in the Fallback controller
                    }
                 );
            });
        }
    }
}
