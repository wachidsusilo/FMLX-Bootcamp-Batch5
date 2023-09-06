namespace Project3.Character;

public class Character
{
    private readonly long _id;
    private float _hp;
    private float _maxHp;
    private float _atk;
    private float _def;

    public const float MinCharacterMaxHp = 1000;
    public const float MaxCharacterMaxHp = 100_000;

    public const float MinAtk = 10;
    public const float MaxAtk = 2000;

    public const float MinDef = 0;
    public const float MaxDef = 1000;

    public string Name { get; init; } = "Unknown";

    public float Hp
    {
        get => _hp;
        set => _hp = float.Max(float.Min(value, _maxHp), 0);
    }

    public float MaxHp
    {
        get => _maxHp;
        set => _hp = float.Max(float.Min(value, MaxCharacterMaxHp), MinCharacterMaxHp);
    }

    public float Atk
    {
        get => _atk;
        set => _atk = float.Max(float.Min(value, MaxAtk), MinAtk);
    }

    public float Def
    {
        get => _def;
        set => _def = float.Max(float.Min(value, MaxDef), MinDef);
    }

    protected Character(long id)
    {
        _id = id;
        _hp = MinCharacterMaxHp;
        _maxHp = MinCharacterMaxHp;
        _atk = MinAtk;
        _def = MinDef;
    }

    public long GetId()
    {
        return _id;
    }

    public void Attack(Character target)
    {
        OnStartAttack(target);
        var damage = _atk - target._def;
        target.OnAttacked(this, damage);
    }

    protected virtual void OnStartAttack(Character target)
    {
        Console.WriteLine($"{Name} is initiating an attack against {target.Name}");
    }

    protected virtual void OnAttacked(Character actor, float damage)
    {
        Hp -= damage;
        Console.WriteLine($"{Name} got attacked by {actor.Name} and lose {damage} health points.");
        
        if (Hp == 0)
        {
            OnDied(actor);
        }
    }

    protected virtual void OnDied(Character actor)
    {
        Console.WriteLine($"{Name} has been slain by {actor.Name}");
    }

    public override string ToString()
    {
        return $"Character(Id: {_id}, Name: {Name}, Hp: {Hp}, MaxHp: {MaxHp}, Atk: {Atk}, Def: {Def})";
    }
}