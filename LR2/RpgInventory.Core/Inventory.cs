namespace RpgInventory.Core;

public sealed class Inventory
{
    private readonly List<IItem> _items = new();
    public IReadOnlyList<IItem> All => _items;
    public int Count => _items.Count;

    public void Add(IItem item) => _items.Add(item);

    public IItem? Find(long id) => _items.FirstOrDefault(x => x.Info.Id == id);

    public bool Remove(long id)
    {
        var idx = _items.FindIndex(x => x.Info.Id == id);
        if (idx < 0) return false;
        _items.RemoveAt(idx);
        return true;
    }
}