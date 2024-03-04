using BattleShip.Models;
using Microsoft.AspNetCore.Components.Web;

public class NavalShipService
{
    public char[,] PlayerGrid { get; set; }
    public char[,] ComputerGrid { get; set; }
    private int gridSize = 10;

    public Ship[] PlayerShips { get; set; }
    public Ship[] ComputerShips { get; set; }
    public Dictionary<string, Game> Games { get; set; }

    public Dictionary<string, GamePVP> GamesPVP { get; set; }

    public NavalShipService()
    {
        PlayerGrid = new char[gridSize, gridSize];
        ComputerGrid = new char[gridSize, gridSize];

        PlayerShips = CreateShipsList();
        ComputerShips = CreateShipsList();

        Games = new Dictionary<string, Game>();
        GamesPVP = new Dictionary<string, GamePVP>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                PlayerGrid[i, j] = '\0';
                ComputerGrid[i, j] = '\0';
            }
        }

        //AddShips(PlayerGrid, PlayerShips);
        //AddShips(ComputerGrid, ComputerShips);
    }

    public Ship[] CreateShipsList()
    {
        return new Ship[] { new Ship("A", 4), new Ship("B", 3), new Ship("C", 2), new Ship("D", 2), new Ship("E", 1), new Ship("F", 1) }; ;
    }

    public void ClearGrids()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                PlayerGrid[i, j] = '\0';
                ComputerGrid[i, j] = '\0';
            }
        }
    }

    public void ClearShips(Ship[] ships)
    {
        foreach (var ship in ships)
        {
            ship.coordinates.Clear();
        }
    }

    public void AddShips(char[,] grid, Ship[] ships)
    {

        Random random = new Random();

        foreach (var ship in ships)
        {

            int shipSize = ship.size;
            bool isShipPlaced = false;

            while (!isShipPlaced)
            {

                int startRow = random.Next(0, 10);
                int startCol = random.Next(0, 10);

                bool isHorizontal = random.Next(2) == 0;

                if (isHorizontal)
                {
                    if (IsShipOverlap(grid, startRow, startCol, shipSize, isHorizontal))
                    {
                        continue;
                    }
                    for (int i = 0; i < shipSize; i++)
                    {
                        ship.coordinates.Add(new Coordinate { Row = startRow, Col = startCol + i });
                        grid[startRow, startCol + i] = ship.name[0];
                    }
                }
                else
                {
                    if (IsShipOverlap(grid, startRow, startCol, shipSize, isHorizontal))
                    {
                        continue;
                    }
                    for (int i = 0; i < shipSize; i++)
                    {
                        ship.coordinates.Add(new Coordinate { Row = startRow + i, Col = startCol });
                        grid[startRow + i, startCol] = ship.name[0];
                    }
                }
                isShipPlaced = true;
            }
        }
    }

    bool IsShipOverlap(char[,] grid, int startRow, int startCol, int shipSize, bool isHorizontal)
    {

        for (int i = 0; i < shipSize; i++)
        {
            if (isHorizontal)
            {
                if (startCol + i >= grid.GetLength(1) || grid[startRow, startCol + i] != '\0')
                {
                    return true;
                }
            }
            else
            {
                if (startRow + i >= grid.GetLength(0) || grid[startRow + i, startCol] != '\0')
                {
                    return true;
                }
            }
        }
        return false;
    }

    public AttackResponse Attack(string gameId, Coordinate coordinate)
    {
        Game game = Games[gameId];
        if (game == null)
        {
            //return "Game not found";
            return null;
        }
        else
        {
            var response = new AttackResponse();

            response = PlayerAttack(game, ComputerGrid, coordinate, response);
            if (game.difficulty == "easy")
            {
                response = ComputerAttackEasy(game, PlayerGrid, response);
            }
            else if (game.difficulty == "hard")
            {
                response = ComputerAttackHard(game, PlayerGrid, response);
            }
            else
            {
                response = ComputerAttackEasy(game, PlayerGrid, response);
            }

            response.ComputerWon = HasSomeoneWon(game.computerAttacksCoordinates, game.playerShips.ToArray());
            response.PlayerWon = HasSomeoneWon(game.computerAttacksCoordinates, game.computerShips.ToArray());

            return response;
        }

    }

    public AttackResponse PlayerAttack(Game game, char[,] grid, Coordinate coordinate, AttackResponse response)
    {
        if (game.IsShipAtCoordinate(coordinate))
        {
            response.PlayerAttackResult = true;
        }
        else
        {
            response.PlayerAttackResult = false;
        }
        return response;
    }

    public AttackResponse ComputerAttackHard(Game game, char[,] grid, AttackResponse response)
    {
        Random random = new Random();
        Coordinate coordinate;

        if (response.ComputerAttack != null)
        {
            // Utilisez lastComputerAttack pour effectuer une attaque par périmètre autour de cette position
            Coordinate[] perimeterDirections = new Coordinate[]
            {
                new Coordinate { Row = -1, Col = 0 }, // Haut
                new Coordinate { Row = 1, Col = 0 },  // Bas
                new Coordinate { Row = 0, Col = -1 }, // Gauche
                new Coordinate { Row = 0, Col = 1 }   // Droite
            };

            foreach (var direction in perimeterDirections)
            {
                Coordinate adjacentCoordinate = new Coordinate
                {
                    Row = game.computerAttacksCoordinates.Last().Row + direction.Row,
                    Col = game.computerAttacksCoordinates.Last().Col + direction.Col
                };

                // Vérifie si la case adjacente est dans la grille et n'a pas déjà été attaquée
                if (adjacentCoordinate.Row >= 0 && adjacentCoordinate.Row < grid.GetLength(0) &&
                    adjacentCoordinate.Col >= 0 && adjacentCoordinate.Col < grid.GetLength(1) &&
                    grid[adjacentCoordinate.Row, adjacentCoordinate.Col] == '\0' &&
                    !game.computerAttacksCoordinates.Any(c => c.Row == adjacentCoordinate.Row && c.Col == adjacentCoordinate.Col))
                {
                    // Attaque la case adjacente
                    game.computerAttacksCoordinates.Add(adjacentCoordinate); // Ajoute la coordonnée à la liste des attaques de l'IA
                    response.ComputerAttack = adjacentCoordinate;
                    return response;
                }
            }
        }
        else
        {
            do
            {
                coordinate = new Coordinate
                {
                    Row = random.Next(grid.GetLength(0)),
                    Col = random.Next(grid.GetLength(1))
                };
            } while (game.computerAttacksCoordinates.Any(c => c.Row == coordinate.Row && c.Col == coordinate.Col));

            game.computerAttacksCoordinates.Add(coordinate);
            response.ComputerAttack = coordinate;
            return response;

        }

        return response;
    }

    public AttackResponse ComputerAttackEasy(Game game, char[,] grid, AttackResponse response)
    {
        Random random = new Random();
        Coordinate coordinate;

        do
        {
            coordinate = new Coordinate
            {
                Row = random.Next(grid.GetLength(0)),
                Col = random.Next(grid.GetLength(1))
            };
        } while (game.computerAttacksCoordinates.Any(c => c.Row == coordinate.Row && c.Col == coordinate.Col));

        game.computerAttacksCoordinates.Add(coordinate);

        response.ComputerAttack = coordinate;
        return response;
    }

    public bool HasSomeoneWon(List<Coordinate> shots, Ship[] ships)
    {
        if (shots == null || ships == null)
        {
            throw new ArgumentNullException("shots or ships are null");
        }

        foreach (var ship in ships)
        {
            foreach (var coordinate in ship.coordinates)
            {
                if (!shots.Any(shot => shot.Row == coordinate.Row && shot.Col == coordinate.Col))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SaveGame(Game game)
    {
        Games.Add(game.gameId, game);
    }

    public void SaveGamePVP(GamePVP gamePVP)
    {
        GamesPVP.Add(gamePVP.gamePVPId, gamePVP);
    }

    public async Task<Game> GetGameFromId(string gameId)
    {
        return Games[gameId];
    }

    public async Task<GamePVP> GetGamePVPFromId(string gamePVPId)
    {
        return GamesPVP[gamePVPId];
    }

}

