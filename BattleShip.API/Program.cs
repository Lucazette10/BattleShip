using System.Text.Json;
using BattleShip.Models;
using Microsoft.AspNetCore.Components;
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



app.MapGet("/start/{difficulty}", (string difficulty, NavalShipService navalShipService) =>
{
    Game game = new Game();
    game.gameId = Guid.NewGuid().ToString();
    game.difficulty = difficulty;
    navalShipService.ClearGrids();
    navalShipService.ClearShips(navalShipService.PlayerShips);
    navalShipService.ClearShips(navalShipService.ComputerShips);
    navalShipService.AddShips(navalShipService.PlayerGrid, navalShipService.PlayerShips);  
    navalShipService.AddShips(navalShipService.ComputerGrid, navalShipService.ComputerShips);
    game.playerShips = navalShipService.PlayerShips.ToList();
    game.computerShips = navalShipService.ComputerShips.ToList();
    navalShipService.SaveGame(game);

    return Results.Ok(game);
});

app.MapGet("/start/pvp", (NavalShipService navalShipService) =>
{
    GamePVP gamePVP = new GamePVP();
    gamePVP.gamePVPId = Guid.NewGuid().ToString();
    navalShipService.SaveGamePVP(gamePVP);
    return Results.Ok(gamePVP);

});

app.MapGet("/join/pvp/{gamePVPId}", async (string gamePVPId, NavalShipService navalShipService) =>
{
    GamePVP gamePVP = await navalShipService.GetGamePVPFromId(gamePVPId);
    if (gamePVP == null) {
        return Results.NotFound("Game not found");
    }
    return Results.Ok(gamePVP);
});

app.MapPost("/attack/{gameId}", (
    [FromRoute] string gameId,
    [FromBody] Coordinate coordinate,
    NavalShipService navalShipService) => 
    {
        return Results.Ok(navalShipService.Attack(gameId, coordinate));
    });

app.Run();
