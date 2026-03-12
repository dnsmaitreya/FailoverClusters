using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public abstract class ControlExecutor
{
	private Cluster m_cluster;

	protected ControlCodesEventHandler m_controlCodesMonitor;

	private unsafe void ExecuteWrappedControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer, UnmanagedBuffer outputBuffer, [MarshalAs(UnmanagedType.U1)] bool throwOnInvalidFunction)
	{
		//IL_002d: Expected I4, but got I8
		uint num = 0u;
		uint num2 = ExecuteRawControl(node, controlCode, inputBuffer, outputBuffer, &num);
		if (outputBuffer != null && num == 0)
		{
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(outputBuffer.Pointer, 0, outputBuffer.Size);
		}
		switch (num2)
		{
		case 234u:
		{
			object[] array = new object[1];
			object[] array2 = new object[1];
			array[0] = "DATA_SIZE";
			array2[0] = num;
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, 234, array, array2);
			return;
		}
		case 0u:
			return;
		}
		if (!throwOnInvalidFunction && num2 == 1)
		{
			outputBuffer?.Free();
			return;
		}
		object[] array3 = new object[3] { controlCode, null, null };
		string text = ((node == null) ? string.Empty : node.Name);
		array3[1] = text;
		array3[2] = num2;
		DebugLog.LogError("There was an error executing control code '{0}' on node '{1}', the error is '{2}'.", array3);
		ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2);
	}

	private void ExecuteWrappedControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer, UnmanagedBuffer outputBuffer)
	{
		ExecuteWrappedControl(node, controlCode, inputBuffer, outputBuffer, throwOnInvalidFunction: true);
	}

	private unsafe UnmanagedBuffer AllocateExecuteControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer, [MarshalAs(UnmanagedType.U1)] bool throwOnInvalidFunction)
	{
		//IL_0008: Expected I8, but got I
		//IL_007c: Expected I, but got I8
		//IL_007c: Expected I, but got I8
		//IL_008c: Expected I, but got I8
		//IL_00bc: Expected I, but got I8
		long num = (nint)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			unmanagedBuffer.Allocate(8192uL);
			try
			{
				ExecuteWrappedControl(node, controlCode, inputBuffer, unmanagedBuffer, throwOnInvalidFunction);
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode != -2147024662)
				{
					throw;
				}
				unmanagedBuffer.Allocate((uint)ex.Data["DATA_SIZE"]);
				ExecuteWrappedControl(node, controlCode, inputBuffer, unmanagedBuffer, throwOnInvalidFunction);
			}
		}
		catch when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			uint exceptionCode = (uint)Marshal.GetExceptionCode();
			return (byte)global::_003CModule_003E.__CxxExceptionFilter((void*)Marshal.GetExceptionPointers(), null, 0, null) != 0;
		}).Invoke())
		{
			uint num2 = 0u;
			global::_003CModule_003E.__CxxRegisterExceptionObject((void*)Marshal.GetExceptionPointers(), (void*)num);
			try
			{
				try
				{
					unmanagedBuffer.Free();
					global::_003CModule_003E._CxxThrowException(null, null);
					goto end_IL_008d;
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
				end_IL_008d:;
			}
			finally
			{
				global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
			}
		}
		return unmanagedBuffer;
	}

	private UnmanagedBuffer AllocateExecuteControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer)
	{
		return AllocateExecuteControl(node, controlCode, inputBuffer, throwOnInvalidFunction: true);
	}

	protected ControlExecutor(Cluster cluster)
	{
		m_cluster = cluster;
		m_controlCodesMonitor = cluster.ControlCodesMonitor;
	}

	public void ExecuteControl(uint controlCode)
	{
		ExecuteWrappedControl(null, controlCode, null, null, throwOnInvalidFunction: true);
	}

	protected unsafe abstract uint ExecuteControl(ClusterNode node, uint controlCode, void* inBuffer, uint inputBufferSize, void* outBuffer, uint outputBufferSize, uint* bytesReturned);

	internal void ExecuteInControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer)
	{
		ExecuteWrappedControl(node, controlCode, inputBuffer, null, throwOnInvalidFunction: true);
	}

	internal void ExecuteInControl(uint controlCode, UnmanagedBuffer inputBuffer)
	{
		ExecuteWrappedControl(null, controlCode, inputBuffer, null, throwOnInvalidFunction: true);
	}

	internal UnmanagedBuffer ExecuteInOutControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer)
	{
		return AllocateExecuteControl(node, controlCode, inputBuffer, throwOnInvalidFunction: true);
	}

	internal UnmanagedBuffer ExecuteInOutControl(uint controlCode, UnmanagedBuffer inputBuffer)
	{
		return AllocateExecuteControl(null, controlCode, inputBuffer, throwOnInvalidFunction: true);
	}

	internal void ExecuteInOutControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer, UnmanagedBuffer outputBuffer)
	{
		ExecuteWrappedControl(node, controlCode, inputBuffer, outputBuffer, throwOnInvalidFunction: true);
	}

	internal void ExecuteInOutControl(uint controlCode, UnmanagedBuffer inputBuffer, UnmanagedBuffer outputBuffer)
	{
		ExecuteWrappedControl(null, controlCode, inputBuffer, outputBuffer, throwOnInvalidFunction: true);
	}

	internal UnmanagedBuffer ExecuteInOutControlWithoutThrowOnInvalidFunction(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer)
	{
		return AllocateExecuteControl(node, controlCode, inputBuffer, throwOnInvalidFunction: false);
	}

	internal UnmanagedBuffer ExecuteInOutControlWithoutThrowOnInvalidFunction(uint controlCode, UnmanagedBuffer inputBuffer)
	{
		return AllocateExecuteControl(null, controlCode, inputBuffer, throwOnInvalidFunction: false);
	}

	internal unsafe StringCollection ExecuteStringCollectionOutControl(uint controlCode, [MarshalAs(UnmanagedType.U1)] bool throwOnInvalidFunction)
	{
		//Discarded unreachable code: IL_0087
		//IL_0065: Expected I, but got I8
		//IL_0075: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = ((!throwOnInvalidFunction) ? AllocateExecuteControl(null, controlCode, null, throwOnInvalidFunction: false) : ExecuteOutControl(controlCode, 0u));
			StringCollection stringCollection = new StringCollection();
			if (unmanagedBuffer.Size != 0)
			{
				ushort* pointer = (ushort*)unmanagedBuffer.Pointer;
				ushort* ptr = pointer;
				while (*ptr != 0 && (ulong)((long)((nint)((byte*)ptr - (nuint)pointer) >> 1) * 2L) <= unmanagedBuffer.Size)
				{
					stringCollection.Add(InteropHelp.WstrToString(ptr));
					ushort* ptr2 = ptr;
					while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
					{
						ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
					}
					ptr = (ushort*)((long)((nint)((byte*)ptr2 - (nuint)ptr) >> 1) * 2L + (nint)ptr + 2);
				}
			}
			return stringCollection;
		}
		finally
		{
			unmanagedBuffer?.Free();
		}
	}

	internal StringCollection ExecuteStringCollectionOutControl(uint controlCode)
	{
		return ExecuteStringCollectionOutControl(controlCode, throwOnInvalidFunction: true);
	}

	internal void ExecuteOutControl(ClusterNode node, uint controlCode, UnmanagedBuffer outputBuffer)
	{
		ExecuteWrappedControl(node, controlCode, null, outputBuffer, throwOnInvalidFunction: true);
	}

	internal void ExecuteOutControl(uint controlCode, UnmanagedBuffer outputBuffer)
	{
		ExecuteWrappedControl(null, controlCode, null, outputBuffer, throwOnInvalidFunction: true);
	}

	internal unsafe UnmanagedBuffer ExecuteOutControl(uint controlCode, uint initialSize)
	{
		//Discarded unreachable code: IL_0054
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			unmanagedBuffer.Allocate(initialSize);
			uint num = 0u;
			uint num2 = ExecuteRawControl(null, controlCode, null, unmanagedBuffer, &num);
			if (num2 == 234)
			{
				unmanagedBuffer.Allocate(num);
				num2 = ExecuteRawControl(null, controlCode, null, unmanagedBuffer, &num);
			}
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2);
			}
		}
		catch (Exception)
		{
			unmanagedBuffer.Free();
			throw;
		}
		return unmanagedBuffer;
	}

	internal UnmanagedBuffer ExecuteOutControl(uint controlCode)
	{
		return ExecuteOutControl(controlCode, 0u);
	}

	internal UnmanagedBuffer ExecuteOutControlWithoutThrowOnInvalidFunction(uint controlCode)
	{
		return AllocateExecuteControl(null, controlCode, null, throwOnInvalidFunction: false);
	}

	internal void ExecuteCommandControl(uint controlCode)
	{
		ExecuteWrappedControl(null, controlCode, null, null, throwOnInvalidFunction: true);
	}

	internal unsafe uint ExecuteRawControl(ClusterNode node, uint controlCode, UnmanagedBuffer inputBuffer, UnmanagedBuffer outputBuffer, uint* pdwBytesReturned)
	{
		//IL_0008: Expected I, but got I8
		//IL_000d: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* inBuffer = null;
		uint inputBufferSize = 0u;
		void* outBuffer = null;
		uint outputBufferSize = 0u;
		*pdwBytesReturned = 0u;
		if (inputBuffer != null)
		{
			inBuffer = inputBuffer.Pointer;
			inputBufferSize = (uint)inputBuffer.Size;
		}
		if (outputBuffer != null)
		{
			outBuffer = outputBuffer.Pointer;
			outputBufferSize = (uint)outputBuffer.Size;
		}
		return ExecuteControl(node, controlCode, inBuffer, inputBufferSize, outBuffer, outputBufferSize, pdwBytesReturned);
	}

	internal unsafe Guid GetId(IClusterItemControlCodes controlCodes)
	{
		//Discarded unreachable code: IL_0042
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		unmanagedBuffer.Allocate(74uL);
		try
		{
			uint getIdCode = controlCodes.GetIdCode;
			ExecuteWrappedControl(null, getIdCode, null, unmanagedBuffer, throwOnInvalidFunction: true);
			string g = InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			return new Guid(g);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	internal string StringControlCode(uint controlCode)
	{
		return controlCode switch
		{
			16777233u => "CLUSCTL_RESOURCE_GET_REQUIRED_DEPENDENCIES", 
			16777229u => "CLUSCTL_RESOURCE_GET_CLASS_INFO", 
			16777225u => "CLUSCTL_RESOURCE_GET_FLAGS", 
			16777221u => "CLUSCTL_RESOURCE_GET_CHARACTERISTICS", 
			16777216u => "CLUSCTL_RESOURCE_UNKNOWN", 
			16777257u => "CLUSCTL_RESOURCE_GET_NAME", 
			16777305u => "CLUSCTL_RESOURCE_GET_COMMON_PROPERTIES", 
			16777301u => "CLUSCTL_RESOURCE_GET_RO_COMMON_PROPERTIES", 
			16777297u => "CLUSCTL_RESOURCE_ENUM_COMMON_PROPERTIES", 
			16777273u => "CLUSCTL_RESOURCE_GET_ID", 
			16777261u => "CLUSCTL_RESOURCE_GET_RESOURCE_TYPE", 
			16777313u => "CLUSCTL_RESOURCE_VALIDATE_COMMON_PROPERTIES", 
			16777353u => "CLUSCTL_RESOURCE_VALIDATE_PRIVATE_PROPERTIES", 
			16777345u => "CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTIES", 
			16777341u => "CLUSCTL_RESOURCE_GET_RO_PRIVATE_PROPERTIES", 
			16777337u => "CLUSCTL_RESOURCE_ENUM_PRIVATE_PROPERTIES", 
			16777317u => "CLUSCTL_RESOURCE_GET_COMMON_PROPERTY_FMTS", 
			16777357u => "CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTY_FMTS", 
			16777581u => "CLUSCTL_RESOURCE_NETNAME_GET_VIRTUAL_SERVER_TOKEN", 
			16777577u => "CLUSCTL_RESOURCE_GET_NETWORK_NAME", 
			16777397u => "CLUSCTL_RESOURCE_GET_CRYPTO_CHECKPOINTS", 
			16777385u => "CLUSCTL_RESOURCE_GET_REGISTRY_CHECKPOINTS", 
			16777586u => "CLUSCTL_RESOURCE_NETNAME_REGISTER_DNS_RECORDS", 
			16777617u => "CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO", 
			16777601u => "CLUSCTL_RESOURCE_NETNAME_VALIDATE_VCO", 
			16777598u => "CLUSCTL_RESOURCE_NETNAME_DELETE_CO", 
			16777594u => "CLUSCTL_RESOURCE_NETNAME_SET_PWD_INFO", 
			16777589u => "CLUSCTL_RESOURCE_GET_DNS_NAME", 
			16777625u => "CLUSCTL_RESOURCE_STORAGE_IS_PATH_VALID", 
			17825885u => "CLUSCTL_RESOURCE_FSWITNESS_GET_EPOCH_INFO", 
			16777809u => "CLUSCTL_RESOURCE_FILESERVER_SHARE_REPORT", 
			16777745u => "CLUSCTL_RESOURCE_STORAGE_GET_MOUNTPOINTS", 
			16777713u => "CLUSCTL_RESOURCE_STORAGE_GET_DISK_INFO_EX", 
			16777697u => "CLUSCTL_RESOURCE_QUERY_MAINTENANCE_MODE", 
			20971614u => "CLUSCTL_RESOURCE_SET_COMMON_PROPERTIES", 
			20971698u => "CLUSCTL_RESOURCE_DELETE_CRYPTO_CHECKPOINT", 
			20971694u => "CLUSCTL_RESOURCE_ADD_CRYPTO_CHECKPOINT", 
			20971686u => "CLUSCTL_RESOURCE_DELETE_REGISTRY_CHECKPOINT", 
			20971682u => "CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT", 
			20971654u => "CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES", 
			20971706u => "CLUSCTL_RESOURCE_UPGRADE_DLL", 
			20971970u => "CLUSCTL_RESOURCE_IPADDRESS_RELEASE_LEASE", 
			20971966u => "CLUSCTL_RESOURCE_IPADDRESS_RENEW_LEASE", 
			20971714u => "CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT_32BIT", 
			20971710u => "CLUSCTL_RESOURCE_ADD_REGISTRY_CHECKPOINT_64BIT", 
			20972006u => "CLUSCTL_RESOURCE_SET_MAINTENANCE_MODE", 
			20972182u => "CLUSCTL_RESOURCE_SET_CSV_MAINTENANCE_MODE", 
			20972110u => "CLUSCTL_RESOURCE_FILESERVER_SHARE_MODIFY", 
			20972106u => "CLUSCTL_RESOURCE_FILESERVER_SHARE_DEL", 
			20972102u => "CLUSCTL_RESOURCE_FILESERVER_SHARE_ADD", 
			20972010u => "CLUSCTL_RESOURCE_STORAGE_SET_DRIVELETTER", 
			22020102u => "CLUSCTL_RESOURCE_DELETE", 
			22020122u => "CLUSCTL_RESOURCE_ADD_OWNER", 
			22020118u => "CLUSCTL_RESOURCE_REMOVE_DEPENDENCY", 
			22020114u => "CLUSCTL_RESOURCE_ADD_DEPENDENCY", 
			22020110u => "CLUSCTL_RESOURCE_EVICT_NODE", 
			22020106u => "CLUSCTL_RESOURCE_INSTALL_NODE", 
			22020126u => "CLUSCTL_RESOURCE_REMOVE_OWNER", 
			22020178u => "CLUSCTL_RESOURCE_PROVIDER_STATE_CHANGE", 
			22020174u => "CLUSCTL_RESOURCE_STATE_CHANGE_REASON", 
			22020166u => "CLUSCTL_RESOURCE_FORCE_QUORUM", 
			22020142u => "CLUSCTL_RESOURCE_CLUSTER_VERSION_CHANGED", 
			22020134u => "CLUSCTL_RESOURCE_SET_NAME", 
			22020182u => "CLUSCTL_RESOURCE_LEAVING_GROUP", 
			23068680u => "CLUSCTL_RESOURCE_VM_CANCEL_MIGRATION", 
			23068676u => "CLUSCTL_RESOURCE_VM_START_MIGRATION", 
			22020194u => "CLUSCTL_RESOURCE_FSWITNESS_SET_EPOCH_INFO", 
			22020186u => "CLUSCTL_RESOURCE_JOINING_GROUP", 
			23068684u => "CLUSCTL_RESOURCE_VM_ACCEPT_MIGRATION", 
			33554441u => "CLUSCTL_RESOURCE_TYPE_GET_FLAGS", 
			33554437u => "CLUSCTL_RESOURCE_TYPE_GET_CHARACTERISTICS", 
			33554432u => "CLUSCTL_RESOURCE_TYPE_UNKNOWN", 
			23068692u => "CLUSCTL_RESOURCE_VM_SET_NEXT_OFFLINE_ACTION", 
			23068688u => "CLUSCTL_RESOURCE_VM_HALT_MIGRATION", 
			33554445u => "CLUSCTL_RESOURCE_TYPE_GET_CLASS_INFO", 
			33554521u => "CLUSCTL_RESOURCE_TYPE_GET_COMMON_PROPERTIES", 
			33554517u => "CLUSCTL_RESOURCE_TYPE_GET_RO_COMMON_PROPERTIES", 
			33554513u => "CLUSCTL_RESOURCE_TYPE_ENUM_COMMON_PROPERTIES", 
			33554449u => "CLUSCTL_RESOURCE_TYPE_GET_REQUIRED_DEPENDENCIES", 
			33554529u => "CLUSCTL_RESOURCE_TYPE_VALIDATE_COMMON_PROPERTIES", 
			33554561u => "CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_PROPERTIES", 
			33554557u => "CLUSCTL_RESOURCE_TYPE_GET_RO_PRIVATE_PROPERTIES", 
			33554553u => "CLUSCTL_RESOURCE_TYPE_ENUM_PRIVATE_PROPERTIES", 
			33554537u => "CLUSCTL_RESOURCE_TYPE_GET_COMMON_RESOURCE_PROPERTY_FMTS", 
			33554533u => "CLUSCTL_RESOURCE_TYPE_GET_COMMON_PROPERTY_FMTS", 
			33554569u => "CLUSCTL_RESOURCE_TYPE_VALIDATE_PRIVATE_PROPERTIES", 
			33554613u => "CLUSCTL_RESOURCE_TYPE_GET_CRYPTO_CHECKPOINTS", 
			33554601u => "CLUSCTL_RESOURCE_TYPE_GET_REGISTRY_CHECKPOINTS", 
			33554577u => "CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_RESOURCE_PROPERTY_FMTS", 
			33554573u => "CLUSCTL_RESOURCE_TYPE_GET_PRIVATE_PROPERTY_FMTS", 
			33554837u => "CLUSCTL_RESOURCE_TYPE_STORAGE_GET_AVAILABLE_DISKS", 
			50331653u => "CLUSCTL_GROUP_GET_CHARACTERISTICS", 
			50331648u => "CLUSCTL_GROUP_UNKNOWN", 
			37748870u => "CLUSCTL_RESOURCE_TYPE_SET_PRIVATE_PROPERTIES", 
			37748830u => "CLUSCTL_RESOURCE_TYPE_SET_COMMON_PROPERTIES", 
			33554873u => "CLUSCTL_RESOURCE_TYPE_QUERY_DELETE", 
			50331657u => "CLUSCTL_GROUP_GET_FLAGS", 
			50331737u => "CLUSCTL_GROUP_GET_COMMON_PROPERTIES", 
			50331733u => "CLUSCTL_GROUP_GET_RO_COMMON_PROPERTIES", 
			50331729u => "CLUSCTL_GROUP_ENUM_COMMON_PROPERTIES", 
			50331705u => "CLUSCTL_GROUP_GET_ID", 
			50331689u => "CLUSCTL_GROUP_GET_NAME", 
			50331745u => "CLUSCTL_GROUP_VALIDATE_COMMON_PROPERTIES", 
			50331785u => "CLUSCTL_GROUP_VALIDATE_PRIVATE_PROPERTIES", 
			50331777u => "CLUSCTL_GROUP_GET_PRIVATE_PROPERTIES", 
			50331773u => "CLUSCTL_GROUP_GET_RO_PRIVATE_PROPERTIES", 
			50331769u => "CLUSCTL_GROUP_ENUM_PRIVATE_PROPERTIES", 
			50331749u => "CLUSCTL_GROUP_GET_COMMON_PROPERTY_FMTS", 
			50331789u => "CLUSCTL_GROUP_GET_PRIVATE_PROPERTY_FMTS", 
			67108864u => "CLUSCTL_NODE_UNKNOWN", 
			54526086u => "CLUSCTL_GROUP_SET_PRIVATE_PROPERTIES", 
			54526046u => "CLUSCTL_GROUP_SET_COMMON_PROPERTIES", 
			50332089u => "CLUSCTL_GROUP_QUERY_DELETE", 
			67108869u => "CLUSCTL_NODE_GET_CHARACTERISTICS", 
			67108949u => "CLUSCTL_NODE_GET_RO_COMMON_PROPERTIES", 
			67108945u => "CLUSCTL_NODE_ENUM_COMMON_PROPERTIES", 
			67108921u => "CLUSCTL_NODE_GET_ID", 
			67108905u => "CLUSCTL_NODE_GET_NAME", 
			67108873u => "CLUSCTL_NODE_GET_FLAGS", 
			67108953u => "CLUSCTL_NODE_GET_COMMON_PROPERTIES", 
			67108993u => "CLUSCTL_NODE_GET_PRIVATE_PROPERTIES", 
			67108989u => "CLUSCTL_NODE_GET_RO_PRIVATE_PROPERTIES", 
			67108985u => "CLUSCTL_NODE_ENUM_PRIVATE_PROPERTIES", 
			67108965u => "CLUSCTL_NODE_GET_COMMON_PROPERTY_FMTS", 
			67108961u => "CLUSCTL_NODE_VALIDATE_COMMON_PROPERTIES", 
			67109001u => "CLUSCTL_NODE_VALIDATE_PRIVATE_PROPERTIES", 
			83886085u => "CLUSCTL_NETWORK_GET_CHARACTERISTICS", 
			83886080u => "CLUSCTL_NETWORK_UNKNOWN", 
			71303302u => "CLUSCTL_NODE_SET_PRIVATE_PROPERTIES", 
			71303262u => "CLUSCTL_NODE_SET_COMMON_PROPERTIES", 
			67109005u => "CLUSCTL_NODE_GET_PRIVATE_PROPERTY_FMTS", 
			83886089u => "CLUSCTL_NETWORK_GET_FLAGS", 
			83886165u => "CLUSCTL_NETWORK_GET_RO_COMMON_PROPERTIES", 
			83886161u => "CLUSCTL_NETWORK_ENUM_COMMON_PROPERTIES", 
			83886137u => "CLUSCTL_NETWORK_GET_ID", 
			83886121u => "CLUSCTL_NETWORK_GET_NAME", 
			83886169u => "CLUSCTL_NETWORK_GET_COMMON_PROPERTIES", 
			83886209u => "CLUSCTL_NETWORK_GET_PRIVATE_PROPERTIES", 
			83886205u => "CLUSCTL_NETWORK_GET_RO_PRIVATE_PROPERTIES", 
			83886201u => "CLUSCTL_NETWORK_ENUM_PRIVATE_PROPERTIES", 
			83886181u => "CLUSCTL_NETWORK_GET_COMMON_PROPERTY_FMTS", 
			83886177u => "CLUSCTL_NETWORK_VALIDATE_COMMON_PROPERTIES", 
			83886217u => "CLUSCTL_NETWORK_VALIDATE_PRIVATE_PROPERTIES", 
			100663301u => "CLUSCTL_NETINTERFACE_GET_CHARACTERISTICS", 
			100663296u => "CLUSCTL_NETINTERFACE_UNKNOWN", 
			88080518u => "CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES", 
			88080478u => "CLUSCTL_NETWORK_SET_COMMON_PROPERTIES", 
			83886221u => "CLUSCTL_NETWORK_GET_PRIVATE_PROPERTY_FMTS", 
			100663305u => "CLUSCTL_NETINTERFACE_GET_FLAGS", 
			100663377u => "CLUSCTL_NETINTERFACE_ENUM_COMMON_PROPERTIES", 
			100663353u => "CLUSCTL_NETINTERFACE_GET_ID", 
			100663349u => "CLUSCTL_NETINTERFACE_GET_NETWORK", 
			100663345u => "CLUSCTL_NETINTERFACE_GET_NODE", 
			100663337u => "CLUSCTL_NETINTERFACE_GET_NAME", 
			100663381u => "CLUSCTL_NETINTERFACE_GET_RO_COMMON_PROPERTIES", 
			100663417u => "CLUSCTL_NETINTERFACE_ENUM_PRIVATE_PROPERTIES", 
			100663397u => "CLUSCTL_NETINTERFACE_GET_COMMON_PROPERTY_FMTS", 
			100663393u => "CLUSCTL_NETINTERFACE_VALIDATE_COMMON_PROPERTIES", 
			100663385u => "CLUSCTL_NETINTERFACE_GET_COMMON_PROPERTIES", 
			100663421u => "CLUSCTL_NETINTERFACE_GET_RO_PRIVATE_PROPERTIES", 
			104857734u => "CLUSCTL_NETINTERFACE_SET_PRIVATE_PROPERTIES", 
			104857694u => "CLUSCTL_NETINTERFACE_SET_COMMON_PROPERTIES", 
			100663437u => "CLUSCTL_NETINTERFACE_GET_PRIVATE_PROPERTY_FMTS", 
			100663433u => "CLUSCTL_NETINTERFACE_VALIDATE_PRIVATE_PROPERTIES", 
			100663425u => "CLUSCTL_NETINTERFACE_GET_PRIVATE_PROPERTIES", 
			117440512u => "CLUSCTL_CLUSTER_UNKNOWN", 
			117440593u => "CLUSCTL_CLUSTER_ENUM_COMMON_PROPERTIES", 
			117440585u => "CLUSCTL_CLUSTER_CHECK_VOTER_DOWN", 
			117440581u => "CLUSCTL_CLUSTER_CHECK_VOTER_EVICT", 
			117440573u => "CLUSCTL_CLUSTER_GET_FQDN", 
			117440597u => "CLUSCTL_CLUSTER_GET_RO_COMMON_PROPERTIES", 
			117440637u => "CLUSCTL_CLUSTER_GET_RO_PRIVATE_PROPERTIES", 
			117440633u => "CLUSCTL_CLUSTER_ENUM_PRIVATE_PROPERTIES", 
			117440613u => "CLUSCTL_CLUSTER_GET_COMMON_PROPERTY_FMTS", 
			117440609u => "CLUSCTL_CLUSTER_VALIDATE_COMMON_PROPERTIES", 
			117440601u => "CLUSCTL_CLUSTER_GET_COMMON_PROPERTIES", 
			117440641u => "CLUSCTL_CLUSTER_GET_PRIVATE_PROPERTIES", 
			121634950u => "CLUSCTL_CLUSTER_SET_PRIVATE_PROPERTIES", 
			121634910u => "CLUSCTL_CLUSTER_SET_COMMON_PROPERTIES", 
			117440653u => "CLUSCTL_CLUSTER_GET_PRIVATE_PROPERTY_FMTS", 
			117440649u => "CLUSCTL_CLUSTER_VALIDATE_PRIVATE_PROPERTIES", 
			_ => "Unknown Control Code", 
		};
	}
}
