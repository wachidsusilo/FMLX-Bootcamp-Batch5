using System.Collections;

namespace Project6.FooBar;

public class FooBar : IEnumerable
{
    private readonly Dictionary<int, string> _specifications;
    private int _count;

    public FooBar(int count = 15)
    {
        _count = count;
        _specifications = new Dictionary<int, string>();
    }

    public FooBar(int count, Dictionary<int, string> specs)
    {
        _count = count;
        _specifications = specs;
    }

    public void SetCount(int count)
    {
        _count = count;
    }

    public void Add(int key, string value)
    {
        _specifications.Add(key, value);
    }

    public bool Remove(int key)
    {
        return _specifications.Remove(key);
    }

    public bool Update(int key, string newValue)
    {
        if (!_specifications.ContainsKey(key))
        {
            return false;
        }
        
        _specifications[key] = newValue;
        return true;

    }

    public bool Contains(int key)
    {
        return _specifications.ContainsKey(key);
    }

    public KeyValuePair<int, string>? Get(int key)
    {
        return _specifications.TryGetValue(key, out var value)
            ? new KeyValuePair<int, string>(key, value)
            : null;
    }

    public Dictionary<int, string> GetSpecs()
    {
        return _specifications;
    }

    public string Execute()
    {
        var results = new List<string> { "0" };

        for (var i = 1; i <= _count; i++)
        {
            var result = _specifications
                .Where(p => i % p.Key == 0)
                .Aggregate("", (current, p) => current + p.Value);

            results.Add(result.Length > 0 ? result : i.ToString());
        }

        return string.Join(", ", results);
    }

    public IEnumerator GetEnumerator()
    {
        return _specifications.GetEnumerator();
    }
}