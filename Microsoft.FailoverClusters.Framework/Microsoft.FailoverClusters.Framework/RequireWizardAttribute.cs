using System;

namespace Microsoft.FailoverClusters.Framework;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public sealed class RequireWizardAttribute : Attribute
{
	public bool CreateRequireWizard { get; private set; }

	public RequireWizardAttribute(bool createRequireWizard)
	{
		CreateRequireWizard = createRequireWizard;
	}
}
