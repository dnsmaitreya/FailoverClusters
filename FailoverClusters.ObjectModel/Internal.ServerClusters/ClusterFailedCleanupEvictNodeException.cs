using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public sealed class ClusterFailedCleanupEvictNodeException : ClusterNodeException
{
	private static string errorMessage = Resources.Exception_FailedCleanupEvictNode_Text;

	private static int errorCode = -2147019000;

	public override TaskDialogIcon Icon => TaskDialogIcon.Warning;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_FailedCleanupEvictNode_Header_Text, args);
		}
	}

	public override string Caption => Resources.Warning_Text;

	public override int NumericError => errorCode;

	public ClusterFailedCleanupEvictNodeException(string node, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, node), inner)
	{
		base.HResult = errorCode;
	}

	public ClusterFailedCleanupEvictNodeException(string node)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, node))
	{
		base.HResult = errorCode;
	}

	public ClusterFailedCleanupEvictNodeException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		base.HResult = errorCode;
	}

	private ClusterFailedCleanupEvictNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
