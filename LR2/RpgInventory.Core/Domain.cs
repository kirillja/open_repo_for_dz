namespace RpgInventory.Core;

public enum ItemType { Weapon, Armor, Potion, Quest }
public enum Rarity { Common, Rare, Epic, Legendary }
public enum Slot { Weapon, Armor }

public readonly record struct ItemInfo(long Id, string Name, Rarity Rarity);

public interface IItem
{
    ItemType Type { get; }
    ItemInfo Info { get; }
}

public sealed record Weapon(ItemInfo Info, int Damage) : IItem { public ItemType Type => ItemType.Weapon; }
public sealed record Armor(ItemInfo Info, int Defense) : IItem { public ItemType Type => ItemType.Armor; }
public sealed record Potion(ItemInfo Info, int Heal) : IItem { public ItemType Type => ItemType.Potion; }
public sealed record QuestItem(ItemInfo Info, string Key) : IItem { public ItemType Type => ItemType.Quest; }