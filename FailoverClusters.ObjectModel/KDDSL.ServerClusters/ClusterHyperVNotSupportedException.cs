using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace KDDSL.ServerClusters;

[Serializable]
public class ClusterHyperVNotSupportedException : ClusterBaseException
{
	private static string errorMessage = Resources.Exception_HyperVNotSupported_Text;

	private static int errorCode = 0;

	private string parameter;

	public override TaskDialogIcon Icon => TaskDialogIcon.Information;

	public override string Header => Resources.Exception_HyperVNotSupported_Header_Text;

	public override string Caption => Resources.TaskDialog_WindowTitle_HyperVNotSupported_Text;

	public ClusterHyperVNotSupportedException(string parameter, Exception inner)
		: base((!(parameter != null)) ? errorMessage : parameter, inner)
	{
		base.HResult = errorCode;
	}

	public ClusterHyperVNotSupportedException(string parameter)
		: base((!(parameter != null)) ? errorMessage : parameter)
	{
		base.HResult = errorCode;
	}

	public ClusterHyperVNotSupportedException()
		: base(errorMessage)
	{
		base.HResult = errorCode;
	}

	protected ClusterHyperVNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
