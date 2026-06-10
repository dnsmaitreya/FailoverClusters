using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterResourcePropertyStoredException : ClusterDialogException
{
	public string ResourceName { get; set; }

	public ClusterResourcePropertyStoredException()
		: this(string.Empty)
	{
	}

	public ClusterResourcePropertyStoredException(string resourceName)
		: base(ExceptionResources.ResourcePropertyStored_Default.FormatCurrentCulture(resourceName))
	{
		base.Caption = ExceptionResources.ResourcePropertyStored_Caption;
		base.Header = ExceptionResources.ResourcePropertyStored_Header;
		base.Icon = TaskDialogStandardIcon.Question;
		base.Buttons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
		ResourceName = resourceName;
	}

	public ClusterResourcePropertyStoredException(string resourceName, Exception innerException)
		: base(ExceptionResources.ResourcePropertyStored_Default.FormatCurrentCulture(resourceName), innerException)
	{
		base.Caption = ExceptionResources.ResourcePropertyStored_Caption;
		base.Header = ExceptionResources.ResourcePropertyStored_Header;
		base.Buttons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
		ResourceName = resourceName;
	}

	protected ClusterResourcePropertyStoredException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

