using NBitcoin;

namespace WalletWasabi.Bridge;

public record Address : IAddress
{
	private Address(PubKey pubKey, Network network, HDFingerprint hdFingerprint, KeyPath fullKeyPath, IEnumerable<string> labels)
	{
		PubKey = pubKey;
		Network = network;
		HdFingerprint = hdFingerprint;
		FullKeyPath = fullKeyPath;
		Labels = labels;
		P2wpkhAddress = pubKey.GetAddress(ScriptPubKeyType.Segwit, network);
	}

	public BitcoinAddress P2wpkhAddress { get; }
	public IEnumerable<string> Labels { get; }

	public static Address From(PubKey pubKey, KeyPath fullKeyPath, IEnumerable<string> labels, Wallets.Wallet wallet)
	{
		return new Address(pubKey, wallet.Network, wallet.KeyManager.MasterFingerprint.Value, fullKeyPath, labels);
	}

	public PubKey PubKey { get; }
	public Network Network { get; }
	public HDFingerprint HdFingerprint { get; }
	public KeyPath FullKeyPath { get; }
}
