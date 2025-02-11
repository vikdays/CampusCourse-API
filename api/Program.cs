
using api.Middleware;
using api.Services;
using api.Services.Email;
using api.Services.Impls;
using api.Services.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl.Matchers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;
           // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            services.AddControllers();

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.AddHttpContextAccessor();
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();
            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey("EmailNotificationJob");
                q.AddJob<EmailNotificationJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("EmailNotificationJob-trigger")
                    .WithCronSchedule("0 01 0/1 * * ?"));
            });
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            JwtOptions jwtOptions = new();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton(provider =>
            {
                return new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateAudience = true
                };
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateAudience = true
                };
            });

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<TokenCleanUpService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();


            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Authorized",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
            });


            var app = builder.Build();

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<TokenCleanUpService>(
                service => service.CleanupExpiredTokensAsync(),
                Cron.Daily);



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseMiddleware<ExceptionMiddleware>();
            app.Run();
        }
    }
}