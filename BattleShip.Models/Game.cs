namespace BattleShip.Models;


public class Game {
    public List<Ship> playerShips { get; set; }
    public List<Ship> computerShips { get; set; }
    public string gameId { get; set; }

    public bool IsShipAtCoordinate(Coordinate coordinate) {
        foreach (var ship in computerShips) {
            foreach (var shipCoordinate in ship.coordinates) {
                if (shipCoordinate.Row == coordinate.Row && shipCoordinate.Col == coordinate.Col) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsShipAtCoordinateInPlayerGrid(Coordinate coordinate) {
        foreach (var ship in playerShips) {
            foreach (var shipCoordinate in ship.coordinates) {
                if (shipCoordinate.Row == coordinate.Row && shipCoordinate.Col == coordinate.Col) {
                    return true;
                }
            }
        }
        return false;
    }
}
