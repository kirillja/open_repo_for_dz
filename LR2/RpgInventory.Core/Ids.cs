namespace RpgInventory.Core;

public interface IIdGen { long Next(); }

public sealed class AtomicIdGen : IIdGen
{
    private long _v = 0;
    public long Next() => System.Threading.Interlocked.Increment(ref _v);
}