using RpgInventory.Core;

namespace RpgInventory.Tests;

public sealed class InventoryTests
{
    [Fact]
    public void Inventory_AddFindRemove_Works()
    {
        var ids = new AtomicIdGen();
        var factory = new SimpleItemFactory();
        var inv = new Inventory();

        var w = factory.Weapon(ids, "Sword", 10);
        inv.Add(w);

        Assert.Equal(1, inv.Count);
        Assert.NotNull(inv.Find(w.Info.Id));

        Assert.True(inv.Remove(w.Info.Id));
        Assert.Equal(0, inv.Count);
        Assert.Null(inv.Find(w.Info.Id));
    }

    [Fact]
    public void Use_Potion_Heals()
    {
        var ids = new AtomicIdGen();
        var factory = new SimpleItemFactory();
        var player = new Player();

        var hp0 = player.Stats.Hp;
        var p = factory.Potion(ids, "Heal", 25);

        var use = new UseService(new DrinkPotion(), new EquipGear());
        use.Use(player, p);

        Assert.Equal(hp0 + 25, player.Stats.Hp);
    }

    [Fact]
    public void Use_Equip_ChangesStats()
    {
        var ids = new AtomicIdGen();
        var factory = new SimpleItemFactory();
        var player = new Player();

        var w = factory.Weapon(ids, "Sword", 10);
        var a = factory.Armor(ids, "Chain", 7);

        var use = new UseService(new DrinkPotion(), new EquipGear());
        use.Use(player, w);
        use.Use(player, a);

        Assert.NotNull(player.Equipped(Slot.Weapon));
        Assert.NotNull(player.Equipped(Slot.Armor));
        Assert.Equal(10, player.Stats.Attack);
        Assert.Equal(7, player.Stats.Defense);
    }

    [Fact]
    public void Combine_Potions_SumsHeal()
    {
        var ids = new AtomicIdGen();
        var factory = new SimpleItemFactory();

        var p1 = factory.Potion(ids, "P1", 10);
        var p2 = factory.Potion(ids, "P2", 20);

        var combine = new CombineService(new PotionMixRule(), new WeaponUpgradeRule(new UpgradedState()));
        var mixed = combine.Combine(ids, p1, p2);

        Assert.NotNull(mixed);
        Assert.IsType<Potion>(mixed);
        Assert.Equal(30, ((Potion)mixed!).Heal);
    }

    [Fact]
    public void Combine_Weapons_UsesState_Legendary()
    {
        var ids = new AtomicIdGen();
        var factory = new SimpleItemFactory();

        var w1 = factory.Weapon(ids, "Sword", 10);
        var w2 = factory.Weapon(ids, "Dagger", 6);

        var combine = new CombineService(new PotionMixRule(), new WeaponUpgradeRule(new LegendaryState()));
        var up = combine.Combine(ids, w1, w2);

        Assert.NotNull(up);
        Assert.IsType<Weapon>(up);

        // base = 10 + 6/2 = 13, legendary => 26
        Assert.Equal(26, ((Weapon)up!).Damage);
    }
}