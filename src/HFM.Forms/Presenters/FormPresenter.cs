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

public abstract class AsyncFormPresenter : IAsyncFormPresenter
{
    public virtual IWin32Form Form { get; protected set; }

    public virtual Task ShowAsync()
    {
        Form = OnCreateForm();
        Form.Closed += async (s, e) => await OnClosed(s, e).ConfigureAwait(true);
        Form.Show();
        return Task.CompletedTask;
    }

    protected abstract IWin32Form OnCreateForm();

    public virtual void Close() => Form?.Close();

    public event EventHandler Closed;

    protected virtual Task OnClosed(object sender, EventArgs e)
    {
        Closed?.Invoke(this, e);
        return Task.CompletedTask;
    }

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

public abstract class AsyncFormPresenter<TViewModel> : AsyncFormPresenter where TViewModel : AsyncViewModelBase
{
    protected AsyncViewModelBase ModelBase { get; }

    protected AsyncFormPresenter(TViewModel model)
    {
        ModelBase = model;
    }

    public override async Task ShowAsync()
    {
        await ModelBase.LoadAsync().ConfigureAwait(true);
        await base.ShowAsync().ConfigureAwait(true);
    }

    protected override async Task OnClosed(object sender, EventArgs e)
    {
        await ModelBase.SaveAsync().ConfigureAwait(true);
        await base.OnClosed(sender, e).ConfigureAwait(true);
    }
}
