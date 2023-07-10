using System.Globalization;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using ReactiveUI;
using SQLitePCL;
using WalletWasabi.Fluent.Behaviors;

namespace WalletWasabi.Fluent.Controls;

public class DecimalTextEntry : TemplatedControl
{
	public static readonly DirectProperty<DecimalTextEntry, string?> FormattedValueProperty = AvaloniaProperty.RegisterDirect<DecimalTextEntry, string?>(nameof(FormattedValue), o => o.FormattedValue, (o, v) => o.FormattedValue = v);
	public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<DecimalTextEntry, string>(nameof(Text), "");
	public static readonly DirectProperty<DecimalTextEntry, decimal?> ValueProperty = AvaloniaProperty.RegisterDirect<DecimalTextEntry, decimal?>(nameof(Value), o => o.Value, (o, v) => o.Value = v, enableDataValidation: true, defaultBindingMode: BindingMode.TwoWay);
	public static readonly StyledProperty<ControlTemplate> RightContentTemplateProperty = AvaloniaProperty.Register<DecimalTextEntry, ControlTemplate>(nameof(RightContentTemplate));
	public static readonly StyledProperty<string?> FormatProperty = AvaloniaProperty.Register<DecimalTextEntry, string?>(nameof(Format));
	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<DecimalTextEntry>();
	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<DecimalTextEntry>();
	public static readonly StyledProperty<ControlTemplate> LeftContentTemplateProperty = AvaloniaProperty.Register<DecimalTextEntry, ControlTemplate>(nameof(LeftContentTemplate));
	public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<DecimalTextEntry, bool>(nameof(IsReadOnly));
	public static readonly StyledProperty<int> MaxDecimalPlacesProperty = AvaloniaProperty.Register<DecimalTextEntry, int>(nameof(MaxDecimalPlaces));
	private TextBox? _mainTextBox;

	public static readonly DirectProperty<DecimalTextEntry, TextBox?> MainTextBoxProperty = AvaloniaProperty.RegisterDirect<DecimalTextEntry, TextBox?>("MainTextBox", o => o.MainTextBox, (o, v) => o.MainTextBox = v);

	public TextBox? MainTextBox
	{
		get => _mainTextBox;
		set => SetAndRaise(MainTextBoxProperty, ref _mainTextBox, value);
	}

	private TextBoxNumberController? _behavior;

	private string? _formattedValue;

	
	private decimal? _value;

	public DecimalTextEntry()
	{
		this
			.WhenAnyValue(x => x.Value, x => x.Format, (value, format) => new { value, format })
			.Do(tuple => FormattedValue = tuple.value?.ToString(tuple.format, CultureInfo.CurrentCulture) ?? "")
			.Subscribe();

		this
			.WhenAnyValue(x => x.Text)
			.Select(Parse)
			.Do(v => Value = v)
			.Subscribe();

		this.WhenAnyValue(x => x.Value)
			.Do(v => Text = v?.ToString(CultureInfo.CurrentCulture) ?? "")
			.Subscribe();

		this.WhenAnyValue(x => x.MainTextBox, x => x.MaxDecimalPlaces)
			.Do(x =>
			{
				_behavior?.Dispose();
				if (x.Item1 is { } textBox)
				{
					_behavior = new TextBoxNumberController(textBox, x.Item2);
				}
				else
				{
					_behavior = null;
				}
			})
			.Subscribe();
	}

	public int MaxDecimalPlaces
	{
		get => GetValue(MaxDecimalPlacesProperty);
		set => SetValue(MaxDecimalPlacesProperty, value);
	}

	public bool IsReadOnly
	{
		get => GetValue(IsReadOnlyProperty);
		set => SetValue(IsReadOnlyProperty, value);
	}

	public ControlTemplate LeftContentTemplate
	{
		get => GetValue(LeftContentTemplateProperty);
		set => SetValue(LeftContentTemplateProperty, value);
	}

	public HorizontalAlignment HorizontalContentAlignment
	{
		get => GetValue(HorizontalContentAlignmentProperty);
		set => SetValue(HorizontalContentAlignmentProperty, value);
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get => GetValue(VerticalContentAlignmentProperty);
		set => SetValue(VerticalContentAlignmentProperty, value);
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

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		MainTextBox = e.NameScope.Find<TextBox>("MainTextBox");
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		MainTextBox?.Focus();
		MainTextBox?.SelectAll();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_behavior.Dispose();
	}

	private static decimal? Parse(string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return default;
		}

		return decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var n) ? n : default(decimal?);
	}
}
