using System.CodeDom.Compiler;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public abstract class MiInstanceBase : MiBase
{
	public abstract void Refresh();

	public abstract void Save();

	public abstract void Delete();
}
