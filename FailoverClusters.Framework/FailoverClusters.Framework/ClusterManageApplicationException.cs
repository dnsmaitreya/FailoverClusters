using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterManageApplicationException : ClusterDialogException
{
	public ClusterManageApplicationException()
		: this(string.Empty)
	{
	}

	public ClusterManageApplicationException(string groupName)
		: this(groupName, null)
	{
	}

	public ClusterManageApplicationException(string groupName, Exception innerException)
		: base(ExceptionResources.ApplicationManage_Text, innerException, CommonResources.Text_ConnectionError, TaskDialogStandardIcon.Error, ExceptionResources.ApplicationManage_Header.FormatCurrentCulture(groupName))
	{
	}

	protected ClusterManageApplicationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

