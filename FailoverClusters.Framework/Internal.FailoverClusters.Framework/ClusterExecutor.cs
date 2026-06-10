using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class ClusterExecutor : IClusterExecutor
{
	private const int DefaultBufferSize = 4096;

	private const int QuorumResourceNameDefaultSize = 128;

	private const int QuorumDeviceNameDefaultSize = 128;

	public void ExecuteOnCommonProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList)
	{
		ExecuteOnProperties(clusterHandle, NativeMethods.CLUSCTL_CLUSTER_GET_COMMON_PROPERTIES, commonPropList);
	}

	public void ExecuteOnCommonReadOnlyProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList)
	{
		ExecuteOnProperties(clusterHandle, NativeMethods.CLUSCTL_CLUSTER_GET_RO_COMMON_PROPERTIES, commonPropList);
	}

	public void ExecuteOnPrivateProperties(SafeClusterHandle clusterHandle, Action<IntPtr, int> commonPropList)
	{
		ExecuteOnProperties(clusterHandle, NativeMethods.CLUSCTL_CLUSTER_GET_PRIVATE_PROPERTIES, commonPropList);
	}

	public void ExecuteOnProperties(SafeClusterHandle clusterHandle, int controlCode, Action<IntPtr, int> propertyList)
	{
		try
		{
			ExecuteOnControlCode(clusterHandle, controlCode, propertyList);
		}
		catch (ClusterControlCodeException innerException)
		{
			throw new ClusterGetPropertiesFailedException(innerException);
		}
	}

	public void ExecuteOnControlCode(SafeClusterHandle clusterHandle, int controlCode, Action<IntPtr, int> controlCodeCallBack)
	{
		IntPtr intPtr = NativeMethods.Alloc(4096);
		int outBufferSize = 4096;
		int bytesReturned = 0;
		try
		{
			int num = NativeMethods.ClusterControl(clusterHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
			if (num == NativeMethods.ErrorCode.MoreData.ToInt())
			{
				intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
				outBufferSize = bytesReturned;
				num = NativeMethods.ClusterControl(clusterHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, IntPtr.Zero, 0, intPtr, outBufferSize, ref bytesReturned);
			}
			if (num != NativeMethods.ErrorCode.None.ToInt() && num != NativeMethods.ErrorCode.IOPending.ToInt())
			{
				ClusterLog.LogWarning("Failed to execute control code {0}. Error = {1}", controlCode, num);
				throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
			}
			controlCodeCallBack.SafeCall(intPtr, bytesReturned);
		}
		finally
		{
			NativeMethods.Free(intPtr);
		}
	}

	public void ExecuteOnControlCode(SafeClusterHandle clusterHandle, int controlCode, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
	{
		IntPtr intPtr = NativeMethods.Alloc(4096);
		int outBufferSize = 4096;
		int bytesReturned = 0;
		try
		{
			int num = NativeMethods.ClusterControl(clusterHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
			if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
			{
				intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
				outBufferSize = bytesReturned;
				num = NativeMethods.ClusterControl(clusterHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
			}
			if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
			{
				invalidFunctionCallback();
				return;
			}
			if (NativeMethods.ErrorCode.PropertiesNotAvailable.IsEqual(num))
			{
				throw new ClusterPropertiesNotAvailableException();
			}
			if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
			{
				throw new ClusterControlCodeException(controlCode, new Win32Exception(num));
			}
			controlCodeCallBack.SafeCall(intPtr, bytesReturned);
		}
		finally
		{
			NativeMethods.Free(intPtr);
		}
	}

	public SafeClusterHandle OpenCluster(string clusterName, ClusterAccessRights desiredAccess, out ClusterAccessRights grantedAccess)
	{
		SafeClusterHandle safeClusterHandle = NativeMethods.OpenClusterEx(clusterName, desiredAccess, out grantedAccess);
		if (safeClusterHandle.IsInvalid)
		{
			throw ExceptionHelper.Build<ClusterOpenClusterException>(Marshal.GetLastWin32Error());
		}
		return safeClusterHandle;
	}

	public string GetConnectedNodeName(SafeClusterHandle clusterHandle)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int informationLength = 255;
		int clusterConnectionInformation = NativeMethods.GetClusterConnectionInformation(clusterHandle, ClusterConnectionInformationClass.ConnectedNode, stringBuilder, ref informationLength);
		if (clusterConnectionInformation != NativeMethods.ErrorCode.None.ToInt())
		{
			ClusterLog.LogWarning("Failed to get connected to node name. Error = {0}", clusterConnectionInformation);
			throw new ClusterConnectionInformationException(new Win32Exception(clusterConnectionInformation));
		}
		return stringBuilder.ToString();
	}

	public SafeClusterKeyHandle GetClusterKey(SafeClusterHandle clusterHandle, RegistryRights queryValues)
	{
		SafeClusterKeyHandle clusterKey = NativeMethods.GetClusterKey(clusterHandle, RegistryRights.QueryValues);
		if (clusterKey.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			ClusterLog.LogWarning("Failed to get cluster registry key");
			throw new ClusterRegistryException(new Win32Exception(lastWin32Error));
		}
		return clusterKey;
	}

	public string GetClusterRegistryValue(SafeClusterKeyHandle clusterKey, string groupName)
	{
		int dataSize = 100;
		StringBuilder stringBuilder = new StringBuilder(dataSize);
		int num = NativeMethods.ClusterRegQueryValue(clusterKey, groupName, IntPtr.Zero, stringBuilder, ref dataSize);
		if (num != NativeMethods.ErrorCode.None.ToInt())
		{
			ClusterLog.LogWarning("Failed to get cluster registry value. Error = {0}", num);
			throw new ClusterRegistryException(new Win32Exception(num));
		}
		return stringBuilder.ToString();
	}

	public void ClusterRegCloseKey(IntPtr clusterKey)
	{
		NativeMethods.ClusterRegCloseKey(clusterKey);
	}

	public string GetClusterInformation(SafeClusterHandle clusterHandle, ref NativeMethods.CLUSTERVERSIONINFO clusterVersionInfo)
	{
		int clusterNameSize = 512;
		StringBuilder stringBuilder = new StringBuilder(clusterNameSize);
		int clusterInformation = NativeMethods.GetClusterInformation(clusterHandle, stringBuilder, ref clusterNameSize, ref clusterVersionInfo);
		if (NativeMethods.ErrorCode.None.Equals(clusterInformation))
		{
			ClusterLog.LogWarning("Failed to get cluster information. Error = {0}", clusterInformation);
			throw new ClusterConnectionInformationException(new Win32Exception(clusterInformation));
		}
		return stringBuilder.ToString();
	}

	public QuorumData GetClusterQuorumConfiguration(SafeClusterHandle clusterHandle)
	{
		StringBuilder stringBuilder = new StringBuilder(128);
		StringBuilder stringBuilder2 = new StringBuilder(128);
		int resourceNameSize = stringBuilder.Capacity;
		int deviceNameSize = stringBuilder2.Capacity;
		int quorumLogSize;
		int clusterQuorumResource = NativeMethods.GetClusterQuorumResource(clusterHandle, stringBuilder, ref resourceNameSize, stringBuilder2, ref deviceNameSize, out quorumLogSize);
		if (NativeMethods.ErrorCode.MoreData.IsEqual(clusterQuorumResource))
		{
			stringBuilder = new StringBuilder(++resourceNameSize);
			stringBuilder2 = new StringBuilder(++deviceNameSize);
			clusterQuorumResource = NativeMethods.GetClusterQuorumResource(clusterHandle, stringBuilder, ref resourceNameSize, stringBuilder2, ref deviceNameSize, out quorumLogSize);
		}
		if (!NativeMethods.ErrorCode.None.IsEqual(clusterQuorumResource))
		{
			ClusterLog.LogWarning("Failed to get cluster quorum resource name. Error = {0}", clusterQuorumResource);
			throw ExceptionHelper.Build(clusterQuorumResource);
		}
		return new QuorumData
		{
			QuorumResourceName = stringBuilder.ToString(),
			QuorumDeviceName = stringBuilder2.ToString(),
			QuorumType = quorumLogSize
		};
	}

	public void DestroyCluster(SafeClusterHandle clusterHandle, NativeMethods.ClusterSetupProgressCallback nativeCallback, bool deleteComputerObjects)
	{
		int num = NativeMethods.DestroyCluster(clusterHandle, nativeCallback, IntPtr.Zero, deleteComputerObjects);
		if (!NativeMethods.ErrorCode.None.IsEqual(num) && !NativeMethods.ErrorCode.IOPending.IsEqual(num))
		{
			ClusterLog.LogWarning("Failed to get destroy cluster. Error = {0}", num);
			throw ExceptionHelper.Build(num);
		}
	}
}

