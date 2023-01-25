using NBitcoin;

namespace WalletWasabi.Bridge;

public interface ITransaction
{
	public uint256 Id { get; }
	Money Amount { get; }
	IEnumerable<string> Labels { get; }
}
