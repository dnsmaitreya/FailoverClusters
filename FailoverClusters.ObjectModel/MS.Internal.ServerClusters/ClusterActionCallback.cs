using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

[return: MarshalAs(UnmanagedType.U1)]
public delegate bool ClusterActionCallback(ClusterSetupPhase membershipPhase, ClusterSetupPhaseType phaseType, ClusterSetupPhaseSeverity phaseSeverity, int percentComplete, string objectName, int status);
