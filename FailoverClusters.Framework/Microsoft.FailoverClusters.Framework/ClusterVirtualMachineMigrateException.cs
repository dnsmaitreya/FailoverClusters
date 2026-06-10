using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineMigrateException : ClusterDialogException
{
	public ClusterVirtualMachineMigrateException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineMigrateException(string virtualMachineName)
		: this(null, virtualMachineName, VirtualMachineMigrationType.Quick, null)
	{
	}

	public ClusterVirtualMachineMigrateException(string virtualMachineName, Exception innerException)
		: this(null, virtualMachineName, VirtualMachineMigrationType.Quick, innerException)
	{
	}

	public ClusterVirtualMachineMigrateException(string virtualMachineName, VirtualMachineMigrationType migrationType, Exception innerException)
		: this(null, virtualMachineName, migrationType, innerException)
	{
	}

	public ClusterVirtualMachineMigrateException(string message, string virtualMachineName, VirtualMachineMigrationType migrationType)
		: this(message, virtualMachineName, migrationType, null)
	{
	}

	public ClusterVirtualMachineMigrateException(string message, string virtualMachineName, VirtualMachineMigrationType migrationType, Exception innerException)
		: base(message ?? ExceptionResources.VirtualMachineMigrate_Default, innerException)
	{
		base.Header = ((migrationType == VirtualMachineMigrationType.Live) ? ExceptionResources.VirtualMachineLiveMigrate_Header.FormatCurrentCulture(virtualMachineName) : ExceptionResources.VirtualMachineQuickMigrate_Header.FormatCurrentCulture(virtualMachineName));
	}

	protected ClusterVirtualMachineMigrateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

