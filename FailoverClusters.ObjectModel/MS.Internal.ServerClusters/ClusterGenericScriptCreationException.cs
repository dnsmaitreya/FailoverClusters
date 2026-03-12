using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace MS.Internal.ServerClusters;

[Serializable]
public class ClusterGenericScriptCreationException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_GenericScriptCreation_Text;

	private static int errorCode = -2147018953;

	private string parameter;

	public override string Header => string.Format(CultureInfo.InvariantCulture, Resources.Exception_GenericScriptCreation_Header_Text, parameter);

	public override string Caption => Resources.TaskDialog_WindowTitle_GenericScriptCreation_Text;

	public ClusterGenericScriptCreationException(string parameter, Exception inner)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter), inner)
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterGenericScriptCreationException(string parameter)
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, parameter))
	{
		this.parameter = parameter;
		base.HResult = errorCode;
	}

	public ClusterGenericScriptCreationException()
		: base(string.Format(Thread.CurrentThread.CurrentCulture, errorMessage, string.Empty))
	{
		parameter = string.Empty;
		base.HResult = errorCode;
	}

	protected ClusterGenericScriptCreationException(SerializationInfo info, StreamingContext context)
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
