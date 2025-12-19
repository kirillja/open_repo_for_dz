namespace RpgInventory.Core;

// Builder
public sealed class ItemBuilder<T> where T : class, IItem
{
    private readonly Func<ItemInfo, T> _ctor;
    private string _name = typeof(T).Name;
    private Rarity _rarity = global::RpgInventory.Core.Rarity.Common;

    private ItemBuilder(Func<ItemInfo, T> ctor) { _ctor = ctor; }

    public static ItemBuilder<T> Create(Func<ItemInfo, T> ctor) => new(ctor);

    public ItemBuilder<T> Name(string name) { _name = name; return this; }
    public ItemBuilder<T> WithRarity(Rarity rarity) { _rarity = rarity; return this; }

    public T Build(IIdGen ids) => _ctor(new ItemInfo(ids.Next(), _name, _rarity));
}

// Abstract Factory
public interface IItemFactory
{
    Weapon Weapon(IIdGen ids, string name, int dmg);
    Armor Armor(IIdGen ids, string name, int def);
    Potion Potion(IIdGen ids, string name, int heal);
    QuestItem Quest(IIdGen ids, string name, string key);
}

public sealed class SimpleItemFactory : IItemFactory
{
    public Weapon Weapon(IIdGen ids, string name, int dmg) =>
        ItemBuilder<Weapon>.Create(info => new Weapon(info, dmg)).Name(name).Build(ids);

    public Armor Armor(IIdGen ids, string name, int def) =>
        ItemBuilder<Armor>.Create(info => new Armor(info, def)).Name(name).Build(ids);

    public Potion Potion(IIdGen ids, string name, int heal) =>
        ItemBuilder<Potion>.Create(info => new Potion(info, heal)).Name(name).Build(ids);

    public QuestItem Quest(IIdGen ids, string name, string key) =>
        ItemBuilder<QuestItem>.Create(info => new QuestItem(info, key))
            .Name(name)
            .WithRarity(Rarity.Rare)
            .Build(ids);
}
