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
}
