using Common.Interfaces;

namespace Common.Comparer;

public class IdentifiableComparer : IComparer<IIdentifiable>
{
#pragma warning disable CS8767
    public int Compare(IIdentifiable x, IIdentifiable y)
#pragma warning restore CS8767
    {
        return x.SortId == y.SortId
            ? 0 // same sortId, equal
            : x.SortId > y.SortId
                ? 1 // sortId of x is bigger, x is greater than y
                : -1; // sortId of y is bigger, x is less than y
    }
}