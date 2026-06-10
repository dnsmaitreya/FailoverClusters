using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using FailoverClusters.Configuration;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Services;
using FailoverClusters.UI.Controls;
using FailoverClusters.UIFramework;
using ManagementConsole;
using ManagementConsole.Advanced;

namespace KDDSL.ServerClusters.Management;

internal class ClusterAdministrator
{
	private delegate void SetStatusBarMessageDelegate(IWpfViewAdapter rolesViewAdapter, string message);

	private delegate void SetStatusBarProgressMessageDelegate(string message, bool success);

	private delegate void AddDownClusterViewDelegate(DownClusterContext downClusterContext);

	internal delegate void SetEventLogFilterDelegate(EventLogFilter filter);

	internal delegate void AddDiskDelegate(ClusterResource disk);

	private class NodeConnectionStatus
	{
		internal bool Connecting { get; set; }
	}

	public const string SnapInName = "Failover Cluster Manager";

	public const string StorageApplianceManagerName = "Storage Appliance Manager";

	public const string SnapInVendor = "Corporation";

	public static readonly Guid GroupContextGuid = new Guid("{141BD9CF-1A26-4CDF-AB68-2A8A53D8D50B}");

	public static readonly Guid ResourceContextGuid = new Guid("{258C9D33-ABB6-4C07-89E5-E4437C43030E}");

	public static readonly Guid NetworksContextGuid = new Guid("{E665B79D-EFB2-4AF8-A147-C9365BC6E7CE}");

	public static readonly Guid ClusterContextGuid = new Guid("{9E48D9FE-87FB-4285-A044-6547071FDEBF}");

	public static readonly Guid NetworkContextGuid = new Guid("{F5545327-F2FD-410a-A56C-0D7B90C4FC72}");

	public static readonly Guid NodesContextGuid = new Guid("{809A257C-390E-41f4-873A-098679E84976}");

	public static readonly Guid NodeContextGuid = new Guid("{2E3CF8C8-81D2-4d6a-B60D-5A35F3A6AAE9}");

	private static bool? isVirtualMachineRoleInstalled;

	private static ConsoleNotifyUser notifyUser;

	private static ClusterAdministrator snapin;

	private SnapInSettings settings;

	private int manageClusterArgSwitchIndex = -1;

	private const string ManageClusterArgSwitch = "/ManageCluster";

	private FormView selectedFormView;

	private readonly CustomStatus customStatus = new CustomStatus();

	private static readonly object AddChildLock = new object();

	private static readonly object LockUiObject = new object();

	private bool isShuttingDown;

	private CluAdminScopeNode rootNode;

	private readonly NamespaceSnapInBase snapInBase;

	private readonly ConcurrentBag<System.Action> shutdownCallbacks = new ConcurrentBag<System.Action>();

	private static IWpfViewAdapter currentRolesViewAdapter = null;

	private static string defaultSystemFontName;

	private static float defaultSystemFontSize;

	internal ClusterSnapInType SnapInType { get; private set; }

	internal string RootContextDisplayName
	{
		get
		{
			if (SnapInType == ClusterSnapInType.FailoverClusterManager)
			{
				return Resources.ServerClustersManagement_Text;
			}
			throw new InvalidOperationException("Unknown ClusterSnapInType");
		}
	}

	internal int RootContentIconIndex
	{
		get
		{
			if (SnapInType == ClusterSnapInType.FailoverClusterManager)
			{
				return Icons.SnapInRootIndex;
			}
			throw new InvalidOperationException("Unknown ClusterSnapInType");
		}
	}

	public static ClusterAdministrator Instance => snapin;

	public static ManagementConsole.Advanced.Console Console => Instance.snapInBase.Console;

	internal static FormView ActiveFormView
	{
		get
		{
			return Instance.selectedFormView;
		}
		set
		{
			Instance.selectedFormView = value;
		}
	}

	public bool Shutdown => isShuttingDown;

	public CluAdminScopeNode RootNode => rootNode;

	public static string DefaultSystemFontName => defaultSystemFontName;

	public static float DefaultSystemFontSize => defaultSystemFontSize;

	internal static INotifyUser NotifyUser => (INotifyUser)(object)notifyUser;

	private bool InitializedDism { get; set; }

	public static bool IsVirtualMachineRoleInstalled
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (!isVirtualMachineRoleInstalled.HasValue)
			{
				isVirtualMachineRoleInstalled = new WindowsFeature().IsVirtualMachineClientToolsInstalled();
			}
			return isVirtualMachineRoleInstalled.GetValueOrDefault();
		}
	}

	internal SnapInSettings Settings => settings;

	internal static int MaxBackgroundRetries => 1;

	internal static int MaxDeletedObjectRetries => 5;

	internal static int DeletedObjectSleepTime => 500;

	internal event EventHandler SettingsLoaded;

	internal static event EventHandler<SelectScopeNodeEventArgs> SelectScopeNode;

	public ClusterAdministrator(NamespaceSnapInBase snapInBase, ClusterSnapInType snapInType)
	{
		AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
		this.snapInBase = snapInBase;
		SnapInType = snapInType;
		WpfEnvironment.Initialize();
		ServiceContainer.Initialize();
		FrameworkServices.Initialize();
		defaultSystemFontName = System.Windows.SystemFonts.MenuFontFamily.Source;
		defaultSystemFontSize = PixelsToPoints(System.Windows.SystemFonts.MenuFontSize);
		ClusterLog.Initialize((TaskCategory)1);
		notifyUser = new ConsoleNotifyUser(snapInBase.Console);
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		System.Windows.Forms.Application.ThreadException += OnUnhandledThreadException;
		Background.ExceptionOccurred += OnUnhandledException;
		foreach (Icon icon in Icons.IconList)
		{
			snapInBase.SmallImages.Add(WinFormsHelp.GetSmallIcon(icon));
			snapInBase.LargeImages.Add(WinFormsHelp.GetLargeIcon(icon));
		}
		SynchronizeInvoke.SynchronizationObject = snapInBase;
		NotifyUser.SynchronizationObject = snapInBase;
		Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcherUnhandledException;
		Global.DefaultDispatcher = Dispatcher.CurrentDispatcher;
		InitializedDism = false;
		Services.Initialize(this.snapInBase);
		snapin = this;
	}

	private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
	{
		if (args.Name.Contains("Virtualization.Client.Common.Types"))
		{
			return Assembly.Load("Virtualization.Client.Common, Version=10.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
		}
		return null;
	}

	private static float PixelsToPoints(double pixels)
	{
		return (float)(pixels / 1.3333333333333333);
	}

	~ClusterAdministrator()
	{
		ClusterLog.Shutdown();
	}

	private void OnUnhandledThreadException(object sender, ThreadExceptionEventArgs e)
	{
		ProcessUnhandledException(e.Exception);
	}

	private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		ProcessUnhandledException((Exception)e.ExceptionObject);
	}

	private void ProcessUnhandledException(Exception e)
	{
		if (isShuttingDown)
		{
			return;
		}
		try
		{
			ClusterLog.LogException(e, "Unloading cluster administrator since an unexpected error occurred.");
			ClusterLog.AdminEvents.WriteFatalError(ExceptionHelp.GetExceptionDetails(e));
		}
		finally
		{
			snapInBase.BeginInvoke((System.Action)delegate
			{
				e.Data.Add("StackTrace", e.StackTrace);
				throw e;
			});
		}
	}

	private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		e.Dispatcher.InvokeShutdown();
		Worker.Start(delegate
		{
			ClusterLog.LogException(e.Exception, "An unhandled exception was encountered by the dispatcher");
			ProcessUnhandledException(e.Exception);
		});
		e.Handled = true;
	}

	public static void ShowError(Exception exception, string message)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (Instance.Shutdown)
		{
			return;
		}
		lock (LockUiObject)
		{
			UIThreadHandlerV val = (UIThreadHandlerV)delegate
			{
				((NotifyUser)notifyUser).ShowError(exception, message);
			};
			if (SynchronizeInvoke.InvokeRequired)
			{
				SynchronizeInvoke.Invoke((Delegate)(object)val, (object[])null);
			}
			else
			{
				val.Invoke();
			}
		}
	}

	public void Initialize()
	{
		try
		{
			DebugLog.LogInfo("Snapin is starting");
			ThreadWatchdog.RegisterUIThread(null);
			settings = new SnapInSettings();
			settings.SettingsChanged += OnSettingsChanged;
			RootContext context = ContextFactory.CreateRootContext(this, ProcessCommandLineArguments());
			if (typeof(SnapIn).IsAssignableFrom(snapInBase.GetType()))
			{
				((SnapIn)snapInBase).RootNode = (rootNode = new CluAdminScopeNode(context));
			}
			else if (typeof(NamespaceExtension).IsAssignableFrom(snapInBase.GetType()))
			{
				((NamespaceExtension)snapInBase).PrimaryNode.Children.Add(rootNode = new CluAdminScopeNode(context));
			}
			else
			{
				DebugLog.LogCritical(string.Format(CultureInfo.CurrentCulture, "SnapIn failed to load since the context from where it is executed in a unknown instance type '{0}'", snapInBase.GetType()));
			}
			DownClusterDataService downClusterDataService = new DownClusterDataService((RootContext)rootNode.Tag);
			ServiceContainer.Container.RegisterServiceInstance(typeof(IDownClusterDataService), (object)downClusterDataService);
			ServiceContainer.Container.RegisterServiceInstance(typeof(IDownClusterDataChangedService), (object)downClusterDataService);
			Worker.Start(delegate
			{
				try
				{
					InitializedDism = Dism.Initialize();
					RegisterShutdownCallback(delegate
					{
						if (InitializedDism)
						{
							Dism.Shutdown();
						}
					});
					if (!IsVirtualMachineRoleInstalled)
					{
						ClusterLog.AdminEvents.WriteHyperVToolsNotInstalledEvent();
					}
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					Exception e2 = ex2;
					snapInBase.BeginInvoke((System.Action)delegate
					{
						ReportUnexpectedError(e2);
					});
				}
			});
			RegisterShutdownCallback(Worker.Shutdown);
			RegisterShutdownCallback(SnapInFormView.ShutdownUIProducer);
			DebugLog.LogInfo("Snapin started");
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
	}

	internal static void CreateDownClusterView(string nodeOrClusterFqdn)
	{
		ConnectedClusterData connectedClusterData = Instance.Settings.ConnectedClusters.FirstOrDefault((ConnectedClusterData item) => item.ClusterName.Equals(nodeOrClusterFqdn, StringComparison.CurrentCultureIgnoreCase) || item.NodeNames.Any((string nodeName) => nodeName.ToLower(CultureInfo.CurrentCulture).Contains(nodeOrClusterFqdn.ToLower(CultureInfo.CurrentCulture))));
		if (connectedClusterData != null)
		{
			CreateDownClusterView(connectedClusterData.NodeNames);
		}
	}

	internal static void CreateDownClusterView(ICollection<string> nodeNames)
	{
		Worker.Start(delegate
		{
			nodeNames.AsParallel().Any((string nodeName) => CreateDownClusterView(new NodeConnectionStatus(), nodeName) != null);
		});
	}

	internal static DownClusterContext CreateDownClusterContext(string nodeName)
	{
		try
		{
			if (!NetworkHelper.CanPing(nodeName))
			{
				return null;
			}
			ClusterDatabase clusterDatabase = new ClusterDatabase(nodeName);
			if (clusterDatabase.NodeNames.Count == 0)
			{
				DebugLog.LogVerbose("Down Cluster View can't be created since the cluster database does not contain any node, very probably cluster was destroyed");
				return null;
			}
			return new DownClusterContext(clusterDatabase);
		}
		catch
		{
		}
		return null;
	}

	private static DownClusterContext CreateDownClusterView(NodeConnectionStatus connectionStatus, string nodeName)
	{
		try
		{
			if (!NetworkHelper.CanPing(nodeName))
			{
				return null;
			}
			ClusterDatabase clusterDatabase = new ClusterDatabase(nodeName);
			if (clusterDatabase.NodeNames.Count == 0)
			{
				DebugLog.LogVerbose("Down Cluster View can't be created since the cluster database does not contain any node, very probably cluster was destroyed");
				return null;
			}
			DownClusterContext downClusterContext = null;
			lock (connectionStatus)
			{
				if (!connectionStatus.Connecting)
				{
					try
					{
						downClusterContext = new DownClusterContext(clusterDatabase);
						AddDownClusterView(downClusterContext);
						connectionStatus.Connecting = true;
					}
					catch (ClusterBaseException caughtException)
					{
						ExceptionHelp.LogException(caughtException, Extensions.FormatCurrentCulture("Cannot connect to node {0}", (object)nodeName));
					}
				}
			}
			return downClusterContext;
		}
		catch (Exception)
		{
		}
		return null;
	}

	private static void AddDownClusterView(DownClusterContext downClusterContext)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			lock (AddChildLock)
			{
				SynchronizeInvoke.BeginInvoke((Delegate)new AddDownClusterViewDelegate(AddDownClusterView), new object[1] { downClusterContext });
				return;
			}
		}
		RootContext rootContext = (RootContext)Instance.RootNode.Tag;
		rootContext.AddChild(downClusterContext, rootContext.GetChildContexts().Count == 0);
	}

	internal static void SetStatusBarMessage(IWpfViewAdapter viewAdapter, string message)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.Invoke((Delegate)new SetStatusBarMessageDelegate(SetStatusBarMessage), new object[2] { viewAdapter, message });
		}
		else
		{
			if (Instance.selectedFormView == null)
			{
				return;
			}
			lock (Instance.customStatus)
			{
				if (currentRolesViewAdapter != viewAdapter || !viewAdapter.StatusBarStarted)
				{
					if (currentRolesViewAdapter != null && currentRolesViewAdapter.StatusBarStarted)
					{
						Instance.customStatus.Complete(string.Empty, success: true);
						currentRolesViewAdapter.StatusBarStarted = false;
					}
					currentRolesViewAdapter = viewAdapter;
					if (!viewAdapter.StatusBarStarted)
					{
						Instance.customStatus.Start(snapin.selectedFormView.ScopeNode);
						viewAdapter.StatusBarStarted = true;
					}
				}
				Instance.customStatus.Title = ((FormView)viewAdapter).ScopeNode.DisplayName;
				Instance.customStatus.ReportProgress(0, 0, string.IsNullOrWhiteSpace(message) ? " " : message);
			}
		}
	}

	internal static void SetStatusBarProgressMessage(string message)
	{
		SetStatusBarProgressMessage(message, success: true);
	}

	internal static void SetStatusBarProgressMessage(string message, bool success)
	{
		if (SynchronizeInvoke.InvokeRequired)
		{
			SynchronizeInvoke.Invoke((Delegate)new SetStatusBarProgressMessageDelegate(SetStatusBarProgressMessage), new object[2] { message, success });
		}
		else
		{
			if (Instance.selectedFormView == null)
			{
				return;
			}
			lock (Instance.customStatus)
			{
				if (currentRolesViewAdapter != null && currentRolesViewAdapter.StatusBarStarted)
				{
					Instance.customStatus.Complete(string.Empty, success: true);
					currentRolesViewAdapter.StatusBarStarted = false;
				}
				currentRolesViewAdapter = null;
				Instance.customStatus.Start(snapin.selectedFormView.ScopeNode);
				Instance.customStatus.Title = Resources.Cluster_Text;
				Instance.customStatus.Complete(message, success);
			}
		}
	}

	private List<string> ProcessCommandLineArguments()
	{
		List<string> list = new List<string>();
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (string.Compare(commandLineArgs[i], "/ManageCluster", StringComparison.OrdinalIgnoreCase) == 0)
			{
				manageClusterArgSwitchIndex = i;
				break;
			}
		}
		int num = manageClusterArgSwitchIndex + 1;
		while (manageClusterArgSwitchIndex != -1 && num < commandLineArgs.Length)
		{
			if (!(commandLineArgs[num].Trim() == ".") && !commandLineArgs[num].Trim().StartsWith("/", StringComparison.OrdinalIgnoreCase) && !commandLineArgs[num].Trim().StartsWith("-", StringComparison.OrdinalIgnoreCase) && !list.Contains(commandLineArgs[num].Trim().ToLower(CultureInfo.InvariantCulture)))
			{
				list.Add(commandLineArgs[num].Trim().ToLower(CultureInfo.InvariantCulture));
			}
			num++;
		}
		return list;
	}

	public byte[] SaveCustomData(SyncStatus status)
	{
		DebugLog.LogInfo("Saving settings");
		try
		{
			StreamingContext context = new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence);
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Context = context;
			binaryFormatter.Serialize(memoryStream, settings);
			return memoryStream.ToArray();
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
		return null;
	}

	public void LoadCustomData(AsyncStatus status, byte[] persistenceData)
	{
		DebugLog.LogInfo("Loading settings");
		try
		{
			StreamingContext context = new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence);
			MemoryStream memoryStream = new MemoryStream(persistenceData);
			if (memoryStream.Length > 0)
			{
				object obj = new BinaryFormatter
				{
					Context = context
				}.Deserialize(memoryStream);
				settings = (SnapInSettings)obj;
				settings.SettingsChanged += OnSettingsChanged;
			}
			OnSettingsLoaded();
		}
		catch (Exception e)
		{
			ReportUnexpectedError(e);
		}
	}

	public void OnShutdown(AsyncStatus status)
	{
		try
		{
			isShuttingDown = true;
			Global.IsProcessShuttingDown = true;
			Parallel.ForEach(shutdownCallbacks, delegate(System.Action callback)
			{
				callback();
			});
			CluAdminScopeNode cluAdminScopeNode = RootNode;
			cluAdminScopeNode.Dispose();
			KDDSL.ServerClusters.Utilities.DisposeObject(cluAdminScopeNode.Context);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, Resources.UnexpectedError_Text);
		}
	}

	private void OnSettingsChanged(object sender, EventArgs e)
	{
		snapInBase.IsModified = true;
	}

	public void RegisterShutdownCallback(System.Action callback)
	{
		Exceptions.ThrowIfNull((object)callback, "callback");
		shutdownCallbacks.Add(callback);
	}

	public void UnregisterShutdownCallback(System.Action callback)
	{
		Exceptions.ThrowIfNull((object)callback, "callback");
		shutdownCallbacks.TryTake(out callback);
	}

	public static DateTime GetClusterEventsStartTime(FailoverClusters.Framework.Cluster frameworkCluster)
	{
		DateTime now = DateTime.Now;
		DateTime dateTime = now - TimeSpan.FromDays(1.0);
		if (frameworkCluster != null)
		{
			DateTime recentEventsResetTime = frameworkCluster.RecentEventsResetTime;
			TimeSpan timeSpan = now.Subtract(recentEventsResetTime);
			if (timeSpan.TotalHours < 24.0)
			{
				dateTime = now - timeSpan;
				ClusterLog.LogVerbose((LogSubcategory)10, "Setting Cluster Events Start Time {0}", new object[1] { dateTime });
			}
		}
		return dateTime;
	}

	public static DateTime GetClusterEventsStartTime(Cluster cluster, bool useWaitDialog = false)
	{
		DateTime clusterEventsStartTime = DateTime.UtcNow - TimeSpan.FromDays(1.0);
		Func<DateTime> getEventsStartTime = delegate
		{
			DateTime dateTime = DateTime.UtcNow - TimeSpan.FromDays(1.0);
			try
			{
				if (cluster.GetCommonProperties(PropertyCollectionSet.ReadWrite).TryGetProperty("RecentEventsResetTime", out var property))
				{
					DateTime value = (DateTime)property.Value;
					TimeSpan timeSpan = DateTime.UtcNow.Subtract(value);
					if (timeSpan.TotalHours < 24.0)
					{
						dateTime = DateTime.UtcNow - timeSpan;
						ClusterLog.LogVerbose((LogSubcategory)10, "Setting Cluster Events Start Time {0}", new object[1] { dateTime });
					}
				}
			}
			catch (ApplicationException ex)
			{
				ClusterLog.LogException((Exception)ex, "There was an error retrieving cluster common properties");
			}
			catch (ClusterBaseException ex2)
			{
				ClusterLog.LogException((Exception)ex2, "There was an error retrieving cluster common properties");
			}
			return dateTime;
		};
		if (cluster != null)
		{
			if (useWaitDialog)
			{
				using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ResetRecentEvents_Text, Resources.ResettingRecentEventsTime_Text);
				cluadminWaitDialog.ShowDialog(NotifyUser, delegate
				{
					clusterEventsStartTime = getEventsStartTime();
				});
			}
			else
			{
				clusterEventsStartTime = getEventsStartTime();
			}
		}
		return clusterEventsStartTime.ToLocalTime();
	}

	private void OnSettingsLoaded()
	{
		this.SettingsLoaded?.Invoke(this, EventArgs.Empty);
	}

	private static void ReportUnexpectedError(Exception e)
	{
		ReportUnexpectedError((INotifyUser)(object)notifyUser, e);
	}

	private static void ReportUnexpectedError(INotifyUser notifyUser, Exception e)
	{
		notifyUser.ShowError(e, Resources.UnexpectedError_Text);
	}

	internal static void RequestScopeNodeSelection(ScopeNode scopeNode)
	{
		OnSelectScopeNode(scopeNode);
	}

	private static void OnSelectScopeNode(ScopeNode scopeNode)
	{
		ClusterAdministrator.SelectScopeNode?.Invoke(null, new SelectScopeNodeEventArgs(scopeNode));
	}
}

