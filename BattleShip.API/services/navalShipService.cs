using BattleShip.Models;

public class NavalShipService
{
    public char[,] PlayerGrid { get; set; }
    public char[,] ComputerGrid { get; set; }
    private int gridSize = 10;

    public Ship[] PlayerShips { get; set; }
    public Ship[] ComputerShips { get; set; }
    public Dictionary<string, Game> Games { get; set; }

    public NavalShipService()
    {
        PlayerGrid = new char[gridSize, gridSize];
        ComputerGrid = new char[gridSize, gridSize];

        PlayerShips = new Ship[] { new Ship("A", 4), new Ship("B", 3), new Ship("C", 2), new Ship("D", 2), new Ship("E", 1), new Ship("F", 1) };
        ComputerShips = new Ship[] { new Ship("A", 4), new Ship("B", 3), new Ship("C", 2), new Ship("D", 2), new Ship("E", 1), new Ship("F", 1) };

        Games = new Dictionary<string, Game>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                PlayerGrid[i, j] = '\0';
                ComputerGrid[i, j] = '\0';
            }
        }

        AddShips(PlayerGrid, PlayerShips);
        AddShips(ComputerGrid, ComputerShips);
    }

    public void AddShips(char[,] grid, Ship[] ships) {

        Random random = new Random();

        foreach (var ship in ships) {

            int shipSize = ship.size;
            bool isShipPlaced = false;

            while (!isShipPlaced) {
                
                int startRow = random.Next(0, 10);
                int startCol = random.Next(0, 10);

                bool isHorizontal = random.Next(2) == 0;

                if (isHorizontal) {
                    if (IsShipOverlap(grid, startRow, startCol, shipSize, isHorizontal)) {
                        continue;
                    }
                    for (int i = 0; i < shipSize; i++) {
                        ship.coordinates.Add(new Coordinate { Row = startRow, Col = startCol + i });
                        grid[startRow, startCol + i] = ship.name[0];
                    }
                }
                else {
                    if (IsShipOverlap(grid, startRow, startCol, shipSize, isHorizontal)) {
                        continue;
                    }
                    for (int i = 0; i < shipSize; i++) {
                        ship.coordinates.Add(new Coordinate { Row = startRow + i, Col = startCol });
                        grid[startRow + i, startCol] = ship.name[0];
                    }
                }
                isShipPlaced = true;
            }
        }
    }

    bool IsShipOverlap(char[,] grid, int startRow, int startCol, int shipSize, bool isHorizontal) {

        for (int i = 0; i < shipSize; i++) {
            if (isHorizontal) {
                if (startCol + i >= grid.GetLength(1) || grid[startRow, startCol + i] != '\0') {
                    return true;
                }
            }
            else {
                if (startRow + i >= grid.GetLength(0) || grid[startRow + i, startCol] != '\0') {
                    return true;
                }
            }
        }
        return false;
    }

    public AttackResponse Attack (string gameId, Coordinate coordinate) {
        Game game = Games[gameId];
        if (game == null) {
            //return "Game not found";
            return null;
        } else {
            var response = new AttackResponse();

            response = PlayerAttack(game, ComputerGrid, coordinate, response);
            response = ComputerAttack(game, PlayerGrid, response);

            response.ComputerWon = HasSomeoneWon(PlayerGrid, game.playerShips.ToArray());
            response.PlayerWon = HasSomeoneWon(ComputerGrid, game.computerShips.ToArray());
        
            return response;
        }
        
    }

    public AttackResponse PlayerAttack(Game game, char[,] grid, Coordinate coordinate, AttackResponse response) {
        if (game.IsShipAtCoordinate(coordinate)) {
            response.PlayerAttackResult = true;
        } else {
            response.PlayerAttackResult = false;
        }
        return response;
    }

    public AttackResponse ComputerAttack(Game game, char[,] grid, AttackResponse response) {
        Random random = new Random();
        Coordinate coordinate = new Coordinate
        {
            Row = random.Next(grid.GetLength(0)),
            Col = random.Next(grid.GetLength(1))
        };

        response.ComputerAttack = coordinate;
        return response;
    }

    public bool HasSomeoneWon(char[,] grid, Ship[] ships) {
        foreach (var ship in ships) {
            foreach (var coordinate in ship.coordinates) {
                if (grid[coordinate.Row, coordinate.Col] != 'X') {
                    return false;
                }
            }
        }
        return true;
    }

    public void SaveGame(Game game) {
        Games.Add(game.gameId, game);
    }

    public async Task<Game> GetGameFromId(string gameId) {
        return Games[gameId];
    }

}

