using System;
using System.ComponentModel;
using System.Threading;

namespace KDDSL.ServerClusters;

internal sealed class ClusApiExceptionFactory
{
	private ClusApiExceptionFactory()
	{
	}

	private static Exception CreateException(Cluster cluster, int sc)
	{
		int num = ((sc > 0) ? ((sc & 0xFFFF) | -2147024896) : sc);
		if (num == -2147023174)
		{
			cluster.OnConnectionChanged(ClusterConnectionState.Disconnected);
		}
		return num switch
		{
			-2147018936 => new ClusterResourceLockedException(), 
			-2147024593 => new ClusterObjectDeletedException(Resources.ObjectDeleted_Text, null), 
			_ => new Win32Exception(num), 
		};
	}

	private static Exception CreateException(Cluster cluster, int sc, string message)
	{
		int num = ((sc > 0) ? ((sc & 0xFFFF) | -2147024896) : sc);
		if (num == -2147023174)
		{
			cluster.OnConnectionChanged(ClusterConnectionState.Disconnected);
		}
		Exception ex;
		switch (num)
		{
		default:
			ex = NativeMethods.AnalyzeAndReturn(num, cluster.CachedName);
			if (ex == null)
			{
				ex = ExceptionHelp.Build<ApplicationException>(num, new string[1] { message });
			}
			break;
		case -2147018936:
			ex = new ClusterResourceLockedException();
			break;
		case -2147024593:
		case -2147019890:
		case -2147019884:
			ex = new ClusterObjectDeletedException(Resources.ObjectDeleted_Text, null);
			break;
		}
		return ex;
	}

	private static Exception CreateObjectDeletedException(Exception innerException)
	{
		return new ClusterObjectDeletedException(Resources.ObjectDeleted_Text, innerException);
	}

	private static Exception CreateObjectDisposedException()
	{
		Exception innerException = new ObjectDisposedException(Resources.ObjectDisposed_Text);
		return new ApplicationException(Resources.ClusterObjectDisposed_Text, innerException);
	}

	private static void HandleRpcErrors(int sc, Cluster cluster)
	{
		if (sc == -2147023174)
		{
			cluster.OnConnectionChanged(ClusterConnectionState.Disconnected);
		}
	}

	public static void ThrowObjectDeletedException(Exception innerException)
	{
		//Discarded unreachable code: IL_0007
		throw CreateObjectDeletedException(innerException);
	}

	public static void ThrowObjectDeletedException()
	{
		//Discarded unreachable code: IL_0007
		throw CreateObjectDeletedException(null);
	}

	public static void ThrowObjectDisposedException()
	{
		//Discarded unreachable code: IL_0006
		throw CreateObjectDisposedException();
	}

	public static void CreateAndThrow(Cluster cluster, int sc, string format, object[] args)
	{
		//Discarded unreachable code: IL_001b
		string message = string.Format(Thread.CurrentThread.CurrentCulture, format, args);
		throw CreateException(cluster, sc, message);
	}

	public static void CreateAndThrow(Cluster cluster, int sc, string format, object arg0, object arg1)
	{
		//Discarded unreachable code: IL_002b
		string message = string.Format(args: new object[2] { arg0, arg1 }, provider: Thread.CurrentThread.CurrentCulture, format: format);
		throw CreateException(cluster, sc, message);
	}

	public static void CreateAndThrow(Cluster cluster, int sc, string format, object arg0)
	{
		//Discarded unreachable code: IL_0026
		string message = string.Format(args: new object[1] { arg0 }, provider: Thread.CurrentThread.CurrentCulture, format: format);
		throw CreateException(cluster, sc, message);
	}

	public static void CreateAndThrow(Cluster cluster, int sc, string message)
	{
		//Discarded unreachable code: IL_0009
		throw CreateException(cluster, sc, message);
	}

	public static void CreateAndThrow(Cluster cluster, int sc, object[] dataKeys, object[] dataValues)
	{
		//Discarded unreachable code: IL_002b
		Exception ex = CreateException(cluster, sc);
		int num = 0;
		if (0 < (nint)dataKeys.LongLength)
		{
			do
			{
				ex.Data.Add(dataKeys[num], dataValues[num]);
				num++;
			}
			while (num < (nint)dataKeys.LongLength);
		}
		throw ex;
	}

	public static void CreateAndThrow(Cluster cluster, int sc)
	{
		//Discarded unreachable code: IL_0008
		throw CreateException(cluster, sc);
	}
}
