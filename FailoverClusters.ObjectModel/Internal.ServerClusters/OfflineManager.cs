using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class OfflineManager
{
	private ClusterResource m_offlineResource;

	private bool m_restoreGroupResourcesState;

	private ResourceState m_offlineResourceInitialState;

	private List<ClusterResource> resourcesToGoOnLine;

	private OfflineManager(ClusterResource resource)
	{
		if (resource == null)
		{
			throw new ArgumentNullException("resource");
		}
		m_offlineResource = resource;
		m_restoreGroupResourcesState = false;
		m_offlineResourceInitialState = ResourceState.Unknown;
		resourcesToGoOnLine = new List<ClusterResource>();
	}

	private void OfflineResources(ClusterResource resource, OfflineOption option)
	{
		resource.TakeOffline();
		if (option != OfflineOption.OfflineDependencies)
		{
			return;
		}
		foreach (ClusterResource dependency in resource.GetDependencies())
		{
			OfflineResources(dependency, option);
		}
	}

	private void CaptureOnlineResources()
	{
		int num = ((m_offlineResource.HasDependencies() || m_offlineResource.HasDependents()) ? 1 : 0);
		byte b = (byte)num;
		m_restoreGroupResourcesState = b != 0;
		if (b == 0)
		{
			m_offlineResourceInitialState = m_offlineResource.State;
			return;
		}
		using ClusterGroup clusterGroup = m_offlineResource.GetOwnerGroup();
		IEnumerator<ClusterResource> enumerator = clusterGroup.GetResources().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ClusterResource current = enumerator.Current;
				if (current.State == ResourceState.Online)
				{
					resourcesToGoOnLine.Add(current);
				}
			}
		}
		finally
		{
			IEnumerator<ClusterResource> enumerator2 = enumerator;
			IDisposable disposable = enumerator;
			enumerator?.Dispose();
		}
	}

	private void OnlineResources([MarshalAs(UnmanagedType.U1)] bool waitForOnline)
	{
		if (!m_restoreGroupResourcesState)
		{
			if (m_offlineResourceInitialState == ResourceState.Online)
			{
				ClusterResource offlineResource = m_offlineResource;
				if (waitForOnline)
				{
					offlineResource.BringOnline();
				}
				else
				{
					offlineResource.BeginBringOnline();
				}
			}
			return;
		}
		m_offlineResource.Refresh();
		List<ClusterResource>.Enumerator enumerator = resourcesToGoOnLine.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				enumerator.Current.Refresh();
			}
			while (enumerator.MoveNext());
		}
		List<ClusterResource> list = new List<ClusterResource>();
		FindResourcesToOnline(list, resourcesToGoOnLine);
		List<ClusterResource>.Enumerator enumerator2 = list.GetEnumerator();
		if (!enumerator2.MoveNext())
		{
			return;
		}
		do
		{
			ClusterResource current = enumerator2.Current;
			if (waitForOnline)
			{
				current.BringOnline();
			}
			else
			{
				current.BeginBringOnline();
			}
		}
		while (enumerator2.MoveNext());
	}

	private static void OnlineResource(ClusterResource resource, [MarshalAs(UnmanagedType.U1)] bool waitForOnline)
	{
		if (waitForOnline)
		{
			resource.BringOnline();
		}
		else
		{
			resource.BeginBringOnline();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool FindResourcesToOnline(List<ClusterResource> listToOnline, List<ClusterResource> candidateResources)
	{
		//Discarded unreachable code: IL_0092
		bool result = false;
		List<ClusterResource>.Enumerator enumerator = candidateResources.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ClusterResource current = enumerator.Current;
			try
			{
				if (current.State != ResourceState.Online && !FindResourcesToOnline(listToOnline, new List<ClusterResource>(current.GetDependents())))
				{
					result = true;
					if (!listToOnline.Contains(current))
					{
						listToOnline.Add(current);
					}
				}
			}
			catch (Exception caughtException)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
				if (firstException != null && (firstException.NativeErrorCode == -2147024894 || firstException.NativeErrorCode == -2147019890 || firstException.NativeErrorCode == -2147019889 || firstException.NativeErrorCode == -2147019854))
				{
					continue;
				}
				throw;
			}
		}
		return result;
	}

	public void TakeOffline()
	{
		CaptureOnlineResources();
		OfflineResources(m_offlineResource, OfflineOption.None);
	}

	public void TakeOffline(OfflineOption option)
	{
		CaptureOnlineResources();
		OfflineResources(m_offlineResource, option);
	}

	public static OfflineManager Create(ClusterResource resource)
	{
		return new OfflineManager(resource);
	}

	public void BeginBringOnline()
	{
		OnlineResources(waitForOnline: false);
	}

	public void BringOnline()
	{
		OnlineResources(waitForOnline: true);
	}
}
