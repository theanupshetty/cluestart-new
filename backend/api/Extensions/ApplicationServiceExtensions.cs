
using dal;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DataContext>(options =>
           {
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
           });
            return builder;
        }

        public static WebApplicationBuilder AddCorsServices(this WebApplicationBuilder builder)
        {
            string origin = builder.Configuration["ReactApp:Url"];
            builder.Services.AddCors(p => p.AddPolicy("allowReactApp", builder =>
            {
                builder.WithOrigins(origin).AllowAnyMethod().AllowAnyHeader();
            }));

            return builder;
        }

        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            return builder;
        }
    }
}