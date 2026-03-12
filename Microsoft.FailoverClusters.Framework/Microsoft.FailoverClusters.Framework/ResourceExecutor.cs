using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class ResourceExecutor : IResourceExecutor
{
	protected const int DefaultBufferSize = 4096;

	public void ExecuteOnResource(SafeClusterHandle clusterHandle, ClusterAccessRights clusteraccessRights, Guid id, string name, Action<SafeClusterResourceHandle> actionOnResource)
	{
		if (id == Guid.Empty && name == null)
		{
			throw new ArgumentException("id and name cannot be both null");
		}
		if (clusterHandle == null)
		{
			return;
		}
		ClusterAccessRights grantedAccess;
		SafeClusterResourceHandle safeClusterResourceHandle = ((id != Guid.Empty) ? NativeMethods.OpenClusterResourceEx(clusterHandle, id.ToString(), clusteraccessRights, out grantedAccess) : NativeMethods.OpenClusterResourceEx(clusterHandle, name, clusteraccessRights, out grantedAccess));
		if (safeClusterResourceHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == NativeMethods.ErrorCode.ResourceNotFound.ToInt())
			{
				throw new ClusterObjectNotFoundException(name, id, typeof(Resource));
			}
			throw ExceptionHelper.ClusterObjectLoadFailedException(name, id, lastWin32Error);
		}
		try
		{
			actionOnResource.SafeCall(safeClusterResourceHandle);
		}
		finally
		{
			safeClusterResourceHandle.Dispose();
		}
	}

	public Guid GetResourceId(SafeClusterResourceHandle resourceHandle, string resourceName)
	{
		Guid guid = Guid.Empty;
		ExecuteOnControlCode(resourceHandle, NativeMethods.CLUSCTL_RESOURCE_GET_ID, resourceName, delegate(IntPtr buffer, int bufferSize)
		{
			string g = Marshal.PtrToStringUni(buffer);
			guid = new Guid(g);
		});
		return guid;
	}

	private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, int controlCode, string resourceName, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
	{
		ExecuteOnControlCode(resourceHandle, controlCode, resourceName, null, controlCodeCallBack, invalidFunctionCallback);
	}

	private void ExecuteOnControlCode(SafeClusterResourceHandle resourceHandle, int controlCode, string resourceName, NativeMethods.UnmanagedBuffer inBuffer, Action<IntPtr, int> controlCodeCallBack, Action invalidFunctionCallback = null)
	{
		IntPtr intPtr = NativeMethods.Alloc(4096);
		int outBufferSize = 4096;
		int bytesReturned = 0;
		try
		{
			int num = NativeMethods.ClusterResourceControl(resourceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
			if (NativeMethods.ErrorCode.MoreData.IsEqual(num))
			{
				intPtr = NativeMethods.ReAlloc(intPtr, bytesReturned);
				outBufferSize = bytesReturned;
				num = NativeMethods.ClusterResourceControl(resourceHandle, SafeClusterNodeHandle.InvalidHandle, controlCode, inBuffer?.IntPtr ?? IntPtr.Zero, inBuffer?.Size ?? 0, intPtr, outBufferSize, ref bytesReturned);
			}
			if (NativeMethods.ErrorCode.InvalidFunction.IsEqual(num) && invalidFunctionCallback != null)
			{
				invalidFunctionCallback();
				return;
			}
			if (NativeMethods.ErrorCode.DeletePending.IsEqual(num))
			{
				throw new ClusterObjectDeletingException();
			}
			if (NativeMethods.ErrorCode.ResourceNotAvailable.IsEqual(num) || NativeMethods.ErrorCode.ResourceNotFound.IsEqual(num))
			{
				throw new ClusterObjectNotFoundException(resourceName, new Win32Exception(num));
			}
			if (NativeMethods.ErrorCode.ResourceFailed.IsEqual(num))
			{
				throw new ClusterResourceFailedException();
			}
			if (NativeMethods.ErrorCode.PropertiesNotAvailable.IsEqual(num))
			{
				throw new ClusterPropertiesNotAvailableException();
			}
			if (NativeMethods.ErrorCode.GroupMoving.IsEqual(num))
			{
				throw new ClusterGroupMovingException();
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
}
