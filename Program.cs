
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

            builder.Services.AddControllers();
            builder.Services.AddDbContext<GameDatabaseContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

			app.UseMiddleware<IpFilteringMiddleware>();

			app.UseMiddleware<GatewayAuthenticationMiddleware>();

			app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
		}
    }
}



