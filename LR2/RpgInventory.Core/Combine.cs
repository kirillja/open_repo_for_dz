namespace RpgInventory.Core;

public interface ICombineRule
{
    bool CanApply(IItem a, IItem b);
    IItem? Apply(IIdGen ids, IItem a, IItem b);
}

public sealed class PotionMixRule : ICombineRule
{
    public bool CanApply(IItem a, IItem b) => a is Potion && b is Potion;

    public IItem Apply(IIdGen ids, IItem a, IItem b)
    {
        var p1 = (Potion)a; var p2 = (Potion)b;
        return new Potion(new ItemInfo(ids.Next(), "Mixed Potion", Rarity.Rare), p1.Heal + p2.Heal);
    }
}

public sealed class WeaponUpgradeRule : ICombineRule
{
    private readonly IUpgradeState _state;
    public WeaponUpgradeRule(IUpgradeState state) => _state = state;

    public bool CanApply(IItem a, IItem b) => a is Weapon && b is Weapon;

    public IItem Apply(IIdGen ids, IItem a, IItem b)
    {
        var w1 = (Weapon)a; var w2 = (Weapon)b;
        var baseValue = w1.Damage + w2.Damage / 2;
        var dmg = _state.Apply(baseValue);
        return new Weapon(new ItemInfo(ids.Next(), $"Upgraded {w1.Info.Name}", Rarity.Epic), dmg);
    }
}

public sealed class CombineService
{
    private readonly IReadOnlyList<ICombineRule> _rules;
    public CombineService(params ICombineRule[] rules) => _rules = rules;

    public IItem? Combine(IIdGen ids, IItem a, IItem b)
        => _rules.FirstOrDefault(r => r.CanApply(a, b))?.Apply(ids, a, b);
}