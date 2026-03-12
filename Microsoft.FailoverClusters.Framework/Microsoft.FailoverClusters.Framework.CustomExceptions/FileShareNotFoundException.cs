using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class FileShareNotFoundException : UNCPathException
{
	public FileShareNotFoundException(string server, string shareName, string fullPath)
		: base(ExceptionResources.FileShareNotFound.FormatCurrentCulture(server, shareName), fullPath)
	{
	}

	protected FileShareNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
