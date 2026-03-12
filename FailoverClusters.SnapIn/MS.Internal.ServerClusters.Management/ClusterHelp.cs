using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.FailoverClusters.WinForms;

namespace MS.Internal.ServerClusters.Management;

internal static class ClusterHelp
{
	internal class CompareString : IComparer<string>
	{
		public int Compare(string s1, string s2)
		{
			return NativeMethods.StrCmpLogicalW(s1, s2);
		}
	}

	internal delegate void NoParamsDelegate();

	internal static byte[] Int64ToByteArray(long int64)
	{
		byte[] array = new byte[8];
		for (int i = 0; i < 8; i++)
		{
			array[array.Length - 1 - i] = (byte)(int64 & 0xFF);
			int64 >>= 8;
		}
		return array;
	}

	internal static byte[] Int32ToByteArray(int int32)
	{
		byte[] array = new byte[4];
		for (int i = 0; i < 4; i++)
		{
			array[array.Length - 1 - i] = (byte)((uint)int32 & 0xFFu);
			int32 >>= 8;
		}
		return array;
	}

	internal static void RecycleResourceAfterPropertiesSave(ClusterResource resource, INotifyUser notifyUser, bool offlineDependencies)
	{
		string displayName = GetDisplayName(resource, notifyUser);
		string text = string.Format(CultureInfo.CurrentCulture, Resources.SavePropertiesRequiresRecycle_Text, displayName);
		if (notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, text) != DialogResult.Yes)
		{
			return;
		}
		CluadminWaitDialog waitDialog = CluadminWaitDialog.Create(Resources.RecyclingResource_Text, string.Empty);
		using (waitDialog)
		{
			waitDialog.DisplayDelay = new TimeSpan(0, 0, 3);
			string resourceName = string.Empty;
			try
			{
				waitDialog.ShowDialog(notifyUser, delegate
				{
					resourceName = resource.Name;
					OfflineManager offlineManager = OfflineManager.Create(resource);
					if (resource.State == ResourceState.Online)
					{
						waitDialog.StatusText = string.Format(CultureInfo.CurrentCulture, Resources.TakingResourceOffline_Text, resource.DisplayName);
						offlineManager.TakeOffline(offlineDependencies ? OfflineOption.OfflineDependencies : OfflineOption.None);
						waitDialog.StatusText = string.Format(CultureInfo.CurrentCulture, Resources.BringingResourceOnline_Text, resource.DisplayName);
						offlineManager.BeginBringOnline();
					}
				});
				if (!waitDialog.IsCanceled)
				{
					displayName = GetDisplayName(resource, notifyUser);
					notifyUser.ShowInformational(Resources.ResourceBackOnline_Text, new object[1] { displayName });
				}
			}
			catch (Exception ex)
			{
				ExceptionHelp.LogException(ex, "Error recycling resource '{0}'", resourceName);
				notifyUser.ShowError(ex, Resources.ErrorRecyclingResource_Text, new object[1] { displayName });
			}
		}
	}

	private static string GetDisplayName(ClusterResource resource, INotifyUser notifyUser)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Resource_GetDisplayName_Text, string.Empty);
		using (cluadminWaitDialog)
		{
			string result = cluadminWaitDialog.ShowDialog(notifyUser, GetResourceDisplayName, resource);
			if (cluadminWaitDialog.IsCanceled)
			{
				throw new OperationCanceledException();
			}
			return result;
		}
	}

	private static string GetResourceDisplayName(CluadminWaitDialog waitDialog, ClusterResource resource)
	{
		return resource.DisplayName;
	}
}
