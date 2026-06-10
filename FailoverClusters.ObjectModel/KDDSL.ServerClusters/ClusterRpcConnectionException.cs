using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterRpcConnectionException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_RpcConnection_Text;

	private static int errorCode = -2147023174;

	private string clusterName;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_RpcConnection_Header_Text, clusterName);

	public override string Caption => Resources.TaskDialog_WindowTitle_ConnectionError_Text;

	public override int NumericError => errorCode;

	public ClusterRpcConnectionException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterRpcConnectionException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterRpcConnectionException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		clusterName = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterRpcConnectionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		clusterName = info.GetString("ClusterName");
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("ClusterName", clusterName);
	}
}
