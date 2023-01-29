using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent.Controls;
public class ReceiveAddressControl : TemplatedControl
{
	public static readonly StyledProperty<object> ExtraContentProperty = AvaloniaProperty.Register<ReceiveAddressControl, object>("ExtraContent");

	public object ExtraContent
	{
		get => GetValue(ExtraContentProperty);
		set => SetValue(ExtraContentProperty, value);
	}

	public static readonly StyledProperty<IReceiveAddressViewModel> ReceiveAddressProperty = AvaloniaProperty.Register<ReceiveAddressControl, IReceiveAddressViewModel>("ReceiveAddress");

	public IReceiveAddressViewModel ReceiveAddress
	{
		get => GetValue(ReceiveAddressProperty);
		set => SetValue(ReceiveAddressProperty, value);
	}

	public static readonly StyledProperty<string> AddressProperty = AvaloniaProperty.Register<ReceiveAddressControl, string>("Address");

	public string Address
	{
		get => GetValue(AddressProperty);
		set => SetValue(AddressProperty, value);
	}

	public static readonly StyledProperty<IEnumerable<string>> LabelsProperty = AvaloniaProperty.Register<ReceiveAddressControl, IEnumerable<string>>("Labels");

	public IEnumerable<string> Labels
	{
		get => GetValue(LabelsProperty);
		set => SetValue(LabelsProperty, value);
	}

	public static readonly StyledProperty<bool[,]> QrCodeProperty = AvaloniaProperty.Register<ReceiveAddressControl, bool[,]>("QrCode");

	public bool[,] QrCode
	{
		get => GetValue(QrCodeProperty);
		set => SetValue(QrCodeProperty, value);
	}
}
