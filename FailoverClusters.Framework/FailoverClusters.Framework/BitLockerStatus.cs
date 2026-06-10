using System;

namespace FailoverClusters.Framework;

[Flags]
public enum BitLockerStatus : long
{
	None = 0L,
	On = 1L,
	NeedReboot = 2L,
	Decrypted = 4L,
	Encrypted = 8L,
	Decrypting = 0x10L,
	Encrypting = 0x20L,
	Paused = 0x40L,
	Stopped = 0x80L,
	HasRecoveryData = 0x100L,
	HasTpmData = 0x200L,
	Disabled = 0x400L,
	Locked = 0x800L,
	Secure = 0x1000L,
	Converting = 0x2000L,
	BootPartition = 0x4000L,
	OfflineError = 0x8000L,
	OfflineRaw = 0x10000L,
	HasExternalKey = 0x20000L,
	HasPassword = 0x40000L,
	HasPin = 0x80000L,
	HasStartupKey = 0x100000L,
	HasPassphrase = 0x200000L,
	RoamingDevice = 0x400000L,
	HasUserCertificateKey = 0x800000L,
	DataOnly = 0x1000000L,
	PreProvisioned = 0x2000000L,
	SupportsEdrv = 0x4000000L,
	UsesEdrv = 0x8000000L,
	ThinProvisioned = 0x10000000L,
	Clustered = 0x20000000L,
	Csv = 0x40000000L,
	CsvMetadata = 0x80000000L
}

