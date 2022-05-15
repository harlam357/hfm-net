using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HFM.Forms.Models;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public virtual string Error => String.Empty;

    public virtual bool HasError => !String.IsNullOrWhiteSpace(Error);

    public virtual bool ValidateAcceptance()
    {
        OnPropertyChanged(String.Empty);
        return !HasError;
    }

    public virtual void Load()
    {

    }

    public virtual void Save()
    {

    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public abstract class AsyncViewModelBase : INotifyPropertyChanged
{
    public virtual string Error => String.Empty;

    public virtual bool HasError => !String.IsNullOrWhiteSpace(Error);

    public virtual bool ValidateAcceptance()
    {
        OnPropertyChanged(String.Empty);
        return !HasError;
    }

    public virtual Task LoadAsync() => Task.CompletedTask;

    public virtual Task SaveAsync() => Task.CompletedTask;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
