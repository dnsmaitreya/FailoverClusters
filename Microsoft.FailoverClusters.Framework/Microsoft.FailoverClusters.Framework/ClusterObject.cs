using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public abstract class ClusterObject : IComparable, IIdentifiable, INotifyPropertyChanged, IClusterObject, IDataItem
{
	private WeakReferenceEx miscellaneousCommandsWeak;

	private WeakReferenceEx showErrorCommandWeak;

	protected WeakReferenceEx<CommandCollection> mCommands;

	protected object mObjectLock = new object();

	private const int MaxEnqueueThrottleRequest = 20;

	private const int MaxRequestsTimeOut = 7200000;

	private Guid memberId;

	private string memberName;

	private ClusterPropertyCollection memberProperties;

	private string information;

	private ClusterException error;

	private bool isProcessing;

	private static HashSet<int> asyncToSyncOperations;

	private static readonly object AsyncToSyncOperationsLockObject;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreGroupDictionary;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreResourceDictionary;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreNodeDictionary;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreNetworkDictionary;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreNetworkInterfaceDictionary;

	private static readonly Dictionary<string, PropertyStoreAttribute> StoreResourceTypeDictionary;

	public IEnumerable<ICommand> Commands => WeakReferenceEx.ReturnInstance(ref mCommands, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.Cluster);
		try
		{
			InitializeCommands(commandCollection);
			return commandCollection;
		}
		catch (ClusterException exception)
		{
			ClusterLog.LogException(exception, "There was an error creating commands");
			return new CommandCollection(ClusterCommandCollectionId.Cluster);
		}
	});

	public virtual CommandCollection MiscellaneousCommands => WeakReferenceEx.ReturnInstance(ref miscellaneousCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.GroupState);
		InitializeMiscellaneousCommands(commandCollection);
		return commandCollection;
	});

	[DefaultValue(OperationType.Async)]
	public OperationType PropertiesOperationType { get; set; }

	public abstract ClusterIdentityType IdentityType { get; }

	public bool IsOpen { get; internal set; }

	public bool IsRemoved { get; internal set; }

	public string Status { get; internal set; }

	public virtual string Information
	{
		get
		{
			return information;
		}
		set
		{
			if (!(information == value))
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					Error = null;
				}
				information = value;
				UIHelper.ExecuteOnDispatcher(delegate
				{
					OnPropertyChanged("Information");
					this.InformationChanged.SafeCall(this, new ClusterInformationEventArgs(memberId, value));
				}, OperationType.Async);
			}
		}
	}

	public virtual ClusterException Error
	{
		get
		{
			return error;
		}
		set
		{
			if (error != value)
			{
				if (value != null)
				{
					Information = null;
				}
				error = value;
				UIHelper.ExecuteOnDispatcher(delegate
				{
					OnPropertyChanged("Error");
					this.ErrorOccurred.SafeCall(this, new ClusterErrorEventArgs(memberId, value));
				}, OperationType.Async);
			}
		}
	}

	public bool IsProcessing
	{
		get
		{
			return isProcessing;
		}
		private set
		{
			if (isProcessing)
			{
				Error = null;
			}
			isProcessing = value;
		}
	}

	public virtual Icon2 Icon => null;

	public int LoadSelection { get; internal set; }

	public Cluster Cluster { get; protected set; }

	[Column(Name = "Id", AutoSync = AutoSync.Always)]
	public virtual Guid Id
	{
		get
		{
			return memberId;
		}
		internal set
		{
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				lockObject.Owner.Id = value;
				memberId = value;
			}, LockAccess.Writer);
			OnPropertyChanged("Id");
		}
	}

	[Column(Name = "Name", AutoSync = AutoSync.Always)]
	public string Name
	{
		get
		{
			return memberName;
		}
		set
		{
			if (Name != value)
			{
				RenameInternal(value, PropertiesOperationType);
			}
		}
	}

	public virtual string DisplayName => Name;

	public virtual ClusterPropertyCollection Properties
	{
		get
		{
			return memberProperties ?? (memberProperties = new ClusterPropertyCollection(Cluster.Id, Id, IdentityType));
		}
		internal set
		{
			memberProperties = value;
			OnPropertiesReplaced();
		}
	}

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreGroupDictionary => StoreGroupDictionary;

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreResourceDictionary => StoreResourceDictionary;

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreNodeDictionary => StoreNodeDictionary;

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreNetworkDictionary => StoreNetworkDictionary;

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreNetworkInterfaceDictionary => StoreNetworkInterfaceDictionary;

	protected static Dictionary<string, PropertyStoreAttribute> BackStoreResourceTypeDictionary => StoreResourceTypeDictionary;

	protected object LockObject { get; private set; }

	internal bool BoundToPrivate { get; set; }

	private bool QueueInLoaderIfNull { get; set; }

	internal static bool ExecuteSynchronous
	{
		get
		{
			if (asyncToSyncOperations == null)
			{
				return false;
			}
			lock (AsyncToSyncOperationsLockObject)
			{
				if (asyncToSyncOperations == null)
				{
					return false;
				}
				return asyncToSyncOperations.Contains(Thread.CurrentThread.ManagedThreadId);
			}
		}
	}

	internal abstract Type OwnerType { get; }

	public event EventHandler<ClusterInformationEventArgs> InformationChanged;

	public event EventHandler<ClusterProcessingEventArgs> Processing;

	public event PropertyChangedEventHandler PropertyChanged;

	public event EventHandler<ClusterErrorEventArgs> ErrorOccurred;

	public event EventHandler<ClusterRenamedEventArgs> Renamed;

	public event EventHandler<ClusterIdChangedEventArgs> IdChanged;

	public event EventHandler<ClusterOpenEventArgs> Opened;

	public event EventHandler<ClusterRemovedEventArgs> Removed;

	public event EventHandler<ClusterLoadedEventArgs> Loaded;

	public event EventHandler<ClusterAddedEventArgs> Created;

	public event EventHandler<ClusterPropertiesEventArgs> PropertiesChanged;

	protected virtual void InitializeCommands(CommandCollection collection)
	{
	}

	protected virtual void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
	}

	protected virtual void InitializeMiscellaneousCommands(CommandCollection commandsCollection)
	{
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref showErrorCommandWeak, () => new ClusterCommand(this, "ShowLastError", ClusterCommandId.DisplayClusterError, ClusterCommandCollectionId.Cluster)
		{
			Text = CommandResources.ShowLastError,
			IgnoreIsProcessing = true,
			CanExecuteDelegate = (object x) => Error != null,
			ExecuteDelegate = delegate
			{
				if (Error != null)
				{
					if (Error is IRedirectToCriticalEvents redirectToCriticalEvents && redirectToCriticalEvents.CriticalEventsParameter != null)
					{
						FindCommand(ClusterCommandId.ShowCriticalEvent)?.Execute(redirectToCriticalEvents.CriticalEventsParameter);
					}
					else
					{
						ClusterDialogException.ShowTaskDialog(Error);
					}
				}
			}
		});
		ErrorOccurred += ErrorCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			ErrorOccurred -= ErrorCommandUpdate;
			showErrorCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand);
		ClusterCommand item = new ClusterCommand(this, "ShowCriticalEvent", ClusterCommandId.ShowCriticalEvent, ClusterCommandCollectionId.Cluster)
		{
			Text = CommandResources.ShowCriticalEvents,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the Show Critical Events.");
			},
			CommandParameter = this
		};
		commandsCollection.Add(item);
	}

	private void ErrorCommandUpdate(object sender, ClusterErrorEventArgs e)
	{
		WeakReferenceEx weakReferenceEx = showErrorCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	protected ClusterCommand FindCommand(ClusterCommandId commandId)
	{
		return FindInContainer(Commands, commandId);
	}

	private ClusterCommand FindInContainer(IEnumerable<ICommand> container, ClusterCommandId commandId)
	{
		foreach (ClusterCommand item in container)
		{
			if (item.Id == commandId)
			{
				return item;
			}
			if (item is ClusterCommandContainer clusterCommandContainer)
			{
				ClusterCommand clusterCommand2 = FindInContainer(clusterCommandContainer.ChildrenInternal, commandId);
				if (clusterCommand2 != null)
				{
					return clusterCommand2;
				}
			}
		}
		return null;
	}

	internal void RefreshCommands()
	{
		OnCommandCollection(delegate(CommandCollection commandCollection)
		{
			commandCollection.SignalRefresh();
		});
	}

	protected void OnCommandCollection(Action<CommandCollection> commandCollection)
	{
		Action action = delegate
		{
			WeakReferenceEx<CommandCollection> weakReferenceEx = mCommands;
			if (weakReferenceEx != null)
			{
				CommandCollection target = weakReferenceEx.Target;
				if (target != null)
				{
					commandCollection(target);
				}
			}
		};
		if (Thread.CurrentThread != Global.DefaultDispatcher.Thread)
		{
			Global.DefaultDispatcher.BeginInvoke(action);
		}
		else
		{
			action();
		}
	}

	protected virtual void OnPropertiesReplaced()
	{
	}

	public bool WaitFor(int millisecondsTimeout, Func<bool> condition)
	{
		if (condition == null)
		{
			return true;
		}
		if (millisecondsTimeout < 0)
		{
			throw new ArgumentException(ExceptionResources.InvalidParameter, "millisecondsTimeout");
		}
		DateTime dateTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, millisecondsTimeout));
		while (DateTime.Now < dateTime)
		{
			if (condition())
			{
				return true;
			}
			Thread.Sleep(100);
		}
		return false;
	}

	public void Refresh(bool targeted)
	{
		Refresh(targeted, delegate(OperationResult refreshOperation)
		{
			Error = refreshOperation.Error;
		});
	}

	public void Refresh(bool targeted, Action<OperationResult> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Refresh(targeted);
		}, operationResult, LockAccess.Reader, setErrorOnObject: false);
	}

	public void SetError(ClusterException exception)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.SetError(exception);
		}, null, LockAccess.Reader, setErrorOnObject: false);
	}

	public void RedirectAsyncOutput(Action asyncActions, Action<OperationResult> asyncResult)
	{
		Worker.Start(delegate
		{
			SendError(Id, null, OperationType.Sync);
			RegisterAsyncToSyncOperations();
			try
			{
				asyncActions();
			}
			finally
			{
				UnregisterAsyncToSyncOperations();
			}
			asyncResult.SafeCall(new OperationResult(this, null));
		}, delegate(ClusterException exception)
		{
			asyncResult.SafeCall(new OperationResult(this, exception));
		});
	}

	public abstract void Delete(bool askConfirmation = false);

	public abstract void Delete(Action<OperationResult> operationResult, bool askConfirmation = false);

	internal void ConfirmOverrideAndExecuteOnLocked(OperationResult result, Action<ILockable> operation, Action<OperationResult> failureAction, bool overrideLocked, bool overrideMaintenanceMode)
	{
		bool flag = false;
		bool flag2 = false;
		ClusterResourceLockedException firstException = ExceptionHelper.GetFirstException<ClusterResourceLockedException>(result.Error);
		if (firstException != null)
		{
			flag = !firstException.MaintenanceMode;
			flag2 = firstException.MaintenanceMode;
		}
		if ((flag && overrideLocked) || (flag2 && overrideMaintenanceMode))
		{
			AskForResourceLockOverride(DisplayName, delegate
			{
				this.ExecuteMethod(operation, SetLastError, LockAccess.Reader, setErrorOnObject: false);
			});
		}
		else
		{
			failureAction(result);
		}
	}

	protected virtual void AskForResourceLockOverride(string item, Action overrideAction)
	{
		if (CreateConfirmationDialog(DialogResources.OverrideObjectLock_ConfirmationBoxTitle, DialogResources.OverrideObjectLock_ConfirmationBoxMessage.FormatCurrentCulture(item)).ShowDialog() == TaskDialogResult.Yes)
		{
			overrideAction();
		}
	}

	static ClusterObject()
	{
		AsyncToSyncOperationsLockObject = new object();
		StoreGroupDictionary = new Dictionary<string, PropertyStoreAttribute>();
		StoreResourceDictionary = new Dictionary<string, PropertyStoreAttribute>();
		StoreNodeDictionary = new Dictionary<string, PropertyStoreAttribute>();
		StoreNetworkDictionary = new Dictionary<string, PropertyStoreAttribute>();
		StoreNetworkInterfaceDictionary = new Dictionary<string, PropertyStoreAttribute>();
		StoreResourceTypeDictionary = new Dictionary<string, PropertyStoreAttribute>();
		Action<Type, Dictionary<string, PropertyStoreAttribute>> obj = delegate(Type type, Dictionary<string, PropertyStoreAttribute> dictionary)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo propertyInfo in properties)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyStoreAttribute), inherit: true);
				if (customAttributes.Length != 0)
				{
					PropertyStoreAttribute propertyStoreAttribute = (PropertyStoreAttribute)customAttributes[0];
					PropertyInfo property = type.GetProperty(propertyStoreAttribute.StoreFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
					propertyStoreAttribute.SourceProperty = propertyInfo;
					propertyStoreAttribute.TargetProperty = property;
					dictionary.Add(propertyInfo.Name, propertyStoreAttribute);
				}
			}
		};
		obj(typeof(Group), StoreGroupDictionary);
		obj(typeof(Resource), StoreResourceDictionary);
		obj(typeof(Node), StoreNodeDictionary);
		obj(typeof(Network), StoreNetworkDictionary);
		obj(typeof(NetworkInterface), StoreNetworkInterfaceDictionary);
		obj(typeof(ResourceType), StoreResourceTypeDictionary);
	}

	protected ClusterObject(Cluster cluster)
	{
		LockObject = new object();
		PerformanceCounters.Increment("Proxy - " + GetType().Name);
		if (cluster == null && !(this is Cluster))
		{
			ClusterLog.LogWarning("A cluster object with type {0} was created and doesn't have a parent cluster associated, this may lead to unexpected results:\r\n" + StackTraceExtensions.GetStackTrace());
		}
		Cluster = cluster;
		QueueInLoaderIfNull = true;
	}

	internal void SetProcessingFlag(bool processing)
	{
		try
		{
			this.ExecuteMethod((ILockable lockObject) => lockObject.Owner.IsProcessing = processing, LockAccess.Reader);
		}
		catch (ClusterException exception)
		{
			if (!Cluster.IgnorableError(exception))
			{
				ClusterLog.LogException(LogLevel.Warning, exception, "There was an error setting the IsProcessing flag");
			}
		}
	}

	protected virtual void ShowDialog(ConfirmationDialog dialog, Action confirmAction, Action cancelAction)
	{
		if (dialog != null && dialog.ShowDialog() != TaskDialogResult.Yes)
		{
			cancelAction?.Invoke();
		}
		else
		{
			confirmAction?.Invoke();
		}
	}

	protected ConfirmationDialog CreateConfirmationDialog(string title, string header)
	{
		return CreateConfirmationDialog(title, header, null);
	}

	protected ConfirmationDialog CreateConfirmationDialog(string title, string header, string content)
	{
		return new ConfirmationDialog
		{
			CustomIcon = Icon.NativeIcon,
			Caption = title,
			Header = header,
			Content = (content ?? string.Empty)
		};
	}

	internal void UpdateBackStoreProperties()
	{
		Dictionary<string, PropertyStoreAttribute> dictionary;
		if (this is Group)
		{
			dictionary = StoreGroupDictionary;
		}
		else if (this is Resource)
		{
			dictionary = StoreResourceDictionary;
		}
		else if (this is Node)
		{
			dictionary = StoreNodeDictionary;
		}
		else
		{
			if (!(this is ResourceType))
			{
				throw new NotSupportedException("type");
			}
			dictionary = StoreResourceTypeDictionary;
		}
		QueueInLoaderIfNull = false;
		try
		{
			foreach (PropertyStoreAttribute value in dictionary.Values)
			{
				value.TargetProperty.SetValue(this, value.SourceProperty.GetValue(this, null), null);
			}
		}
		finally
		{
			QueueInLoaderIfNull = true;
		}
	}

	internal static PropertyInfo GetBackStoreProperty(Type type, string name)
	{
		Dictionary<string, PropertyStoreAttribute> dictionary;
		if (type == typeof(Group))
		{
			dictionary = StoreGroupDictionary;
		}
		else if (type == typeof(Resource))
		{
			dictionary = StoreResourceDictionary;
		}
		else if (type == typeof(Node))
		{
			dictionary = StoreNodeDictionary;
		}
		else if (type == typeof(Network))
		{
			dictionary = StoreNetworkDictionary;
		}
		else if (type == typeof(NetworkInterface))
		{
			dictionary = StoreNetworkInterfaceDictionary;
		}
		else
		{
			if (!(type == typeof(ResourceType)))
			{
				throw new NotSupportedException("type");
			}
			dictionary = StoreResourceTypeDictionary;
		}
		dictionary.TryGetValue(name, out var value);
		return value?.TargetProperty;
	}

	internal void SetIdInternal(Guid id)
	{
		memberId = id;
	}

	internal void SetNameInternal(string newName)
	{
		memberName = newName;
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, CommonLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, ResourceLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, GroupLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, NetworkLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, NodeLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, ResourceTypeLoadSelection parameter)
	{
		LoadAsync(onBackground, (int)parameter);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground)
	{
		LoadAsync(onBackground, 268435456);
	}

	public void LoadAsync(int loadSelection)
	{
		LoadAsync(null, loadSelection);
	}

	protected void LoadAsync(ResourceLoadSelection loadSelection)
	{
		LoadAsync(null, loadSelection);
	}

	protected void LoadAsync(GroupLoadSelection loadSelection)
	{
		LoadAsync(null, loadSelection);
	}

	protected void LoadAsync(NodeLoadSelection loadSelection)
	{
		LoadAsync(null, loadSelection);
	}

	protected void LoadAsync(ResourceTypeLoadSelection loadSelection)
	{
		LoadAsync(null, loadSelection);
	}

	public void LoadAsync(Action<ClusterLoadedEventArgs> onBackground, int loadSelection)
	{
		if (Cluster == null)
		{
			ClusterLog.LogWarning("Cluster parent object is null, a Load operation was executed on the object '{0}' where has not a Cluster parent".FormatCurrentCulture(Name));
			return;
		}
		if ((loadSelection & 0x10000000) == 268435456)
		{
			loadSelection ^= 268435456;
			loadSelection |= this.AllLoadSelectionMask();
		}
		if ((loadSelection & LoadSelection) == loadSelection)
		{
			ClusterLoadedEventArgs loadedArgs = new ClusterLoadedEventArgs(this, loaded: true, LoadSelection, null);
			Worker.Start(delegate
			{
				onBackground.SafeCall(loadedArgs);
			}, delegate(ClusterException exception)
			{
				Error = exception;
			});
			return;
		}
		Worker.Start(delegate
		{
			using ILockable lockable = Cluster.GetLock(LockAccess.Reader);
			if (lockable == null)
			{
				throw new ClusterObjectNotFoundException(Cluster.Name, Cluster.Id, Cluster.GetType());
			}
			((PCluster)lockable.Owner).ClusterObjectLoader.EnqueueClusterObject(new ClusterObjectLoaderParam(this, onBackground, loadSelection));
		}, delegate(ClusterException exception)
		{
			ClusterLoadedEventArgs operationResult = new ClusterLoadedEventArgs(this, loaded: false, LoadSelection, exception);
			try
			{
				onBackground.SafeCall(operationResult);
			}
			catch (Exception innerException)
			{
				Error = new ClusterDefaultException(innerException);
			}
		});
	}

	internal TResult LoadAsync<TResult, TKey>(TKey? obj, int loadSelection, Func<TResult> defaultValue = null) where TResult : struct where TKey : struct
	{
		if (!obj.HasValue || (loadSelection & 0x20000000) == 536870912)
		{
			if (QueueInLoaderIfNull && loadSelection != 0)
			{
				LoadAsync(loadSelection);
			}
			if (defaultValue != null)
			{
				return defaultValue();
			}
			if (typeof(TKey).IsEnum)
			{
				return (TResult)(object)EnumConstants.Fetching;
			}
			return default(TResult);
		}
		return (TResult)(object)obj.Value;
	}

	internal T? LoadAsync<T>(T? obj, int loadSelection) where T : struct
	{
		if ((!obj.HasValue || (loadSelection & 0x20000000) == 536870912) && QueueInLoaderIfNull)
		{
			LoadAsync(loadSelection);
		}
		return obj;
	}

	internal T LoadAsync<T>(T obj, int loadSelection) where T : class
	{
		if ((obj == null || (loadSelection & 0x20000000) == 536870912) && QueueInLoaderIfNull)
		{
			LoadAsync(loadSelection);
		}
		return obj;
	}

	protected bool ParseProperty<TResult>(ClusterPropertyCollection properties, string propertyName, ref TResult memberVariable, List<string> propertiesChanged) where TResult : class
	{
		ClusterProperty clusterProperty = properties[propertyName];
		if (clusterProperty == null)
		{
			return false;
		}
		if (memberVariable != null && memberVariable.Equals(clusterProperty.Value))
		{
			return false;
		}
		_ = clusterProperty.Value;
		if (typeof(TResult) == typeof(IList<string>) && clusterProperty is ClusterPropertyMultipleStrings)
		{
			memberVariable = (TResult)(object)((ClusterPropertyMultipleStrings)clusterProperty).TypedValue.ToList();
		}
		else
		{
			memberVariable = (TResult)clusterProperty.Value;
		}
		propertiesChanged.TryAdd(propertyName);
		return true;
	}

	protected bool ParseProperty<TResult>(ClusterPropertyCollection properties, string propertyName, ref TResult? memberVariable, List<string> propertiesChanged) where TResult : struct
	{
		ClusterProperty clusterProperty = properties[propertyName];
		if (clusterProperty == null)
		{
			return false;
		}
		if (typeof(TResult).IsEnum)
		{
			if (((TResult?)Enum.ToObject(typeof(TResult), clusterProperty.Value)).Equals(memberVariable))
			{
				return false;
			}
			memberVariable = (TResult?)Enum.ToObject(typeof(TResult), clusterProperty.Value);
			propertiesChanged.TryAdd(propertyName);
			return true;
		}
		if (memberVariable.Equals(clusterProperty.Value))
		{
			return false;
		}
		_ = clusterProperty.Value;
		if (typeof(TResult) == typeof(Guid) && clusterProperty is ClusterPropertyString)
		{
			string text = (string)clusterProperty.Value;
			memberVariable = (TResult?)(object)(string.IsNullOrEmpty(text) ? Guid.Empty : new Guid(text));
		}
		else if (typeof(TResult) == typeof(bool) && clusterProperty is ClusterPropertyUInt)
		{
			memberVariable = (TResult?)(object)(((ClusterPropertyUInt)clusterProperty).TypedValue != 0);
		}
		else
		{
			memberVariable = (TResult?)clusterProperty.Value;
		}
		propertiesChanged.TryAdd(propertyName);
		return true;
	}

	internal void SendError(Guid id, ClusterException ex, OperationType operationType)
	{
		Action<ClusterException> sendError = delegate(ClusterException exception)
		{
			Error = exception;
		};
		if (operationType == OperationType.Async)
		{
			Worker.Start(delegate
			{
				sendError(ex);
			});
		}
		else
		{
			sendError(ex);
		}
	}

	internal virtual void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		IsProcessing = privateObject.IsProcessing;
		if (subscribeToEvents)
		{
			SubscribeToEvents(privateObject);
		}
	}

	protected void OnPropertyChanged(string propertyName)
	{
		UIHelper.ExecuteOnDispatcher(delegate
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}, OperationType.Async);
	}

	private void RenameInternal(string newName, OperationType operationType)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Rename(newName);
		}, operationType, LockAccess.Reader);
	}

	protected void SetInformationInternal(string newInformation, OperationType operationType)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.SetInformation(newInformation);
		}, operationType, LockAccess.Writer);
	}

	internal void SubscribeToEvents(PClusterObject privateClusterObject)
	{
		Action<PClusterObject> func = delegate(PClusterObject pObject)
		{
			BoundToPrivate = true;
			pObject.PipeEvent += ProcessPrivateEvent;
		};
		if (privateClusterObject != null)
		{
			func(privateClusterObject);
			return;
		}
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			func(lockObject.Owner);
		}, LockAccess.Reader);
	}

	private void ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e)
	{
		ProcessPrivateEvent(sender, e, null);
	}

	internal virtual bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		if (e.EventType == EventType.BatchChanges)
		{
			ClusterBatchChangesEventArgs obj = e.EventArgument as ClusterBatchChangesEventArgs;
			Queue<Action> queue = null;
			if (obj.Changes.Count > 1)
			{
				queue = new Queue<Action>();
			}
			foreach (ClusterWrapperEventArgs change in obj.Changes)
			{
				ProcessPrivateEvent(sender, change, queue);
			}
			if (queue != null && queue.Count > 0)
			{
				UIHelper.ExecuteOnDispatcher(queue);
			}
			return true;
		}
		return ProcessPrivateEventInternal(sender, e, queueOnDispatcher);
	}

	internal void ExecuteSafe(int loadSelection, Action<OperationResult> operationResult, Action<ILockable> operation, Func<Exception, ClusterException> onException)
	{
		LoadAsync(delegate(ClusterLoadedEventArgs loaded)
		{
			this.ExecuteMethod(delegate(ILockable clusterObject)
			{
				ClusterException ex = null;
				try
				{
					if (loaded.Error != null)
					{
						ex = loaded.Error;
					}
					else
					{
						operation(clusterObject);
					}
				}
				catch (Exception ex2)
				{
					ex = ((onException != null) ? onException(ex2) : new ClusterDefaultException(ex2));
				}
				finally
				{
					operationResult.SafeCall(new OperationResult(this, ex));
				}
			}, LockAccess.Reader);
		}, loadSelection);
	}

	internal bool ProcessPrivateEventInternal(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.Refreshed:
		{
			ClusterRefreshedEventArgs clusterRefreshedEventArgs = e.EventArgument as ClusterRefreshedEventArgs;
			OnRefresh(clusterRefreshedEventArgs.Targeted);
			return true;
		}
		case EventType.Created:
		{
			ClusterAddedEventArgs args4 = e.EventArgument as ClusterAddedEventArgs;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				this.Created.SafeCall(this, args4);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Processing:
		{
			ClusterProcessingEventArgs args7 = e.EventArgument as ClusterProcessingEventArgs;
			IsProcessing = args7.IsProcessing;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("IsProcessing");
				this.Processing.SafeCall(this, args7);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.IdChanged:
		{
			ClusterIdChangedEventArgs clusterIdChangedEventArgs = e.EventArgument as ClusterIdChangedEventArgs;
			if (clusterIdChangedEventArgs.NewId == Id && clusterIdChangedEventArgs.Error == null)
			{
				return true;
			}
			((PClusterObject)sender).Cluster.CacheManager.ReplaceObject((PClusterObject)sender, clusterIdChangedEventArgs.NewId);
			SetIdInternal(clusterIdChangedEventArgs.NewId);
			OnPropertyChanged("Id");
			this.IdChanged.SafeCall(this, clusterIdChangedEventArgs);
			return true;
		}
		case EventType.Opened:
		{
			ClusterOpenEventArgs args6 = e.EventArgument as ClusterOpenEventArgs;
			if (IsOpen == args6.IsOpen && args6.Error == null)
			{
				return true;
			}
			IsOpen = args6.IsOpen;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("IsOpen");
				this.Opened.SafeCall(this, args6);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Renamed:
		{
			ClusterRenamedEventArgs args = e.EventArgument as ClusterRenamedEventArgs;
			if (args.NewName == Name && args.Error == null)
			{
				return true;
			}
			SetNameInternal(args.NewName);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Name");
				OnPropertyChanged("DisplayName");
				this.Renamed.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Removed:
		{
			ClusterRemovedEventArgs args2 = e.EventArgument as ClusterRemovedEventArgs;
			IsRemoved = true;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("IsRemoved");
				this.Removed.SafeCall(this, args2);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Loaded:
		{
			ClusterLoadedEventArgs args8 = e.EventArgument as ClusterLoadedEventArgs;
			LoadSelection = args8.LoadSelection;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				LoadSelection &= -536870913;
				OnPropertyChanged("LoadSelection");
				this.Loaded.SafeCall(this, new ClusterLoadedEventArgs(this, args8.Loaded, args8.LoadSelection, args8.Error));
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Information:
		{
			ClusterInformationEventArgs args5 = e.EventArgument as ClusterInformationEventArgs;
			if (information == args5.Information && args5.Error == null)
			{
				return true;
			}
			information = args5.Information;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Information");
				this.InformationChanged.SafeCall(this, args5);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Error:
			Error = e.EventArgument.Error;
			return true;
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs args3 = e.EventArgument as ClusterPropertiesEventArgs;
			if (args3.Error != null)
			{
				return true;
			}
			Properties = args3.Properties;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("PropertiesChanged");
				this.PropertiesChanged.SafeCall(this, args3);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		default:
			return false;
		}
	}

	public virtual int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (obj.GetType() != GetType())
		{
			return GetHashCode() - obj.GetHashCode();
		}
		if (this == (ClusterObject)obj)
		{
			return 0;
		}
		if (Name == null)
		{
			return -1;
		}
		if (((ClusterObject)obj).Name == null)
		{
			return 1;
		}
		return NativeMethods.StrCmpLogicalW(Name, ((ClusterObject)obj).Name);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is ClusterObject)
		{
			ClusterObject clusterObject = (ClusterObject)obj;
			if (clusterObject.GetType() == GetType() && clusterObject.memberId == memberId)
			{
				return true;
			}
		}
		return false;
	}

	public static bool operator ==(ClusterObject clusterObject1, ClusterObject clusterObject2)
	{
		return clusterObject1?.Equals(clusterObject2) ?? ((object)clusterObject2 == null);
	}

	public static bool operator !=(ClusterObject clusterObject1, ClusterObject clusterObject2)
	{
		return !(clusterObject1 == clusterObject2);
	}

	public static bool operator <(ClusterObject clusterObject1, ClusterObject clusterObject2)
	{
		return string.Compare(clusterObject1.Name, clusterObject2.Name, StringComparison.CurrentCultureIgnoreCase) < 0;
	}

	public static bool operator >(ClusterObject clusterObject1, ClusterObject clusterObject2)
	{
		return string.Compare(clusterObject1.Name, clusterObject2.Name, StringComparison.CurrentCultureIgnoreCase) > 0;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected void RegisterAsyncToSyncOperations()
	{
		lock (AsyncToSyncOperationsLockObject)
		{
			if (asyncToSyncOperations == null)
			{
				asyncToSyncOperations = new HashSet<int>();
			}
			asyncToSyncOperations.Add(Thread.CurrentThread.ManagedThreadId);
		}
	}

	protected void UnregisterAsyncToSyncOperations()
	{
		lock (AsyncToSyncOperationsLockObject)
		{
			asyncToSyncOperations.Remove(Thread.CurrentThread.ManagedThreadId);
			if (asyncToSyncOperations.Count == 0)
			{
				asyncToSyncOperations = null;
			}
		}
	}

	protected static void EnqueueAndThrottleRequests(IEnumerable<ClusterObject> clusterObjects, Action<ClusterObject, Action<OperationResult>> operationToExecute)
	{
		clusterObjects.Where((ClusterObject clusterObject) => !(clusterObject is Cluster)).ForEach(delegate(ClusterObject clusterObject)
		{
			try
			{
				clusterObject.ExecuteMethod((ILockable lockObject) => lockObject.Owner.IsProcessing = true, LockAccess.Reader);
			}
			catch (ClusterException exception5)
			{
				if (!Cluster.IgnorableError(exception5))
				{
					throw;
				}
			}
		});
		Worker.Start(delegate(object[] parameter, string stackTrace)
		{
			Semaphore currentOperations = new Semaphore(20, 20);
			foreach (ClusterObject item in (IEnumerable<ClusterObject>)parameter[0])
			{
				if (!currentOperations.WaitOne(7200000))
				{
					ClusterLog.LogError("The request has fail to respond in the last 2 hours, the operation has been aborted");
					break;
				}
				bool isSemaphoreReleased = false;
				try
				{
					ClusterObject localClusterObject = item;
					operationToExecute(localClusterObject, delegate(OperationResult operationResult)
					{
						localClusterObject.Error = operationResult.Error;
						try
						{
							currentOperations.Release();
						}
						catch (SemaphoreFullException exception4)
						{
							ClusterLog.LogException(exception4, "A semaphore exception was thrown on EnqueueAndThrottleRequests");
						}
						isSemaphoreReleased = true;
					});
				}
				catch (Exception exception3)
				{
					if (!isSemaphoreReleased)
					{
						try
						{
							currentOperations.Release();
						}
						catch (SemaphoreFullException exception2)
						{
							ClusterLog.LogException(exception2, "A semaphore exception was thrown on EnqueueAndThrottleRequests");
						}
					}
					ClusterLog.LogException(exception3, "There was an error processing an enqueued operation");
				}
			}
		}, new object[1]
		{
			new List<ClusterObject>(clusterObjects)
		}, delegate(ClusterException exception)
		{
			ClusterLog.LogException(exception, "There was an error iterating the objects on a multiple enqueued operation");
		});
	}

	protected static void ProtectedScope(Action task, Action<ClusterException> actionAfterComplete)
	{
		ProtectedScope(task, delegate(ClusterException exception)
		{
			actionAfterComplete(exception);
			return exception;
		});
	}

	protected static void ProtectedScope(Action task, Func<ClusterException, ClusterException> actionAfterComplete)
	{
		ClusterException arg = null;
		try
		{
			task();
		}
		catch (ClusterException ex)
		{
			arg = ex;
		}
		finally
		{
			actionAfterComplete(arg);
		}
	}

	protected static T ProtectedScope<T>(Func<T> task, Action<ClusterException> actionAfterComplete)
	{
		return ProtectedScope(task, delegate(ClusterException exception)
		{
			actionAfterComplete(exception);
			return exception;
		});
	}

	protected static T ProtectedScope<T>(Func<T> task, Func<ClusterException, ClusterException> actionAfterComplete)
	{
		ClusterException ex = null;
		T val = default(T);
		try
		{
			return task();
		}
		catch (Exception ex2)
		{
			if (!(ex2 is ClusterException))
			{
				ex = new ClusterDefaultException(ex2);
				throw ex;
			}
			ex = (ClusterException)ex2;
			throw;
		}
		finally
		{
			try
			{
				ex = actionAfterComplete(ex);
			}
			catch (Exception ex3)
			{
				if (!(ex3 is ClusterException))
				{
					ex = new ClusterDefaultException(ex3);
				}
				else
				{
					ex = (ClusterException)ex3;
				}
				ClusterLog.LogException(ex3, "Error processing the asynchronous operation");
			}
		}
	}

	protected Icon2 ReturnInstance(ref Icon2 reference, Func<Icon2> createInstanceCallback)
	{
		if (reference == null)
		{
			lock (mObjectLock)
			{
				if (reference == null)
				{
					reference = createInstanceCallback();
				}
			}
		}
		return reference;
	}

	internal void SetLastErrorIfNecessary(OperationResult operationResult)
	{
		if (operationResult.Error != null)
		{
			Error = operationResult.Error;
		}
	}

	protected void SetLastError(OperationResult operationResult)
	{
		Error = operationResult.Error;
	}

	protected virtual void OnRefresh(bool targeted)
	{
		LoadSelection = 0;
		IsProcessing = false;
		information = null;
		Error = null;
		mCommands = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("IsProcessing");
			OnPropertyChanged("Information");
			OnPropertyChanged("LoadSelection");
			OnPropertyChanged("Properties");
			OnPropertyChanged("Commands");
		}, OperationType.Async);
	}
}
