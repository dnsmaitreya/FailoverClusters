using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class InvalidFileSharePathException : UNCPathException
{
	public InvalidFileSharePathException(string share)
		: base(ExceptionResources.InvalidFileSharePath.FormatCurrentCulture(share), share)
	{
	}

	protected InvalidFileSharePathException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

