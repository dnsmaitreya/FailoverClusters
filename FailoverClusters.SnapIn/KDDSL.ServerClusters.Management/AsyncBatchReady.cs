using System.Collections.Generic;

namespace KDDSL.ServerClusters.Management;

internal delegate void AsyncBatchReady<T>(ICollection<T> items);
