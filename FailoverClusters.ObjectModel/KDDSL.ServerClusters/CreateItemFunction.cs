using System;

namespace KDDSL.ServerClusters;

internal delegate T CreateItemFunction<T>(string name, Guid id);
