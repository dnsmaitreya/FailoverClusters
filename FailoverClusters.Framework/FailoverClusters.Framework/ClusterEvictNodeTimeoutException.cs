using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterEvictNodeTimeoutException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterEvictNodeTimeoutException()
		: this(string.Empty, null)
	{
	}

	public ClusterEvictNodeTimeoutException(string name, Exception ex)
		: base(CommonResources.ClusterEvictNodeTimeoutException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterEvictNodeTimeoutException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterEvictNodeTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

