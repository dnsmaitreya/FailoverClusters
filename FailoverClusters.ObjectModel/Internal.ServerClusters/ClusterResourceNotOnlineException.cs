using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterResourceNotOnlineException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ResourceNotOnline_Text;

	private static int errorCode = -2147019892;

	public override string Header
	{
		get
		{
			object[] args = new object[0];
			return string.Format(CultureInfo.InvariantCulture, Resources.Exception_ResourceNotOnline_Header_Text, args);
		}
	}

	public override string Caption => Resources.TaskDialog_WindowTitle_ResourceNotOnline_Text;

	public override int NumericError => errorCode;

	public ClusterResourceNotOnlineException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		base.HResult = errorCode;
	}

	public ClusterResourceNotOnlineException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		base.HResult = errorCode;
	}

	public ClusterResourceNotOnlineException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		base.HResult = errorCode;
	}

	protected ClusterResourceNotOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
