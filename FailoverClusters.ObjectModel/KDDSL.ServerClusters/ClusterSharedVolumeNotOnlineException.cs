using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterSharedVolumeNotOnlineException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_SharedVolumeNotOnline_Text;

	private static int errorCode = -2147019858;

	private string parameter;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_SharedVolumeNotOnline_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_SharedVolumeNotOnline_Text;

	public ClusterSharedVolumeNotOnlineException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterSharedVolumeNotOnlineException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterSharedVolumeNotOnlineException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterSharedVolumeNotOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		parameter = info.GetString("Parameter");
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("Parameter", parameter);
	}
}
