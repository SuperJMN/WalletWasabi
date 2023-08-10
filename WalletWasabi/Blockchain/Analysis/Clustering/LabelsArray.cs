using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WalletWasabi.Blockchain.Analysis.Clustering;

public class LabelsArray : IComparable<LabelsArray>, IEnumerable<string>
{
	private readonly HashSet<string> _labels;
	private static LabelsArray? EmptyInstance;

	public LabelsArray() : this(Enumerable.Empty<string>().ToArray())
	{
	}

	public LabelsArray(IEnumerable<string> labels) : this(labels.ToArray())
	{
	}

	public LabelsArray(IEnumerable<LabelsArray> labels) : this(labels.SelectMany(x => x._labels))
	{
	}

	public LabelsArray(params string[] labels)
	{
		_labels = new HashSet<string>(labels, StringComparer.OrdinalIgnoreCase);
	}

	public bool IsEmpty => !_labels.Any();

	public static LabelsArray Empty => EmptyInstance ??= new();

	public int Count => _labels.Count;

	public int CompareTo(LabelsArray? other)
	{
		return StringComparer.CurrentCultureIgnoreCase.Compare(this, other);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<string> GetEnumerator()
	{
		return _labels.GetEnumerator();
	}

	public override string ToString()
	{
		return string.Join(", ", _labels);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj?.GetType() != GetType())
		{
			return false;
		}

		return Equals((LabelsArray) obj);
	}

	public override int GetHashCode()
	{
		return 0;
	}

	protected bool Equals(LabelsArray other)
	{
		return _labels.SetEquals(other._labels);
	}

	public static implicit operator LabelsArray(string label)
	{
		return new LabelsArray(label);
	}

	public static implicit operator string?(LabelsArray label)
	{
		return label._labels.FirstOrDefault();
	}

	public static bool operator ==(LabelsArray array, string label)
	{
		return new LabelsArray(label).Equals(array);
	}

	public static bool operator !=(LabelsArray array, string label)
	{
		return !new LabelsArray(label).Equals(array);
	}

	public static LabelsArray Merge(params LabelsArray[] labels)
	{
		return new LabelsArray(labels.SelectMany(array => array));
	}
}
