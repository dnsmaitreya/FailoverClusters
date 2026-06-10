using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Win32;

namespace MS.Internal.ServerClusters;

public static class Crypto
{
	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool GetPasswordForResource(RegistryKey clusterKey, ref byte[] knownPassword, ref byte[] proposedPassword)
	{
		//Discarded unreachable code: IL_0475, IL_0477
		//IL_003a: Expected I, but got I8
		//IL_006f: Expected I, but got I8
		//IL_0118: Expected I, but got I8
		//IL_015b: Expected I, but got I8
		//IL_021e: Expected I4, but got I8
		//IL_0272: Expected I, but got I8
		//IL_0292: Expected I, but got I8
		//IL_02ba: Expected I, but got I8
		//IL_032e: Expected I4, but got I8
		//IL_0371: Expected I4, but got I8
		//IL_03ee: Expected I4, but got I8
		//IL_0429: Expected I4, but got I8
		//The blocks IL_032e, IL_033f, IL_035b, IL_036c, IL_0371, IL_0374, IL_0378, IL_0380 are reachable both inside and outside the pinned region starting at IL_031c. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0371, IL_0374, IL_0378, IL_0380 are reachable both inside and outside the pinned region starting at IL_035b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0371, IL_0374, IL_0378, IL_0380 are reachable both inside and outside the pinned region starting at IL_035b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_03ee, IL_03fb, IL_0417, IL_0424, IL_0429 are reachable both inside and outside the pinned region starting at IL_03dd. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0429 are reachable both inside and outside the pinned region starting at IL_0417. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0429 are reachable both inside and outside the pinned region starting at IL_0417. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		string text = null;
		byte[] array = null;
		byte[] oldCnoCryptoData = null;
		string text2 = null;
		string text3 = null;
		string[] array2 = null;
		Exception ex = null;
		if (clusterKey == null)
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "clusterKey");
		}
		bool result = false;
		text = string.Empty;
		array = null;
		oldCnoCryptoData = null;
		byte* ptr = null;
		text2 = null;
		CRYPTO_KEY_INFO* ptr2 = (CRYPTO_KEY_INFO*)global::_003CModule_003E.@new(24uL);
		CRYPTO_KEY_INFO* ptr3;
		try
		{
			if (ptr2 != null)
			{
				*(int*)ptr2 = 3;
				*(int*)((ulong)(nint)ptr2 + 4uL) = 0;
				*(long*)((ulong)(nint)ptr2 + 8uL) = 0L;
				*(long*)((ulong)(nint)ptr2 + 16uL) = 0L;
				ptr3 = ptr2;
			}
			else
			{
				ptr3 = null;
			}
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr2);
			throw;
		}
		CRYPTO_KEY_INFO* ptr4 = ptr3;
		_CRYPTO_KEY_FILE_DATA* ptr5 = (_CRYPTO_KEY_FILE_DATA*)global::_003CModule_003E.@new(48uL);
		ulong num = 0uL;
		ulong num2 = 0uL;
		knownPassword = null;
		proposedPassword = null;
		try
		{
			text = "Getting Source Cluster CNO Resource Password Blob";
			GetOldCnoResourceData(&array, clusterKey);
			text = "Getting Source Cluster CNO Resource Crypto Blob";
			GetOldCnoCryptoData(&oldCnoCryptoData, &text2, clusterKey);
			text = "Converting container path to structure";
			PathToKeyInfo(ptr3, text2);
			text = "Reading Crypto Key into structure";
			GetCryptoStructure(ptr5, oldCnoCryptoData);
			text = "Acquiring Crypto Context";
			int cryptoContext = GetCryptoContext(&num, ptr3, bEnableRestore: false);
			if (0 != cryptoContext)
			{
				text = "Retry Acquiring Crypto Context with Restore Privilege";
				cryptoContext = GetCryptoContext(&num, ptr3, bEnableRestore: true);
				if (0 != cryptoContext)
				{
					throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CryptoBlobIsWrongSize_Text });
				}
			}
			_CRYPTO_KEY_FILE_DATA* ptr6 = (_CRYPTO_KEY_FILE_DATA*)((ulong)(nint)ptr5 + 4uL);
			uint num3 = *(uint*)ptr6;
			if (num3 != 0)
			{
				text = "Importing Signature Key";
				cryptoContext = ImportKey(&num, (int)num3, 48, oldCnoCryptoData, &num2);
				if (cryptoContext != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CryptoBlobIsWrongSize_Text });
				}
			}
			_CRYPTO_KEY_FILE_DATA* ptr7 = (_CRYPTO_KEY_FILE_DATA*)((ulong)(nint)ptr5 + 8uL);
			uint num4 = *(uint*)ptr7;
			if (num4 != 0)
			{
				text = "Importing Exchange Key";
				cryptoContext = ImportKey(&num, (int)num4, (int)((long)(uint)(*(int*)ptr6) + 48L), oldCnoCryptoData, &num2);
				if (cryptoContext != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CryptoBlobIsWrongSize_Text });
				}
			}
			uint num5 = *(uint*)((ulong)(nint)ptr5 + 12uL);
			if (num5 != 0)
			{
				text = "Setting Security Descriptor";
				cryptoContext = SetSecurityDescriptor(&num, (int)num5, (int)((long)(uint)(*(int*)ptr7) + (long)(uint)(*(int*)ptr6) + 48), oldCnoCryptoData);
				if (cryptoContext != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CryptoBlobIsWrongSize_Text });
				}
			}
			int num6 = (int)(array.LongLength - 4);
			ulong num7 = (ulong)num6;
			byte* ptr8 = (byte*)global::_003CModule_003E.new_005B_005D(num7);
			ptr = ptr8;
			fixed (byte* ptr9 = &array[0])
			{
				uint num8 = *(uint*)ptr9;
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr8, ref System.Runtime.CompilerServices.Unsafe.Add(ref *ptr9, 4), num7);
				uint num9 = (uint)num6;
				switch (num8)
				{
				case 2u:
				{
					text3 = (string)clusterKey.GetValue("ClusterNameResource");
					if (string.IsNullOrEmpty(text3))
					{
						throw new ArgumentException(Resources.Migration_ErrorReadingSourceClusterData_Text);
					}
					char[] separator = new char[1] { '\\' };
					array2 = text2.Split(separator);
					_HCLUSCRYPTPROVIDER* ptr13 = null;
					ptr13 = global::_003CModule_003E.OpenClusterCryptProviderEx(InteropHelp.StringToWstr(text3), InteropHelp.StringToWstr(array2[3]), (ushort*)(*(ulong*)((ulong)(nint)ptr3 + 8uL)), *(uint*)((ulong)(nint)ptr3 + 4uL), 0u);
					if (ptr13 == null)
					{
						throw ExceptionHelp.Build<ApplicationException>((int)global::_003CModule_003E.GetLastError(), new string[1] { Resources.Migration_CouldNotOpenCryptoContainer_Text });
					}
					byte* ptr14 = null;
					System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num12);
					cryptoContext = (int)global::_003CModule_003E.ClusterDecrypt(ptr13, ptr8, num9, &ptr14, &num12);
					if (cryptoContext != 0)
					{
						throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CouldNotDecryptCryptoContainer_Text });
					}
					if (num12 == 524)
					{
						_NNPWD_STRUCTURE* ptr15 = (_NNPWD_STRUCTURE*)ptr14;
						uint num13 = *(uint*)ptr14;
						uint num14;
						if (num13 != 0)
						{
							fixed (byte* ptr16 = &(knownPassword = new byte[(int)((long)(num13 + 1) * 2L)])[0])
							{
								// IL cpblk instruction
								System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr16, (long)(nint)ptr14 + 4L, knownPassword.LongLength);
								num14 = *(uint*)((ulong)(nint)ptr15 + 260uL);
								if (num14 != 0)
								{
									fixed (byte* ptr17 = &(proposedPassword = new byte[(int)((long)(num14 + 1) * 2L)])[0])
									{
										// IL cpblk instruction
										System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr17, (long)(nint)ptr15 + 264L, proposedPassword.LongLength);
										result = true;
										if (ptr14 != null)
										{
											global::_003CModule_003E.FreeClusterCrypt(ptr14);
										}
										global::_003CModule_003E.CloseClusterCryptProvider(ptr13);
									}
								}
								else
								{
									result = true;
									if (ptr14 != null)
									{
										global::_003CModule_003E.FreeClusterCrypt(ptr14);
									}
									global::_003CModule_003E.CloseClusterCryptProvider(ptr13);
								}
							}
							break;
						}
						num14 = *(uint*)((ulong)(nint)ptr15 + 260uL);
						if (num14 != 0)
						{
							fixed (byte* ptr17 = &(proposedPassword = new byte[(int)((long)(num14 + 1) * 2L)])[0])
							{
								// IL cpblk instruction
								System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr17, (long)(nint)ptr15 + 264L, proposedPassword.LongLength);
								result = true;
								if (ptr14 != null)
								{
									global::_003CModule_003E.FreeClusterCrypt(ptr14);
								}
								global::_003CModule_003E.CloseClusterCryptProvider(ptr13);
							}
							break;
						}
						result = true;
					}
					if (ptr14 != null)
					{
						global::_003CModule_003E.FreeClusterCrypt(ptr14);
					}
					global::_003CModule_003E.CloseClusterCryptProvider(ptr13);
					break;
				}
				case 1u:
					if (global::_003CModule_003E.CryptDecrypt(num2, 0uL, 1, 0u, ptr8, &num9) != 0)
					{
						num6 = (int)num9;
						if (num9 != 76)
						{
							break;
						}
						_NNPWD_STRUCTURE_PREV* ptr10 = (_NNPWD_STRUCTURE_PREV*)ptr8;
						uint num10 = *(uint*)ptr8;
						uint num11;
						if (num10 != 0)
						{
							fixed (byte* ptr11 = &(knownPassword = new byte[(int)((long)(num10 + 1) * 2L)])[0])
							{
								// IL cpblk instruction
								System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr11, (long)(nint)ptr8 + 4L, knownPassword.LongLength);
								num11 = *(uint*)((ulong)(nint)ptr8 + 36uL);
								if (num11 != 0)
								{
									fixed (byte* ptr12 = &(proposedPassword = new byte[(int)((long)(num11 + 1) * 2L)])[0])
									{
										// IL cpblk instruction
										System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr12, (long)(nint)ptr8 + 40L, proposedPassword.LongLength);
										result = true;
									}
								}
								else
								{
									result = true;
								}
							}
							break;
						}
						num11 = *(uint*)((ulong)(nint)ptr8 + 36uL);
						if (num11 != 0)
						{
							fixed (byte* ptr12 = &(proposedPassword = new byte[(int)((long)(num11 + 1) * 2L)])[0])
							{
								// IL cpblk instruction
								System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr12, (long)(nint)ptr8 + 40L, proposedPassword.LongLength);
								result = true;
							}
						}
						else
						{
							result = true;
						}
						break;
					}
					cryptoContext = (int)global::_003CModule_003E.GetLastError();
					throw ExceptionHelp.Build<ApplicationException>(cryptoContext, new string[1] { Resources.Migration_CouldNotDecryptCryptoCheckPoint_Text });
				}
			}
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, text);
			throw ExceptionHelp.Build<ApplicationException>(ex2, new string[1] { Resources.Migration_ErrorReadingSourceClusterData_Text });
		}
		finally
		{
			DeleteCryptoContext(&num, ptr4);
			global::_003CModule_003E.delete(ptr);
			if (ptr4 != null)
			{
				global::_003CModule_003E.CRYPTO_KEY_INFO_002E__delDtor(ptr4, 1u);
			}
			global::_003CModule_003E.delete(ptr5);
		}
		return result;
	}

	private unsafe static void GetOldCnoResourceData(byte[]* oldCnoResourceData, RegistryKey clusterKey)
	{
		//Discarded unreachable code: IL_00cb
		string text = null;
		object obj = null;
		RegistryKey registryKey = null;
		string text2 = null;
		RegistryKey registryKey2 = null;
		RegistryKey registryKey3 = null;
		Exception ex = null;
		text = string.Empty;
		obj = null;
		try
		{
			registryKey = null;
			text = "Opening Source Cluster Resources key";
			obj = null;
			registryKey = clusterKey.OpenSubKey("Resources");
			text = "Getting Source Cluster CNO ID";
			obj = null;
			text2 = (string)clusterKey.GetValue("ClusterNameResource");
			if (string.IsNullOrEmpty(text2))
			{
				throw new ArgumentException(Resources.Migration_ErrorReadingSourceClusterData_Text);
			}
			registryKey2 = null;
			text = "Opening Source Cluster CNO Resources key {0}";
			obj = text2;
			registryKey2 = registryKey.OpenSubKey(text2);
			registryKey3 = null;
			text = "Opening Source Cluster CNO Parameters key";
			obj = null;
			registryKey3 = registryKey2.OpenSubKey("Parameters");
			text = "Getting Source Cluster CNO ResourceData";
			obj = null;
			*oldCnoResourceData = (byte[])registryKey3.GetValue("ResourceData");
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, text, obj);
			throw ExceptionHelp.Build<ApplicationException>(ex2, new string[1] { Resources.Migration_ErrorReadingSourceClusterData_Text });
		}
	}

	private unsafe static void GetOldCnoCryptoData(byte[]* oldCnoCryptoData, string* cryptoContainer, RegistryKey clusterKey)
	{
		//Discarded unreachable code: IL_00fd, IL_0125
		string text = null;
		object obj = null;
		RegistryKey registryKey = null;
		string text2 = null;
		RegistryKey registryKey2 = null;
		RegistryKey registryKey3 = null;
		string[] array = null;
		RegistryKey registryKey4 = null;
		Exception ex = null;
		text = string.Empty;
		obj = null;
		try
		{
			registryKey = null;
			text = "Opening Source Cluster Checkpoints key";
			obj = null;
			registryKey = clusterKey.OpenSubKey("Checkpoints");
			text = "Getting Source Cluster CNO ID";
			obj = null;
			text2 = (string)clusterKey.GetValue("ClusterNameResource");
			if (string.IsNullOrEmpty(text2))
			{
				throw new ArgumentException(Resources.Migration_ErrorReadingSourceClusterData_Text);
			}
			registryKey2 = null;
			text = "Opening Source Cluster CNO Checkpoint key {0}";
			obj = text2;
			registryKey2 = registryKey.OpenSubKey(text2);
			registryKey3 = null;
			text = "Opening Source Cluster CNO Crypto key";
			obj = null;
			registryKey3 = registryKey2.OpenSubKey("Crypto");
			text = "Getting Source Cluster CNO Checkpoints";
			obj = null;
			array = (string[])registryKey3.GetValue("Checkpoints");
			if (array != null && (nint)array.LongLength >= 2)
			{
				registryKey4 = null;
				text = "Getting Source Cluster CNO Crypto Data";
				obj = null;
				registryKey4 = registryKey3.OpenSubKey(array[1]);
				*cryptoContainer = (string)registryKey4.GetValue("CryptoContainer");
				*oldCnoCryptoData = (byte[])registryKey4.GetValue("Data");
				return;
			}
			throw new ArgumentException(Resources.Migration_ErrorReadingSourceClusterData_Text);
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, text, obj);
			throw ExceptionHelp.Build<ApplicationException>(ex2, new string[1] { Resources.Migration_ErrorReadingSourceClusterData_Text });
		}
	}

	private unsafe static void PathToKeyInfo(CRYPTO_KEY_INFO* pCryptoKeyInfo, string CryptoPath)
	{
		//IL_003d: Expected I8, but got I
		//IL_0055: Expected I8, but got I
		string[] array = CryptoPath.Split('\\');
		if ((nint)array.LongLength >= 3)
		{
			*(int*)((ulong)(nint)pCryptoKeyInfo + 4uL) = Convert.ToInt32(array[0], CultureInfo.CurrentCulture);
			*(long*)((ulong)(nint)pCryptoKeyInfo + 8uL) = (nint)InteropHelp.StringToWstr(array[1]);
			*(long*)((ulong)(nint)pCryptoKeyInfo + 16uL) = (nint)InteropHelp.StringToWstr(Guid.NewGuid().ToString());
		}
	}

	private unsafe static void GetCryptoStructure(_CRYPTO_KEY_FILE_DATA* pCrypto, byte[] oldCnoCryptoData)
	{
		fixed (byte* ptr = &oldCnoCryptoData[0])
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(pCrypto, ptr, 48);
			if (*(int*)pCrypto != 3)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.Migration_CryptoBlobIsWrongVersion_Text });
			}
			if ((int)((uint)(*(int*)((ulong)(nint)pCrypto + 4uL)) + ((long)(uint)(*(int*)((ulong)(nint)pCrypto + 12uL)) + (long)(uint)(*(int*)((ulong)(nint)pCrypto + 8uL))) + 48) > (nint)oldCnoCryptoData.LongLength)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.Migration_CryptoBlobIsWrongSize_Text });
			}
		}
	}

	private unsafe static int GetCryptoContext(ulong* hProv, CRYPTO_KEY_INFO* pCryptoKeyInfo, [MarshalAs(UnmanagedType.U1)] bool bEnableRestore)
	{
		//IL_002f: Expected I, but got I8
		//IL_002f: Expected I, but got I8
		//IL_004f: Expected I, but got I8
		//IL_004f: Expected I, but got I8
		//IL_006d: Expected I, but got I8
		//IL_006d: Expected I, but got I8
		//IL_0087: Expected I, but got I8
		//IL_0087: Expected I, but got I8
		int num = 0;
		bool flag = false;
		if (bEnableRestore)
		{
			num = (int)global::_003CModule_003E.ClRtlEnableThreadPrivilege(18u, (byte*)(&flag));
			if (num != 0)
			{
				goto IL_00a9;
			}
		}
		if (global::_003CModule_003E.CryptAcquireContextW(hProv, (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 16uL)), (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 8uL)), *(uint*)((ulong)(nint)pCryptoKeyInfo + 4uL), 40u) == 0)
		{
			num = (int)global::_003CModule_003E.GetLastError();
			if (global::_003CModule_003E.CryptAcquireContextW(hProv, (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 16uL)), (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 8uL)), *(uint*)((ulong)(nint)pCryptoKeyInfo + 4uL), 32u) != 0)
			{
				num = 0;
			}
			else if (global::_003CModule_003E.CryptAcquireContextW(hProv, (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 16uL)), (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 8uL)), *(uint*)((ulong)(nint)pCryptoKeyInfo + 4uL), 48u) != 0)
			{
				num = ((global::_003CModule_003E.CryptAcquireContextW(hProv, (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 16uL)), (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 8uL)), *(uint*)((ulong)(nint)pCryptoKeyInfo + 4uL), 40u) == 0) ? num : 0);
			}
		}
		if (bEnableRestore)
		{
			ExceptionHelp.LogException(new Win32Exception((int)global::_003CModule_003E.ClRtlRestoreThreadPrivilege(18u, flag ? ((byte)1) : ((byte)0))), "Non fatal error occurred during ClRtlRestoreThreadPrivilege, but code execution can go on.");
		}
		goto IL_00a9;
		IL_00a9:
		return num;
	}

	private unsafe static int DeleteCryptoContext(ulong* hProv, CRYPTO_KEY_INFO* pCryptoKeyInfo)
	{
		//IL_0019: Expected I, but got I8
		//IL_0019: Expected I, but got I8
		int result = 0;
		if (global::_003CModule_003E.CryptAcquireContextW(hProv, (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 16uL)), (ushort*)(*(ulong*)((ulong)(nint)pCryptoKeyInfo + 8uL)), *(uint*)((ulong)(nint)pCryptoKeyInfo + 4uL), 48u) == 0)
		{
			result = (int)global::_003CModule_003E.GetLastError();
		}
		return result;
	}

	private unsafe static int ImportKey(ulong* hProv, int cbSize, int offset, byte[] oldCnoCryptoData, ulong* hKey)
	{
		//IL_001e: Expected I4, but got I8
		int result = 0;
		fixed (byte* ptr2 = &oldCnoCryptoData[0])
		{
			ulong num = (ulong)cbSize;
			byte* ptr = (byte*)global::_003CModule_003E.new_005B_005D(num);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr, ref System.Runtime.CompilerServices.Unsafe.Add(ref *ptr2, offset), num);
			if (global::_003CModule_003E.CryptImportKey(*hProv, ptr, (uint)cbSize, 0uL, 1u, hKey) == 0)
			{
				result = (int)global::_003CModule_003E.GetLastError();
			}
			global::_003CModule_003E.delete(ptr);
			return result;
		}
	}

	private unsafe static int SetSecurityDescriptor(ulong* hProv, int cbSize, int offset, byte[] oldCnoCryptoData)
	{
		//IL_001e: Expected I4, but got I8
		int result = 0;
		fixed (byte* ptr2 = &oldCnoCryptoData[0])
		{
			ulong num = (ulong)cbSize;
			byte* ptr = (byte*)global::_003CModule_003E.new_005B_005D(num);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr, ref System.Runtime.CompilerServices.Unsafe.Add(ref *ptr2, offset), num);
			if (global::_003CModule_003E.CryptSetProvParam(*hProv, 8u, ptr, 15u) == 0)
			{
				result = (int)global::_003CModule_003E.GetLastError();
			}
			global::_003CModule_003E.delete(ptr);
			return result;
		}
	}
}

