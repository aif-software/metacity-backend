public interface IIdIndexService
{
    int GetId();
}
public static class IdIndexService
{
    private static int idIndex = 0;
    private static readonly object _lock = new();
    public static int GetId()
    {
        lock (_lock)
        {
            idIndex++;
            return idIndex;
        }
    }
}
