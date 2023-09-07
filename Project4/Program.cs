using Project4.Controller;

namespace Project4;

internal static class Program
{
    private static void Main()
    {
        var playerController = new PlayerController();
        var petController = new PetController();
        var towerController = new TowerController();
        
        Console.WriteLine();
        Console.WriteLine(playerController);
        Console.WriteLine(petController);
        Console.WriteLine(towerController);
        
        Console.WriteLine();
        playerController.Walk(BaseController.DirectionForward);
        playerController.Run(BaseController.DirectionLeft);
        playerController.Stop();
        playerController.Shoot();
        
        Console.WriteLine();
        petController.Walk(BaseController.DirectionRight);
        petController.Run(BaseController.DirectionForward);
        petController.Stop();
        petController.Shoot();
        
        Console.WriteLine();
        towerController.Walk(BaseController.DirectionBackward);
        towerController.Run(BaseController.DirectionLeft);
        towerController.Stop();
        towerController.Shoot();
    }

}

