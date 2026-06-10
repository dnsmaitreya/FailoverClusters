using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

public class ActionMultiplexor<TInputData, TMultiplexedData, TOutputData>
{
	private MultiplexorFunction<TInputData, TMultiplexedData, TOutputData> m_function;

	private IList<IAsyncResult> StartExecution(TInputData inputData, ICollection<TMultiplexedData> multiplexData)
	{
		List<IAsyncResult> list = new List<IAsyncResult>();
		foreach (TMultiplexedData multiplexDatum in multiplexData)
		{
			IAsyncResult item = m_function.BeginInvoke(inputData, multiplexDatum, null, null);
			list.Add(item);
		}
		return list;
	}

	private unsafe ICollection<TOutputData> WaitForFinish(IList<IAsyncResult> results)
	{
		//IL_0008: Expected I8, but got I
		//IL_0062: Expected I, but got I8
		//IL_0062: Expected I, but got I8
		//IL_0072: Expected I, but got I8
		//IL_00af: Expected I, but got I8
		long num = (nint)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		List<TOutputData> list = new List<TOutputData>();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint exceptionCode);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
		while (results.Count > 0)
		{
			int index = WaitForOne(results);
			IAsyncResult result = results[index];
			results.RemoveAt(index);
			try
			{
				TOutputData item = m_function.EndInvoke(result);
				list.Add(item);
			}
			catch when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				exceptionCode = (uint)Marshal.GetExceptionCode();
				return (byte)global::_003CModule_003E.__CxxExceptionFilter((void*)Marshal.GetExceptionPointers(), null, 0, null) != 0;
			}).Invoke())
			{
				num2 = 0u;
				global::_003CModule_003E.__CxxRegisterExceptionObject((void*)Marshal.GetExceptionPointers(), (void*)num);
				try
				{
					try
					{
						ThreadPool.QueueUserWorkItem(CompletionCallback, results);
						global::_003CModule_003E._CxxThrowException(null, null);
					}
					catch when (((Func<bool>)delegate
					{
						// Could not convert BlockContainer to single expression
						num2 = (uint)global::_003CModule_003E.__CxxDetectRethrow((void*)Marshal.GetExceptionPointers());
						return (byte)num2 != 0;
					}).Invoke())
					{
					}
					if (num2 != 0)
					{
						throw;
					}
				}
				finally
				{
					global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
				}
			}
		}
		return list;
	}

	private int WaitForOne(IList<IAsyncResult> results)
	{
		WaitHandle[] array = new WaitHandle[results.Count];
		int num = 0;
		foreach (IAsyncResult result in results)
		{
			array[num] = result.AsyncWaitHandle;
			num++;
		}
		return WaitHandle.WaitAny(array);
	}

	private void CompletionCallback(object data)
	{
		try
		{
			IEnumerator<IAsyncResult> enumerator = ((IList<IAsyncResult>)data).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IAsyncResult current = enumerator.Current;
					try
					{
						m_function.EndInvoke(current);
					}
					catch (Exception caughtException)
					{
						ExceptionHelp.HandleSpecialExceptions(caughtException);
					}
				}
			}
			finally
			{
				IEnumerator<IAsyncResult> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (ThreadAbortException)
		{
		}
		catch (Exception caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "An exception occurred in a background thread.");
		}
	}

	public ActionMultiplexor(MultiplexorFunction<TInputData, TMultiplexedData, TOutputData> function)
	{
		if (function == null)
		{
			throw new ArgumentNullException("function");
		}
		m_function = function;
	}

	public ICollection<TOutputData> Execute(TInputData inputData, ICollection<TMultiplexedData> multiplexData)
	{
		if (multiplexData == null)
		{
			throw new ArgumentNullException("multiplexData");
		}
		if (multiplexData.Count == 0)
		{
			throw new ArgumentOutOfRangeException("multiplexData");
		}
		IList<IAsyncResult> results = StartExecution(inputData, multiplexData);
		return WaitForFinish(results);
	}

	public IList<IAsyncResult> AsyncExecute(TInputData inputData, ICollection<TMultiplexedData> multiplexData)
	{
		if (multiplexData == null)
		{
			throw new ArgumentNullException("multiplexData");
		}
		if (multiplexData.Count == 0)
		{
			throw new ArgumentOutOfRangeException("multiplexData");
		}
		return StartExecution(inputData, multiplexData);
	}
}
