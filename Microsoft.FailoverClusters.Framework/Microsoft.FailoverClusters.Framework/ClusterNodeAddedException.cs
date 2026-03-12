using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterNodeAddedException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterNodeAddedException()
		: this(string.Empty)
	{
	}

	public ClusterNodeAddedException(string name)
		: this(name, null)
	{
	}

	public ClusterNodeAddedException(string name, Exception ex)
		: base(ExceptionResources.ClusterNodeAdded_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = ExceptionResources.ClusterNodeAdded_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterNodeAddedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
