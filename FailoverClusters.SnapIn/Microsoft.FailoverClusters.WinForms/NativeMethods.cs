using System;
using System.Runtime.InteropServices;

namespace FailoverClusters.WinForms;

internal static class NativeMethods
{
	internal enum TASK_TRIGGER_TYPE
	{
		TASK_TIME_TRIGGER_ONCE,
		TASK_TIME_TRIGGER_DAILY,
		TASK_TIME_TRIGGER_WEEKLY,
		TASK_TIME_TRIGGER_MONTHLYDATE,
		TASK_TIME_TRIGGER_MONTHLYDOW,
		TASK_EVENT_TRIGGER_ON_IDLE,
		TASK_EVENT_TRIGGER_AT_SYSTEMSTART,
		TASK_EVENT_TRIGGER_AT_LOGON
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct TASK_TRIGGER
	{
		public ushort cbTriggerSize;

		public ushort Reserved1;

		public ushort wBeginYear;

		public ushort wBeginMonth;

		public ushort wBeginDay;

		public ushort wEndYear;

		public ushort wEndMonth;

		public ushort wEndDay;

		public ushort wStartHour;

		public ushort wStartMinute;

		public uint MinutesDuration;

		public uint MinutesInterval;

		public uint rgFlags;

		public TASK_TRIGGER_TYPE TriggerType;

		public TRIGGER_TYPE_UNION Type;

		public ushort Reserved2;

		public ushort wRandomMinutesInterval;
	}

	[StructLayout(LayoutKind.Explicit, Pack = 4)]
	internal struct TRIGGER_TYPE_UNION
	{
		[FieldOffset(0)]
		public DAILY Daily;

		[FieldOffset(0)]
		public WEEKLY Weekly;

		[FieldOffset(0)]
		public MONTHLYDATE MonthlyDate;

		[FieldOffset(0)]
		public MONTHLYDOW MonthlyDOW;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct DAILY
	{
		public ushort DaysInterval;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct WEEKLY
	{
		public ushort WeeksInterval;

		public ushort rgfDaysOfTheWeek;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct MONTHLYDATE
	{
		public uint rgfDays;

		public ushort rgfMonths;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct MONTHLYDOW
	{
		public ushort wWhichWeek;

		public ushort rgfDaysOfTheWeek;

		public ushort rgfMonths;
	}

	internal enum CLUSTER_TASK_TYPE
	{
		CLUSTER_TASK_TYPE_NONE,
		CLUSTER_TASK_TYPE_RESOURCE_DEPENDENT,
		CLUSTER_TASK_TYPE_ANY_NODE,
		CLUSTER_TASK_TYPE_CLUSTER_WIDE,
		CLUSTER_TASK_TYPE_TS_V1
	}

	internal struct CLUSTER_TASK_INFO
	{
		public IntPtr Next;

		public string TaskName;

		public CLUSTER_TASK_TYPE TaskType;

		public string ResourceId;

		public string XMLText;

		public string ApplicationName;

		public string ApplicationParams;

		public string CurrentDirectory;

		public uint TriggerCount;

		public IntPtr TriggerArray;

		public uint TriggerArraySize;
	}

	public static uint NT9_MAJOR_VERSION = 8u;

	public static uint NT10_MAJOR_VERSION = 9u;

	public const int TASK_SUNDAY = 1;

	public const int TASK_MONDAY = 2;

	public const int TASK_TUESDAY = 4;

	public const int TASK_WEDNESDAY = 8;

	public const int TASK_THURSDAY = 16;

	public const int TASK_FRIDAY = 32;

	public const int TASK_SATURDAY = 64;

	public const int TASK_FIRST_WEEK = 1;

	public const int TASK_SECOND_WEEK = 2;

	public const int TASK_THIRD_WEEK = 3;

	public const int TASK_FOURTH_WEEK = 4;

	public const int TASK_LAST_WEEK = 5;

	public const int TASK_JANUARY = 1;

	public const int TASK_FEBRUARY = 2;

	public const int TASK_MARCH = 4;

	public const int TASK_APRIL = 8;

	public const int TASK_MAY = 16;

	public const int TASK_JUNE = 32;

	public const int TASK_JULY = 64;

	public const int TASK_AUGUST = 128;

	public const int TASK_SEPTEMBER = 256;

	public const int TASK_OCTOBER = 512;

	public const int TASK_NOVEMBER = 1024;

	public const int TASK_DECEMBER = 2048;

	[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int StrCmpLogicalW(string x, string y);

	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern uint EditVssTaskSchedule(byte[] triggers, int size, ref IntPtr newTriggers, ref int newSize, string caption, IntPtr handleParent);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern uint ClusterTaskCreate_TS_V1(string clusterName, string taskName, int type, string currentDirectory, string applicationName, string applicationParams, int taskTriggersCount, byte[] triggers, int triggerArrayLength, string resourceName);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern uint ClusterTaskChange_TS_V1(string clusterName, string taskName, int taskTriggersCount, byte[] triggers, int triggerArrayLength);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern uint ClusterTaskDelete_TS_V1(string clusterName, string taskName);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern uint ClusterTaskExists_TS_V1(string clusterName, string taskName, string resourceName, ref bool exists);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int ClusterTaskQuery(string clusterName, string taskName, out IntPtr taskInfo);

	[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern void ClusterFreeTaskInfo(IntPtr taskInfo);
}

