namespace Microsoft.FailoverClusters.Framework;

public class PriorityWrapper
{
	private readonly Priority priority;

	private readonly string priorityValue;

	public Priority Priority => priority;

	public PriorityWrapper(Priority priority)
	{
		this.priority = priority;
	}

	public PriorityWrapper(string priorityValue)
	{
		this.priorityValue = priorityValue;
		priority = Priority.Unknown;
	}

	public override string ToString()
	{
		return priorityValue ?? priority.Translate();
	}
}
