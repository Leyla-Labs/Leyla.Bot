namespace Common.Statics.BaseClasses;

public class StaticClass<T> where T : StaticField
{
    private readonly List<T> _list;

    protected StaticClass(List<T> list)
    {
        _list = list;
    }

    public IOrderedEnumerable<T> Get()
    {
        return _list.OrderBy(x => x.SortId);
    }

    public IOrderedEnumerable<T> Get(IEnumerable<int> ids)
    {
        return _list.Where(x => ids.Contains(x.Id)).OrderBy(x => x.SortId);
    }

    public IOrderedEnumerable<T> Get(IEnumerable<ulong> ids)
    {
        return _list.Where(x => ids.Select(Convert.ToInt32).Contains(x.Id)).OrderBy(x => x.SortId);
    }

    public T Get(int id)
    {
        return _list.First(x => x.Id == id);
    }

    public T Get(ulong id)
    {
        return _list.First(x => x.Id == Convert.ToInt32(id));
    }

    public T Get(string name)
    {
        return _list.First(x => x.Name.Equals(name));
    }
}