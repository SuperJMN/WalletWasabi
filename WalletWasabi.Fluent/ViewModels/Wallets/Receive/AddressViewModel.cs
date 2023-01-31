using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Bridge;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Receive;

public partial class AddressViewModel : ViewModelBase
{
	[AutoNotify] private BitcoinAddress _address;

	public AddressViewModel(ReceiveAddressesViewModel parent, HdPubKey model, IWallet myWallet, IAddress address)
	{
		_address = address.P2wpkhAddress;
		
		Label = new SmartLabel(address.Labels);

		CopyAddressCommand =
			ReactiveCommand.CreateFromTask(async () =>
			{
				if (Application.Current is { Clipboard: { } clipboard })
				{
					await clipboard.SetTextAsync(Address.ToString());
				}
			});

		HideAddressCommand =
			ReactiveCommand.CreateFromTask(async () => await parent.HideAddressAsync(model, Address.ToString()));

		EditLabelCommand =
			ReactiveCommand.Create(() => parent.NavigateToAddressEdit(model, parent.Wallet.KeyManager));

		NavigateCommand = ReactiveCommand.Create(() =>
		{
			var receiveAddressHostViewModel = ViewModelLocator.GetAddressViewModel(myWallet, address);
			parent.Navigate().To(receiveAddressHostViewModel);
		});
	}

	public ICommand CopyAddressCommand { get; }

	public ICommand HideAddressCommand { get; }

	public ICommand EditLabelCommand { get; }

	public ReactiveCommand<Unit, Unit> NavigateCommand { get; }

	public SmartLabel Label { get; }

	public static Comparison<AddressViewModel?> SortAscending<T>(Func<AddressViewModel, T> selector)
	{
		return (x, y) =>
		{
			if (x is null && y is null)
			{
				return 0;
			}
			else if (x is null)
			{
				return -1;
			}
			else if (y is null)
			{
				return 1;
			}
			else
			{
				return Comparer<T>.Default.Compare(selector(x), selector(y));
			}
		};
	}

	public static Comparison<AddressViewModel?> SortDescending<T>(Func<AddressViewModel, T> selector)
	{
		return (x, y) =>
		{
			if (x is null && y is null)
			{
				return 0;
			}
			else if (x is null)
			{
				return 1;
			}
			else if (y is null)
			{
				return -1;
			}
			else
			{
				return Comparer<T>.Default.Compare(selector(y), selector(x));
			}
		};
	}
}
