
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    public class DataErrorTextBox : TextBox
    {
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ToolTip ErrorToolTip { get; set; }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Point ErrorToolTipPoint { get; set; }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ErrorToolTipDuration { get; set; }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design", typeof(UITypeEditor)), Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ErrorToolTipText { get; set; }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ErrorBackColor { get; set; }

        public DataErrorTextBox()
        {
            DoubleBuffered = true;
            DataBindings.CollectionChanged += DataBindingsOnCollectionChanged;
        }

        private string _bindingMember;
        private INotifyPropertyChanged _dataSource;
        
        private void DataBindingsOnCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Disposing || IsDisposed || DataBindings.Count == 0)
            {
                if (_dataSource != null)
                {
                    _dataSource.PropertyChanged -= DataSourceOnPropertyChanged;
                }
                _dataSource = null;
                return;
            }

            var textBinding = DataBindings[nameof(Text)];
            _bindingMember = textBinding?.BindingMemberInfo.BindingMember;
            _dataSource = textBinding?.DataSource as INotifyPropertyChanged;
            if (_dataSource != null)
            {
                _dataSource.PropertyChanged += DataSourceOnPropertyChanged;
            }
        }

        private void DataSourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Enabled)
            {
                if ((String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == _bindingMember) && _dataSource is IDataErrorInfo dataErrorInfo)
                {
                    string errorText = dataErrorInfo[_bindingMember];
                    ShowToolTip(errorText);
                    SetErrorColor(errorText);
                }
            }
            else
            {
                ShowToolTip(null);
                SetErrorColor(null);
            }
        }

        private void ShowToolTip(string errorText)
        {
            if (ErrorToolTip == null) return;
            if (!Enabled) return;

            if (!String.IsNullOrWhiteSpace(errorText))
            {
                ErrorToolTip.RemoveAll();
                ErrorToolTip.Tag = Name;
                ErrorToolTipText = errorText;
                ErrorToolTip.Show(ErrorToolTipText, this, ErrorToolTipPoint, ErrorToolTipDuration);
            }
            else
            {
                ErrorToolTip.RemoveAll();
            }
        }

        private void SetErrorColor(string errorText)
        {
            if (!Enabled) return;

            var newColor = SystemColors.Window;
            if (!String.IsNullOrWhiteSpace(errorText))
            {
                newColor = ErrorBackColor;
            }

            BackColor = newColor;
        }
    }
}
