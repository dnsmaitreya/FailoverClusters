namespace MS.Internal.ServerClusters;

public enum S2DCacheState
{
	Disabled = 0,
	Enabled = 2,
	ReadOnly = 4,
	ReadWrite = 12,
	Provisioning = 14,
	ProvisioningFlash = 15,
	ProvisioningSpinningMedia = 20,
	Disabling = 120,
	Dormant = 200,
	IneligibleNoDisks = 1001,
	IneligibleNoFlash = 1002,
	IneligibleNotMixedMedia = 1003
}
