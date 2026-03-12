using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework.CustomExceptions;

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
