using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterReplicationFeatureNotInstalledException : ClusterDialogException
{
	public ClusterReplicationFeatureNotInstalledException()
		: this(string.Empty, null)
	{
	}

	public ClusterReplicationFeatureNotInstalledException(string message)
		: this(message, null)
	{
	}

	public ClusterReplicationFeatureNotInstalledException(string message, Exception ex)
		: base(ExceptionResources.ClusterReplicationFeatureNotInstalledException_Text.FormatCurrentCulture(message), ex)
	{
		base.Header = ExceptionResources.ClusterReplicationFeatureNotInstalledException_Header;
		base.Caption = ExceptionResources.ClusterReplicationFeatureNotInstalledException_Title;
		base.Icon = TaskDialogStandardIcon.Error;
	}

	protected ClusterReplicationFeatureNotInstalledException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
