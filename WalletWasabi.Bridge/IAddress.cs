using NBitcoin;

namespace WalletWasabi.Bridge;

public interface IAddress
{
	PubKey PubKey { get; }
	Network Network { get; }
	HDFingerprint HdFingerprint { get; }
	KeyPath FullKeyPath { get; }
	BitcoinAddress P2wpkhAddress { get; }
	IEnumerable<string> Labels { get; }
}
