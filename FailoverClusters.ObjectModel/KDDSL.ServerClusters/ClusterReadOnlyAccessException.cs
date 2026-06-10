using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterReadOnlyAccessException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ReadOnlyAccess_Text;

	private static int errorCode = -2147024891;

	private string clusterName;

	public override string Footer => Resources.Exception_ReadOnlyAccess_Footer_Text;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_ReadOnlyAccess_Header_Text, clusterName);

	public override string Caption => Resources.TaskDialog_WindowTitle_ConnectionError_Text;

	public override int NumericError => errorCode;

	public ClusterReadOnlyAccessException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterReadOnlyAccessException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterReadOnlyAccessException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		clusterName = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterReadOnlyAccessException(SerializationInfo info, StreamingContext context)
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
