namespace BattleShip.Models;

public class AttackResponse
{
    public bool PlayerAttackResult { get; set; }
    public Coordinate ComputerAttack { get; set; }
    public bool PlayerWon { get; set; } = false;
    public bool ComputerWon { get; set; } = false;
}
