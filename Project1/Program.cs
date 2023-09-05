namespace Project1;
using Animal;

static class Program
{
    static void Main()
    {
        var miu = new Cat("Miu", "Orange", "Male", 1, "Anggora", false);
        
        miu.Meow();
        miu.Meow("loud");
        miu.Eat();
        miu.Jump();
        
    }
}