using System;
using System.Runtime.Serialization;

namespace FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class UNCPathException : Exception
{
	private readonly string path;

	public UNCPathException(string message, string path)
		: base(message)
	{
		this.path = path;
	}

	public UNCPathException(string message, string path, Exception innerException)
		: base(message, innerException)
	{
		this.path = path;
	}

	protected UNCPathException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

