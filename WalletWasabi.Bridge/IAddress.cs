using NBitcoin;

namespace WalletWasabi.Bridge;

public interface IAddress
{
	//HdPubKey HdPubKey { get; }
	public PubKey PubKey { get; }

	Network Network { get; }
	HDFingerprint HdFingerprint { get; }
	KeyPath FullKeyPath { get; }
	BitcoinAddress P2shOverP2wpkhAddress { get; }
	IEnumerable<string> Labels { get; }
}
