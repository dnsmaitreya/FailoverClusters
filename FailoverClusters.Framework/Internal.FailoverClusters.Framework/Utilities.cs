using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal static class Utilities
{
	private const string GuidPattern = "^\\{?[0-9a-f]{8}(-?[0-9a-f]{4}){3}-?[0-9a-f]{12}\\}?$";

	private static readonly Regex Expression;

	static Utilities()
	{
		Expression = new Regex("^\\{?[0-9a-f]{8}(-?[0-9a-f]{4}){3}-?[0-9a-f]{12}\\}?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
	}

	public static bool IsGuid(string value)
	{
		return Expression.IsMatch(value);
	}

	public unsafe static uint CheckSum(IntPtr buffer, int bufferSize)
	{
		byte* ptr = (byte*)buffer.ToPointer();
		uint num = 2166136261u;
		for (int i = 0; i < bufferSize; i++)
		{
			num = (num ^ *ptr) * 16777619;
			ptr++;
		}
		num += num << 13;
		num ^= num >> 7;
		num += num << 3;
		num ^= num >> 17;
		return num + (num << 5);
	}

	public static void UnreferencedParameter(object param)
	{
	}

	public static T ReturnInstance<T>(ref WeakReferenceEx weakReference, object objectLock, Func<T> createInstanceCallback)
	{
		T val = default(T);
		if (weakReference != null)
		{
			val = (T)weakReference.Target;
		}
		if (weakReference == null || val == null)
		{
			lock (objectLock)
			{
				if (weakReference == null || val == null)
				{
					val = createInstanceCallback();
					weakReference = new WeakReferenceEx(val);
				}
			}
		}
		return val;
	}

	public static T ReturnInstance<T>(ref WeakReferenceEx<T> weakReference, object objectLock, Func<T> createInstanceCallback) where T : class
	{
		T val = null;
		if (weakReference != null)
		{
			val = weakReference.Target;
		}
		if (weakReference == null || val == null)
		{
			lock (objectLock)
			{
				if (weakReference == null || val == null)
				{
					val = createInstanceCallback();
					weakReference = new WeakReferenceEx<T>(val);
				}
			}
		}
		return val;
	}

	public static string CreateFqdn(string networkName, string dnsSuffix)
	{
		return "{0}.{1}".FormatInvariantCulture(networkName, dnsSuffix);
	}

	public static uint Win32ToHResult(int statusCode)
	{
		if (statusCode == 0)
		{
			return (uint)statusCode;
		}
		return ((uint)statusCode & 0xFFFFu) | (NativeMethods.FACILITY_WIN32 << 16) | 0x80000000u;
	}

	public static string FormatError(int statusCode)
	{
		StringBuilder stringBuilder = new StringBuilder(1024);
		int num = NativeMethods.FormatMessage(NativeMethods.FormatMessageFlags.FromSystem, IntPtr.Zero, statusCode, MakeLanguageId(NativeMethods.LanguageId.Neutral, NativeMethods.SubLanaguageId.Neutral), stringBuilder, stringBuilder.Capacity, IntPtr.Zero);
		if (num == 0)
		{
			num = NativeMethods.FormatMessage(NativeMethods.FormatMessageFlags.IgnoreInserts | NativeMethods.FormatMessageFlags.FromHModule, NativeMethods.GetModuleHandle("NTDLL.DLL"), statusCode, MakeLanguageId(NativeMethods.LanguageId.Neutral, NativeMethods.SubLanaguageId.Neutral), stringBuilder, stringBuilder.Capacity, IntPtr.Zero);
			if (num == 0)
			{
				StringBuilder stringBuilder2 = new StringBuilder(261);
				if (NativeMethods.GetSystemDirectory(stringBuilder2, stringBuilder2.Capacity) > 0)
				{
					stringBuilder2.Append("\\ACTIVEDS.DLL");
					IntPtr intPtr = NativeMethods.LoadLibrary(stringBuilder2.ToString());
					if (intPtr != IntPtr.Zero)
					{
						num = NativeMethods.FormatMessage(NativeMethods.FormatMessageFlags.IgnoreInserts | NativeMethods.FormatMessageFlags.FromHModule, intPtr, statusCode, MakeLanguageId(NativeMethods.LanguageId.Neutral, NativeMethods.SubLanaguageId.Neutral), stringBuilder, stringBuilder.Capacity, IntPtr.Zero);
						if (!NativeMethods.FreeLibrary(intPtr))
						{
							int lastWin32Error = Marshal.GetLastWin32Error();
							ClusterLog.LogWarning("FreeLibrary failed with error {0}", lastWin32Error);
						}
					}
				}
			}
		}
		if (num == 0)
		{
			return CommonResources.GenericErrorFormatString_Text.FormatCurrentCulture(statusCode);
		}
		return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray()).Replace("\r\n", "");
	}

	public static ushort MakeLanguageId(NativeMethods.LanguageId languageId, NativeMethods.SubLanaguageId subLanguageId)
	{
		ushort num = (ushort)languageId;
		return (ushort)(((uint)subLanguageId << 10) | num);
	}

	public static ushort PrimaryLanguageId(NativeMethods.LanguageId languageId)
	{
		return (ushort)(languageId & (NativeMethods.LanguageId)1023);
	}

	public static ushort SubLanguageId(NativeMethods.SubLanaguageId subLanguageId)
	{
		return (ushort)((int)subLanguageId >> 10);
	}
}

