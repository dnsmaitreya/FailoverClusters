using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterServerUnavailableException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterServerUnavailableException()
		: base(CommonResources.ClusterServerUnavailableException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterServerUnavailableException(string name, Exception ex)
		: base(CommonResources.ClusterServerUnavailableException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterServerUnavailableException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterServerUnavailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

