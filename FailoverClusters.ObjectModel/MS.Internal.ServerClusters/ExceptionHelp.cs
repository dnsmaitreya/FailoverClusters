#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.ServerClusters;

public sealed class ExceptionHelp
{
	private ExceptionHelp()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool IsRethrowableException(Exception caughtException)
	{
		return false;
	}

	private static void HandleSystemError(Exception caughtException)
	{
	}

	private static void HandleProgrammingError(Exception caughtException)
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool IsProgrammingException(Exception caughtException)
	{
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool IsSystemException(Exception caughtException)
	{
		return false;
	}

	private static void BuildExceptionDetails(Exception exception, StringBuilder details)
	{
		if (exception != null)
		{
			details.AppendLine(exception.ToString());
			details.Append(Environment.NewLine);
			BuildExceptionDetails(exception.InnerException, details);
		}
	}

	private static T BuildArgumentException<T>(params string[] args) where T : Exception
	{
		Type typeFromHandle = typeof(T);
		string text = null;
		string text2 = null;
		int num = args.Length;
		if (num >= 1)
		{
			text = args[0];
			object[] array = new object[num - 1];
			Array.Copy(args, 1, array, 0, array.Length);
			string text3 = FormatArgs(array);
			text2 = ((!text3.Equals(string.Empty)) ? text3 : null);
		}
		if (typeFromHandle == typeof(ArgumentException))
		{
			if (text2 != null)
			{
				return (T)(Exception)new ArgumentException(text2, text);
			}
			return (T)(Exception)new ArgumentException(text);
		}
		if (typeFromHandle == typeof(ArgumentNullException))
		{
			if (text2 != null)
			{
				return (T)(Exception)new ArgumentNullException(text, text2);
			}
			return (T)(Exception)new ArgumentNullException(text);
		}
		throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error creating exception. '{0}' is not a supported type, please use ArgumentException or a ArgumentNullException subclass", typeFromHandle.ToString()));
	}

	private static T BuildObjectDeletedException<T>(Exception innerException, params string[] args) where T : Exception
	{
		Type typeFromHandle = typeof(T);
		string message = FormatArgs(args);
		if (typeFromHandle == typeof(ClusterObjectDeletedException))
		{
			if (innerException != null)
			{
				return (T)(Exception)new ClusterObjectDeletedException(message, innerException);
			}
			return (T)(Exception)new ClusterObjectDeletedException(message);
		}
		throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error creating exception. '{0}' is not a supported type, please use BuildObjectDeletedException or a BuildObjectDeletedException subclass", typeFromHandle.ToString()));
	}

	public static T Build<T>(Exception innerException, params string[] args) where T : Exception
	{
		T val = null;
		Type typeFromHandle = typeof(T);
		if (!(typeFromHandle == typeof(ArgumentException)) && !(typeFromHandle == typeof(ArgumentNullException)))
		{
			if (typeFromHandle == typeof(ClusterObjectDeletedException))
			{
				return BuildObjectDeletedException<T>(innerException, args);
			}
			string text = FormatArgs(args);
			if (typeFromHandle == typeof(ManagementException))
			{
				return (T)(Exception)new ManagementException(text);
			}
			if (typeFromHandle == typeof(NotSupportedException))
			{
				return (T)(Exception)new NotSupportedException(text, innerException);
			}
			if (typeFromHandle == typeof(ApplicationException))
			{
				if (innerException != null)
				{
					if (typeof(ClusterObjectDeletedException).IsAssignableFrom(innerException.GetType()))
					{
						throw innerException;
					}
					if (typeof(ClusterBaseException).IsAssignableFrom(innerException.GetType()))
					{
						throw innerException;
					}
				}
				if (string.IsNullOrEmpty(text) && innerException != null)
				{
					text = GetExceptionMessage(innerException);
				}
				return (T)(Exception)new ApplicationException(text, innerException);
			}
			if (typeof(ClusterInputValidationException).IsAssignableFrom(typeFromHandle))
			{
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentNullException("args", "Args parameter cannot be null for a ClusterInputValidation Exception");
				}
				ClusterLog.AdminEvents.WriteClusterInputValidationErrorEvent(text);
			}
			if (!typeof(ClusterBaseException).IsAssignableFrom(typeFromHandle))
			{
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Error creating exception. '{0}' is not a supported type, please use ApplicationException or a ClusterBaseException subclass", typeFromHandle.ToString()));
			}
			return (T)(Exception)(ClusterBaseException)typeFromHandle.GetConstructor(new Type[2]
			{
				typeof(string),
				typeof(Exception)
			}).Invoke(new object[2] { text, innerException });
		}
		return BuildArgumentException<T>(args);
	}

	public static T Build<T>(int resultCode, params string[] args) where T : Exception
	{
		Win32Exception innerException = null;
		if (resultCode != 0)
		{
			if ((resultCode & -65536) == 0)
			{
				resultCode = ((resultCode > 0) ? ((resultCode & 0xFFFF) | -2147024896) : resultCode);
			}
			innerException = new Win32Exception(resultCode);
		}
		return Build<T>(innerException, args);
	}

	public static T Build<T>(params string[] args) where T : Exception
	{
		return Build<T>(null, args);
	}

	public static Exception Build(Exception innerException, params string[] args)
	{
		Win32Exception firstException = GetFirstException<Win32Exception>(innerException);
		if (firstException != null)
		{
			switch (firstException.NativeErrorCode)
			{
			case -2147024894:
			case -2147024593:
			case -2147019890:
			case -2147019889:
			case -2147019884:
			case -2147019883:
			case 2:
			case 303:
			case 5006:
			case 5007:
			case 5012:
			case 5013:
				return Build<ClusterObjectDeletedException>(innerException, args);
			}
		}
		return Build<ClusterGenericException>(innerException, args);
	}

	public static Exception Build(int resultCode, params string[] args)
	{
		switch (resultCode)
		{
		default:
			return Build<ClusterGenericException>(resultCode, args);
		case -2147024894:
		case -2147024593:
		case -2147019890:
		case -2147019889:
		case -2147019884:
		case -2147019883:
		case 2:
		case 303:
		case 5006:
		case 5007:
		case 5012:
		case 5013:
			return Build<ClusterObjectDeletedException>(args);
		}
	}

	public static Exception Build(params string[] args)
	{
		return Build<ClusterGenericException>(args);
	}

	public static string FormatArgs(params object[] args)
	{
		if (args != null)
		{
			int num = args.Length;
			switch (num)
			{
			case 1:
			{
				object obj2 = args[0];
				return (obj2 == null) ? string.Empty : obj2.ToString();
			}
			default:
			{
				object[] array = new object[num - 1];
				Array.Copy(args, 1, array, 0, array.Length);
				object obj = args[0];
				return string.Format(format: (obj == null) ? string.Empty : obj.ToString(), provider: CultureInfo.CurrentCulture, args: array);
			}
			case 0:
				break;
			}
		}
		return string.Empty;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool HandleSpecialExceptions(Exception caughtException)
	{
		return false;
	}

	public static void LogException(Exception caughtException, string format, params object[] args)
	{
		string debugMessage = string.Format(CultureInfo.CurrentCulture, format, args);
		LogException(caughtException, debugMessage);
	}

	public static void LogException(Exception caughtException, string format, object arg0, object arg1)
	{
		string debugMessage = string.Format(CultureInfo.CurrentCulture, format, arg0, arg1);
		LogException(caughtException, debugMessage);
	}

	public static void LogException(Exception caughtException, string format, object arg0)
	{
		string debugMessage = string.Format(CultureInfo.CurrentCulture, format, arg0);
		LogException(caughtException, debugMessage);
	}

	public static void LogException(Exception caughtException, string debugMessage)
	{
		if (caughtException == null || !typeof(ClusterObjectDeletedException).IsAssignableFrom(caughtException.GetType()))
		{
			DebugLog.LogException(caughtException, debugMessage);
		}
	}

	public static string GetExceptionMessage(Exception exception)
	{
		if (exception is ClusterBaseException ex)
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} {1}", ex.Header, ex.Message);
		}
		if (exception is ManagementException ex2 && ex2.ErrorInformation != null)
		{
			string text = null;
			if (ex2.ErrorInformation.Properties["Description"] != null && ex2.ErrorInformation.Properties["Description"].Value != null)
			{
				text = ex2.ErrorInformation.Properties["Description"].Value.ToString();
			}
			if (ex2.ErrorInformation.Properties["StatusCode"] != null && ex2.ErrorInformation.Properties["StatusCode"].Value != null)
			{
				return string.Format(arg0: (!string.IsNullOrWhiteSpace(text)) ? text : exception.Message, provider: CultureInfo.CurrentCulture, format: "{0} (0x{1:X})", arg1: (uint)ex2.ErrorInformation.Properties["StatusCode"].Value);
			}
			return string.Format(arg0: (!string.IsNullOrWhiteSpace(text)) ? text : exception.Message, provider: CultureInfo.CurrentCulture, format: "{0}");
		}
		if (exception is CimException ex3)
		{
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo property = ex3.GetType().GetProperty("HResult");
			if (property != null)
			{
				int num = (int)property.GetValue(ex3, null);
				stringBuilder.AppendLine($"ERROR CODE : 0x{num:X}; ");
			}
			PropertyInfo property2 = ex3.GetType().GetProperty("NativeErrorCode");
			if (property2 != null)
			{
				int num2 = (int)property2.GetValue(ex3, null);
				stringBuilder.AppendLine($"NATIVE ERROR CODE : {num2}.");
			}
			return string.Format(CultureInfo.CurrentCulture, "{0} {1}", stringBuilder.ToString(), ex3.Message);
		}
		return exception?.Message;
	}

	public static string GetExceptionReportMessage(Exception exception, params string[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = args.Length;
		if (num > 0)
		{
			string[] array = new string[num - 1];
			Array.Copy(args, 1, array, 0, array.Length);
			stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, args[0], array));
		}
		bool flag = false;
		Exception ex = exception;
		if (exception != null)
		{
			do
			{
				_ = DebugLog.ExtraExceptionData;
				if (flag)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				flag = true;
				stringBuilder.Append(GetExceptionMessage(ex));
				if (!GetExceptionMessage(ex).EndsWith("\n", ignoreCase: true, CultureInfo.CurrentCulture))
				{
					stringBuilder.Append(Environment.NewLine);
				}
				ex = ex.InnerException;
			}
			while (ex != null);
		}
		return stringBuilder.ToString();
	}

	public static string GetExceptionDetails(Exception exception)
	{
		StringBuilder stringBuilder = new StringBuilder();
		BuildExceptionDetails(exception, stringBuilder);
		return stringBuilder.ToString();
	}

	public static T GetFirstException<T>(Exception caughtException) where T : Exception
	{
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

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsFirstExceptionFound<T>(Exception caughtException) where T : Exception
	{
		return GetFirstException<T>(caughtException) != null;
	}

	[Conditional("DEBUG")]
	public static void CustomExceptionReflectionTest()
	{
		//Discarded unreachable code: IL_00e3
		Type[] types = typeof(ExceptionHelp).Assembly.GetTypes();
		for (int i = 0; i < (nint)types.LongLength; i++)
		{
			Type type = types[i];
			try
			{
				if (type.IsSubclassOf(typeof(ClusterBaseException)) && !type.IsAbstract)
				{
					ConstructorInfo constructor = type.GetConstructor(new Type[2]
					{
						typeof(string),
						typeof(Exception)
					});
					Debug.Assert(constructor != null, string.Format(CultureInfo.CurrentCulture, "Constructor(string, exception) not found in Custom Exception '{0}', this certanly will lead to a bug, please implement proper constructor for '{0}'", type.Name));
					ClusterBaseException ex = (ClusterBaseException)constructor.Invoke(new object[2]
					{
						"Message test",
						new Win32Exception(0)
					});
					byte condition = ((ex != null) ? ((byte)1) : ((byte)0));
					Debug.Assert(condition != 0, string.Format(CultureInfo.CurrentCulture, "Error creating Custom Exception {0}", type.Name));
					try
					{
						throw ex;
					}
					catch (Exception ex2)
					{
						Debug.Assert(ex2.GetType() == type, string.Format(CultureInfo.CurrentCulture, "Error testing Custom Exception {0}", type.Name), ex2.ToString());
					}
				}
			}
			catch (Exception ex3)
			{
				Debug.Assert(condition: false, ex3.ToString());
			}
		}
	}
}
