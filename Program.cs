
using game_service.classes;
using game_service.classes.games;
using game_service.context;
using game_service.middleware;
using game_service.models;
using game_service.models.DTOs;
using game_service.repositories;
using game_service.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace game_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<GameDatabaseContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IGameService, GameService>();
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

			/**
             * Authorization with gateway api by ip filtering
             */
			app.UseMiddleware<IpFilteringMiddleware>();

			/**
             * Authorization with gateway api by secret key
             */
			app.UseMiddleware<GatewayAuthenticationMiddleware>();

			app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
		}
    }
}



