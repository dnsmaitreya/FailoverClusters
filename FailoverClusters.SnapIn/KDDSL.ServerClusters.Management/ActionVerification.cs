using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal static class ActionVerification
{
	internal static VerifyActionCanBePerformedData BuildVerifyActionData(VerifyAction verifications, string quourumLossMessage, QuorumVoterActionCheck quorumActionCheck, string actionName, string actionConfirmationMessage)
	{
		return BuildVerifyActionData(verifications, quourumLossMessage, QuorumLossCheck.None, quorumActionCheck, actionName, actionConfirmationMessage);
	}

	internal static VerifyActionCanBePerformedData BuildVerifyActionData(VerifyAction verifications, string quourumLossMessage, QuorumLossCheck quorumCheck, string actionName, string actionConfirmationMessage)
	{
		return BuildVerifyActionData(verifications, quourumLossMessage, quorumCheck, QuorumVoterActionCheck.None, actionName, actionConfirmationMessage);
	}

	internal static VerifyActionCanBePerformedData BuildVerifyActionData(VerifyAction verifications, string quourumLossMessage, QuorumLossCheck quorumCheck, QuorumVoterActionCheck quorumActionCheck, string actionName, string actionConfirmationMessage)
	{
		VerifyActionCanBePerformedData verifyActionCanBePerformedData = default(VerifyActionCanBePerformedData);
		verifyActionCanBePerformedData.ActionData = new VerifyActionData
		{
			Name = actionName,
			ConfirmationMessage = actionConfirmationMessage
		};
		verifyActionCanBePerformedData.Verifications = verifications;
		VerifyActionCanBePerformedData result = verifyActionCanBePerformedData;
		if ((verifications & VerifyAction.QuorumLoss) == VerifyAction.QuorumLoss)
		{
			result.QuorumData.QuorumLossMessage = quourumLossMessage;
			result.QuorumData.QuorumCheck = quorumCheck;
			result.QuorumData.QuorumVoterAction = quorumActionCheck;
		}
		return result;
	}

	internal static bool ProcessVerifyActionResult(INotifyUser notifyUser, VerifyActionCanBePerformedData verifyActionData, bool canceled, VerifyAction verified, ConfirmationMessage confirmText)
	{
		bool result = false;
		if (!canceled)
		{
			if ((verified & VerifyAction.QuorumLoss) == VerifyAction.QuorumLoss)
			{
				notifyUser.ShowWarning(verifyActionData.QuorumData.QuorumLossMessage);
			}
			else if ((verified & VerifyAction.NetworkName) == VerifyAction.NetworkName || (verified & VerifyAction.HostedGroups) == VerifyAction.HostedGroups)
			{
				if (notifyUser.ShowWindowsCodePackDialog(verifyActionData.ActionData.Name, confirmText.MessageText, verifyActionData.ActionData.ConfirmationMessage) == DialogResult.Yes)
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
		}
		return result;
	}
}
