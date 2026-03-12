using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterStatusNotReadyException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_StatusNotReady_Text;

	private static int errorCode = -2147023071;

	private string parameter;

	public override TaskDialogIcon Icon => TaskDialogIcon.Information;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_StatusNotReady_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_StatusNotReady_Text;

	public override int NumericError => errorCode;

	public ClusterStatusNotReadyException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterStatusNotReadyException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterStatusNotReadyException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterStatusNotReadyException(SerializationInfo info, StreamingContext context)
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
