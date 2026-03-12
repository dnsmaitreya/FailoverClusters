using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterEvictNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public int CleanupStatus { get; set; }

	public ClusterEvictNodeException()
		: base(CommonResources.ClusterEvictNodeException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterEvictNodeException(string name, Exception ex, int cleanUpStatus)
		: base(CommonResources.ClusterEvictNodeException_Default.FormatCurrentCulture(name, cleanUpStatus), ex)
	{
		base.Header = CommonResources.ClusterEvictNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		CleanupStatus = cleanUpStatus;
		Name = name;
	}

	protected ClusterEvictNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
