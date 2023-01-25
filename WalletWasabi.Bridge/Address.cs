using NBitcoin;
using WalletWasabi.Blockchain.Keys;

namespace WalletWasabi.Bridge;

public record Address : IAddress
{
	private Address(HdPubKey hdPubKey, Network network, HDFingerprint hdFingerprint)
	{
		HdPubKey = hdPubKey;
		Network = network;
		HdFingerprint = hdFingerprint;
	}

	public static Address From(HdPubKey hdPubKey, WalletWasabi.Wallets.Wallet wallet)
	{
		return new Address(hdPubKey, wallet.Network, wallet.KeyManager.MasterFingerprint.Value);
	}

	public HdPubKey HdPubKey { get; }
	public Network Network { get; }
	public HDFingerprint HdFingerprint { get; }
}
