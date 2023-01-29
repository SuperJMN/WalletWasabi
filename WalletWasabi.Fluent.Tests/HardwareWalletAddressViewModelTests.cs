using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using WalletWasabi.Bridge;
using WalletWasabi.Fluent.ViewModels.Wallets.Receive;

namespace WalletWasabi.Fluent.Tests;

public class HardwareWalletAddressViewModelTests
{
	[Fact]
	public async Task Fail_to_display_address_set_error_message()
	{
		var address = Mock.Of<IAddress>();
		var walletMock = Mock.Of<IHardwareWallet>(x => x.Display(address) == Observable.Return(Result.Failure("Failure")));
		var sut = new HardwareWalletAddressViewModel(address, walletMock);

		await sut.ShowOnHwWalletCommand.Execute();

		sut.ErrorMessage.Should().Be("Failure");
	}

	[Fact]
	public async Task Success_to_display_address_error_message_is_empty()
	{
		var address = Mock.Of<IAddress>();
		var walletMock = Mock.Of<IHardwareWallet>(x => x.Display(address) == Observable.Return(Result.Success()));
		var sut = new HardwareWalletAddressViewModel(address, walletMock);

		await sut.ShowOnHwWalletCommand.Execute();

		sut.ErrorMessage.Should().BeEmpty();
	}
}
