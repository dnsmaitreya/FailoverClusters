using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterCleanupNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterCleanupNodeException()
		: this(string.Empty, null)
	{
	}

	public ClusterCleanupNodeException(string name, Exception ex)
		: base(CommonResources.ClusterCleanupNodeException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterCleanupNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Warning;
		base.Icon = TaskDialogStandardIcon.Warning;
		Name = name;
	}

	protected ClusterCleanupNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

