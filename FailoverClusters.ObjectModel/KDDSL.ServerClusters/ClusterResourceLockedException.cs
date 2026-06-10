using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterResourceLockedException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ResourceLocked_Header_Text;

	private static int errorCode = -2147018936;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_ResourceLocked_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_ResourceLocked_Text;

	public override int NumericError => errorCode;

	public ClusterResourceLockedException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		base.HResult = errorCode;
	}

	public ClusterResourceLockedException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		base.HResult = errorCode;
	}

	public ClusterResourceLockedException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		base.HResult = errorCode;
	}

	protected ClusterResourceLockedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
