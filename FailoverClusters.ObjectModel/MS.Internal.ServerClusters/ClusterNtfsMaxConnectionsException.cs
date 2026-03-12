using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterNtfsMaxConnectionsException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ClusterNtfsMaxConnections_Text;

	private static int errorCode = -2147023546;

	private string sharePath;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_ClusterNtfsMaxConnections_Header_Text, sharePath);

	public override string Caption => Resources.TaskDialog_WindowTitle_FolderError_Text;

	public override int NumericError => errorCode;

	public ClusterNtfsMaxConnectionsException(string sharePath, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, sharePath), inner)
	{
		this.sharePath = sharePath;
		base.HResult = errorCode;
	}

	public ClusterNtfsMaxConnectionsException(string sharePath)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, sharePath))
	{
		this.sharePath = sharePath;
		base.HResult = errorCode;
	}

	public ClusterNtfsMaxConnectionsException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		sharePath = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterNtfsMaxConnectionsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		sharePath = info.GetString("SharePath");
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("SharePath", sharePath);
	}
}
