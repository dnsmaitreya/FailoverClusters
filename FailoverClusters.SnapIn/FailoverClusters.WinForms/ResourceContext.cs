using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using ManagementConsole;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

public static class ResourceContext
{
	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPropertySheetExtensionStatus([MarshalAs(UnmanagedType.LPWStr)] string extensionDllClassId);

	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Auto)]
	private static extern int RegisterAdminExtensionsServer([In][MarshalAs(UnmanagedType.LPWStr)] string extensionDll);

	public static int GetPropertyPages(PropertyPageCollection pages, ResourceType resourceType, FailoverClusters.Framework.Cluster cluster, Guid resourceId)
	{
		if (pages == null)
		{
			throw new ArgumentNullException("pages");
		}
		try
		{
			ClusterSnapinPropertyPage clusterSnapinPropertyPage = null;
			if (pages.Count == 0)
			{
				SnapinPropertyPageControlBase snapinPropertyPageControlBase = null;
				switch (resourceType.ResourceKind)
				{
				case ResourceKind.DhcpService:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case (ResourceKind)45:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.DistributedFileSystem:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.GenericApplication:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.GenericScript:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.GenericService:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.IPAddress:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.IPv6Address:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.IPv6TunnelAddress:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.ClusterFileSystem:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.PhysicalDisk:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.VolumeShadowCopyServiceTask:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.WinsService:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.DfsReplicatedFolder:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				case ResourceKind.Msmsq:
				case ResourceKind.MsmsqTrigger:
				case ResourceKind.Dtc:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId, renamable: false);
					break;
				default:
					snapinPropertyPageControlBase = new ResourceGeneralPropertyPage(cluster, resourceId);
					break;
				}
				clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
				clusterSnapinPropertyPage.SetControl(snapinPropertyPageControlBase);
				pages.Add(clusterSnapinPropertyPage);
			}
			if (resourceType.ResourceKind == ResourceKind.ClusterFileSystem)
			{
				return pages.Count;
			}
			ResourceDependenciesPropertyPage control = new ResourceDependenciesPropertyPage(cluster, resourceId);
			clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(control);
			pages.Add(clusterSnapinPropertyPage);
			clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new ResourcePoliciesPropertyPage(cluster, resourceId));
			pages.Add(clusterSnapinPropertyPage);
			clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new ResourceAdvancedPropertyPage(cluster, resourceId));
			pages.Add(clusterSnapinPropertyPage);
			clusterSnapinPropertyPage = AddPropertyGridPage(resourceType, cluster, resourceId);
			if (clusterSnapinPropertyPage != null)
			{
				pages.Add(clusterSnapinPropertyPage);
			}
		}
		catch (Exception e)
		{
			ErrorPropertyPage.CreateErrorPropertySheet(pages, e, Resources.Properties_Text);
		}
		return pages.Count;
	}

	private static ClusterSnapinPropertyPage AddPropertyGridPage(ResourceType resourceType, FailoverClusters.Framework.Cluster cluster, Guid resourceId)
	{
		ClusterSnapinPropertyPage clusterSnapinPropertyPage = null;
		bool addPropertyGrid = false;
		if (cluster != null && resourceId != Guid.Empty)
		{
			cluster.GetResourceType(resourceType.Name, delegate(OperationResult<ResourceType> resTypeOpGet)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				if (resTypeOpGet.Error != null)
				{
					ClusterDialogException.ShowTaskDialog(resTypeOpGet.Error, IntPtr.Zero);
					return;
				}
				ResourceType resType = resTypeOpGet.Result;
				if (resType.ResourceKind == ResourceKind.Other)
				{
					addPropertyGrid = true;
				}
				ManualResetEventSlim waitEvent = new ManualResetEventSlim(initialState: false);
				try
				{
					resType.LoadAsync(delegate(ClusterLoadedEventArgs result)
					{
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						try
						{
							if (result.Error != null)
							{
								ClusterDialogException.ShowTaskDialog(result.Error, IntPtr.Zero);
							}
							else
							{
								ClusterPropertyMultipleStrings clusterPropertyMultipleStrings = (ClusterPropertyMultipleStrings)resType.Properties["AdminExtensions"];
								if (clusterPropertyMultipleStrings != null && clusterPropertyMultipleStrings.TypedValue.Count > 0)
								{
									StringCollection stringCollection = new StringCollection();
									stringCollection.AddRange(clusterPropertyMultipleStrings.TypedValue.ToArray());
									SetupAdminExtensions(cluster, stringCollection);
									addPropertyGrid = false;
								}
							}
						}
						finally
						{
							try
							{
								waitEvent.Set();
							}
							catch (ObjectDisposedException)
							{
							}
						}
					}, ResourceTypeLoadSelection.CommonProperties);
					waitEvent.Wait(TimeSpan.FromSeconds(5.0));
				}
				finally
				{
					if (waitEvent != null)
					{
						((IDisposable)waitEvent).Dispose();
					}
				}
			}, OperationType.Sync);
		}
		if (addPropertyGrid)
		{
			clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new PropertyGridPropertyPage(cluster, resourceId));
		}
		return clusterSnapinPropertyPage;
	}

	public static Guid[] GetNodeTypes()
	{
		Guid[] array = new Guid[1];
		Guid resourceContextGuid = ClusterAdministrator.ResourceContextGuid;
		array[0] = new Guid(resourceContextGuid.ToString());
		return array;
	}

	public static WritableSharedData GetSharedData(Resource resource)
	{
		string text = null;
		WritableSharedData writableSharedData = new WritableSharedData();
		try
		{
			text = resource.Cluster.ConnectedTo;
		}
		catch (ClusterObjectNotFoundException)
		{
			return writableSharedData;
		}
		WritableSharedDataItem writableSharedDataItem = new WritableSharedDataItem("CLUSTER_NAME", requiresCallback: false);
		writableSharedDataItem.SetData(Encoding.Unicode.GetBytes(text + "\0"));
		writableSharedData.Add(writableSharedDataItem);
		WritableSharedDataItem writableSharedDataItem2 = new WritableSharedDataItem("CLUSTER_RESOURCE_NAME", requiresCallback: false);
		writableSharedDataItem2.SetData(Encoding.Unicode.GetBytes(resource.Name + "\0"));
		writableSharedData.Add(writableSharedDataItem2);
		WritableSharedDataItem writableSharedDataItem3 = new WritableSharedDataItem("CLUSTER_RESOURCE_TYPE_NAME", requiresCallback: false);
		ResourceType resourceType = resource.ResourceType.ActualResourceType ?? resource.ResourceType;
		writableSharedDataItem3.SetData(Encoding.Unicode.GetBytes(resourceType.Name + "\0"));
		writableSharedData.Add(writableSharedDataItem3);
		WritableSharedDataItem writableSharedDataItem4 = new WritableSharedDataItem("CLUSTER_LCID", requiresCallback: false);
		writableSharedDataItem4.SetData(KDDSL.ServerClusters.Management.ClusterHelp.Int32ToByteArray(CultureInfo.CurrentCulture.LCID));
		writableSharedData.Add(writableSharedDataItem4);
		return writableSharedData;
	}

	private static void SetupAdminExtensions(FailoverClusters.Framework.Cluster cluster, StringCollection adminExtensions)
	{
		try
		{
			bool flag = true;
			StringEnumerator enumerator = adminExtensions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (!string.IsNullOrEmpty(current) && GetPropertySheetExtensionStatus(current) == 2147746132u)
					{
						flag = false;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			if (!flag)
			{
				InstallExtensionDlls(cluster);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void InstallExtensionDlls(FailoverClusters.Framework.Cluster cluster)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		DirectoryInfo parameter = new DirectoryInfo(Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("windir") ?? throw new ApplicationException(CommonResources.CannotFindWindir_Text), "cluster"), "extensions"));
		cluster.Nodes.ExecuteQuery(ResultExecution.Sync, NodesQuery, parameter);
	}

	private static void NodesQuery(OperationResult<IClusterList<FailoverClusters.Framework.Node>> nodeResult)
	{
		DirectoryInfo directoryInfo = (DirectoryInfo)nodeResult.Parameter;
		if (nodeResult.Error != null)
		{
			throw nodeResult.Error;
		}
		FileInfo[] files;
		foreach (FailoverClusters.Framework.Node item in nodeResult.Result)
		{
			try
			{
				CheckClusterBinaryPath(item);
				DirectoryInfo directoryInfo2 = new DirectoryInfo(CreateClusterBinaryPath(item, "extensions"));
				if (!directoryInfo2.Exists)
				{
					continue;
				}
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				files = directoryInfo2.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					string text = Path.Combine(directoryInfo.FullName, fileInfo.Name);
					if (!File.Exists(text))
					{
						File.Copy(fileInfo.FullName, text);
						CopyMuiFile(fileInfo.FullName, directoryInfo.FullName);
					}
				}
				break;
			}
			catch (Exception)
			{
			}
		}
		files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo2 in files)
		{
			if (fileInfo2.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
			{
				int num = RegisterAdminExtensionsServer(fileInfo2.FullName);
				if (num != 0)
				{
					DebugLog.LogError("Cannot register admin extension '{0}', error: {1}", fileInfo2.FullName, num);
				}
			}
		}
	}

	private static void CheckClusterBinaryPath(FailoverClusters.Framework.Node node)
	{
		try
		{
			File.OpenRead(CreateClusterBinaryPath(node, "clussvc.exe")).Close();
		}
		catch (Exception innerException)
		{
			throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, CommonResources.CannotFindClusterService_Text, node.Name), innerException);
		}
	}

	private static string CreateClusterBinaryPath(FailoverClusters.Framework.Node node, string binary)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		if (binary == null)
		{
			throw new ArgumentNullException("binary");
		}
		return string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\admin$\\cluster\\{1}", node.Name, binary);
	}

	private static void CopyMuiFile(string binarySourcePath, string targetDirectory)
	{
		string directoryName = Path.GetDirectoryName(binarySourcePath);
		string path = Path.GetFileName(binarySourcePath) + ".mui";
		for (CultureInfo cultureInfo = CultureInfo.CurrentUICulture; cultureInfo != CultureInfo.InvariantCulture; cultureInfo = cultureInfo.Parent)
		{
			string text = Path.Combine(Path.Combine(directoryName, cultureInfo.Name), path);
			if (File.Exists(text))
			{
				string text2 = Path.Combine(targetDirectory, cultureInfo.Name);
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				File.Copy(text, Path.Combine(text2, path), overwrite: true);
				break;
			}
		}
	}
}

