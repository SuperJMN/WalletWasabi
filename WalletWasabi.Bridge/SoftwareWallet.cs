using RawWallet = WalletWasabi.Wallets.Wallet;

namespace WalletWasabi.Bridge;

public class SoftwareWallet : SuperWallet
{
	public SoftwareWallet(RawWallet wallet) : base(wallet)
	{
	}
}
