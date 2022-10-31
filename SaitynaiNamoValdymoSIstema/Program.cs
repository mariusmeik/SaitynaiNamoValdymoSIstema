using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SaitynaiNamoValdymoSIstema.DataDB;
using SaitynaiNamoValdymoSIstema.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace SaitynaiNamoValdymoSIstema;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.Services.Transcient<SaitynaiNamoValdymoSistemaDBContext>();
        //builder.Services.AddDbContext<SaitynaiNamoValdymoSistemaDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SaitynaiNamoValdymoSIstemaContext")));
        builder.Services.AddDbContext<SaitynaiNamoValdymoSistemaDBContext>(options => options.UseSqlServer("Server = tcp:saitynaidb.database.windows.net,1433; Initial Catalog = SaitynaiNamoValdymoSistemaDB; Persist Security Info = False; User ID = marjas10; Password = MariusMeik12345; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"));
        
        builder.Services.AddScoped<JWTAuthService>();
        builder.Services.AddScoped<SignInManager>();


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });


    //    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuerSigningKey = true,
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
    //            .GetBytes("123")),
    //        ValidateIssuer = false,
    //        ValidateAudience = false
    //    };
    //});



        var jwtTokenConfig = builder.Configuration.GetSection("jwt").Get<JwtTokenConfig>();
        builder.Services.AddSingleton(jwtTokenConfig);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtTokenConfig.Audience,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret))
            };
        });




        builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
            }));





        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddHttpClient();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllers();
        //});

        app.MapControllers();

        app.Run();
    }
}