using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class UNCLocalPathNotFoundException : UNCPathException
{
	public UNCLocalPathNotFoundException(string uncPath, string server)
		: base(ExceptionResources.UNCLocalPathNotFound.FormatCurrentCulture(uncPath, server), uncPath)
	{
	}

	protected UNCLocalPathNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
