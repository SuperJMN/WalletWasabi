using System.Collections.Generic;
using System.ComponentModel;

namespace WalletWasabi.Fluent.ViewModels.CoinControl.Core;

public interface ISelectableModel : INotifyPropertyChanged
{
	IEnumerable<ISelectableModel> Selectables { get; }
	public bool? IsSelected { get; set; }
}
