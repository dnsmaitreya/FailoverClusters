using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework.CustomExceptions;

[Serializable]
internal class ShareNotAFileSystemShareException : UNCPathException
{
	public ShareNotAFileSystemShareException(string server, string shareName, string fullPath)
		: base(ExceptionResources.ShareNotAFileSystemShare.FormatCurrentCulture(server, shareName), fullPath)
	{
	}

	protected ShareNotAFileSystemShareException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

