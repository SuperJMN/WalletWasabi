using System.Collections.Generic;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Buy;

public interface IConversation
{
	string Title { get; init; }
	IReadOnlyCollection<Message> Messages { get; }
}
