using System;

namespace KDDSL.FailoverClusters.Framework;

internal delegate void WeakEventHandler(object sender, EventArgs e);
internal delegate void WeakEventHandler<T>(object sender, T e) where T : EventArgs;
