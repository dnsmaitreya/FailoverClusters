namespace MS.Internal.ServerClusters.Management;

internal delegate O BackgroundWaitDialogOperation<I, O>(CluadminWaitDialog waitDialog, I data);
