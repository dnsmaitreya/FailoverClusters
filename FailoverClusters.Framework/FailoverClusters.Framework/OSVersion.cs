using System;
using System.Globalization;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class OSVersion
{
	private static readonly OSVersion[] Versions;

	private const int BuildVersionIndex = 3;

	private static readonly OSVersion BuildVersion;

	public int Value { get; private set; }

	public static OSVersion Windows2008 => new OSVersion(100663296);

	public static OSVersion Windows2008R2 => new OSVersion(100728832);

	public static OSVersion Windows2012 => new OSVersion(100794368);

	public static OSVersion Windows2012R2 => new OSVersion(100859904);

	static OSVersion()
	{
		Versions = new OSVersion[4] { Windows2008, Windows2008R2, Windows2012, Windows2012R2 };
		BuildVersion = Versions[3];
	}

	private OSVersion(int value)
	{
		Value = value;
	}

	public OSVersion(byte majorVersion, byte minorVersion)
	{
		Value = FormatVersion(majorVersion, minorVersion);
	}

	internal OSVersion(PCluster cluster)
	{
		Value = FormatVersion((byte)cluster.MajorVersion, (byte)cluster.MinorVersion);
	}

	public static int Compare(OSVersion left, OSVersion right)
	{
		if (!(left > right))
		{
			if (!(left < right))
			{
				return 0;
			}
			return -1;
		}
		return 1;
	}

	public static bool operator >(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value > right.Value;
	}

	public static bool operator >=(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value >= right.Value;
	}

	public static bool operator <(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value < right.Value;
	}

	public static bool operator <=(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value <= right.Value;
	}

	public static bool operator ==(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value == right.Value;
	}

	public static bool operator !=(OSVersion left, OSVersion right)
	{
		Exceptions.ThrowIfNull(left, "left");
		Exceptions.ThrowIfNull(right, "right");
		return left.Value != right.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		OSVersion oSVersion = obj as OSVersion;
		if (oSVersion == null)
		{
			return false;
		}
		return Value == oSVersion.Value;
	}

	public bool IsCurrentVersion()
	{
		return this == BuildVersion;
	}

	public bool IsPreviousVersion()
	{
		return this == Versions[2];
	}

	public bool IsOlderVersion()
	{
		return Value < Versions[2].Value;
	}

	public bool IsFutureVersion()
	{
		return Value > BuildVersion.Value;
	}

	public override int GetHashCode()
	{
		return Value;
	}

	private static int FormatVersion(byte majorVersion, byte minorVersion)
	{
		return Convert.ToInt32(majorVersion.ToString("x2", CultureInfo.InvariantCulture) + minorVersion.ToString("x2", CultureInfo.InvariantCulture) + "0000", 16);
	}
}

