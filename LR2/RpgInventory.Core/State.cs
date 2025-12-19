namespace RpgInventory.Core;

public interface IUpgradeState { int Apply(int value); string Name { get; } }

public sealed class NormalState : IUpgradeState { public int Apply(int v) => v; public string Name => "Normal"; }
public sealed class UpgradedState : IUpgradeState { public int Apply(int v) => v + v / 2; public string Name => "Upgraded"; }
public sealed class LegendaryState : IUpgradeState { public int Apply(int v) => v * 2; public string Name => "Legendary"; }