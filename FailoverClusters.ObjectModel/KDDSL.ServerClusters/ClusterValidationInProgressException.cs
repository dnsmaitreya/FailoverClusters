using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterValidationInProgressException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_ValidationInProgress_Text;

	private static int errorCode = 0;

	private string parameter;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_ValidationInProgress_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_ValidationInProgress_Text;

	public ClusterValidationInProgressException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterValidationInProgressException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterValidationInProgressException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterValidationInProgressException(SerializationInfo info, StreamingContext context)
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
