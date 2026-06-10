using System;
using System.Collections.Generic;
using System.ComponentModel;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal abstract class PClusterObject : IComparable, FailoverClusters.Framework.IIdentifiable, IWeakEventContainer, IDisposable
{
	private readonly ClusterLock lockObject;

	private string name;

	private bool isProcessing;

	private ClusterException error;

	private List<Action> executeOnReaderCallbacks;

	private ClusterPropertyCollection properties;

	private WeakEventHandler<ClusterWrapperEventArgs> pipeEvent = WeakEvent.Create<ClusterWrapperEventArgs>();

	public abstract ClusterIdentityType IdentityType { get; }

	public PCluster Cluster { get; private set; }

	public Guid Id { get; set; }

	public string Information { get; private set; }

	public bool IsOpen { get; internal set; }

	public bool IsRemoved { get; internal set; }

	public int LoadedSelection { get; set; }

	public bool? NeedCompactation { get; set; }

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			lockObject.LockObject.Name = "{0}:{1}".FormatCurrentCulture(GetType().Name, name);
		}
	}

	public ClusterPropertyCollection Properties
	{
		get
		{
			return properties ?? (properties = new ClusterPropertyCollection(Cluster.Id, Id, IdentityType));
		}
		set
		{
			if (properties == null)
			{
				properties = new ClusterPropertyCollection(Cluster.Id, Id, IdentityType);
			}
			properties.AddOrUpdate(value);
		}
	}

	public bool IsProcessing
	{
		get
		{
			return isProcessing;
		}
		set
		{
			if (value != isProcessing)
			{
				isProcessing = value;
				pipeEvent(this, new ClusterWrapperEventArgs(EventType.Processing, new ClusterProcessingEventArgs(Id, value)));
			}
		}
	}

	protected List<Action> ExecuteOnReaderCallbacks => executeOnReaderCallbacks;

	public ClusterLock LockObject => lockObject;

	public event EventHandler<ClusterWrapperEventArgs> PipeEvent
	{
		add
		{
			pipeEvent.Add(value);
		}
		remove
		{
			pipeEvent.Remove(value);
		}
	}

	protected void RouteEvent(ClusterWrapperEventArgs e)
	{
		pipeEvent(this, e);
	}

	protected PClusterObject()
	{
		NeedCompactation = false;
		PerformanceCounters.Increment("Private - " + GetType().Name);
		lockObject = new ClusterLock(this)
		{
			Owner = this,
			LockObject = 
			{
				Name = GetType().Name
			}
		};
	}

	protected PClusterObject(PCluster cluster)
		: this()
	{
		Cluster = cluster ?? ((this is PCluster) ? ((PCluster)this) : null);
		_ = Cluster;
	}

	public abstract ClusterObject GetProxyObject();

	public void PipeEventCopyDelegate(PClusterObject sourceObject)
	{
		pipeEvent = sourceObject.pipeEvent;
	}

	public void Dispose()
	{
		if (Cluster != null)
		{
			Cluster.Dispose();
		}
		lockObject.Dispose();
	}

	public abstract void Delete();

	public abstract void Rename(string newName);

	protected virtual void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
	}

	protected virtual void OnInformationChanged(object sender, ClusterInformationEventArgs e)
	{
	}

	protected virtual void OnErrorChanged(object sender, ClusterErrorEventArgs e)
	{
	}

	public virtual void Refresh(bool targeted)
	{
		LoadedSelection = 0;
		if (Properties != null)
		{
			Properties.Clear();
		}
		pipeEvent(this, new ClusterWrapperEventArgs(EventType.Refreshed, new ClusterRefreshedEventArgs(Id, name, targeted)));
	}

	public void SetInformation(string newInformation)
	{
		if (!(Information == newInformation))
		{
			Information = newInformation;
			ClusterInformationEventArgs clusterInformationEventArgs = new ClusterInformationEventArgs(Id, Information);
			OnInformationChanged(this, clusterInformationEventArgs);
			pipeEvent(this, new ClusterWrapperEventArgs(EventType.Information, clusterInformationEventArgs));
		}
	}

	public void SetError(uint errorCode)
	{
		SetError(ClusterLastErrorException.Create(errorCode, name));
	}

	public void SetError(ClusterException exception)
	{
		if (error != exception)
		{
			error = exception;
			ClusterErrorEventArgs clusterErrorEventArgs = new ClusterErrorEventArgs(Id, error);
			OnErrorChanged(this, clusterErrorEventArgs);
			pipeEvent(this, new ClusterWrapperEventArgs(EventType.Error, clusterErrorEventArgs));
		}
	}

	public virtual List<Action> ProcessNotification(Notification notification)
	{
		if (!(this is PCluster))
		{
			IsProcessing = false;
		}
		executeOnReaderCallbacks = new List<Action>();
		ClusterPropertiesEventArgs clusterPropertiesEventArgs = notification.Payload as ClusterPropertiesEventArgs;
		if (clusterPropertiesEventArgs != null)
		{
			if (clusterPropertiesEventArgs.IsVirtual)
			{
				if (clusterPropertiesEventArgs.VirtualPropertyPayloadStatus != 0)
				{
					ClusterLog.LogVerbose(LogSubcategory.FxCache, "Got Private Property Payload from cache for resource {0} : {1}".FormatCurrentCulture(Name, clusterPropertiesEventArgs.VirtualPropertyPayloadStatus));
				}
				else
				{
					Cluster.Server.Resource.FetchVirtualPropertiesPayload(clusterPropertiesEventArgs);
				}
				switch (clusterPropertiesEventArgs.VirtualPropertyPayloadStatus)
				{
				case VirtualPropertyPayloadStatus.Fetching:
					return executeOnReaderCallbacks;
				case VirtualPropertyPayloadStatus.Deleted:
					return executeOnReaderCallbacks;
				}
			}
			if (clusterPropertiesEventArgs.Error == null)
			{
				Properties = clusterPropertiesEventArgs.Properties;
				clusterPropertiesEventArgs.Properties = Properties;
			}
			OnPropertiesChanged(this, clusterPropertiesEventArgs);
			executeOnReaderCallbacks.Add(delegate
			{
				pipeEvent(this, new ClusterWrapperEventArgs(EventType.PropertiesChanged, clusterPropertiesEventArgs));
			});
		}
		ClusterInformationEventArgs clusterInformationEventArgs = notification.Payload as ClusterInformationEventArgs;
		if (clusterInformationEventArgs != null && Information != clusterInformationEventArgs.Information)
		{
			Information = clusterInformationEventArgs.Information;
			OnInformationChanged(this, clusterInformationEventArgs);
			executeOnReaderCallbacks.Add(delegate
			{
				pipeEvent(this, new ClusterWrapperEventArgs(EventType.Information, clusterInformationEventArgs));
			});
		}
		return executeOnReaderCallbacks;
	}

	public abstract ClusterLoadedEventArgs LoadObject(int loadSelection);

	public void SendEventToProxy(ClusterWrapperEventArgs events)
	{
		pipeEvent(this, events);
	}

	internal virtual void OnRemovedFromCache()
	{
	}

	protected void ProtectedScope(Action task, Action<ClusterException> actionAfterComplete, bool resetIsProcessing = true, bool affectsIsProcessing = true)
	{
		ProtectedScope(task, delegate(ClusterException exception)
		{
			actionAfterComplete(exception);
			return exception;
		}, resetIsProcessing, affectsIsProcessing);
	}

	internal bool DisconnectForRpcError(ClusterException ex)
	{
		Exception ex2 = ex;
		bool flag = false;
		while (ex2 != null)
		{
			if (ex2 is Win32Exception ex3 && !(ex3 is ClusterWmiWin32Exception) && (NativeMethods.ErrorCode.RpcServerUnavailable.IsEqual(ex3.NativeErrorCode) || NativeMethods.ErrorCode.RpcCallFailed.IsEqual(ex3.NativeErrorCode) || NativeMethods.ErrorCode.RpcCallFailedDne.IsEqual(ex3.NativeErrorCode)))
			{
				flag = true;
				break;
			}
			ex2 = ex2.InnerException;
		}
		if (flag)
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCoreNotification, "Disconnection on progress because of RPC Error");
			Cluster.SendEventToProxy(new ClusterWrapperEventArgs(EventType.Disconnected, new ClusterDisconnectedEventArgs(Cluster.Id, ex)));
		}
		return flag;
	}

	protected void ProtectedScope(Action task, Func<ClusterException, ClusterException> actionAfterComplete = null, bool resetIsProcessing = true, bool affectsIsProcessing = true)
	{
		Func<object> task2 = delegate
		{
			task();
			return null;
		};
		ProtectedScope(task2, actionAfterComplete, resetIsProcessing, affectsIsProcessing);
	}

	protected T ProtectedScope<T>(Func<T> task, Func<ClusterException, ClusterException> actionAfterComplete = null, bool resetIsProcessing = true, bool affectsIsProcessing = true)
	{
		ClusterException ex = null;
		try
		{
			if (affectsIsProcessing)
			{
				IsProcessing = true;
			}
			return task();
		}
		catch (ClusterException ex2)
		{
			ex = ((actionAfterComplete != null) ? actionAfterComplete(ex2) : ex2);
			DisconnectForRpcError(ex2);
			if (ex == ex2)
			{
				throw;
			}
			throw ex;
		}
		catch (Win32Exception innerException)
		{
			ClusterException ex3 = new ClusterDefaultException(innerException);
			ex = ((actionAfterComplete != null) ? actionAfterComplete(ex3) : ex3);
			DisconnectForRpcError(ex3);
			throw ex;
		}
		finally
		{
			if (ex == null)
			{
				ex = actionAfterComplete?.Invoke(null);
			}
			if (ex != null || (affectsIsProcessing && resetIsProcessing))
			{
				IsProcessing = false;
			}
		}
	}

	public virtual void Compact()
	{
		WeakEvent.Compact(pipeEvent);
	}

	public virtual int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (obj.GetType() != GetType())
		{
			throw new NotSupportedException(ExceptionResources.ObjectTypeACannotCompareTypeB.FormatCurrentCulture(GetType().ToString(), obj.GetType().ToString()));
		}
		if (this == obj)
		{
			return 0;
		}
		if (Name == null)
		{
			return -1;
		}
		PClusterObject pClusterObject = (PClusterObject)obj;
		if (pClusterObject.Name == null)
		{
			return 1;
		}
		return NativeMethods.StrCmpLogicalW(Name, pClusterObject.Name);
	}

	internal void SaveProperties()
	{
		ProtectedScope(delegate
		{
			Cluster.Server.SaveProperties(this, Properties);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterSavePropertiesException(Name, ex);
			}
			return ex;
		});
	}

	internal void SaveProperties(ClusterPropertyCollection propertyCollection)
	{
		ProtectedScope(delegate
		{
			Cluster.Server.SaveProperties(this, propertyCollection);
		}, delegate(ClusterException ex)
		{
			if (ex != null && !(ex is ClusterResourcePropertyStoredException))
			{
				ex = new ClusterSavePropertiesException(Name, ex);
			}
			return ex;
		});
	}

	protected void ReleaseExecuteAndReacquire(Action action)
	{
		bool flag = LockObject.IsReaderLockHeld();
		if (flag)
		{
			LockObject.UnlockReader();
		}
		try
		{
			action();
		}
		finally
		{
			if (flag)
			{
				LockObject.Reader();
			}
		}
	}

	protected T ReleaseExecuteAndReacquire<T>(Func<T> action)
	{
		bool flag = LockObject.IsReaderLockHeld();
		if (flag)
		{
			LockObject.UnlockReader();
		}
		try
		{
			return action();
		}
		finally
		{
			if (flag)
			{
				LockObject.Reader();
			}
		}
	}

	public abstract void TransferFrom(PClusterObject privateClusterObject, bool cacheIsLocked, int loadSelection);

	public abstract void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false);

	protected void LockTransferAndBroadCast(PClusterObject source, int loadSelection, Action transferAction)
	{
		if (this != source && loadSelection != 0 && LoadedSelection != loadSelection)
		{
			LockObject.Writer();
			try
			{
				transferAction();
			}
			finally
			{
				LockObject.UnlockWriter();
			}
			BroadcastChanges(loadSelection);
		}
	}
}

