namespace FailoverClusters.Framework;

public interface IKeyQueryable
{
	string Key { get; }
}
public interface IKeyQueryable<in T> : IKeyQueryable
{
	void CopyFrom(T source);
}

