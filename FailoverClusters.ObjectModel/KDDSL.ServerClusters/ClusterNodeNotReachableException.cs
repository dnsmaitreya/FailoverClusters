using System;
using System.Runtime.Serialization;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterNodeNotReachableException : ClusterInputValidationException
{
	private static string errorMessage = Resources.Exception_NodeNotReachable_Text;

	public ClusterNodeNotReachableException(string parameter, Exception inner)
		: base(errorMessage, parameter, inner)
	{
	}

	public ClusterNodeNotReachableException(string parameter)
		: base(errorMessage, parameter)
	{
	}

	public ClusterNodeNotReachableException()
		: base(errorMessage)
	{
	}

	protected ClusterNodeNotReachableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
