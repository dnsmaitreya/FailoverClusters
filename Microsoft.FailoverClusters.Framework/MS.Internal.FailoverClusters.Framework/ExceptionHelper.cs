using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal static class ExceptionHelper
{
	public static Exception Build<T>(string message) where T : ClusterException, new()
	{
		return Build<T>(message, null);
	}

	public static Exception Build(int errorCode)
	{
		return Build(errorCode, parse: true);
	}

	public static Exception Build(int errorCode, bool parse)
	{
		if (errorCode == 0)
		{
			return null;
		}
		if ((errorCode & 0xFFFF0000u) == 0L)
		{
			errorCode = (errorCode & 0xFFFF) | (int)(NativeMethods.FACILITY_WIN32 << 16) | int.MinValue;
		}
		return new Win32Exception(errorCode);
	}

	public static Exception Build<T>(int errorCode) where T : ClusterException, new()
	{
		return Build<T>(null, null, errorCode);
	}

	public static Exception Build<T>(string message, int errorCode) where T : ClusterException, new()
	{
		return Build<T>(message, null, errorCode);
	}

	public static Exception Build<T>(string message, Exception innerException) where T : ClusterException, new()
	{
		return Build<T>(message, innerException, 0);
	}

	private static Exception Build<T>(string message, Exception innerException, int errorCode) where T : ClusterException, new()
	{
		if (innerException != null && errorCode != 0)
		{
			throw new ArgumentException("InnerException must be null when errorCode is set");
		}
		if (errorCode != 0)
		{
			if ((errorCode & 0xFFFF0000u) == 0L)
			{
				errorCode = (errorCode & 0xFFFF) | (int)(NativeMethods.FACILITY_WIN32 << 16) | int.MinValue;
			}
			innerException = new Win32Exception(errorCode);
		}
		ConstructorInfo constructorInfo = null;
		if (message != null)
		{
			constructorInfo = typeof(T).GetConstructor(new Type[2]
			{
				typeof(string),
				typeof(Exception)
			});
			if (constructorInfo != null)
			{
				return (Exception)constructorInfo.Invoke(new object[2] { message, innerException });
			}
			return new ClusterDefaultException(message, innerException);
		}
		constructorInfo = typeof(T).GetConstructor(new Type[1] { typeof(Exception) });
		if (constructorInfo != null)
		{
			ConstructorInfo constructorInfo2 = constructorInfo;
			object[] parameters = new Exception[1] { innerException };
			return (Exception)constructorInfo2.Invoke(parameters);
		}
		return new ClusterDefaultException(innerException);
	}

	public static ClusterObjectLoadFailedException ClusterObjectLoadFailedException(string name, Guid id, int errorCode)
	{
		Exception ex = null;
		throw new ClusterObjectLoadFailedException(name, id, errorCode switch
		{
			5013 => new ClusterObjectNotFoundException(name, id, typeof(Group)), 
			5007 => new ClusterObjectNotFoundException(name, id, typeof(Resource)), 
			_ => Build(errorCode), 
		});
	}

	public static T GetFirstException<T>(Exception caughtException) where T : Exception, new()
	{
		if (caughtException == null)
		{
			return null;
		}
		T val = null;
		Exception ex = caughtException;
		Type typeFromHandle = typeof(T);
		do
		{
			Type type = ex.GetType();
			if (typeFromHandle.IsAssignableFrom(type))
			{
				val = (T)ex;
			}
			ex = ex.InnerException;
		}
		while (val == null && ex != null);
		return val;
	}
}
