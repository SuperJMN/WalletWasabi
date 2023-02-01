using RawWallet = WalletWasabi.Wallets.Wallet;

namespace WalletWasabi.Bridge;

public class SoftwareWallet : ImprovedWallet
{
	public SoftwareWallet(RawWallet wallet) : base(wallet)
	{
	}
}
