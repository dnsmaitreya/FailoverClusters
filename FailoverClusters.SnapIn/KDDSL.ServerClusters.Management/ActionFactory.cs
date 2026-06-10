using System.Globalization;
using System.Linq;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal static class ActionFactory
{
	internal static ActionBase CreateDisabledAction(string name, string description, int imageIndex)
	{
		Action action = new Action();
		ConfigureAction(action, name, description, imageIndex);
		action.Enabled = false;
		return action;
	}

	internal static ActionBase CreateAction(string name, string description, int imageIndex, SnapinActionEventHandler actionHandler, object tag, bool isSync = true)
	{
		ActionBase actionBase = CreateSyncAction(name, description, imageIndex, isSync);
		ActionData tag2 = new ActionData(actionBase, actionHandler, tag);
		actionBase.Tag = tag2;
		return actionBase;
	}

	private static ActionBase CreateSyncAction(string name, string description, int imageIndex, bool isSync = true)
	{
		ActionBase actionBase = ((!isSync) ? ((ActionBase)new Action()) : ((ActionBase)new SyncAction()));
		ConfigureAction(actionBase, name, description, imageIndex);
		return actionBase;
	}

	private static void ConfigureAction(ActionsPaneExtendedItem action, string name, string description, int imageIndex)
	{
		string displayName = GenerateDisplayName(name);
		action.DisplayName = displayName;
		action.MnemonicDisplayName = name;
		action.Description = description;
		action.ImageIndex = imageIndex;
	}

	internal static string GenerateDisplayName(string name)
	{
		string text = name;
		while (true)
		{
			int num = text.IndexOf('&');
			if (num < 0)
			{
				break;
			}
			text = text.Remove(num, 1);
		}
		return text;
	}

	internal static ActionBase CreateAction(string name, string description, int imageIndex, SnapinActionEventHandler actionHandler)
	{
		return CreateAction(name, description, imageIndex, actionHandler, null);
	}

	internal static ActionGroup CreateActionGroup(string name, string description, int imageIndex)
	{
		ActionGroup actionGroup = new ActionGroup();
		ConfigureAction(actionGroup, name, description, imageIndex);
		return actionGroup;
	}

	internal static void AssignAscendingMnemonics(ActionGroup group)
	{
		int num = 1;
		foreach (ActionBase item in group.Items.OfType<ActionBase>())
		{
			AssignMnemonic(item, num++);
		}
	}

	private static void AssignMnemonic(ActionBase action, int mnemonicIndex)
	{
		string mnemonic = GetMnemonic(mnemonicIndex);
		string mnemonicDisplayName = string.Format(CultureInfo.CurrentCulture, "{0} - {1}", mnemonic, action.DisplayName);
		action.MnemonicDisplayName = mnemonicDisplayName;
	}

	private static string GetMnemonic(int mnemonicIndex)
	{
		return mnemonicIndex switch
		{
			1 => Resources.Mnemonic_1_Text, 
			2 => Resources.Mnemonic_2_Text, 
			3 => Resources.Mnemonic_3_Text, 
			4 => Resources.Mnemonic_4_Text, 
			5 => Resources.Mnemonic_5_Text, 
			6 => Resources.Mnemonic_6_Text, 
			7 => Resources.Mnemonic_7_Text, 
			8 => Resources.Mnemonic_8_Text, 
			9 => Resources.Mnemonic_9_Text, 
			10 => Resources.Mnemonic_10_Text, 
			11 => Resources.Mnemonic_11_Text, 
			12 => Resources.Mnemonic_12_Text, 
			13 => Resources.Mnemonic_13_Text, 
			14 => Resources.Mnemonic_14_Text, 
			15 => Resources.Mnemonic_15_Text, 
			16 => Resources.Mnemonic_16_Text, 
			17 => Resources.Mnemonic_17_Text, 
			18 => Resources.Mnemonic_18_Text, 
			19 => Resources.Mnemonic_19_Text, 
			20 => Resources.Mnemonic_20_Text, 
			21 => Resources.Mnemonic_21_Text, 
			22 => Resources.Mnemonic_22_Text, 
			23 => Resources.Mnemonic_23_Text, 
			24 => Resources.Mnemonic_24_Text, 
			25 => Resources.Mnemonic_25_Text, 
			26 => Resources.Mnemonic_26_Text, 
			27 => Resources.Mnemonic_27_Text, 
			28 => Resources.Mnemonic_28_Text, 
			29 => Resources.Mnemonic_29_Text, 
			30 => Resources.Mnemonic_30_Text, 
			31 => Resources.Mnemonic_31_Text, 
			32 => Resources.Mnemonic_32_Text, 
			33 => Resources.Mnemonic_33_Text, 
			34 => Resources.Mnemonic_34_Text, 
			35 => Resources.Mnemonic_35_Text, 
			_ => string.Empty, 
		};
	}
}

