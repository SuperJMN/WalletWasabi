using WalletWasabi.Gui.ViewModels;
using WalletWasabi.Helpers;
using System;
using System.Reactive;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WalletWasabi.Gui.Helpers;
using WalletWasabi.Logging;
using Splat;

namespace WalletWasabi.Gui.Controls.LockScreen
{
	public class PinLockScreenViewModel : LockScreenViewModelBase
	{
		private string _pinInput;		

		public PinLockScreenViewModel()
		{
			KeyPadCommand = ReactiveCommand.Create<string>((arg) =>
			{
				if (arg == "BACK")
				{
					if (PinInput.Length > 0)
					{
						PinInput = PinInput.Substring(0, PinInput.Length - 1);
					}
				}
				else if (arg == "CLEAR")
				{
					PinInput = string.Empty;
				}
				else
				{
					PinInput += arg;
				}
			});

			KeyPadCommand.ThrownExceptions
				.ObserveOn(RxApp.TaskpoolScheduler)
				.Subscribe(ex => Logger.LogError(ex));

			this.WhenAnyValue(x => x.PinInput)
				.Throttle(TimeSpan.FromSeconds(2.5))
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x =>
				{
					var global = Locator.Current.GetService<Global>();

					if (global.UiConfig.LockScreenPinHash != HashHelpers.GenerateSha256Hash(x))
					{
						NotificationHelpers.Error("PIN is incorrect!");
					}
				});

			this.WhenAnyValue(x => x.PinInput)
				.Select(Guard.Correct)
				.Where(x => x.Length != 0)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x =>
				{
					var global = Locator.Current.GetService<Global>();

					if (global.UiConfig.LockScreenPinHash == HashHelpers.GenerateSha256Hash(x))
					{
						IsLocked = false;
						PinInput = string.Empty;
					}
				});

			this.WhenAnyValue(x => x.IsLocked)
				.Where(x => !x)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(_ => PinInput = string.Empty);
		}

		protected override void OnInitialise(CompositeDisposable disposables)
		{
			var global = Locator.Current.GetService<Global>();
		}

		public string PinInput
		{
			get => _pinInput;
			set => this.RaiseAndSetIfChanged(ref _pinInput, value);
		}

		public ReactiveCommand<string, Unit> KeyPadCommand { get; }
	}
}
