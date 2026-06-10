using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
[EnvironmentPermission(SecurityAction.InheritanceDemand, Unrestricted = true)]
public class ClusterWmiWin32Exception : Win32Exception
{
	private readonly string stackTrace;

	public override string StackTrace => stackTrace;

	public ClusterWmiWin32Exception()
		: this(0, string.Empty)
	{
	}

	public ClusterWmiWin32Exception(string message)
		: base(message)
	{
	}

	public ClusterWmiWin32Exception(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterWmiWin32Exception(int errorCode, string stackTrace)
		: base(errorCode)
	{
		this.stackTrace = stackTrace ?? StackTraceExtensions.GetStackTrace(2);
	}

	public ClusterWmiWin32Exception(int errorCode, string message, string stackTrace)
		: base(errorCode, message)
	{
		this.stackTrace = stackTrace ?? StackTraceExtensions.GetStackTrace(2);
	}

	protected ClusterWmiWin32Exception(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

