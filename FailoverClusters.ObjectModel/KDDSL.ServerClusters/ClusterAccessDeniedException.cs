using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterAccessDeniedException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_AccessDenied_Text;

	private static int errorCode = -2147024891;

	private string clusterName;

	public override TaskDialogIcon Icon => TaskDialogIcon.Warning;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_AccessDenied_Header_Text, clusterName);

	public override string Caption => Resources.TaskDialog_WindowTitle_ConnectionError_Text;

	public override int NumericError => errorCode;

	public ClusterAccessDeniedException(string clusterName, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName), inner)
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterAccessDeniedException(string clusterName, string errorMessage)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterAccessDeniedException(string clusterName)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, clusterName))
	{
		this.clusterName = clusterName;
		base.HResult = errorCode;
	}

	public ClusterAccessDeniedException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		clusterName = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterAccessDeniedException(SerializationInfo info, StreamingContext context)
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
