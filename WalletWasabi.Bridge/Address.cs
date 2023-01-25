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
		P2shOverP2wpkhAddress = pubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, network);
	}

	public BitcoinAddress P2shOverP2wpkhAddress { get; }
	public IEnumerable<string> Labels { get; }

	public static Address From(PubKey hdPubKey, KeyPath fullKeyPath, IEnumerable<string> labels, Wallets.Wallet wallet)
	{
		return new Address(hdPubKey, wallet.Network, wallet.KeyManager.MasterFingerprint.Value, fullKeyPath, labels);
	}

	public PubKey PubKey { get; }
	public Network Network { get; }
	public HDFingerprint HdFingerprint { get; }
	public KeyPath FullKeyPath { get; }
}
