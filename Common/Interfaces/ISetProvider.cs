namespace Common.Interfaces;

public interface ISetProvider<T>
{
    public static abstract SortedSet<T> CreateSet();
}