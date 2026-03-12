using System.Collections.Generic;
using System.Diagnostics;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public static class PerformanceCounters
{
	private static readonly List<string> externalCounters = new List<string>();

	public static List<string> ExternalCounters => externalCounters;

	public static void Increment(string className)
	{
		Utilities.UnreferencedParameter(className);
	}

	public static void Decrement(string className)
	{
		Utilities.UnreferencedParameter(className);
	}

	public static PerformanceCounter GetPerformanceCounterAndIncrement(string className)
	{
		return GetPerformanceCounterAndIncrement(null, className);
	}

	public static PerformanceCounter GetPerformanceCounterAndIncrement(PerformanceCounter currentInstance, string className)
	{
		Utilities.UnreferencedParameter(currentInstance);
		Utilities.UnreferencedParameter(className);
		return null;
	}

	public static PerformanceCounter GetPerformanceCounter(string className)
	{
		Utilities.UnreferencedParameter(className);
		return null;
	}

	public static void InstallPerformanceCounters()
	{
	}
}
