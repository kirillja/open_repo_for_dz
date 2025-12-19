namespace RpgInventory.Core;

public interface IItemActionStrategy
{
    bool CanHandle(IItem item);
    void Execute(Player player, IItem item);
}

public sealed class DrinkPotion : IItemActionStrategy
{
    public bool CanHandle(IItem item) => item is Potion;
    public void Execute(Player player, IItem item) => player.Heal(((Potion)item).Heal);
}

public sealed class EquipGear : IItemActionStrategy
{
    public bool CanHandle(IItem item) => item is Weapon or Armor;

    public void Execute(Player player, IItem item)
    {
        if (item is Weapon w)
        {
            player.Equip(Slot.Weapon, w);
            player.AddAttack(w.Damage);
        }
        else if (item is Armor a)
        {
            player.Equip(Slot.Armor, a);
            player.AddDefense(a.Defense);
        }
    }
}

public sealed class UseService
{
    private readonly IReadOnlyList<IItemActionStrategy> _strategies;
    public UseService(params IItemActionStrategy[] strategies) => _strategies = strategies;

    public void Use(Player player, IItem item)
        => _strategies.FirstOrDefault(x => x.CanHandle(item))?.Execute(player, item);
}