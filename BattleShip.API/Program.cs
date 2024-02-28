using System.Text.Json;
using BattleShip.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<NavalShipService>();

builder.Services.AddCors(options => {
    options.AddPolicy(name : "nametest", policy => {
        policy.WithOrigins("http://localhost:5211")
            .AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("nametest");

app.MapGet("/grid", () =>
{
    NavalShipService navalShipService = new NavalShipService();
    return navalShipService;
});

app.MapGet("/start", (NavalShipService navalShipService) =>
{
    Game game = new Game();
    game.gameId = Guid.NewGuid().ToString();
    game.playerShips = navalShipService.PlayerShips.ToList();
    game.computerShips = navalShipService.ComputerShips.ToList();
    navalShipService.SaveGame(game);

    return Results.Ok(game);
});

app.MapPost("/attack/{gameId}", (
    [FromRoute] string gameId,
    [FromBody] Coordinate coordinate,
    NavalShipService navalShipService) => 
    {
        return Results.Ok(navalShipService.Attack(gameId, coordinate));
    });

        /*var game = await navalShipService.GetGameFromId(gameId);
        if (game == null) {
            return Results.NotFound("Game not found");
        }

        var response = new AttackResponse();

        if (game.IsShipAtCoordinate(coordinate)) {
            response.PlayerAttackResult = true;
        } else {
            response.PlayerAttackResult = false;
        }

        //response.ComputerAttack = navalShipService.AIAttack(gameId, game.playerShips);

        //Il faudrait vérifier ici si y'a un gagnant et le retourner dans le response
        Console.WriteLine($"response pattack : {response.PlayerAttackResult}");
        return Results.Ok(response);*/



app.Run();
