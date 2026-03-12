namespace Microsoft.FailoverClusters.Framework;

public class OperationResult
{
	public ClusterObject Sender { get; private set; }

	public ClusterException Error { get; private set; }

	public OperationResult(ClusterObject sender, ClusterException error)
	{
		Sender = sender;
		Error = error;
	}
}
public class OperationResult<T> : OperationResult
{
	public T Result { get; private set; }

	public object Parameter { get; private set; }

	public OperationResult(ClusterObject sender, T resultObject, ClusterException error)
		: this(sender, resultObject, error, (object)null)
	{
	}

	public OperationResult(ClusterObject sender, T resultObject, ClusterException error, object parameter)
		: base(sender, error)
	{
		Result = resultObject;
		Parameter = parameter;
	}
}
