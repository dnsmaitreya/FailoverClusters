using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class SmbShareGrantAccessException : Exception
{
	public SmbShareGrantAccessException(string user, string path, Exception e)
		: base(ExceptionResources.SmbShareGrantAccess.FormatCurrentCulture(user, path), e)
	{
	}

	public SmbShareGrantAccessException(string message, Exception e)
		: base(message, e)
	{
	}

	protected SmbShareGrantAccessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
