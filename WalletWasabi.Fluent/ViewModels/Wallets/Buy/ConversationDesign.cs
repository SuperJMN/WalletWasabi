using System.Collections.Generic;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Buy;

public class ConversationDesign : IConversation
{
	public ConversationDesign()
	{
		Messages = new List<Message>(
			new[]
			{
				new Message("Hi, I'm your host.", SenderKind.Backend),
				new Message("I know, I know. Send me an angel. Right now. Right now.", SenderKind.User),
				new Message("Listening to good music of the 80s and hodling hard is well worth a Lambo.", SenderKind.Backend),
				new Message("Thanks. I'm getting two.", SenderKind.User)
			});
	}

	public string Title { get; init; }
	public IReadOnlyCollection<Message> Messages { get; init; }
}
