using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace WalletWasabi.Fluent.Controls;

public class ReceiveAddressControl : TemplatedControl
{
	public static readonly StyledProperty<object> ExtraContentProperty = AvaloniaProperty.Register<ReceiveAddressControl, object>(nameof(ExtraContent));

	public static readonly StyledProperty<string> AddressProperty = AvaloniaProperty.Register<ReceiveAddressControl, string>(nameof(Address));

	public static readonly StyledProperty<IEnumerable<string>> LabelsProperty = AvaloniaProperty.Register<ReceiveAddressControl, IEnumerable<string>>(nameof(Labels));

	public static readonly StyledProperty<bool[,]> QrCodeProperty = AvaloniaProperty.Register<ReceiveAddressControl, bool[,]>(nameof(QrCode));

	public object ExtraContent
	{
		get => GetValue(ExtraContentProperty);
		set => SetValue(ExtraContentProperty, value);
	}

	public string Address
	{
		get => GetValue(AddressProperty);
		set => SetValue(AddressProperty, value);
	}

	public IEnumerable<string> Labels
	{
		get => GetValue(LabelsProperty);
		set => SetValue(LabelsProperty, value);
	}

	public bool[,] QrCode
	{
		get => GetValue(QrCodeProperty);
		set => SetValue(QrCodeProperty, value);
	}
}
