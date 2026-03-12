using System;
using System.Security.AccessControl;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IClusterExecutor
{
	void ExecuteOnCommonProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList);

	void ExecuteOnCommonReadOnlyProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList);

	void ExecuteOnPrivateProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList);

	void ExecuteOnProperties(SafeClusterHandle clusterHandle, int controlCode, Action<IntPtr, int> propertyList);

	void ExecuteOnControlCode(SafeClusterHandle clusterHandle, int controlCode, Action<IntPtr, int> controlCodeCallBack);

	void ExecuteOnControlCode(SafeClusterHandle clusterHandle, int controlCode, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null);

	SafeClusterHandle OpenCluster(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess);

	string GetConnectedNodeName(SafeClusterHandle clusterHandle);

	SafeClusterKeyHandle GetClusterKey(SafeClusterHandle clusterHandle, RegistryRights queryValues);

	string GetClusterRegistryValue(SafeClusterKeyHandle clusterKey, string groupName);

	void ClusterRegCloseKey(IntPtr clusterKey);

	string GetClusterInformation(SafeClusterHandle clusterHandle, ref NativeMethods.CLUSTERVERSIONINFO clusterVersionInfo);

	QuorumData GetClusterQuorumConfiguration(SafeClusterHandle clusterHandle);

	void DestroyCluster(SafeClusterHandle clusterHandle, NativeMethods.ClusterSetupProgressCallback nativeCallback, bool deleteComputerObjects);
}
