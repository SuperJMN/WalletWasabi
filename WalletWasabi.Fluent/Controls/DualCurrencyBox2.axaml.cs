using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using NBitcoin;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DualCurrencyBox2 : TemplatedControl
{
	public static readonly StyledProperty<decimal?> BtcBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(BtcBoxValue), defaultBindingMode: BindingMode.TwoWay);
	public static readonly StyledProperty<decimal?> UsdBoxValueProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal?>(nameof(UsdBoxValue), defaultBindingMode: BindingMode.TwoWay);
	public static readonly DirectProperty<DualCurrencyBox2, Money?> ValueProperty = AvaloniaProperty.RegisterDirect<DualCurrencyBox2, Money?>(nameof(Value), o => o.Value, (o, v) => o.Value = v, enableDataValidation: true);
	public static readonly StyledProperty<decimal> ExchangeRateProperty = AvaloniaProperty.Register<DualCurrencyBox2, decimal>(nameof(ExchangeRate), new decimal(1));
	public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<DualCurrencyBox2, bool>(nameof(IsReadOnly));
	public static readonly StyledProperty<bool> IsInvertedProperty = AvaloniaProperty.Register<DualCurrencyBox2, bool>(nameof(IsInverted), defaultBindingMode: BindingMode.TwoWay);

	private bool _areChangeNotificationsDisabled;

	private Money? _value;

	public DualCurrencyBox2()
	{
		this
			.WhenAnyValue(x => x.BtcBoxValue)
			.Do(OnBtcValueChanged)
			.Subscribe();

		this
			.WhenAnyValue(x => x.UsdBoxValue)
			.Do(OnUsdValueChanged)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(OnValueChanged)
			.Subscribe();
	}

	public bool IsInverted
	{
		get => GetValue(IsInvertedProperty);
		set => SetValue(IsInvertedProperty, value);
	}

	public bool IsReadOnly
	{
		get => GetValue(IsReadOnlyProperty);
		set => SetValue(IsReadOnlyProperty, value);
	}

	public Money? Value
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

	protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
	{
		DataValidationErrors.SetError(this, value.Error);
	}

	private void ExclusiveSet(Action action)
	{
		if (_areChangeNotificationsDisabled)
		{
			return;
		}

		_areChangeNotificationsDisabled = true;

		action();

		_areChangeNotificationsDisabled = false;
	}

	private void OnValueChanged(Money? btcValue)
	{
		ExclusiveSet(
			() =>
			{
				BtcBoxValue = btcValue?.ToDecimal(MoneyUnit.BTC);
				UsdBoxValue = BtcToUsd(btcValue);
			});
	}

	private void OnUsdValueChanged(decimal? x)
	{
		ExclusiveSet(
			() =>
			{
				Value = x.HasValue ? new Money((decimal) x / ExchangeRate, MoneyUnit.BTC) : null;
				BtcBoxValue = Value?.ToDecimal(MoneyUnit.BTC);
			});
	}

	private void OnBtcValueChanged(decimal? btcValue)
	{
		ExclusiveSet(
			() =>
			{
				Value = btcValue.HasValue ? new Money((decimal) btcValue, MoneyUnit.BTC) : null;
				UsdBoxValue = BtcToUsd(btcValue);
			});
	}

	private decimal? BtcToUsd(Money? money)
	{
		if (money is null)
		{
			return default;
		}

		if (money == Money.Zero)
		{
			return 0;
		}

		return Math.Round(money.ToDecimal(MoneyUnit.BTC) * ExchangeRate, 2);
	}

	private decimal? BtcToUsd(decimal? money)
	{
		if (money is null)
		{
			return default;
		}

		return Math.Round(money.Value * ExchangeRate, 2);
	}
}
