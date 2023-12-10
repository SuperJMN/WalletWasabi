using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using WalletWasabi.BuyAnything;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Buy.Workflows.ShopinBit;

public partial class ShopinBitWorkflowManager : WorkflowManager
{
	private readonly string _walletId;
	private readonly Country[] _countries;
	private readonly BehaviorSubject<bool> _idChangedSubject;

	[AutoNotify(SetterModifier = AccessModifier.Private)]
	private Workflow? _currentWorkflow;

	[AutoNotify(SetterModifier = AccessModifier.Private)]
	private ConversationId _id = ConversationId.Empty;

	public ShopinBitWorkflowManager(string walletId, Country[] countries)
	{
		_walletId = walletId;
		_countries = countries;
		_idChangedSubject = new BehaviorSubject<bool>(false);
		IdChangedObservable = _idChangedSubject.AsObservable();
	}

	public string WalletId => _walletId;

	public IObservable<bool> IdChangedObservable { get; }

	public void UpdateConversationId(ConversationId newId)
	{
		if (Id != ConversationId.Empty)
		{
			throw new InvalidOperationException("ID cannot be modified!");
		}

		Id = newId;
		_idChangedSubject.OnNext(true);
	}

	/// <summary>
	/// Selects next scripted workflow or use conversationStatus to override.
	/// </summary>
	/// <param name="conversationStatus">The remote conversationStatus override to select next workflow.</param>
	/// <param name="args"></param>
	/// <returns>True is next workflow selected successfully or current workflow will continue.</returns>
	public bool SelectNextShopinBitWorkflow(string? conversationStatus, object? args)
	{
		var states = args as WebClients.ShopWare.Models.State[];

		if (conversationStatus is not null)
		{
			if (_currentWorkflow?.CanCancel() ?? true)
			{
				CurrentWorkflow = GetShopinBitWorkflowFromConversation(conversationStatus, states);
				return true;
			}

			return false;
		}

		CurrentWorkflow = _currentWorkflow switch
		{
			null => new InitialWorkflow(WorkflowValidator, _countries),
			InitialWorkflow => new SupportChatWorkflow(WorkflowValidator),
			DeliveryWorkflow => new SupportChatWorkflow(WorkflowValidator),
			SupportChatWorkflow => new SupportChatWorkflow(WorkflowValidator),
			_ => CurrentWorkflow
		};

		return true;
	}

	public override bool OnSelectNextWorkflow(
		string? conversationStatus,
		object? statesArgs,
		Action<string> onAssistantMessage,
		CancellationToken cancellationToken)
	{
		var states = statesArgs as WebClients.ShopWare.Models.State[];

		SelectNextShopinBitWorkflow(conversationStatus, states);

		WorkflowValidator.Signal(false);
		RunNoInputWorkflows(onAssistantMessage, cancellationToken);

		// Continue the loop until next workflow is there and is completed.
		if (CurrentWorkflow is not null)
		{
			if (CurrentWorkflow.IsCompleted)
			{
				SelectNextShopinBitWorkflow(null, cancellationToken);
			}
		}

		return true;
	}

	private Workflow? GetShopinBitWorkflowFromConversation(string? conversationStatus, WebClients.ShopWare.Models.State[] states)
	{
		switch (conversationStatus)
		{
			case "Started":
				return new InitialWorkflow(WorkflowValidator, _countries);

			case "OfferReceived":
				return new DeliveryWorkflow(WorkflowValidator, states);

			case "PaymentDone":
				return new SupportChatWorkflow(WorkflowValidator);

			case "PaymentConfirmed":
				return new SupportChatWorkflow(WorkflowValidator);

			case "OfferAccepted":
				return new SupportChatWorkflow(WorkflowValidator);

			case "InvoiceReceived":
				return new SupportChatWorkflow(WorkflowValidator);

			case "InvoiceExpired":
				return new SupportChatWorkflow(WorkflowValidator);

			case "InvoicePaidAfterExpiration":
				return new SupportChatWorkflow(WorkflowValidator);

			case "Shipped":
				return new SupportChatWorkflow(WorkflowValidator);

			case "Finished":
				return new SupportChatWorkflow(WorkflowValidator);

			case "Support":
				return new SupportChatWorkflow(WorkflowValidator);

			default:
				return null;
		}
	}
}
