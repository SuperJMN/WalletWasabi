using System.Collections.Generic;
using System.Linq;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using Xunit;

namespace WalletWasabi.Tests.UnitTests;

public class LabelRankingTests
{
	[Fact]
	public void Rank()
	{
		var receiveLabels = new List<SmartLabel>()
		{
			new("A"), new("B"), new("C"),
		};

		IEnumerable<SmartLabel> receiveAddressLabels = new List<SmartLabel>()
		{
			new("A"), new("B"), new("D"),
		};

		IEnumerable<SmartLabel> changeAddressLabels = new List<SmartLabel>
		{
			new("C"),
		};

		IEnumerable<SmartLabel> transactionLabels = new List<SmartLabel>
		{
			new("A"),
		};
		var input = new RankInput(receiveLabels, receiveAddressLabels, changeAddressLabels, transactionLabels);
		var result = LabelRanking.Rank(input, Intent.Receive);

		var expected = new[]
		{
			("C", 10001),
			("B", 10000),
			("A", 10000),
			("D", 100),
		}.ToHashSet();

		var actual = result.Select(x => (x.Key, x.Value)).ToHashSet();
		Assert.True(expected.SetEquals(actual));
	}
}
