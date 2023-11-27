using System.Collections.ObjectModel;
using System.Linq;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Buy;

public interface IConversationViewModel
{
	Conversation? CurrentConversation { get; }
	ReadOnlyObservableCollection<Conversation> Conversations { get; }
}

internal class ConversationViewModelDesign : IConversationViewModel
{
	public ConversationViewModelDesign()
	{
		Conversations = new ReadOnlyObservableCollection<Conversation>(
			new ObservableCollection<Conversation>(
				new[]
				{
					new Conversation(Guid.NewGuid()) { Title = "Order 001" },
					new Conversation(Guid.NewGuid()) { Title = "Order 002" },
					new Conversation(Guid.NewGuid()) { Title = "Order 003" }
				}));

		CurrentConversation = Conversations.First();
	}

	public Conversation? CurrentConversation { get; set; }
	public ReadOnlyObservableCollection<Conversation> Conversations { get; set; }
}
