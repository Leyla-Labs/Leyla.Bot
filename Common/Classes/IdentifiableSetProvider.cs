using Common.Interfaces;

namespace Common.Classes;

public abstract class IdentifiableSetProvider<T> where T : IIdentifiable
{
    protected IdentifiableSetProvider(SortedSet<T> set)
    {
        CheckSet(set);
        Set = set;
    }

    protected SortedSet<T> Set { get; }

    public SortedSet<T> Get()
    {
        return Set;
    }

    public T Get(int id)
    {
        return Set.First(x => x.Id == id);
    }

    public T Get(string name)
    {
        return Set.First(x => x.Name == name);
    }

    private static void CheckSet(IReadOnlyCollection<T> set)
    {
        // check if any pk is missing

        var i = set.First().Id;

        foreach (var element in set.OrderBy(x => x.Id).Skip(1))
        {
            if (element.Id > i + 1)
            {
                throw new InvalidDataException($"Id skips a value: {i + 1}");
            }

            i++;
        }

        // check if any pk duplicated

        if (set.GroupBy(x => x.Id).FirstOrDefault(x => x.Count() > 1) is { } result)
        {
            throw new InvalidDataException($"Id value is duplicated in list: {result.First().Id}");
        }
    }
}