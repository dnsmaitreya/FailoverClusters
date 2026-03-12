using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace MS.Internal.ServerClusters;

public class ClusterRegistryKey : IDisposable
{
	private SafeRegistryHandle m_handle;

	private Cluster m_cluster;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private unsafe HKEY__* Handle
	{
		get
		{
			m_lifetimeHelper.CheckObjectState();
			return m_handle.DangerousGetRegistryHandle();
		}
	}

	private static uint ValueKindToValueType(RegistryValueKind valueKind)
	{
		return (uint)valueKind;
	}

	private static RegistryValueKind ValueTypeToValueKind(uint valueType)
	{
		return (RegistryValueKind)valueType;
	}

	private static object ConvertToManagedObject(UnmanagedBuffer buffer, uint dwValueType)
	{
		//Discarded unreachable code: IL_0056
		switch (dwValueType)
		{
		case 11u:
			return TypeConverter.ConvertToManagedType(buffer, DataType.UInt64);
		case 7u:
			return TypeConverter.ConvertToManagedType(buffer, DataType.MultiString);
		case 4u:
			return TypeConverter.ConvertToManagedType(buffer, DataType.UInt32);
		case 3u:
			return TypeConverter.ConvertToManagedType(buffer, DataType.BinaryBlob);
		case 1u:
		case 2u:
			return TypeConverter.ConvertToManagedType(buffer, DataType.String);
		default:
			throw new NotSupportedException(Resources.Registry_UnsupportedType_Text);
		}
	}

	public unsafe void SetValue(string name, object value, RegistryValueKind valueKind)
	{
		if (valueKind == RegistryValueKind.DWord)
		{
			int num = Convert.ToInt32(value, CultureInfo.CurrentCulture);
			IntPtr intPtr = new IntPtr(&num);
			IntPtr intPtr2 = intPtr;
			SetValue(name, (byte*)intPtr2.ToPointer(), 4, RegistryValueKind.DWord);
			return;
		}
		if (valueKind == RegistryValueKind.QWord)
		{
			long num2 = Convert.ToInt64(value, CultureInfo.CurrentCulture);
			IntPtr intPtr3 = new IntPtr(&num2);
			IntPtr intPtr4 = intPtr3;
			SetValue(name, (byte*)intPtr4.ToPointer(), 8, RegistryValueKind.QWord);
			return;
		}
		if (valueKind > RegistryValueKind.Unknown)
		{
			UnmanagedBuffer unmanagedBuffer;
			if (valueKind > RegistryValueKind.ExpandString)
			{
				if (valueKind != RegistryValueKind.Binary)
				{
					if (valueKind != RegistryValueKind.MultiString)
					{
						goto IL_00f5;
					}
					if (value.GetType() == typeof(StringCollection))
					{
						unmanagedBuffer = TypeConverter.ConvertToNativeType((StringCollection)value);
					}
					else
					{
						StringCollection stringCollection = new StringCollection();
						stringCollection.AddRange((string[])value);
						unmanagedBuffer = TypeConverter.ConvertToNativeType(stringCollection);
					}
				}
				else
				{
					unmanagedBuffer = TypeConverter.ConvertToNativeType((byte[])value);
				}
			}
			else
			{
				unmanagedBuffer = TypeConverter.ConvertToNativeType((string)value);
			}
			try
			{
				SetValue(name, (byte*)unmanagedBuffer.Pointer, (int)unmanagedBuffer.Size, valueKind);
				return;
			}
			finally
			{
				unmanagedBuffer.Free();
			}
		}
		goto IL_00f5;
		IL_00f5:
		throw new NotSupportedException(Resources.Registry_UnsupportedType_Text);
	}

	public unsafe void SetValue(string name, StringCollection value)
	{
		UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(value);
		try
		{
			SetValue(name, (byte*)unmanagedBuffer.Pointer, (int)unmanagedBuffer.Size, RegistryValueKind.MultiString);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	public unsafe void SetValue(string name, byte[] value)
	{
		UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(value);
		try
		{
			SetValue(name, (byte*)unmanagedBuffer.Pointer, (int)unmanagedBuffer.Size, RegistryValueKind.Binary);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	public unsafe void SetValue(string name, string value)
	{
		UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(value);
		try
		{
			SetValue(name, (byte*)unmanagedBuffer.Pointer, (int)unmanagedBuffer.Size, RegistryValueKind.String);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	public unsafe void SetValue(string name, ulong value)
	{
		IntPtr intPtr = new IntPtr(&value);
		IntPtr intPtr2 = intPtr;
		SetValue(name, (byte*)intPtr2.ToPointer(), 8, RegistryValueKind.QWord);
	}

	public unsafe void SetValue(string name, uint value)
	{
		IntPtr intPtr = new IntPtr(&value);
		IntPtr intPtr2 = intPtr;
		SetValue(name, (byte*)intPtr2.ToPointer(), 4, RegistryValueKind.DWord);
	}

	public unsafe void SetValue(string name, long value)
	{
		IntPtr intPtr = new IntPtr(&value);
		IntPtr intPtr2 = intPtr;
		SetValue(name, (byte*)intPtr2.ToPointer(), 8, RegistryValueKind.QWord);
	}

	public unsafe void SetValue(string name, int value)
	{
		IntPtr intPtr = new IntPtr(&value);
		IntPtr intPtr2 = intPtr;
		SetValue(name, (byte*)intPtr2.ToPointer(), 4, RegistryValueKind.DWord);
	}

	private unsafe void SetValue(string name, byte* value, int size, RegistryValueKind valueKind)
	{
		ushort* ptr = InteropHelp.StringToWstr(name);
		try
		{
			int num = (int)global::_003CModule_003E.ClusterRegSetValue(Handle, ptr, (uint)valueKind, value, (uint)size);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num, Resources.Registry_SetValueFailed_Text, name);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private unsafe void SetValue(string name, IntPtr value, int size, RegistryValueKind valueKind)
	{
		SetValue(name, (byte*)value.ToPointer(), size, valueKind);
	}

	private unsafe void SetValue(string name, UnmanagedBuffer inputBuffer, RegistryValueKind valueKind)
	{
		SetValue(name, (byte*)inputBuffer.Pointer, (int)inputBuffer.Size, valueKind);
	}

	internal static uint RegistryRightsToRegSam(RegistryRights rights)
	{
		return (uint)rights;
	}

	internal ClusterRegistryKey(Cluster cluster, SafeRegistryHandle handle)
	{
		m_cluster = cluster;
		m_handle = handle;
		m_lifetimeHelper = new ObjectLifetimeHelper();
	}

	private void _007EClusterRegistryKey()
	{
		if (!m_lifetimeHelper.IsDisposed)
		{
			m_handle.Close();
			m_lifetimeHelper.MarkAsDisposed();
		}
	}

	public void Close()
	{
		m_handle.Close();
	}

	public unsafe ClusterRegistryKey CreateSubkey(string subkey, RegistrySecurity registrySecurity)
	{
		ushort* ptr = InteropHelp.StringToWstr(subkey);
		IntPtr intPtr = IntPtr.Zero;
		ClusterRegistryKey clusterRegistryKey = null;
		try
		{
			uint num = 0u;
			if (registrySecurity != null)
			{
				byte[] securityDescriptorBinaryForm = registrySecurity.GetSecurityDescriptorBinaryForm();
				IntPtr intPtr2 = Marshal.AllocHGlobal(securityDescriptorBinaryForm.Length);
				intPtr = intPtr2;
				Marshal.Copy(securityDescriptorBinaryForm, 0, intPtr2, securityDescriptorBinaryForm.Length);
			}
			_SECURITY_ATTRIBUTES* ptr2 = (_SECURITY_ATTRIBUTES*)intPtr.ToPointer();
			System.Runtime.CompilerServices.Unsafe.SkipInit(out HKEY__* hNativeHandle);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			int num3 = global::_003CModule_003E.ClusterRegCreateKey(Handle, ptr, 0u, 983103u, ptr2, &hNativeHandle, &num2);
			SafeRegistryHandle handle = new SafeRegistryHandle(hNativeHandle);
			if (num3 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num3, Resources.Registry_CreateSubKeyFailed_Text, subkey);
			}
			return new ClusterRegistryKey(m_cluster, handle);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public unsafe void DeleteSubkey(string subkey)
	{
		ushort* ptr = InteropHelp.StringToWstr(subkey);
		try
		{
			int num = global::_003CModule_003E.ClusterRegDeleteKey(Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num, Resources.Registry_KeyDeleteFailed_Text, subkey);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe void DeleteValue(string name)
	{
		ushort* ptr = InteropHelp.StringToWstr(name);
		try
		{
			int num = (int)global::_003CModule_003E.ClusterRegDeleteValue(Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num, Resources.Registry_ValueDeleteFailed_Text, name);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe object GetValue(string name, object defaultValue)
	{
		//IL_0024: Expected I, but got I8
		ushort* ptr = InteropHelp.StringToWstr(name);
		uint num = 0u;
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		object obj = null;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint dwValueType);
			int num2 = global::_003CModule_003E.ClusterRegQueryValue(Handle, ptr, &dwValueType, null, &num);
			if (num2 == 234 || num2 == 0)
			{
				unmanagedBuffer.Allocate(num);
				num2 = global::_003CModule_003E.ClusterRegQueryValue(Handle, ptr, &dwValueType, (byte*)unmanagedBuffer.Pointer, &num);
			}
			switch (num2)
			{
			case 2:
				return defaultValue;
			default:
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num2, Resources.Registry_QueryValueFailed_Text, name);
				break;
			case 0:
				break;
			}
			return ConvertToManagedObject(unmanagedBuffer, dwValueType);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			unmanagedBuffer.Free();
		}
	}

	public object GetValue(string name)
	{
		object value = GetValue(name, null);
		if (value == null)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, 1168, Resources.Registry_QueryValueFailed_Text, name);
		}
		return value;
	}

	public unsafe RegistryValueKind GetValueKind(string name)
	{
		RegistryValueKind result = RegistryValueKind.Unknown;
		if (!TryGetValueKind(name, &result))
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, 2, Resources.Registry_QueryValueFailed_Text, name);
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool TryGetValueKind(string name, [Out] RegistryValueKind* kind)
	{
		//IL_001c: Expected I, but got I8
		ushort* ptr = InteropHelp.StringToWstr(name);
		uint num = 0u;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
		try
		{
			int num3 = global::_003CModule_003E.ClusterRegQueryValue(Handle, ptr, &num2, null, &num);
			switch (num3)
			{
			case 2:
				return false;
			default:
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num3, Resources.Registry_QueryValueFailed_Text, name);
				break;
			case 0:
				break;
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		*kind = (RegistryValueKind)num2;
		return true;
	}

	public unsafe ClusterRegistryKey OpenSubkey(string subkey, RegistryRights rights)
	{
		ushort* ptr = InteropHelp.StringToWstr(subkey);
		ClusterRegistryKey clusterRegistryKey = null;
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out HKEY__* hNativeHandle);
			int num = global::_003CModule_003E.ClusterRegOpenKey(Handle, ptr, (uint)rights, &hNativeHandle);
			SafeRegistryHandle handle = new SafeRegistryHandle(hNativeHandle);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, num, Resources.Registry_OpenSubKeyFailed_Text, subkey);
			}
			return new ClusterRegistryKey(m_cluster, handle);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe IEnumerable<Tuple<string, string, object>> BulkRegistryRead(IEnumerable<string> keys, IEnumerable<Tuple<string, RegistryValueKind>> valuesToGet)
	{
		//IL_0055: Expected I, but got I8
		//IL_0059: Expected I, but got I8
		//IL_00b1: Expected I, but got I8
		//IL_00b5: Expected I, but got I8
		//IL_0181: Expected I, but got I8
		//IL_018e: Expected I, but got I8
		//IL_01a2: Expected I, but got I8
		List<Tuple<string, string, object>> list = new List<Tuple<string, string, object>>();
		Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
		foreach (Tuple<string, RegistryValueKind> item4 in valuesToGet)
		{
			RegistryValueKind item = item4.Item2;
			dictionary[item4.Item1] = (uint)item;
		}
		_HREGREADBATCH* ptr = null;
		_HREGREADBATCHREPLY* ptr2 = null;
		try
		{
			uint num = (uint)global::_003CModule_003E.ClusterRegCreateReadBatch(Handle, &ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.Registry_Batch_Read_Failed);
			}
			uint num2;
			try
			{
				IEnumerator<string> enumerator2 = keys.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						IEnumerator<Tuple<string, RegistryValueKind>> enumerator3 = valuesToGet.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								Tuple<string, RegistryValueKind> current3 = enumerator3.Current;
								ushort* ptr3 = null;
								ushort* ptr4 = null;
								try
								{
									ptr3 = InteropHelp.StringToWstr(current2);
									ptr4 = InteropHelp.StringToWstr(current3.Item1);
									num = (uint)global::_003CModule_003E.ClusterRegReadBatchAddCommand(ptr, ptr3, ptr4);
									if (num != 0)
									{
										ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.Registry_Batch_Read_Failed);
									}
								}
								finally
								{
									InteropHelp.FreeWstr(ptr3);
									InteropHelp.FreeWstr(ptr4);
								}
							}
						}
						finally
						{
							IEnumerator<Tuple<string, RegistryValueKind>> enumerator4 = enumerator3;
							IDisposable disposable = enumerator3;
							enumerator3?.Dispose();
						}
					}
				}
				finally
				{
					IEnumerator<string> enumerator5 = enumerator2;
					IDisposable disposable2 = enumerator2;
					enumerator2?.Dispose();
				}
			}
			finally
			{
				num2 = (uint)global::_003CModule_003E.ClusterRegCloseReadBatch(ptr, &ptr2);
			}
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.Registry_Batch_Read_Failed);
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSTER_READ_BATCH_COMMAND cLUSTER_READ_BATCH_COMMAND);
			while (true)
			{
				num2 = (uint)global::_003CModule_003E.ClusterRegReadBatchReplyNextCommand(ptr2, &cLUSTER_READ_BATCH_COMMAND);
				switch (num2)
				{
				default:
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.Registry_Batch_Read_Failed);
					break;
				case 0u:
					if (*(int*)(&cLUSTER_READ_BATCH_COMMAND) == 8)
					{
						string item2 = InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.As<_CLUSTER_READ_BATCH_COMMAND, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTER_READ_BATCH_COMMAND, 8)));
						string text = InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.As<_CLUSTER_READ_BATCH_COMMAND, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTER_READ_BATCH_COMMAND, 16)));
						object item3 = ConvertToManagedObject(new UnmanagedBuffer((void*)System.Runtime.CompilerServices.Unsafe.As<_CLUSTER_READ_BATCH_COMMAND, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTER_READ_BATCH_COMMAND, 24)), System.Runtime.CompilerServices.Unsafe.As<_CLUSTER_READ_BATCH_COMMAND, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTER_READ_BATCH_COMMAND, 32))), dictionary[text]);
						list.Add(new Tuple<string, string, object>(item2, text, item3));
					}
					continue;
				case 259u:
					break;
				}
				break;
			}
		}
		finally
		{
			global::_003CModule_003E.ClusterRegCloseReadBatchReply(ptr2);
		}
		return list;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EClusterRegistryKey();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
