namespace Common.Interfaces;

public interface IIdentifiable
{
    public int Id { get; }
    public string Name { get; }
    public int SortId { get; }
}