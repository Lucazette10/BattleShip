namespace BattleShip.Models;


public class GamePVP {
    public List<Ship> playerShips1 { get; set; }
    public List<Ship> playerShips2 { get; set; }
    public List<Coordinate> player1AttacksCoordinates { get; set; }
    public List<Coordinate> player2AttacksCoordinates { get; set; }
    public string gameId { get; set; }

    public GamePVP() {

        playerShips1 = new List<Ship>();
        playerShips2 = new List<Ship>();

        player1AttacksCoordinates = new List<Coordinate>();
        player2AttacksCoordinates = new List<Coordinate>();
    }

}
