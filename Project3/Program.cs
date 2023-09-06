namespace Project3;
using Character;

internal static class Program
{
    private static void Main()
    {
        var player = new Player(0L, "HarryPotter")
        {
            Hp = 1000,
            MaxHp = 1000,
            Atk = 50,
            Def = 50
        };

        var enemy = new Enemy(1L, "Voldemort")
        {
            Hp = 1000,
            MaxHp = 1000,
            Atk = 1200,
            Def = 0
        };
        
        Console.WriteLine();
        
        Console.WriteLine(player);  // Print Player instance
        Console.WriteLine(enemy);   // Print Enemy instance
        
        Console.WriteLine();

        player.Attack(enemy);   // Player attacks Enemy
        
        Console.WriteLine();

        enemy.Attack(player);   // Enemy attacks Player
        
    }
}