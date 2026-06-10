using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterResourceLockedException : ClusterException
{
	private Guid resourceId;

	private string resourceName;

	private bool maintenanceMode;

	public Guid ResourceId
	{
		get
		{
			return resourceId;
		}
		set
		{
			resourceId = value;
		}
	}

	public string ResourceName
	{
		get
		{
			return resourceName;
		}
		set
		{
			resourceName = value;
		}
	}

	public bool MaintenanceMode
	{
		get
		{
			return maintenanceMode;
		}
		set
		{
			maintenanceMode = value;
		}
	}

	public ClusterResourceLockedException()
		: this(Guid.Empty, maintenanceMode: false)
	{
	}

	public ClusterResourceLockedException(Guid resourceId, bool maintenanceMode)
		: base(ExceptionResources.Resource_Locked.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
		MaintenanceMode = maintenanceMode;
	}

	public ClusterResourceLockedException(string resourceName)
		: this(resourceName, maintenanceMode: false)
	{
	}

	public ClusterResourceLockedException(string resourceName, bool maintenanceMode)
		: base(ExceptionResources.Resource_Locked.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
		MaintenanceMode = maintenanceMode;
	}

	public ClusterResourceLockedException(string resourceName, Exception innerException)
		: this(resourceName, maintenanceMode: false, innerException)
	{
	}

	public ClusterResourceLockedException(Guid resourceId, bool maintenanceMode, Exception innerException)
		: base(ExceptionResources.Resource_Locked.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
		MaintenanceMode = maintenanceMode;
	}

	public ClusterResourceLockedException(string resourceName, bool maintenanceMode, Exception innerException)
		: base(ExceptionResources.Resource_Locked.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
		MaintenanceMode = maintenanceMode;
	}

	protected ClusterResourceLockedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		if (info != null)
		{
			info.AddValue("ResourceId", resourceId);
			info.AddValue("ResourceName", resourceName);
		}
	}
}

