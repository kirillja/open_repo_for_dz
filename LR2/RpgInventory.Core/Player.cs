namespace RpgInventory.Core;

public sealed record PlayerStats(int Hp, int Attack, int Defense)
{
    public static PlayerStats Default => new(100, 0, 0);
}

public sealed class Player
{
    public PlayerStats Stats { get; private set; } = PlayerStats.Default;
    private readonly Dictionary<Slot, IItem> _eq = new();

    public IItem? Equipped(Slot slot) => _eq.TryGetValue(slot, out var it) ? it : null;

    public void Heal(int v) => Stats = Stats with { Hp = Stats.Hp + v };
    public void AddAttack(int v) => Stats = Stats with { Attack = Stats.Attack + v };
    public void AddDefense(int v) => Stats = Stats with { Defense = Stats.Defense + v };

    public void Equip(Slot slot, IItem item) => _eq[slot] = item;
}