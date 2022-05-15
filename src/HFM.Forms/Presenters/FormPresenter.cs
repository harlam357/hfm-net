using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters;

public abstract class FormPresenter : IFormPresenter
{
    public virtual IWin32Form Form { get; protected set; }

    public virtual void Show()
    {
        Form = OnCreateForm();
        Form.Closed += OnClosed;
        Form.Show();
    }

    protected abstract IWin32Form OnCreateForm();

    public virtual void Close() => Form?.Close();

    public event EventHandler Closed;

    protected virtual void OnClosed(object sender, EventArgs e) => Closed?.Invoke(this, e);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Form?.Dispose();
            }
        }
        _disposed = true;
    }
}

public abstract class FormPresenter<TViewModel> : FormPresenter where TViewModel : ViewModelBase
{
    protected ViewModelBase ModelBase { get; }

    protected FormPresenter(TViewModel model)
    {
        ModelBase = model;
    }

    public override void Show()
    {
        ModelBase.Load();
        base.Show();
    }

    protected override void OnClosed(object sender, EventArgs e)
    {
        ModelBase.Save();
        base.OnClosed(sender, e);
    }
}
