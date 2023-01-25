using NBitcoin;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Bridge;

namespace WalletWasabi.Fluent.Tests;

public class TestAddress : IAddress
{
	public HdPubKey HdPubKey { get; }
	public PubKey PubKey { get; set; }
	public Network Network { get; }
	public HDFingerprint HdFingerprint { get; }
	public KeyPath FullKeyPath { get; }
	public BitcoinAddress P2wpkhAddress { get; }
	public IEnumerable<string> Labels { get; }
}
