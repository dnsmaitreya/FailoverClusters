using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using _003CCppImplementationDetails_003E;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

internal static class NativeMethods
{
	private static object lockObject;

	private unsafe static HINSTANCE__* clusApiModule;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<ushort*, _HCLUSTER*> pOpenCluster;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<ushort*, uint, uint*, _HCLUSTER*> pOpenClusterEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNODE*> pOpenClusterNode;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNODE*> pOpenClusterNodeEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*> pOpenClusterGroup;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HGROUP*> pOpenClusterGroupEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETWORK*> pOpenClusterNetwork;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETWORK*> pOpenClusterNetworkEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETINTERFACE*> pOpenClusterNetworkInterface;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETINTERFACE*> pOpenClusterNetworkInterfaceEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HRESOURCE*> pOpenClusterResource;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HRESOURCE*> pOpenClusterResourceEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint> pAddResourceToClusterSharedVolumes;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint> pRemoveResourceFromClusterSharedVolumes;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, _HNODEENUM*> pClusterNodeOpenEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, void*, _HNODEENUMEX*> pClusterNodeOpenEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint> pClusterNodeGetEnumCount;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint> pClusterNodeGetEnumCountEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint, uint*, ushort*, uint*, uint> pClusterNodeEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint, _CLUSTER_ENUM_ITEM*, uint*, uint> pClusterNodeEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint> pClusterNodeCloseEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint> pClusterNodeCloseEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, _HCLUSENUM*> pClusterOpenEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, void*, _HCLUSENUMEX*> pClusterOpenEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint> pClusterGetEnumCount;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint> pClusterGetEnumCountEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint, uint*, ushort*, uint*, uint> pClusterEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint, _CLUSTER_ENUM_ITEM*, uint*, uint> pClusterEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint> pClusterCloseEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint> pClusterCloseEnumEx;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, _HRESTYPEENUM*> pClusterResourceTypeOpenEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint> pClusterResourceTypeGetEnumCount;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint, uint*, ushort*, uint*, uint> pClusterResourceTypeEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint> pClusterResourceTypeCloseEnum;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*> pCreateClusterGroup;

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _CLUSTER_CREATE_GROUP_INFO*, _HGROUP*> pCreateClusterGroupEx;

	unsafe static NativeMethods()
	{
		//IL_0011: Expected I, but got I8
		//IL_0018: Expected I, but got I8
		//IL_001f: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_002d: Expected I, but got I8
		//IL_0034: Expected I, but got I8
		//IL_003b: Expected I, but got I8
		//IL_0042: Expected I, but got I8
		//IL_0049: Expected I, but got I8
		//IL_0050: Expected I, but got I8
		//IL_0057: Expected I, but got I8
		//IL_005e: Expected I, but got I8
		//IL_0065: Expected I, but got I8
		//IL_006c: Expected I, but got I8
		//IL_0073: Expected I, but got I8
		//IL_007a: Expected I, but got I8
		//IL_0081: Expected I, but got I8
		//IL_0088: Expected I, but got I8
		//IL_008f: Expected I, but got I8
		//IL_0096: Expected I, but got I8
		//IL_009d: Expected I, but got I8
		//IL_00a4: Expected I, but got I8
		//IL_00ab: Expected I, but got I8
		//IL_00b2: Expected I, but got I8
		//IL_00b9: Expected I, but got I8
		//IL_00c0: Expected I, but got I8
		//IL_00c7: Expected I, but got I8
		//IL_00ce: Expected I, but got I8
		//IL_00d5: Expected I, but got I8
		//IL_00dc: Expected I, but got I8
		//IL_00e3: Expected I, but got I8
		//IL_00ea: Expected I, but got I8
		//IL_00f1: Expected I, but got I8
		//IL_00f8: Expected I, but got I8
		//IL_00ff: Expected I, but got I8
		//IL_0106: Expected I, but got I8
		//IL_010d: Expected I, but got I8
		lockObject = new object();
		clusApiModule = null;
		pOpenCluster = null;
		pOpenClusterEx = null;
		pOpenClusterNode = null;
		pOpenClusterNodeEx = null;
		pOpenClusterGroup = null;
		pOpenClusterGroupEx = null;
		pOpenClusterNetwork = null;
		pOpenClusterNetworkEx = null;
		pOpenClusterNetworkInterface = null;
		pOpenClusterNetworkInterfaceEx = null;
		pOpenClusterResource = null;
		pOpenClusterResourceEx = null;
		pAddResourceToClusterSharedVolumes = null;
		pRemoveResourceFromClusterSharedVolumes = null;
		pClusterNodeOpenEnum = null;
		pClusterNodeOpenEnumEx = null;
		pClusterNodeGetEnumCount = null;
		pClusterNodeGetEnumCountEx = null;
		pClusterNodeEnum = null;
		pClusterNodeEnumEx = null;
		pClusterNodeCloseEnum = null;
		pClusterNodeCloseEnumEx = null;
		pClusterOpenEnum = null;
		pClusterOpenEnumEx = null;
		pClusterGetEnumCount = null;
		pClusterGetEnumCountEx = null;
		pClusterEnum = null;
		pClusterEnumEx = null;
		pClusterCloseEnum = null;
		pClusterCloseEnumEx = null;
		pClusterResourceTypeOpenEnum = null;
		pClusterResourceTypeGetEnumCount = null;
		pClusterResourceTypeEnum = null;
		pClusterResourceTypeCloseEnum = null;
		pCreateClusterGroup = null;
		pCreateClusterGroupEx = null;
		clusApiModule = global::_003CModule_003E.LoadLibraryW((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BI_0040EDENJLGE_0040_003F_0024AAC_003F_0024AAl_003F_0024AAu_003F_0024AAs_003F_0024AAA_003F_0024AAP_003F_0024AAI_003F_0024AA_003F4_003F_0024AAd_003F_0024AAl_003F_0024AAl_0040));
		if (clusApiModule == null)
		{
			int num = (((int)global::_003CModule_003E.GetLastError() > 0) ? ((int)(global::_003CModule_003E.GetLastError() & 0xFFFF) | -2147024896) : ((int)global::_003CModule_003E.GetLastError()));
			Win32Exception inner = new Win32Exception(num, Utilities.FormatError(num));
			object[] args = new object[0];
			Exception ex = new DllNotFoundException(string.Format(Thread.CurrentThread.CurrentCulture, Resources.NativeMessage_DllNotFound, args), inner);
			ClusterLog.AdminEvents.WriteClusterApiLoadFailedEvent();
			DebugLog.LogException(LogLevel.Critical, ex, "ClusAPI.dll could not be found in the system.");
			throw ex;
		}
		pOpenCluster = (delegate* unmanaged[Cdecl, Cdecl]<ushort*, _HCLUSTER*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040CCEBBGGG_0040OpenCluster_0040));
		pOpenClusterEx = (delegate* unmanaged[Cdecl, Cdecl]<ushort*, uint, uint*, _HCLUSTER*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0O_0040FGMEOHOE_0040OpenClusterEx_0040));
		pOpenClusterNode = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNODE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BA_0040OJDGLFNO_0040OpenClusterNode_0040));
		pOpenClusterNodeEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNODE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BC_0040DFAHPKHB_0040OpenClusterNodeEx_0040));
		pOpenClusterGroup = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BB_0040DKOFJGBK_0040OpenClusterGroup_0040));
		pOpenClusterGroupEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HGROUP*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BD_0040DIPGEKJK_0040OpenClusterGroupEx_0040));
		pOpenClusterNetwork = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETWORK*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BD_0040NOKAKHIH_0040OpenClusterNetwork_0040));
		pOpenClusterNetworkEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETWORK*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BF_0040KNMHFKLC_0040OpenClusterNetworkEx_0040));
		pOpenClusterNetworkInterface = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETINTERFACE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BI_0040GDLBHIAH_0040OpenClusterNetInterface_0040));
		pOpenClusterNetworkInterfaceEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETINTERFACE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BK_0040IACILAKN_0040OpenClusterNetInterfaceEx_0040));
		pOpenClusterResource = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HRESOURCE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BE_0040CPBNPIEA_0040OpenClusterResource_0040));
		pOpenClusterResourceEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HRESOURCE*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BG_0040NCKIKGOD_0040OpenClusterResourceEx_0040));
		pAddResourceToClusterSharedVolumes = (delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0CC_0040IFMICADH_0040AddResourceToClusterSharedVolum_0040));
		pRemoveResourceFromClusterSharedVolumes = (delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0CH_0040CPCGCDPK_0040RemoveResourceFromClusterShared_0040));
		pClusterNodeOpenEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, _HNODEENUM*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BE_0040MPILOMCJ_0040ClusterNodeOpenEnum_0040));
		pClusterNodeOpenEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, void*, _HNODEENUMEX*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BG_0040HMENECOG_0040ClusterNodeOpenEnumEx_0040));
		pClusterNodeGetEnumCount = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BI_0040LOGIJAHK_0040ClusterNodeGetEnumCount_0040));
		pClusterNodeGetEnumCountEx = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BK_0040LEGIAAIF_0040ClusterNodeGetEnumCountEx_0040));
		pClusterNodeEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint, uint*, ushort*, uint*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BA_0040HPGAHGFG_0040ClusterNodeEnum_0040));
		pClusterNodeEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint, _CLUSTER_ENUM_ITEM*, uint*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BC_0040MEDAONGO_0040ClusterNodeEnumEx_0040));
		pClusterNodeCloseEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BF_0040HBLPIPDC_0040ClusterNodeCloseEnum_0040));
		pClusterNodeCloseEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HNODEENUMEX*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BH_0040BMAAHKK_0040ClusterNodeCloseEnumEx_0040));
		pClusterOpenEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, _HCLUSENUM*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BA_0040BDALKOF_0040ClusterOpenEnum_0040));
		pClusterOpenEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, void*, _HCLUSENUMEX*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BC_0040JJAKOANO_0040ClusterOpenEnumEx_0040));
		pClusterGetEnumCount = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BE_0040JBCPALAC_0040ClusterGetEnumCount_0040));
		pClusterGetEnumCountEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BG_0040DEFDJGPA_0040ClusterGetEnumCountEx_0040));
		pClusterEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint, uint*, ushort*, uint*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040DOKKFBBJ_0040ClusterEnum_0040));
		pClusterEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint, _CLUSTER_ENUM_ITEM*, uint*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0O_0040BGNLEKAF_0040ClusterEnumEx_0040));
		pClusterCloseEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BB_0040ODKDLKPP_0040ClusterCloseEnum_0040));
		pClusterCloseEnumEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSENUMEX*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BD_0040CJCHPIJG_0040ClusterCloseEnumEx_0040));
		pClusterResourceTypeOpenEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, _HRESTYPEENUM*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BM_0040EAKNHFJA_0040ClusterResourceTypeOpenEnum_0040));
		pClusterResourceTypeGetEnumCount = (delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0CA_0040JCAJAPIA_0040ClusterResourceTypeGetEnumCount_0040));
		pClusterResourceTypeEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint, uint*, ushort*, uint*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BI_0040KOKFFJKP_0040ClusterResourceTypeEnum_0040));
		pClusterResourceTypeCloseEnum = (delegate* unmanaged[Cdecl, Cdecl]<_HRESTYPEENUM*, uint>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BN_0040MDINKCID_0040ClusterResourceTypeCloseEnum_0040));
		pCreateClusterGroup = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BD_0040ONIBAGGE_0040CreateClusterGroup_0040));
		pCreateClusterGroupEx = (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _CLUSTER_CREATE_GROUP_INFO*, _HGROUP*>)global::_003CModule_003E.GetProcAddress(clusApiModule, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BF_0040HJEFFKMC_0040CreateClusterGroupEx_0040));
	}

	private static void FalseGetLastError()
	{
		uint lastError = global::_003CModule_003E.GetLastError();
	}

	private static Exception ClusApiEntryPointNotFoundException(string functionName)
	{
		Win32Exception inner = new Win32Exception(-2147024776, Utilities.FormatError(-2147024776));
		Exception ex = new EntryPointNotFoundException(Resources.NativeMessage_EntryPointNotFound, inner);
		string debugMessage = string.Format(Thread.CurrentThread.CurrentCulture, "Function '{0}' not found in 'ClusAPI'., try reinstalling Failover Cluster Feature", functionName);
		ClusterLog.AdminEvents.WriteClusterApiEntryPointEvent(functionName);
		DebugLog.LogException(LogLevel.Error, ex, debugMessage);
		return ex;
	}

	private static Exception ClusApiNotSupportedException(string functionName)
	{
		Exception ex = ExceptionHelp.Build<NotSupportedException>(12, new string[1] { Resources.NativeMessage_NotSupportedException });
		ClusterLog.AdminEvents.WriteClusterApiReadOnlyAccessEvent();
		DebugLog.LogException(LogLevel.Warning, ex, string.Format(Thread.CurrentThread.CurrentCulture, "The client does not support calling '{0}' with ReadOnly access.", functionName));
		return ex;
	}

	internal unsafe static SafeClusterHandle OpenCluster(string clusterName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		//IL_0012: Expected I, but got I8
		SafeClusterHandle safeClusterHandle = null;
		ushort* ptr = ((!clusterName.Equals(".")) ? InteropHelp.StringToWstr(clusterName) : null);
		try
		{
			int num = 0;
			if (pOpenClusterEx != (delegate* unmanaged[Cdecl, Cdecl]<ushort*, uint, uint*, _HCLUSTER*>)null)
			{
				uint num2 = 0u;
				IntPtr intPtr = new IntPtr(pOpenClusterEx(ptr, (uint)desiredAccess, &num2));
				IntPtr intPtr2 = intPtr;
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				safeClusterHandle = new SafeClusterHandle(intPtr);
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenCluster != (delegate* unmanaged[Cdecl, Cdecl]<ushort*, _HCLUSTER*>)null && (num == 1745 || pOpenClusterEx == (delegate* unmanaged[Cdecl, Cdecl]<ushort*, uint, uint*, _HCLUSTER*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenCluster");
				}
				IntPtr clusterHandle = new IntPtr(pOpenCluster(ptr));
				safeClusterHandle = new SafeClusterHandle(clusterHandle);
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeClusterHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterEx or OpenCluster");
			}
			if (safeClusterHandle.IsInvalid)
			{
				int num3 = ((num > 0) ? ((num & 0xFFFF) | -2147024896) : num);
				int num4 = num3;
				AnalyzeAndThrow(num3, clusterName);
				throw ExceptionHelp.Build<ApplicationException>(num3, new string[2]
				{
					Resources.OpenClusterFail_Text,
					clusterName
				});
			}
			if (desiredAccess == ClusterAccessRights.GenericAll && grantedAccess != ClusterAccessRights.GenericAll)
			{
				throw ExceptionHelp.Build<ClusterReadOnlyAccessException>(-2147024891, new string[1] { clusterName });
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return safeClusterHandle;
	}

	internal static SafeClusterHandle OpenCluster(string clusterName)
	{
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenCluster(clusterName, ClusterAccessRights.GenericAll, ref grantedAccess);
	}

	internal unsafe static SafeNodeHandle OpenClusterNode(Cluster cluster, string nodeName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ushort* ptr = InteropHelp.StringToWstr(nodeName);
		SafeNodeHandle safeNodeHandle = null;
		try
		{
			int num = 0;
			if (pOpenClusterNodeEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNODE*>)null)
			{
				uint num2 = 0u;
				safeNodeHandle = new SafeNodeHandle(pOpenClusterNodeEx(cluster.Handle, ptr, (uint)desiredAccess, &num2));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenClusterNode != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNODE*>)null && (num == 1745 || pOpenClusterNodeEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNODE*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenClusterNode");
				}
				safeNodeHandle = new SafeNodeHandle(pOpenClusterNode(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeNodeHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterNodeEx or OpenClusterNode");
			}
			if (safeNodeHandle.IsInvalid)
			{
				ClusApiExceptionFactory.CreateAndThrow(cluster, num, Resources.OpenNodeFail_Text, nodeName);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return safeNodeHandle;
	}

	internal static SafeNodeHandle OpenClusterNode(Cluster cluster, string nodeName)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenClusterNode(cluster, nodeName, cluster.ApiAccessLevel, ref grantedAccess);
	}

	internal unsafe static SafeGroupHandle OpenClusterGroup(Cluster cluster, string groupName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ushort* ptr = InteropHelp.StringToWstr(groupName);
		SafeGroupHandle safeGroupHandle = null;
		try
		{
			int num = 0;
			if (pOpenClusterGroupEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HGROUP*>)null)
			{
				uint num2 = 0u;
				safeGroupHandle = new SafeGroupHandle(pOpenClusterGroupEx(cluster.Handle, ptr, (uint)desiredAccess, &num2));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenClusterGroup != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*>)null && (num == 1745 || pOpenClusterGroupEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HGROUP*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenClusterGroup");
				}
				safeGroupHandle = new SafeGroupHandle(pOpenClusterGroup(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeGroupHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterGroupEx or OpenClusterGroup");
			}
			if (safeGroupHandle.IsInvalid)
			{
				int num3 = ((num > 0) ? ((num & 0xFFFF) | -2147024896) : num);
				int num4 = num3;
				if (num3 != -2147024826)
				{
					throw ExceptionHelp.Build(num, Resources.OpenGroupFail_Text, groupName);
				}
				throw ExceptionHelp.Build<ClusterSharingPausedException>(-2147024826, new string[1] { groupName });
			}
			return safeGroupHandle;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	internal static SafeGroupHandle OpenClusterGroup(Cluster cluster, string groupName)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenClusterGroup(cluster, groupName, cluster.ApiAccessLevel, ref grantedAccess);
	}

	internal unsafe static SafeNetworkHandle OpenClusterNetwork(Cluster cluster, string networkName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ushort* ptr = InteropHelp.StringToWstr(networkName);
		SafeNetworkHandle safeNetworkHandle = null;
		try
		{
			int num = 0;
			if (pOpenClusterNetworkEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETWORK*>)null)
			{
				uint num2 = 0u;
				safeNetworkHandle = new SafeNetworkHandle(pOpenClusterNetworkEx(cluster.Handle, ptr, (uint)desiredAccess, &num2));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenClusterNetwork != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETWORK*>)null && (num == 1745 || pOpenClusterNetworkEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETWORK*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenClusterNetwork");
				}
				safeNetworkHandle = new SafeNetworkHandle(pOpenClusterNetwork(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeNetworkHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterNetworkEx or OpenClusterNetwork");
			}
			if (safeNetworkHandle.IsInvalid)
			{
				ClusApiExceptionFactory.CreateAndThrow(cluster, num, Resources.OpenNetworkFail_Text, networkName);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return safeNetworkHandle;
	}

	internal static SafeNetworkHandle OpenClusterNetwork(Cluster cluster, string networkName)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenClusterNetwork(cluster, networkName, cluster.ApiAccessLevel, ref grantedAccess);
	}

	internal unsafe static SafeNetworkInterfaceHandle OpenClusterNetworkInterface(Cluster cluster, string networkInterfaceName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ushort* ptr = InteropHelp.StringToWstr(networkInterfaceName);
		SafeNetworkInterfaceHandle safeNetworkInterfaceHandle = null;
		try
		{
			int num = 0;
			if (pOpenClusterNetworkInterfaceEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETINTERFACE*>)null)
			{
				uint num2 = 0u;
				safeNetworkInterfaceHandle = new SafeNetworkInterfaceHandle(pOpenClusterNetworkInterfaceEx(cluster.Handle, ptr, (uint)desiredAccess, &num2));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenClusterNetworkInterface != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HNETINTERFACE*>)null && (num == 1745 || pOpenClusterNetworkInterfaceEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HNETINTERFACE*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenClusterNetworkInterface");
				}
				safeNetworkInterfaceHandle = new SafeNetworkInterfaceHandle(pOpenClusterNetworkInterface(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeNetworkInterfaceHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterNetworkInterfaceEx or OpenClusterNetworkInterface");
			}
			if (safeNetworkInterfaceHandle.IsInvalid)
			{
				ClusApiExceptionFactory.CreateAndThrow(cluster, num, Resources.OpenNetworkInterfaceFail_Text, networkInterfaceName);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return safeNetworkInterfaceHandle;
	}

	internal static SafeNetworkInterfaceHandle OpenClusterNetworkInterface(Cluster cluster, string networkInterfaceName)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenClusterNetworkInterface(cluster, networkInterfaceName, cluster.ApiAccessLevel, ref grantedAccess);
	}

	internal unsafe static SafeResourceHandle OpenClusterResource(Cluster cluster, string resourceName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ushort* ptr = InteropHelp.StringToWstr(resourceName);
		SafeResourceHandle safeResourceHandle = null;
		try
		{
			int num = 0;
			if (pOpenClusterResourceEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HRESOURCE*>)null)
			{
				uint num2 = 0u;
				safeResourceHandle = new SafeResourceHandle(pOpenClusterResourceEx(cluster.Handle, ptr, (uint)desiredAccess, &num2));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = (ClusterAccessRights)num2;
			}
			if (pOpenClusterResource != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HRESOURCE*>)null && (num == 1745 || pOpenClusterResourceEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, uint*, _HRESOURCE*>)null))
			{
				if (desiredAccess == ClusterAccessRights.GenericRead)
				{
					throw ClusApiNotSupportedException("OpenClusterResource");
				}
				safeResourceHandle = new SafeResourceHandle(pOpenClusterResource(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				grantedAccess = ClusterAccessRights.GenericAll;
			}
			if (safeResourceHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("OpenClusterResourceEx or OpenClusterResource");
			}
			if (safeResourceHandle.IsInvalid)
			{
				throw ExceptionHelp.Build(num, Resources.OpenResourceFail_Text, resourceName);
			}
			return safeResourceHandle;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	internal static SafeResourceHandle OpenClusterResource(Cluster cluster, string resourceName)
	{
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		ClusterAccessRights grantedAccess = ClusterAccessRights.None;
		return OpenClusterResource(cluster, resourceName, cluster.ApiAccessLevel, ref grantedAccess);
	}

	internal unsafe static void AddResourceToClusterSharedVolumes(Cluster cluster, ClusterResource resource)
	{
		if (pAddResourceToClusterSharedVolumes != (delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint>)null)
		{
			uint num = pAddResourceToClusterSharedVolumes(resource.Handle);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num, Resources.AddStorageToClusterSharedVolumesFailedFormat_Text, resource.Name);
			}
			return;
		}
		throw ClusApiEntryPointNotFoundException("AddResourceToClusterSharedVolumes");
	}

	internal unsafe static void RemoveResourceFromClusterSharedVolumes(Cluster cluster, ClusterResource resource)
	{
		if (pRemoveResourceFromClusterSharedVolumes != (delegate* unmanaged[Cdecl, Cdecl]<_HRESOURCE*, uint>)null)
		{
			uint num = pRemoveResourceFromClusterSharedVolumes(resource.Handle);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num, Resources.RemoveStorageFromClusterSharedVolumesFailedFormat_Text, resource.Name);
			}
			return;
		}
		throw ClusApiEntryPointNotFoundException("RemoveResourceFromClusterSharedVolumes");
	}

	internal unsafe static string GetNodeConnectedTo(Cluster cluster)
	{
		int num = 260;
		ushort* ptr = (ushort*)global::_003CModule_003E.new_005B_005D(520uL);
		int num2 = (int)global::_003CModule_003E.ClRtlGetClusterConnectionInformation(cluster.Handle, (_CLUSTER_CONNECTION_INFORMATION_CLASS)1, ptr, (uint*)(&num));
		if (num2 != 0)
		{
			object[] args = new object[1] { num2 };
			object[] args2 = new object[0];
			DebugLog.LogWarning(string.Format(CultureInfo.CurrentCulture, "Error '{0}' when trying to get node name where the client is connected to", args2), args);
		}
		return InteropHelp.WstrToString(ptr);
	}

	internal unsafe static SafeClusterEnumHandle ClusterOpenEnum(Cluster cluster, ClusterEnumType enumType, SafeClusterEnumHandleOptions options)
	{
		//Discarded unreachable code: IL_00cf
		//IL_0033: Expected I, but got I8
		if (cluster == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "cluster" });
		}
		if (pClusterOpenEnumEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, void*, _HCLUSENUMEX*>)null)
		{
			void* ptr = pClusterOpenEnumEx(cluster.Handle, (uint)enumType, null);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (ptr != null || global::_003CModule_003E.GetLastError() != 1745)
			{
				return new SafeClusterEnumHandle(ptr, cluster, enumType, options, SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUMEX);
			}
		}
		if (pClusterOpenEnum != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, _HCLUSENUM*>)null && (global::_003CModule_003E.GetLastError() == 1745 || pClusterOpenEnumEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, uint, void*, _HCLUSENUMEX*>)null))
		{
			if (enumType != ClusterEnumType.ClusterFileSystem)
			{
				_HCLUSENUM* enumHandle = pClusterOpenEnum(cluster.Handle, (uint)enumType);
				uint lastError2 = global::_003CModule_003E.GetLastError();
				return new SafeClusterEnumHandle(enumHandle, cluster, enumType, options, SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUM);
			}
			_HCLUSENUM* enumHandle2 = pClusterOpenEnum(cluster.Handle, 4u);
			uint lastError3 = global::_003CModule_003E.GetLastError();
			return new SafeClusterEnumHandle(enumHandle2, cluster, ClusterEnumType.ClusterFileSystem, options, SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUM);
		}
		throw ClusApiEntryPointNotFoundException("ClusterOpenEnumEx or ClusterOpenEnum");
	}

	internal unsafe static int ClusterGetEnumCount(SafeClusterEnumHandle clusterEnumHandle)
	{
		int result = 0;
		if (clusterEnumHandle.handleType == SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUMEX)
		{
			result = (int)pClusterGetEnumCountEx((_HCLUSENUMEX*)clusterEnumHandle.DangerousGetEnumHandle());
		}
		if (clusterEnumHandle.handleType == SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUM)
		{
			result = (int)pClusterGetEnumCount((_HCLUSENUM*)clusterEnumHandle.DangerousGetEnumHandle());
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		return result;
	}

	internal unsafe static int ClusterEnum(SafeClusterEnumHandle clusterEnumHandle, int index, _CLUSTER_ENUM_ITEM* pItem, ref int bytesNeeded)
	{
		//IL_00a1: Expected I8, but got I
		//IL_00b5: Expected I, but got I8
		//IL_00b8: Expected I, but got I8
		//IL_010d: Expected I, but got I8
		//IL_010d: Expected I, but got I8
		int num = 0;
		switch (clusterEnumHandle.handleType)
		{
		case SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUMEX:
		{
			uint num8 = (uint)bytesNeeded;
			num = (int)pClusterEnumEx((_HCLUSENUMEX*)clusterEnumHandle.DangerousGetEnumHandle(), (uint)index, pItem, &num8);
			bytesNeeded = (int)num8;
			uint lastError2 = global::_003CModule_003E.GetLastError();
			return num;
		}
		case SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUM:
		{
			uint num2 = *(uint*)((ulong)(nint)pItem + 24uL);
			uint num3 = num2 >> 1;
			uint num4 = 0u;
			ushort* ptr = (ushort*)global::_003CModule_003E.malloc(num2);
			num = (int)pClusterEnum((_HCLUSENUM*)clusterEnumHandle.DangerousGetEnumHandle(), (uint)index, &num4, ptr, &num3);
			uint lastError = global::_003CModule_003E.GetLastError();
			*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num3 + 1) * 2L);
			if (num != 0)
			{
				return num;
			}
			*(uint*)((ulong)(nint)pItem + 4uL) = num4;
			*(long*)((ulong)(nint)pItem + 32uL) = (nint)ptr;
			if (clusterEnumHandle.enumType != ClusterEnumType.ClusterFileSystem)
			{
				break;
			}
			ushort* ptr2 = null;
			ushort* ptr3 = null;
			_HRESOURCE* ptr4 = global::_003CModule_003E.OpenClusterResource(clusterEnumHandle.cluster.Handle, ptr);
			if (ptr4 == null)
			{
				num = (int)global::_003CModule_003E.GetLastError();
				if (num != 5007 && num != 5006 && num != 303)
				{
					return num;
				}
				*(int*)pItem = 0;
				return 0;
			}
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0PP_0040G _0024ArrayType_0024_0024_0024BY0PP_0040G);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num5);
				num = (int)global::_003CModule_003E.ClusterResourceControl(ptr4, null, 16777261u, null, 0u, &_0024ArrayType_0024_0024_0024BY0PP_0040G, 255u, &num5);
				switch (num)
				{
				default:
					return num;
				case 303:
				case 5006:
				case 5007:
					*(int*)pItem = 0;
					return 0;
				case 0:
				{
					string b = InteropHelp.WstrToString((ushort*)(&_0024ArrayType_0024_0024_0024BY0PP_0040G));
					if (!string.Equals("Physical Disk", b, StringComparison.CurrentCultureIgnoreCase))
					{
						*(int*)pItem = 0;
						return 0;
					}
					CLUSTER_RESOURCE_STATE cLUSTER_RESOURCE_STATE = (CLUSTER_RESOURCE_STATE)(-1);
					uint num6 = 64u;
					uint num7 = 64u;
					ptr2 = InteropHelp.AllocateWCharArray(64u);
					ptr3 = InteropHelp.AllocateWCharArray(num7);
					CLUSTER_RESOURCE_STATE clusterResourceState = global::_003CModule_003E.GetClusterResourceState(ptr4, ptr2, &num6, ptr3, &num7);
					num = (int)global::_003CModule_003E.GetLastError();
					if (clusterResourceState == (CLUSTER_RESOURCE_STATE)(-1) && num == 234)
					{
						num6++;
						InteropHelp.ReallocateWCharArray(&ptr2, num6);
						num7++;
						InteropHelp.ReallocateWCharArray(&ptr3, num7);
						clusterResourceState = global::_003CModule_003E.GetClusterResourceState(ptr4, ptr2, &num6, ptr3, &num7);
						num = (int)global::_003CModule_003E.GetLastError();
					}
					switch (num)
					{
					default:
						return num;
					case 303:
					case 5006:
					case 5007:
						*(int*)pItem = 0;
						return 0;
					case 0:
					{
						string groupName = InteropHelp.WstrToString(ptr3);
						if (ClusterGroup.CreateObject(clusterEnumHandle.cluster, groupName).GroupType != GroupType.ClusterSharedVolume)
						{
							*(int*)pItem = 0;
							return 0;
						}
						break;
					}
					}
					break;
				}
				}
			}
			finally
			{
				InteropHelp.FreeWstr(ptr2);
				InteropHelp.FreeWstr(ptr3);
				if (global::_003CModule_003E.CloseClusterResource(ptr4) == 0)
				{
					DebugLog.LogError("There was an error closing the cluster resource while doing a resource type enumeration: '{0}'.", global::_003CModule_003E.GetLastError());
				}
			}
			break;
		}
		}
		*(int*)((ulong)(nint)pItem + 8uL) = 0;
		*(long*)((ulong)(nint)pItem + 16uL) = 0L;
		return num;
	}

	internal unsafe static int ClusterCloseEnum(SafeClusterEnumHandle clusterEnumHandle)
	{
		int num = 0;
		if (clusterEnumHandle.handleType == SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUMEX)
		{
			num = (int)pClusterCloseEnumEx((_HCLUSENUMEX*)clusterEnumHandle.DangerousGetEnumHandle());
			uint lastError = global::_003CModule_003E.GetLastError();
		}
		if (clusterEnumHandle.handleType == SafeClusterEnumHandle.SafeClusEnumHandleType.HCLUSENUM)
		{
			num = (int)pClusterCloseEnum((_HCLUSENUM*)clusterEnumHandle.DangerousGetEnumHandle());
			uint lastError2 = global::_003CModule_003E.GetLastError();
		}
		if (num == 1745)
		{
			throw ClusApiEntryPointNotFoundException("ClusterCloseEnumEx or ClusterCloseEnum");
		}
		return num;
	}

	internal unsafe static SafeNodeEnumHandle ClusterNodeOpenEnum(ClusterNode node, NodeEnumType enumType, SafeNodeEnumHandleOptions options)
	{
		//Discarded unreachable code: IL_00c0
		//IL_001a: Expected I, but got I8
		if (pClusterNodeOpenEnumEx != (delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, void*, _HNODEENUMEX*>)null)
		{
			_HNODEENUMEX* ptr = pClusterNodeOpenEnumEx(node.Handle, (uint)enumType, null);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (ptr != null || global::_003CModule_003E.GetLastError() != 1745)
			{
				return new SafeNodeEnumHandle(ptr, node, enumType, options, SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUMEX);
			}
		}
		if (pClusterNodeOpenEnum != (delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, _HNODEENUM*>)null && (global::_003CModule_003E.GetLastError() == 1745 || pClusterNodeOpenEnumEx == (delegate* unmanaged[Cdecl, Cdecl]<_HNODE*, uint, void*, _HNODEENUMEX*>)null))
		{
			if (enumType == NodeEnumType.NetworkInterface)
			{
				_HNODEENUM* handle = pClusterNodeOpenEnum(node.Handle, 1u);
				uint lastError2 = global::_003CModule_003E.GetLastError();
				return new SafeNodeEnumHandle(handle, node, NodeEnumType.NetworkInterface, options, SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUM);
			}
			uint num = 0u;
			num = (((enumType & NodeEnumType.Groups) != 0) ? 8u : num);
			_HCLUSENUM* handle2 = pClusterOpenEnum(node.Cluster.Handle, num);
			uint lastError3 = global::_003CModule_003E.GetLastError();
			return new SafeNodeEnumHandle(handle2, node, enumType, options, SafeNodeEnumHandle.SafeNodeEnumHandleType.HCLUSENUM);
		}
		throw ClusApiEntryPointNotFoundException("ClusterNodeOpenEnumEx or ClusterNodeOpenEnum");
	}

	internal unsafe static int ClusterNodeGetEnumCount(SafeNodeEnumHandle nodeEnumHandle)
	{
		int result = 0;
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUMEX)
		{
			result = (int)pClusterNodeGetEnumCountEx((_HNODEENUMEX*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUM)
		{
			result = (int)pClusterNodeGetEnumCount((_HNODEENUM*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HCLUSENUM)
		{
			result = (int)pClusterGetEnumCount((_HCLUSENUM*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		return result;
	}

	internal unsafe static int ClusterNodeEnum(SafeNodeEnumHandle nodeEnumHandle, int index, _CLUSTER_ENUM_ITEM* pItem, ref int bytesNeeded)
	{
		//IL_0097: Expected I8, but got I
		//IL_019e: Expected I8, but got I
		int num = 0;
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUMEX)
		{
			uint num2 = (uint)bytesNeeded;
			num = (int)pClusterNodeEnumEx((_HNODEENUMEX*)nodeEnumHandle.DangerousGetEnumHandle(), (uint)index, pItem, &num2);
			bytesNeeded = (int)num2;
			uint lastError = global::_003CModule_003E.GetLastError();
			return num;
		}
		uint num3 = *(uint*)((ulong)(nint)pItem + 24uL);
		uint num4 = num3 >> 1;
		uint num5 = 0u;
		ushort* ptr = (ushort*)global::_003CModule_003E.malloc(num3);
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUM)
		{
			num = (int)pClusterNodeEnum((_HNODEENUM*)nodeEnumHandle.DangerousGetEnumHandle(), (uint)index, &num5, ptr, &num4);
			uint lastError2 = global::_003CModule_003E.GetLastError();
			*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num4 + 1) * 2L);
			if (num != 0)
			{
				return num;
			}
			*(uint*)((ulong)(nint)pItem + 4uL) = num5;
			*(long*)((ulong)(nint)pItem + 32uL) = (nint)ptr;
		}
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HCLUSENUM)
		{
			num = (int)pClusterEnum((_HCLUSENUM*)nodeEnumHandle.DangerousGetEnumHandle(), (uint)index, &num5, ptr, &num4);
			uint lastError3 = global::_003CModule_003E.GetLastError();
			*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num4 + 1) * 2L);
			if (num == 0)
			{
				_HGROUP* ptr2 = global::_003CModule_003E.OpenClusterGroup(nodeEnumHandle.node.Cluster.Handle, ptr);
				if (ptr2 == null)
				{
					return (int)global::_003CModule_003E.GetLastError();
				}
				uint num6 = 260u;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BAE_0040G _0024ArrayType_0024_0024_0024BY0BAE_0040G);
				CLUSTER_GROUP_STATE clusterGroupState = global::_003CModule_003E.GetClusterGroupState(ptr2, (ushort*)(&_0024ArrayType_0024_0024_0024BY0BAE_0040G), &num6);
				num = (int)global::_003CModule_003E.GetLastError();
				if (num == 5013 || num == 5012 || num == 303)
				{
					*(int*)pItem = 0;
					return 0;
				}
				if (global::_003CModule_003E.CloseClusterGroup(ptr2) == 0)
				{
					DebugLog.LogError("There was an error closing the cluster group while doing a node enumeration: '{0}'.", global::_003CModule_003E.GetLastError());
				}
				if (clusterGroupState == (CLUSTER_GROUP_STATE)(-1))
				{
					return num;
				}
				string b = InteropHelp.WstrToString((ushort*)(&_0024ArrayType_0024_0024_0024BY0BAE_0040G));
				if (!string.Equals(nodeEnumHandle.node.Name, b, StringComparison.CurrentCultureIgnoreCase))
				{
					*(int*)pItem = 0;
					return num;
				}
				*(int*)((ulong)(nint)pItem + 4uL) = 0;
				*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num4 + 1) * 2L);
				*(long*)((ulong)(nint)pItem + 32uL) = (nint)ptr;
			}
		}
		*(int*)((ulong)(nint)pItem + 8uL) = 0;
		*(long*)((ulong)(nint)pItem + 16uL) = 0L;
		return num;
	}

	internal unsafe static int ClusterNodeCloseEnum(SafeNodeEnumHandle nodeEnumHandle)
	{
		int result = 0;
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUMEX)
		{
			result = (int)pClusterNodeCloseEnumEx((_HNODEENUMEX*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HNODEENUM)
		{
			result = (int)pClusterNodeCloseEnum((_HNODEENUM*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		if (nodeEnumHandle.handleType == SafeNodeEnumHandle.SafeNodeEnumHandleType.HCLUSENUM)
		{
			result = (int)pClusterCloseEnum((_HCLUSENUM*)nodeEnumHandle.DangerousGetEnumHandle());
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		return result;
	}

	internal unsafe static SafeResourceTypeEnumHandle ClusterResourceTypeOpenEnum(ClusterResourceType resourceType, ResourceTypeEnumType enumType, ResourceTypeEnumOptions options)
	{
		//Discarded unreachable code: IL_00af
		if (pClusterResourceTypeOpenEnum != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, uint, _HRESTYPEENUM*>)null)
		{
			if (enumType == ResourceTypeEnumType.Node || !(resourceType.Cluster.CurrentVersion == ClusterVersion.WindowsServer2008))
			{
				ushort* ptr = InteropHelp.StringToWstr(resourceType.Name);
				_HRESTYPEENUM* enumHandle = pClusterResourceTypeOpenEnum(resourceType.Cluster.Handle, ptr, (uint)enumType);
				uint lastError = global::_003CModule_003E.GetLastError();
				int lastError2 = (int)global::_003CModule_003E.GetLastError();
				InteropHelp.FreeWstr(ptr);
				if (enumType != ResourceTypeEnumType.Resources || lastError2 != 87)
				{
					return new SafeResourceTypeEnumHandle(enumHandle, resourceType, enumType, ResourceTypeEnumOptions.None, SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HRESTYPEENUM);
				}
			}
			uint num = 0u;
			num = (((enumType & ResourceTypeEnumType.Resources) != 0) ? 4u : num);
			_HCLUSENUM* enumHandle2 = pClusterOpenEnum(resourceType.Cluster.Handle, num);
			uint lastError3 = global::_003CModule_003E.GetLastError();
			return new SafeResourceTypeEnumHandle(enumHandle2, resourceType, enumType, ResourceTypeEnumOptions.None, SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HCLUSENUM);
		}
		throw ClusApiEntryPointNotFoundException("ClusterResourceTypeOpenEnum");
	}

	internal unsafe static int ClusterResourceTypeGetEnumCount(SafeResourceTypeEnumHandle resourceTypeEnumHandle)
	{
		int result = 0;
		if (resourceTypeEnumHandle.handleType == SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HRESTYPEENUM)
		{
			result = (int)pClusterResourceTypeGetEnumCount((_HRESTYPEENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle());
		}
		if (resourceTypeEnumHandle.handleType == SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HCLUSENUM)
		{
			result = (int)pClusterGetEnumCount((_HCLUSENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle());
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		return result;
	}

	internal unsafe static int ClusterResourceTypeEnum(SafeResourceTypeEnumHandle resourceTypeEnumHandle, int index, _CLUSTER_ENUM_ITEM* pItem, ref int bytesNeeded)
	{
		//IL_0065: Expected I8, but got I
		//IL_00fe: Expected I, but got I8
		//IL_00fe: Expected I, but got I8
		//IL_0180: Expected I8, but got I
		int num = 0;
		uint num2 = *(uint*)((ulong)(nint)pItem + 24uL);
		uint num3 = num2 >> 1;
		uint num4 = 0u;
		ushort* ptr = (ushort*)global::_003CModule_003E.malloc(num2);
		if (resourceTypeEnumHandle.handleType == SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HRESTYPEENUM)
		{
			num = (int)pClusterResourceTypeEnum((_HRESTYPEENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle(), (uint)index, &num4, ptr, &num3);
			uint lastError = global::_003CModule_003E.GetLastError();
			*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num3 + 1) * 2L);
			if (num != 0)
			{
				return num;
			}
			*(uint*)((ulong)(nint)pItem + 4uL) = num4;
			*(long*)((ulong)(nint)pItem + 32uL) = (nint)ptr;
		}
		if (resourceTypeEnumHandle.handleType == SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HCLUSENUM)
		{
			num = (int)pClusterEnum((_HCLUSENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle(), (uint)index, &num4, ptr, &num3);
			uint lastError2 = global::_003CModule_003E.GetLastError();
			*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num3 + 1) * 2L);
			if (num == 0)
			{
				_HRESOURCE* ptr2 = global::_003CModule_003E.OpenClusterResource(resourceTypeEnumHandle.resourceType.Cluster.Handle, ptr);
				if (ptr2 == null)
				{
					num = (int)global::_003CModule_003E.GetLastError();
					if (num != 5007 && num != 5006 && num != 303)
					{
						return num;
					}
					*(int*)pItem = 0;
					return 0;
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0PP_0040G _0024ArrayType_0024_0024_0024BY0PP_0040G);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num5);
				num = (int)global::_003CModule_003E.ClusterResourceControl(ptr2, null, 16777261u, null, 0u, &_0024ArrayType_0024_0024_0024BY0PP_0040G, 255u, &num5);
				if (global::_003CModule_003E.CloseClusterResource(ptr2) == 0)
				{
					DebugLog.LogError("There was an error closing the cluster resource while doing a resource type enumeration: '{0}'.", global::_003CModule_003E.GetLastError());
				}
				if (num != 0)
				{
					return num;
				}
				string b = InteropHelp.WstrToString((ushort*)(&_0024ArrayType_0024_0024_0024BY0PP_0040G));
				if (!string.Equals(resourceTypeEnumHandle.resourceType.Name, b, StringComparison.CurrentCultureIgnoreCase))
				{
					*(int*)pItem = 0;
					return 0;
				}
				*(int*)((ulong)(nint)pItem + 4uL) = 0;
				*(int*)((ulong)(nint)pItem + 4uL) = (((num4 & 4u) != 0) ? 2 : (*(int*)((ulong)(nint)pItem + 4uL)));
				*(int*)((ulong)(nint)pItem + 24uL) = (int)((long)(num3 + 1) * 2L);
				*(long*)((ulong)(nint)pItem + 32uL) = (nint)ptr;
			}
		}
		*(int*)((ulong)(nint)pItem + 8uL) = 0;
		*(long*)((ulong)(nint)pItem + 16uL) = 0L;
		return num;
	}

	internal unsafe static int ClusterResourceTypeCloseEnum(SafeResourceTypeEnumHandle resourceTypeEnumHandle)
	{
		int result = 0;
		switch (resourceTypeEnumHandle.handleType)
		{
		case SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HRESTYPEENUM:
			result = (int)pClusterResourceTypeCloseEnum((_HRESTYPEENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle());
			break;
		case SafeResourceTypeEnumHandle.SafeResourceTypeEnumHandleType.HCLUSENUM:
			result = (int)pClusterCloseEnum((_HCLUSENUM*)resourceTypeEnumHandle.DangerousGetEnumHandle());
			break;
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		return result;
	}

	internal unsafe static SafeGroupHandle CreateClusterGroup(Cluster cluster, string groupName, GroupType groupType)
	{
		SafeGroupHandle safeGroupHandle = null;
		if (groupName == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "groupName" });
		}
		ushort* ptr = InteropHelp.StringToWstr(groupName);
		try
		{
			int num = 0;
			if (pCreateClusterGroupEx != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _CLUSTER_CREATE_GROUP_INFO*, _HGROUP*>)null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSTER_CREATE_GROUP_INFO cLUSTER_CREATE_GROUP_INFO);
				*(int*)(&cLUSTER_CREATE_GROUP_INFO) = 1;
				System.Runtime.CompilerServices.Unsafe.As<_CLUSTER_CREATE_GROUP_INFO, GroupType>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTER_CREATE_GROUP_INFO, 4)) = groupType;
				safeGroupHandle = new SafeGroupHandle(pCreateClusterGroupEx(cluster.Handle, ptr, &cLUSTER_CREATE_GROUP_INFO));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
			}
			if (pCreateClusterGroup != (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _HGROUP*>)null && (num == 1745 || pCreateClusterGroupEx == (delegate* unmanaged[Cdecl, Cdecl]<_HCLUSTER*, ushort*, _CLUSTER_CREATE_GROUP_INFO*, _HGROUP*>)null))
			{
				safeGroupHandle = new SafeGroupHandle(pCreateClusterGroup(cluster.Handle, ptr));
				FalseGetLastError();
				num = (int)global::_003CModule_003E.GetLastError();
				if (num == 0)
				{
					ClusterGroup clusterGroup = ClusterGroup.CreateObject(cluster, groupName);
					clusterGroup.SetGroupType(groupType);
					clusterGroup.Close();
				}
			}
			if (safeGroupHandle == null)
			{
				throw ClusApiEntryPointNotFoundException("CreateClusterGroupEx or CreateClusterGroup");
			}
			if (safeGroupHandle.IsInvalid)
			{
				int num2 = ((num > 0) ? ((num & 0xFFFF) | -2147024896) : num);
				int num3 = num2;
				if (num2 != -2147024826)
				{
					throw ExceptionHelp.Build(num, Resources.OpenGroupFail_Text, groupName);
				}
				throw ExceptionHelp.Build<ClusterSharingPausedException>(-2147024826, new string[1] { groupName });
			}
			return safeGroupHandle;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	internal static ClusterBaseException AnalyzeAndReturn(int hResult, string clusterName)
	{
		return hResult switch
		{
			-2147023071 => ExceptionHelp.Build<ClusterStatusNotReadyException>(new string[1] { clusterName }), 
			-2147023143 => ExceptionHelp.Build<ClusterEndpointNotRegisteredException>(-2147023143, new string[1] { clusterName }), 
			-2147023169 => ExceptionHelp.Build<ClusterRpcConnectionException>(-2147023169, new string[1] { clusterName }), 
			-2147023170 => ExceptionHelp.Build<ClusterRpcConnectionException>(-2147023170, new string[1] { clusterName }), 
			-2147023174 => ExceptionHelp.Build<ClusterRpcConnectionException>(-2147023174, new string[1] { clusterName }), 
			-2147024826 => ExceptionHelp.Build<ClusterSharingPausedException>(-2147024826, new string[1] { clusterName }), 
			-2147024891 => ExceptionHelp.Build<ClusterAccessDeniedException>(-2147024891, new string[1] { clusterName }), 
			_ => null, 
		};
	}

	internal static void AnalyzeAndThrow(int hResult, string clusterName)
	{
		ClusterBaseException ex = AnalyzeAndReturn(hResult, clusterName);
		if (ex != null)
		{
			throw ex;
		}
	}
}

