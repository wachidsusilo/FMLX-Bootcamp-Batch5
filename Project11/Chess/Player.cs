namespace Project11.Chess;

public class Player
{
    public int Id { get; }
    public string Name { get; set; }

    public Player(int id, string name)
    {
        Id = id;
        Name = name;
    }

    #region HashCode and Equals Implementation

    public override int GetHashCode()
    {
        return Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player player && player.Id == Id;
    }
    #endregion
    
}