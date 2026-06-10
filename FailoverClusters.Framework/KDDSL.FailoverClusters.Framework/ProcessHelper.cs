using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal static class ProcessHelper
{
	public static string FindSystem32Application(string application)
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), application);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static ProcessStartInfo CreateSnapinStartInfo(string snapinName, string notFoundMessage)
	{
		string text = FindSystem32Application(snapinName);
		if (text == null)
		{
			ClusterLog.LogError(notFoundMessage);
			throw new DllNotFoundException(notFoundMessage);
		}
		return CreateSnapinStartInfo(text);
	}

	public static ProcessStartInfo CreateSnapinStartInfo(string snapinName, ClusterException exceptionToDisplay)
	{
		return CreateSnapinStartInfo(FindSystem32Application(snapinName) ?? throw exceptionToDisplay);
	}

	public static ProcessStartInfo CreateSnapinStartInfo(string snapinName, GroupType groupType)
	{
		return CreateSnapinStartInfo(FindSystem32Application(snapinName) ?? throw new ClusterManagerNotFoundException(groupType));
	}

	public static ProcessStartInfo CreateSnapinExtensionStartInfo()
	{
		return CreateSnapinStartInfo("FailoverClusters.SnapInHelper.msc", ExceptionResources.CannotFindHelperSnapin_Text);
	}

	private static ProcessStartInfo CreateSnapinStartInfo(string snapin)
	{
		string text = FindSystem32Application("mmc.exe");
		if (text == null)
		{
			ClusterLog.LogError("mmc.exe was not found in the system directory");
			throw new FileNotFoundException(CommonResources.CannotFindMmc_Text);
		}
		return new ProcessStartInfo(text)
		{
			UseShellExecute = false,
			Arguments = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", snapin)
		};
	}
}

