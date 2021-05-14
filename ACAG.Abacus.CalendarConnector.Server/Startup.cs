using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using ACAG.Abacus.CalendarConnector.Logic;
using ACAG.Abacus.CalendarConnector.Logic.Abacus;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Exchange;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Scheduler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using static ACAG.Abacus.CalendarConnector.Logic.Services.HeaderParam;

namespace ACAG.Abacus.CalendarConnector.Server
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

      services.AddControllers(options =>
        options.Filters.Add(new HttpResponseExceptionFilter()));

      services.AddLocalization(options => options.ResourcesPath = "Resources");
      services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ACAG.Abacus.CalendarConnector.Server", Version = "v2" });
        c.EnableAnnotations();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
          }
        });
      });

      services.AddCors(options =>
      {
        options.AddPolicy(
            "Open",
            builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      });

      services.ReadConnectionSettingConfig(Configuration, "CalendarConnectorContext");
      services.AddCustomizedDbContextFactory<CalendarConnectorContextFactory>();
      services.Configure<AbacusApiSettings>(Configuration.GetSection("AbacusApiSettings"));

      services.AddCustomizedService<IdentityApiService>();
      services.AddScoped<ITenantService, TenantService>();
      services.AddScoped<IAbacusSettingService, AbacusSettingService>();
      services.AddScoped<IPayTypeService, PayTypeService>();
      services.AddScoped<IExchangeSettingService, ExchangeSettingService>();
      services.AddScoped<ILogDiaryService, LogDiaryService>();
      services.AddScoped<ICultureService, CultureService>();
      services.AddScoped<IMailService, MailService>();
      services.AddScoped<IAuthenticationService, AuthenticationService>();
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IRoleService, RoleService>();
      services.AddScoped<IAppSettingService, AppSettingService>();
      services.AddScoped<IAbacusService, AbacusService>();
      services.AddSingleton<IAbacusAPI, AbacusAPI>();

      services.AddSingleton<IScheduler, AbacusProvider>();

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<IHeaderAPI, HeaderAPI>();
      services.AddSingleton<WrapperAPI>();

      var abacusSettings = Configuration.GetSection(nameof(AbacusApiSettings)).Get<AbacusApiSettings>() ?? new AbacusApiSettings();
      services.AddHttpClient(abacusSettings.UrlClientName, c =>
      {
        c.BaseAddress = new Uri(abacusSettings.Url);
        c.DefaultRequestHeaders.Accept.Clear();
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      });

      services.AddSignalR(e =>
      {
        e.MaximumReceiveMessageSize = 102400000;
      });

      services.AddHttpContextAccessor();
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
        .AddJwtBearer(options =>
      {
        options.Events = new JwtBearerEvents
        {
          OnTokenValidated = context =>
          {            
            return Task.CompletedTask;
          },
          OnAuthenticationFailed = context =>
          {
            return Task.CompletedTask;
          }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = Configuration["JwtIssuer"],
          ValidAudience = Configuration["JwtAudience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
        };
      });
      services.AddAuthorization(config =>
      {
        config.AddPolicy(Policies.IsSysAdmin, Policies.IsSysAdminPolicy());
        config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
        config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {

      }
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
#if DEBUG
      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ACAG.Abacus.CalendarConnector.Server v2");
      });
#else
      app.UseSwaggerUI(c => { 
        c.SwaggerEndpoint("/server/swagger/v1/swagger.json", "ACAG.Abacus.CalendarConnector.Server v2");
      });
#endif
     
      app.UseCors("Open");

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      var IsSchedulerRun = Configuration.GetSection("IsSchedulerRun").Get<bool>();
      if (IsSchedulerRun)
      {
        StartSheduler(app.ApplicationServices);
      }
    }

    private void StartSheduler(IServiceProvider serviceProvider)
    {
      Task.Factory.StartNew(() =>
      {
        System.Threading.Thread.Sleep(10 * 1000);
        using (var scope = serviceProvider.CreateScope())
        {
          var scheduler = scope.ServiceProvider.GetService<IScheduler>();
          scheduler.StartAll();
        }
        while (true)
        {
          System.Threading.Thread.Sleep(1000);
        }
      });
    }
  }

  public static class Policies
  {
    public const string IsSysAdmin = "IsSysAdmin";
    public const string IsAdmin = "IsAdmin";
    public const string IsUser = "IsUser";

    public static AuthorizationPolicy IsSysAdminPolicy()
    {
      return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("SysAdmin")
                                             .Build();
    }

    public static AuthorizationPolicy IsAdminPolicy()
    {
      return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("Admin")
                                             .Build();
    }

    public static AuthorizationPolicy IsUserPolicy()
    {
      return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("User")
                                             .Build();
    }
  }
}
