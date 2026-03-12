using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public sealed class ClusterShipAssert
{
	private ClusterShipAssert()
	{
	}

	public static void Assert([MarshalAs(UnmanagedType.U1)] bool condition, uint tagId, int data, string format, params object[] args)
	{
		string message = string.Format(CultureInfo.InvariantCulture, format, args);
		if (!condition)
		{
			Fail(tagId, data, message);
		}
	}

	public static void Assert([MarshalAs(UnmanagedType.U1)] bool condition, uint tagId, int data, string message)
	{
		if (!condition)
		{
			Fail(tagId, data, message);
		}
	}

	public static void Assert([MarshalAs(UnmanagedType.U1)] bool condition, uint tagId, string format, params object[] args)
	{
		string message = string.Format(CultureInfo.InvariantCulture, format, args);
		if (!condition)
		{
			Fail(tagId, 0, message);
		}
	}

	public static void Assert([MarshalAs(UnmanagedType.U1)] bool condition, uint tagId, string message)
	{
		if (!condition)
		{
			Fail(tagId, 0, message);
		}
	}

	public static void Fail(uint tagId, Exception exception, string format, params object[] args)
	{
		Fail(tagId, exception, string.Format(CultureInfo.InvariantCulture, format, args));
	}

	public static void Fail(uint tagId, Exception exception, string message)
	{
		Fail(tagId, 0, string.Format(CultureInfo.InvariantCulture, "{0}: {1}", message, exception.ToString()));
	}

	public static void Fail(uint tagId, int data, string format, params object[] args)
	{
		Fail(tagId, data, string.Format(CultureInfo.InvariantCulture, format, args));
	}

	public unsafe static void Fail(uint tagId, int data, string message)
	{
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(message);
			global::_003CModule_003E.ShipAssertMsgW(tagId, data, ptr);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public static void Fail(uint tagId, string format, object[] args)
	{
		string message = string.Format(CultureInfo.InvariantCulture, format, args);
		Fail(tagId, 0, message);
	}

	public static void Fail(uint tagId, string message)
	{
		Fail(tagId, 0, message);
	}
}
