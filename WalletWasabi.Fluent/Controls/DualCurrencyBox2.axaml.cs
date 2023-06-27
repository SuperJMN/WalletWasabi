using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DualCurrencyBox2 : TemplatedControl
{
	private const decimal Tolerance = (decimal) 0.00000100;

	public static readonly StyledProperty<decimal?> BtcBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(
		nameof(BtcBoxValue),
		defaultBindingMode: BindingMode.TwoWay,
		coerce: (_, newValue) =>
		{
			if (newValue == null)
			{
				return default;
			}

			return Math.Round(newValue.Value, 8);
		});

	public static readonly StyledProperty<decimal?> UsdBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(UsdBoxValue), defaultBindingMode: BindingMode.TwoWay);

	public static readonly DirectProperty<DualCurrencyBox2, decimal?> ValueProperty = AvaloniaProperty.RegisterDirect<DualCurrencyBox2, decimal?>(nameof(Value), o => o.Value, (o, v) => o.Value = SetCoercedValue(o.Value, v), enableDataValidation: true);

	public static readonly StyledProperty<decimal> ExchangeRateProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal>(nameof(ExchangeRate), new decimal(1));

	public static readonly DirectProperty<DualCurrencyBox2, DecimalTextEntry?> BtcEntryProperty = AvaloniaProperty.RegisterDirect<DualCurrencyBox2, DecimalTextEntry?>("BtcEntry", o => o.BtcEntry, (o, v) => o.BtcEntry = v);

	public static readonly DirectProperty<DualCurrencyBox2, DecimalTextEntry?> UsdEntryProperty = AvaloniaProperty.RegisterDirect<DualCurrencyBox2, DecimalTextEntry?>("UsdEntry", o => o.UsdEntry, (o, v) => o.UsdEntry = v);

	private DecimalTextEntry? _btcEntry;

	private DecimalTextEntry? _usdEntry;

	private decimal? _value;

	public DualCurrencyBox2()
	{
		this
			.WhenAnyValue(x => x.BtcBoxValue)
			.Do(
				btcValue =>
				{
					Value = btcValue;
					UsdBoxValue = BtcToUsd(btcValue);
				})
			.Subscribe();

		this
			.WhenAnyValue(x => x.UsdBoxValue)
			.Do(x => Value = x / ExchangeRate)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(
				btcValue =>
				{
					BtcBoxValue = btcValue;
					UsdBoxValue = BtcToUsd(btcValue);
				})
			.Subscribe();

		BtcTextBox = this.WhenAnyValue(x => x.BtcEntry.TextBox);
		UsdTextBox = this.WhenAnyValue(x => x.UsdEntry.TextBox);
	}

	public decimal? Value
	{
		get => _value;
		set => SetAndRaise(ValueProperty, ref _value, value);
	}

	public decimal? BtcBoxValue
	{
		get => GetValue(BtcBoxValueProperty);
		set => SetValue(BtcBoxValueProperty, value);
	}

	public decimal? UsdBoxValue
	{
		get => GetValue(UsdBoxValueProperty);
		set => SetValue(UsdBoxValueProperty, value);
	}

	public decimal ExchangeRate
	{
		get => GetValue(ExchangeRateProperty);
		set => SetValue(ExchangeRateProperty, value);
	}

	public IObservable<TextBox?> UsdTextBox { get; private set; }

	public DecimalTextEntry? BtcEntry
	{
		get => _btcEntry;
		set => SetAndRaise(BtcEntryProperty, ref _btcEntry, value);
	}

	public DecimalTextEntry? UsdEntry
	{
		get => _usdEntry;
		set => SetAndRaise(UsdEntryProperty, ref _usdEntry, value);
	}

	public IObservable<TextBox?> BtcTextBox { get; private set; }

	protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
	{
		DataValidationErrors.SetError(this, value.Error);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		BtcEntry = e.NameScope.Find<DecimalTextEntry>("BtcEntry");
		UsdEntry = e.NameScope.Find<DecimalTextEntry>("UsdEntry");
	}

	private static decimal? SetCoercedValue(decimal? currentValue, decimal? newValue)
	{
		if (currentValue is null)
		{
			return newValue;
		}

		if (newValue is null)
		{
			return default;
		}

		var diff = (decimal) (currentValue - newValue);

		if (Math.Abs(diff) <= Tolerance)
		{
			return currentValue;
		}

		return newValue.Value;
	}

	private decimal? BtcToUsd(decimal? x)
	{
		if (x is null)
		{
			return default;
		}

		return Math.Round((decimal) (x * ExchangeRate), 2);
	}
}
