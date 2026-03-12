using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterManagerNotFoundException : ClusterDialogException
{
	public ClusterManagerNotFoundException()
		: this(ExceptionResources.CouldNotFindManageToolGeneric_Default)
	{
	}

	public ClusterManagerNotFoundException(string message)
		: this(message, null)
	{
	}

	public ClusterManagerNotFoundException(GroupType groupType)
		: this(ExceptionResources.CouldNotFindManageTool_Default.FormatCurrentCulture(groupType.Translate()), null, ExceptionResources.CouldNotFindManageTool_Header.FormatCurrentCulture(groupType.Translate()))
	{
	}

	public ClusterManagerNotFoundException(string message, Exception innerException)
		: this(message, innerException, string.Empty)
	{
	}

	public ClusterManagerNotFoundException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_ToolNotFound, TaskDialogStandardIcon.ShieldWarningBackground, header)
	{
	}

	protected ClusterManagerNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
