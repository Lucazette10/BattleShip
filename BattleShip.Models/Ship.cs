namespace BattleShip.Models;

public class Ship
{
    public string name { get; set; }
    public int size { get; set; }
    public List<Coordinate> coordinates { get; set; }
    
    public Ship(string name, int size){
        this.name = name;
        this.size = size;
        this.coordinates = new List<Coordinate>();
    }
}
