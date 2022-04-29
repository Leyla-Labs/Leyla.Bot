namespace Db.Statics.BaseClasses;

public class StaticClass<T> where T : StaticField
{
    private readonly List<T> _list;

    protected StaticClass(List<T> list)
    {
        _list = list;
    }

    public List<T> Get()
    {
        return _list.OrderBy(x => x.SortId).ToList();
    }

    public List<T> Get(IEnumerable<int> ids)
    {
        return _list.Where(x => ids.Contains(x.Id)).OrderBy(x => x.SortId).ToList();
    }

    public T Get(int id)
    {
        return _list.First(x => x.Id == id);
    }

    public T Get(string name)
    {
        return _list.First(x => x.Name.Equals(name));
    }
}