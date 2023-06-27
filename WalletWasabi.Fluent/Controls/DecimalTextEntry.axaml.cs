using System.Globalization;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using ReactiveUI;

namespace WalletWasabi.Fluent.Controls;

public class DecimalTextEntry : TemplatedControl
{
	public static readonly DirectProperty<DecimalTextEntry, string?> FormattedValueProperty = AvaloniaProperty.RegisterDirect<DecimalTextEntry, string?>(nameof(FormattedValue), o => o.FormattedValue, (o, v) => o.FormattedValue = v);

	public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<DecimalTextEntry, string>(nameof(Text), "");

	public static readonly DirectProperty<DecimalTextEntry, decimal?> ValueProperty = AvaloniaProperty.RegisterDirect<DecimalTextEntry, decimal?>(nameof(Value), o => o.Value, (o, v) => o.Value = v, enableDataValidation: true, defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<ControlTemplate> RightContentTemplateProperty = AvaloniaProperty.Register<DecimalTextEntry, ControlTemplate>(nameof(RightContentTemplate));

	public static readonly StyledProperty<string?> FormatProperty = AvaloniaProperty.Register<DecimalTextEntry, string?>(nameof(Format));

	private string? _formattedValue;

	private decimal? _value;

	public DecimalTextEntry()
	{
		this
			.WhenAnyValue(x => x.Value, x => x.Format, (value, format) => new { value, format })
			.Do(tuple =>
			{
				FormattedValue = tuple.value?.ToString(tuple.format, CultureInfo.CurrentCulture) ?? "";
			})
			.Subscribe();

		this
			.WhenAnyValue(x => x.Text)
			.Select(Parse)
			.Do(x => Value = x)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(v => Text = v?.ToString(CultureInfo.CurrentCulture) ?? "")
			.Subscribe();
	}

	public string? Format
	{
		get => GetValue(FormatProperty);
		set => SetValue(FormatProperty, value);
	}

	public ControlTemplate RightContentTemplate
	{
		get => GetValue(RightContentTemplateProperty);
		set => SetValue(RightContentTemplateProperty, value);
	}

	public decimal? Value
	{
		get => _value;
		set => SetAndRaise(ValueProperty, ref _value, value);
	}

	public string? FormattedValue
	{
		get => _formattedValue;
		private set => SetAndRaise(FormattedValueProperty, ref _formattedValue, value);
	}

	public string Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
	{
		DataValidationErrors.SetError(this, value.Error);
	}

	private static decimal? Parse(string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return default;
		}

		if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var n))
		{
			return n;
		}

		return default;
	}
}
