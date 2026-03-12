using System;
using System.Runtime.CompilerServices;

namespace Microsoft.FailoverClusters.NativeHelp;

public class NativeGroupHelp
{
	[Flags]
	public enum GroupMoveFlags
	{
		IgnoreResourceStatus = 1,
		ReturnToSourceNodeOnError = 2,
		Queued = 4,
		HighPriority = 8,
		IgnoreAffinityRule = 0x20
	}

	[Flags]
	public enum SubStatus
	{
		None = 0,
		Locked = 1,
		Preempted = 2,
		WaitingInQueueForMove = 4,
		PhysicalResourcesLacking = 8,
		WaitingToStart = 0x10,
		EmbeddedFailure = 0x20,
		AffinityConflict = 0x40,
		WaitingForDependencies = 0x1000
	}

	public enum VmMigrationType
	{
		TurnOff,
		Quick,
		Shutdown,
		ShutdownForce,
		Live
	}

	public static readonly GroupMoveFlags LiveMigrationFlags = GroupMoveFlags.ReturnToSourceNodeOnError | GroupMoveFlags.Queued | GroupMoveFlags.HighPriority;

	public unsafe static PropertyListWrapper GetMigrationTypePropertyList(VmMigrationType type)
	{
		PropertyListWrapper propertyListWrapper = new PropertyListWrapper();
		CClusPropList* propertyList = propertyListWrapper.PropertyList;
		switch (type)
		{
		default:
			throw new ArgumentOutOfRangeException("type");
		case VmMigrationType.Live:
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CA_0040FDMDBMNJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 4u, 0u);
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1DM_0040PDKDCODJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 4u, 0u);
			break;
		case VmMigrationType.ShutdownForce:
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CA_0040FDMDBMNJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 3u, 0u);
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1DM_0040PDKDCODJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 3u, 0u);
			break;
		case VmMigrationType.Shutdown:
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CA_0040FDMDBMNJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 2u, 0u);
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1DM_0040PDKDCODJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 2u, 0u);
			break;
		case VmMigrationType.Quick:
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CA_0040FDMDBMNJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 1u, 0u);
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1DM_0040PDKDCODJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 1u, 0u);
			break;
		case VmMigrationType.TurnOff:
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CA_0040FDMDBMNJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 0u, 0u);
			global::_003CModule_003E.CClusPropList_002EScAddProp(propertyList, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1DM_0040PDKDCODJ_0040_003F_0024AAV_003F_0024AAi_003F_0024AAr_003F_0024AAt_003F_0024AAu_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAM_003F_0024AAa_003F_0024AAc_003F_0024AAh_003F_0024AAi_003F_0024AAn_003F_0024AAe_0040), 0u, 0u);
			break;
		}
		return propertyListWrapper;
	}
}
