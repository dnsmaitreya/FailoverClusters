using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace KDDSL.ServerClusters;

public class FileShareQuorumSettings : QuorumSettings, IHasQuorumResource
{
	private static string sharePathPropertyName = "SharePath";

	private static string ntfsCleanupNeeded = "CleanupNTFSPerms";

	private static string shareCleanupNeeded = "CleanupSharePerms";

	private static FileSystemRights accessRights = FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.Delete;

	internal string m_share;

	internal string m_fileSharePath;

	internal string m_serverName;

	internal string m_shareName;

	internal string m_subDir;

	internal string m_userName;

	internal string m_password;

	internal ClusterResource m_resource;

	public virtual ClusterResource QuorumResource => m_resource;

	public string FileSharePath => m_fileSharePath;

	public override QuorumType QuorumType => QuorumType.FileShareWitness;

	private void Construct()
	{
		if (!NetShareInfo.TryParseFileShare(m_fileSharePath, out m_serverName, out m_shareName, out m_subDir))
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidFileSharePathArg_Text,
				m_fileSharePath
			});
		}
		m_share = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\{1}", m_serverName, m_shareName);
	}

	private string GetShareFromResource(ClusterResource resource)
	{
		return (string)resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite).GetProperty(sharePathPropertyName).Value;
	}

	private void GrantClusterPermissionsToShare()
	{
		//Discarded unreachable code: IL_0037
		try
		{
			CreateNetShareInfo(m_share).GrantClusterPermissionToShare(base.Cluster);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.FileShareWitnessSettingPermissionsFailed_Text,
				m_share
			});
		}
	}

	private void GrantClusterPermissionsToFileSystem()
	{
		//Discarded unreachable code: IL_006f
		Exception ex = null;
		try
		{
			string clusterCnoName = GetClusterCnoName(base.Cluster);
			NetShareInfo netShareInfo = CreateNetShareInfo(m_share);
			string path = BuildSharePath(netShareInfo.AdminSharePath);
			DirectorySecurity accessControl = Directory.GetAccessControl(path, AccessControlSections.Access);
			accessControl.AddAccessRule(new FileSystemAccessRule(clusterCnoName, accessRights, AccessControlType.Allow));
			Directory.SetAccessControl(path, accessControl);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.FileShareWitnessSettingPermissionsFailed_Text,
				m_fileSharePath
			});
		}
	}

	private static NetShareInfo CreateNetShareInfo(string fileSharePath)
	{
		string serverName = null;
		string shareName = null;
		string subDir = null;
		if (!NetShareInfo.TryParseFileShare(fileSharePath, out serverName, out shareName, out subDir))
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.InvalidFileSharePathArg_Text,
				fileSharePath
			});
		}
		return NetShareInfo.Get(string.Format(CultureInfo.InvariantCulture, "\\\\{0}", serverName), shareName);
	}

	private void RemoveCNOPermissionsFromShare()
	{
		//Discarded unreachable code: IL_0037
		try
		{
			CreateNetShareInfo(m_share).RemoveClusterPermissionFromShare(base.Cluster);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.FileShareWitnessFailedToRemoveCNOPermissionsFormat_Text,
				m_share
			});
		}
	}

	private void RemoveCNOPermissionsFromFileSystem()
	{
		//Discarded unreachable code: IL_0070
		Exception ex = null;
		try
		{
			string clusterCnoName = GetClusterCnoName(base.Cluster);
			NetShareInfo netShareInfo = CreateNetShareInfo(m_share);
			string path = BuildSharePath(netShareInfo.AdminSharePath);
			DirectorySecurity accessControl = Directory.GetAccessControl(path, AccessControlSections.Access);
			accessControl.RemoveAccessRule(new FileSystemAccessRule(clusterCnoName, accessRights, AccessControlType.Allow));
			Directory.SetAccessControl(path, accessControl);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.FileShareWitnessFailedToRemoveCNOPermissionsFromFileSystemFormat_Text,
				m_fileSharePath
			});
		}
	}

	private ClusterResource CreateFileShareWitnessResource()
	{
		string resourceName = ResourceHelp.GenerateResourceName(base.Cluster, Resources.FileShareWitnessResourceName_Text);
		ClusterGroup coreClusterGroup = base.Cluster.GetCoreClusterGroup();
		string resourceTypeName = "File Share Witness";
		return coreClusterGroup.CreateResource(resourceName, resourceTypeName);
	}

	private void ConfigureFileShareWitnessResource(ClusterResource fileShareWitness)
	{
		PropertyCollection privateProperties = fileShareWitness.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		if (m_userName != null)
		{
			privateProperties.GetProperty("UserName").Value = m_userName;
			privateProperties.GetProperty("Password").Value = m_password;
		}
		privateProperties.GetProperty(sharePathPropertyName).Value = m_fileSharePath;
		privateProperties.SaveChanges();
	}

	private string BuildSharePath(string shareAdminPath)
	{
		string result = shareAdminPath;
		if (m_subDir != null && m_subDir.Length > 0)
		{
			result = shareAdminPath + m_subDir;
		}
		return result;
	}

	private void SaveNTFSPermsCleanupNeeded(ClusterResource fileShareWitness)
	{
		SetSettingsValue(fileShareWitness, ntfsCleanupNeeded);
	}

	private void SaveSharePermsCleanupNeeded(ClusterResource fileShareWitness)
	{
		SetSettingsValue(fileShareWitness, shareCleanupNeeded);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsNTFSPermsCleanupNeeded(ClusterResource fileShareWitness)
	{
		return GetSettingsValue(fileShareWitness, ntfsCleanupNeeded);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsSharePermsCleanupNeeded(ClusterResource fileShareWitness)
	{
		return GetSettingsValue(fileShareWitness, shareCleanupNeeded);
	}

	private static string VerifyFileShare(string fileSharePath)
	{
		//Discarded unreachable code: IL_00ac
		string text = null;
		string text2 = null;
		string text3 = null;
		if (string.IsNullOrEmpty(fileSharePath))
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.FileShareWitnessVerifyingSettingsFailed_Text });
		}
		text = null;
		text2 = null;
		text3 = null;
		if (!NetShareInfo.TryParseFileShare(fileSharePath, out text, out text2, out text3))
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidFileSharePathArg_Text,
				fileSharePath
			});
		}
		text = DnsSupport.PromoteLocalComputerName(text);
		string text4 = string.Format(CultureInfo.CurrentCulture, "\\\\{0}\\{1}", text, text2);
		try
		{
			NetShareInfo netShareInfo = null;
			netShareInfo = CreateNetShareInfo(text4);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Failed to get the share info for {0}!", text4);
			throw ExceptionHelp.Build<ClusterInputValidationException>(ex, new string[2]
			{
				Resources.InvalidFileSharePathArg_Text,
				text4
			});
		}
		if (!string.IsNullOrEmpty(text3))
		{
			text4 += text3;
		}
		return text4;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool IsCurrentQuorumShare(Cluster cluster, string fileSharePath)
	{
		string text = null;
		string text2 = null;
		string text3 = null;
		string text4 = null;
		string text5 = null;
		string text6 = null;
		QuorumSettings quorumSettings = cluster.GetQuorumSettings();
		if (quorumSettings.QuorumType == QuorumType.FileShareWitness)
		{
			FileShareQuorumSettings obj = (FileShareQuorumSettings)quorumSettings;
			text = null;
			text2 = null;
			text3 = null;
			NetShareInfo.ParseFileShare(obj.m_fileSharePath, out text, out text2, out text3);
			text4 = null;
			text5 = null;
			text6 = null;
			NetShareInfo.ParseFileShare(fileSharePath, out text4, out text5, out text6);
			if (text2.Equals(text5, StringComparison.OrdinalIgnoreCase) && ServerNamesEqual(text, text4))
			{
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool ServerNamesEqual(string server1, string server2)
	{
		string text = null;
		string text2 = null;
		string text3 = null;
		string text4 = null;
		if (server1.Equals(server2, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (DnsSupport.IsComputerNameFullyQualified(server1) && DnsSupport.IsComputerNameFullyQualified(server2))
		{
			return false;
		}
		if (!DnsSupport.IsComputerNameFullyQualified(server1) && !DnsSupport.IsComputerNameFullyQualified(server2))
		{
			return false;
		}
		text = null;
		text2 = null;
		text3 = null;
		text4 = null;
		DnsSupport.ParseFullyQualifiedComputerName(server1, out text, out text2);
		DnsSupport.ParseFullyQualifiedComputerName(server2, out text3, out text4);
		return text.Equals(text3, StringComparison.OrdinalIgnoreCase);
	}

	private void SetSettingsValue(ClusterResource fileShareWitness, string valueName)
	{
		ClusterRegistryKey registryKey = fileShareWitness.GetRegistryKey(RegistryRights.SetValue);
		try
		{
			registryKey.SetValue(valueName, 1u);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Saving {0} value.", valueName);
		}
		finally
		{
			registryKey.Close();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool GetSettingsValue(ClusterResource fileShareWitness, string valueName)
	{
		bool result = false;
		ClusterRegistryKey registryKey = fileShareWitness.GetRegistryKey(RegistryRights.ExecuteKey);
		try
		{
			result = (uint)registryKey.GetValue(valueName) == 1;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Looking for {0} value.", valueName);
		}
		finally
		{
			registryKey.Close();
		}
		return result;
	}

	private static void ValidateSharePath(string fileSharePath)
	{
		//Discarded unreachable code: IL_0042, IL_0069
		string text = "\\File Share Witness.tmp";
		string path = fileSharePath + text;
		try
		{
			Directory.CreateDirectory(path);
			Directory.Delete(path);
		}
		catch (UnauthorizedAccessException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Current user failed to create a folder on share '{0}' because they lacked access.", fileSharePath);
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidFileShareAccess_Text,
				fileSharePath
			});
		}
		catch (IOException caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "Current user failed to create a folder on share '{0}'.", fileSharePath);
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidFileShareIO_Text,
				fileSharePath
			});
		}
	}

	private unsafe static void ValidateSharePathFromCluster(Cluster cluster, string fileSharePath)
	{
		//Discarded unreachable code: IL_0102, IL_0104
		//IL_000f: Expected I, but got I8
		//IL_0013: Expected I, but got I8
		//IL_0016: Expected I, but got I8
		//IL_0045: Expected I, but got I8
		//IL_00ba: Expected I, but got I8
		//IL_0135: Expected I, but got I8
		//IL_0140: Expected I, but got I8
		ResourceTypeControlExecutor resourceTypeControlExecutor = null;
		UnmanagedBuffer unmanagedBuffer = null;
		Exception ex = null;
		ClusterResourceType clusterResourceType = null;
		ushort* ptr = null;
		ushort* ptr2 = null;
		CClusPropList* ptr3 = null;
		uint num = 0u;
		try
		{
			string resourceTypeName = "File Share Witness";
			clusterResourceType = cluster.GetResourceType(resourceTypeName);
			CClusPropList* ptr4 = (CClusPropList*)global::_003CModule_003E.@new(112uL);
			CClusPropList* ptr5;
			try
			{
				ptr5 = ((ptr4 == null) ? null : global::_003CModule_003E.CClusPropList_002E_007Bctor_007D(ptr4, 1));
				CClusPropList* ptr6 = ptr5;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.delete(ptr4);
				throw;
			}
			ptr3 = ptr5;
			ptr = InteropHelp.StringToWstr(fileSharePath);
			ptr2 = InteropHelp.StringToWstr(sharePathPropertyName);
			ushort* pwszName = ptr2;
			ushort* intPtr = ptr;
			num = global::_003CModule_003E.CClusPropList_002EScAddProp(ptr5, pwszName, intPtr, intPtr);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.StringSaveFail_Text,
					sharePathPropertyName
				});
			}
			resourceTypeControlExecutor = new ResourceTypeControlExecutor(clusterResourceType.Name, cluster);
			ulong size = *(ulong*)((ulong)(nint)ptr5 + 24uL);
			CLUSPROP_LIST* pMem = (CLUSPROP_LIST*)(*(ulong*)((ulong)(nint)ptr5 + 8uL));
			unmanagedBuffer = new UnmanagedBuffer(pMem, size);
			resourceTypeControlExecutor.ExecuteInControl(33554993u, unmanagedBuffer);
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null)
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(firstException, new string[2]
				{
					Resources.InvalidFileSharePathArg_Text,
					fileSharePath
				});
			}
			throw;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr2);
			InteropHelp.FreeWstr(ptr);
			CClusPropList* ptr7 = ptr3;
			if (ptr3 != null)
			{
				try
				{
					global::_003CModule_003E.CClusPropList_002EDeletePropList(ptr3);
				}
				catch
				{
					//try-fault
					global::_003CModule_003E.___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<CClusPropValueList*, void>)(&global::_003CModule_003E.CClusPropValueList_002E_007Bdtor_007D), (void*)((ulong)(nint)ptr7 + 64uL));
					throw;
				}
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList((CClusPropValueList*)((ulong)(nint)ptr3 + 64uL));
				global::_003CModule_003E.delete(ptr3);
			}
			((IDisposable)clusterResourceType)?.Dispose();
		}
	}

	private static void VerifyFileSharePathFormat(string fileSharePath)
	{
		string serverName = null;
		string shareName = null;
		string subDir = null;
		if (!NetShareInfo.TryParseFileShare(fileSharePath, out serverName, out shareName, out subDir))
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(new string[2]
			{
				Resources.InvalidFileSharePathArg_Text,
				fileSharePath
			});
		}
	}

	internal static string GetClusterCnoName(Cluster cluster)
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}$", Utilities.GetClusterActiveDirectoryDomain(cluster), cluster.Name);
	}

	public FileShareQuorumSettings(ClusterResource fileShareResource)
		: base(fileShareResource.Cluster)
	{
		m_resource = fileShareResource;
		m_fileSharePath = GetShareFromResource(fileShareResource);
		Construct();
	}

	public FileShareQuorumSettings(Cluster cluster, string fileShare, string userName, string password)
		: base(cluster)
	{
		m_userName = userName;
		m_password = password;
		m_fileSharePath = fileShare;
		m_resource = null;
		Construct();
	}

	public override void Configure()
	{
		//Discarded unreachable code: IL_0070, IL_00c5, IL_01a0
		Exception ex = null;
		Exception ex2 = null;
		Exception ex3 = null;
		bool flag = false;
		bool flag2 = false;
		ReportOperationProcess(10, Resources.FileShareWitnessConfiguringQuorumFormat_Text, m_share);
		ClusterResource clusterResource = null;
		try
		{
			ReportOperationProcess(20, Resources.CreatingFileShareWitnessResource_Text);
			clusterResource = CreateFileShareWitnessResource();
			try
			{
				ReportOperationProcess(30, Resources.ConfiguringFileShareWitnessResource_Text);
				ConfigureFileShareWitnessResource(clusterResource);
			}
			catch (Exception caughtException)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
				if (firstException != null && firstException.NativeErrorCode == -2147024891)
				{
					flag = true;
					goto IL_0072;
				}
				throw;
			}
			goto IL_0121;
			IL_0072:
			if (flag)
			{
				try
				{
					ReportOperationProcess(40, Resources.FileShareWitnessGrantingAccessFormat_Text, m_share);
					GrantClusterPermissionsToShare();
					SetSettingsValue(clusterResource, shareCleanupNeeded);
				}
				catch (Exception ex4)
				{
					ExceptionHelp.HandleSpecialExceptions(ex4);
					throw ExceptionHelp.Build<ClusterInputValidationException>(ex4, new string[2]
					{
						Resources.FileShareWitnessGrantingShareAccessFailed_Text,
						m_share
					});
				}
				try
				{
					ReportOperationProcess(50, Resources.ConfiguringFileShareWitnessResource_Text);
					ConfigureFileShareWitnessResource(clusterResource);
				}
				catch (Exception)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				ReportOperationProcess(60, Resources.FileShareWitnessGrantingAccessToFileSystemFormat_Text, m_fileSharePath);
				GrantClusterPermissionsToFileSystem();
				SetSettingsValue(clusterResource, ntfsCleanupNeeded);
				ReportOperationProcess(70, Resources.ConfiguringFileShareWitnessResource_Text);
				ConfigureFileShareWitnessResource(clusterResource);
			}
			goto IL_0121;
			IL_0121:
			ReportOperationProcess(80, Resources.FileShareWitnessBringResourceOnline_Text);
			clusterResource.BringOnline();
			ReportOperationProcess(90, Resources.FileShareWitnessSettingAsQuorum_Text);
			base.Cluster.SetMajorityQuorum(clusterResource, null);
			m_resource = clusterResource;
		}
		catch (Exception innerException)
		{
			try
			{
				if (clusterResource != null)
				{
					try
					{
						RemoveCNOPermissionsFromFileSystem();
						RemoveCNOPermissionsFromShare();
					}
					finally
					{
						clusterResource.Delete();
					}
				}
			}
			catch (Exception caughtException2)
			{
				ExceptionHelp.HandleSpecialExceptions(caughtException2);
			}
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.FileShareWitnessConfigurationFailedFormat_Text,
				m_share
			});
		}
	}

	public override void Cleanup()
	{
		ClusterResource clusterResource = null;
		Exception ex = null;
		ClusterResource clusterResource2 = null;
		try
		{
			bool flag = true;
			bool flag2 = true;
			ClusterResource resource = m_resource;
			flag2 = GetSettingsValue(resource, ntfsCleanupNeeded);
			clusterResource2 = m_resource;
			flag = GetSettingsValue(clusterResource2, shareCleanupNeeded);
			string fileSharePath = m_fileSharePath;
			if (IsCurrentQuorumShare(base.Cluster, fileSharePath))
			{
				if (flag)
				{
					clusterResource = base.Cluster.GetQuorumResource();
					SetSettingsValue(clusterResource, shareCleanupNeeded);
				}
				flag = false;
			}
			ReportOperationProcess(40, Resources.FileShareWitnessDeletingResource_Text);
			m_resource.Delete();
			if (flag2)
			{
				ReportOperationProcess(70, Resources.FileShareWitnessRevokingFileSystemPermissionsFormat_Text, m_share);
				RemoveCNOPermissionsFromFileSystem();
			}
			if (flag)
			{
				ReportOperationProcess(80, Resources.FileShareWitnessRevokingPermissionsFormat_Text, m_share);
				RemoveCNOPermissionsFromShare();
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Cleaning up file share witness quorum");
		}
	}

	public override void VerifySettings()
	{
		VerifyFileSharePathFormat(m_share);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool AreQuorumSettingsEqual(QuorumSettings settings)
	{
		bool flag = false;
		if (QuorumType == settings.QuorumType && settings is FileShareQuorumSettings fileShareQuorumSettings)
		{
			string fileSharePath = m_fileSharePath;
			string fileSharePath2 = fileShareQuorumSettings.m_fileSharePath;
			flag = 0 == string.Compare(fileSharePath2, fileSharePath, StringComparison.OrdinalIgnoreCase) || flag;
		}
		return flag;
	}

	public static string VerifyShare(Cluster cluster, string share)
	{
		VerifyFileSharePathFormat(share);
		try
		{
			ValidateSharePathFromCluster(cluster, share);
			return share;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "The cluster does not currently have access to the file share {0}. Granting the cluster access will be tried next.", share);
		}
		string text = VerifyFileShare(share);
		if (IsCurrentQuorumShare(cluster, text))
		{
			return text;
		}
		ValidateSharePath(text);
		return text;
	}
}
