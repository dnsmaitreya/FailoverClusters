using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework.CustomExceptions;

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

