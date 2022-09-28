using System.Collections.Generic;
using NBitcoin;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Models;

namespace WalletWasabi.Blockchain.Transactions;

public class TransactionSummary
{
	public DateTimeOffset DateTime { get; set; }
	public Height Height { get; set; }
	public Money Amount { get; set; }
	public SmartLabel Label { get; set; }
	public uint256 TransactionId { get; set; }
	public uint256? BlockHash { get; set; }
	public int BlockIndex { get; set; }
	public bool IsOwnCoinjoin { get; set; }
	public IEnumerable<Output> Outputs { get; set; }
	public IEnumerable<Input> Inputs { get; set; }
	public int VirtualSize { get; set; }
	public int Version { get; set; }
	public long BlockTime { get; set; }
}
