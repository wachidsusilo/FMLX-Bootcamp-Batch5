namespace Project3.Character;

public class Player : Character
{
    public Player(long id, string name) : base(id)
    {
        Name = name;
    }

    protected override void OnStartAttack(Character target)
    {
        base.OnStartAttack(target);
        Console.WriteLine($"Player {Name} is playing sword_swing animation");
    }

    protected override void OnAttacked(Character actor, float damage)
    {
        base.OnAttacked(actor, damage);
        Console.WriteLine($"Player {Name} is playing blood_splash animation");
    }

    protected override void OnDied(Character actor)
    {
        base.OnDied(actor);
        Console.WriteLine($"Player {Name} is playing fall_down animation");
    }

    public override string ToString()
    {
        return $"Player(Id: {GetId()}, Name: {Name}, Hp: {Hp}, MaxHp: {MaxHp}, Atk: {Atk}, Def: {Def})";
    }
}