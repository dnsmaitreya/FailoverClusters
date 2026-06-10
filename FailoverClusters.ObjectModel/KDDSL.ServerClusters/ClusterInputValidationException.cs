using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterInputValidationException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_InputValidation_Text;

	private static int errorCode = -2147024809;

	private string parameter;

	public override TaskDialogIcon Icon => TaskDialogIcon.Error;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_InputValidation_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_InputValidation_Text;

	public override int NumericError => errorCode;

	public ClusterInputValidationException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterInputValidationException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterInputValidationException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterInputValidationException(string errMessage, string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	protected ClusterInputValidationException(string errMessage, string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	protected ClusterInputValidationException(SerializationInfo info, StreamingContext context)
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
