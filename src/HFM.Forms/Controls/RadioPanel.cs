using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    /// <summary>
    /// A Panel that supports binding an enum property to a set of RadioButton controls. 
    /// </summary>
    /// <remarks>
    /// Add any number of RadioButtons. Assign the numeric value of your enum option to the Tag property of each button.
    ///
    /// Example: 
    /// MyEnum.Foo = 0, MyEnum.Bar = 1
    /// RadioButton1.Text = "Foo", RadioButton1.Tag = 0,
    /// RadioButton2.Text = "Bar", RadioButton2.tag = 1
    /// 
    /// RadioPanel will set the tagged RadioButton control and communicate changes to/from the property when the user selects another option. 
    /// </remarks>
    public class RadioPanel : Panel
    {
        private readonly PropertyChangedEventHandler _propertyChangedEventHandler;

        public RadioPanel()
        {
            _propertyChangedEventHandler = OnDataSourcePropertyChanged;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is RadioButton rb)
            {
                rb.CheckedChanged += RadioButton_CheckedChanged;
            }
        }

        private string _valueMember;

        public string ValueMember
        {
            get => _valueMember;
            set
            {
                if (_valueMember != value)
                {
                    _valueMember = value;
                    OnValueMemberChanged();
                }
            }
        }

        protected virtual void OnValueMemberChanged()
        {
            SetRadioButtonBinding();
        }

        protected EventInfo DataSourcePropertyChangedEventInfo { get; private set; }

        private object _dataSource;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                if (DataSourcePropertyChangedEventInfo != null && _dataSource != null)
                {
                    DataSourcePropertyChangedEventInfo.RemoveEventHandler(_dataSource, _propertyChangedEventHandler);
                }

                DataSourcePropertyChangedEventInfo = null;
                _dataSource = value;

                if (_dataSource != null)
                {
                    var notifyPropertyChanged = _dataSource.GetType().GetInterface(nameof(INotifyPropertyChanged));
                    if (notifyPropertyChanged != null)
                    {
                        DataSourcePropertyChangedEventInfo = notifyPropertyChanged.GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
                        if (DataSourcePropertyChangedEventInfo != null)
                        {
                            DataSourcePropertyChangedEventInfo.AddEventHandler(_dataSource, _propertyChangedEventHandler);
                        }
                    }
                }

                OnDataSourceChanged();
            }
        }

        protected virtual void OnDataSourceChanged()
        {
            SetRadioButtonBinding();
        }

        protected PropertyInfo DataSourceValueMemberPropertyInfo { get; private set; }

        protected void SetRadioButtonBinding()
        {
            if (DataSource is null || String.IsNullOrWhiteSpace(ValueMember))
            {
                DataSourceValueMemberPropertyInfo = null;
                return;
            }

            var pi = DataSource.GetType().GetProperty(ValueMember);
            if (pi != null && pi.PropertyType.IsEnum)
            {
                DataSourceValueMemberPropertyInfo = pi;
                SetRadioButtonValue();
            }
        }

        private int GetDataSourceValueMemberPropertyValue()
        {
            var value = DataSourceValueMemberPropertyInfo.GetValue(DataSource);
            return Convert.ToInt32(value);
        }

        private void SetRadioButtonValue()
        {
            if (DataSourceValueMemberPropertyInfo == null) return;

            var propertyValue = GetDataSourceValueMemberPropertyValue();
            foreach (Control c in Controls)
            {
                if (c is RadioButton rb)
                {
                    int? tagValue = GetTagValue(rb);
                    if (tagValue.HasValue && tagValue.Value == propertyValue)
                    {
                        rb.Checked = true;
                    }
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DataSource is null || DataSourceValueMemberPropertyInfo is null) return;

            if (sender is RadioButton rb && rb.Checked)
            {
                int? tagValue = GetTagValue(rb);
                if (tagValue.HasValue)
                {
                    // Convert the int into its corresponding enum
                    var value = Enum.ToObject(DataSourceValueMemberPropertyInfo.PropertyType, tagValue.Value);

                    // Stop listening to property changes while we change the property - otherwise, stack overflow.
                    _processPropertyChange = false;

                    DataSourceValueMemberPropertyInfo.SetValue(DataSource, value, null);

                    _processPropertyChange = true;
                }
            }
        }

        private static int? GetTagValue(RadioButton rb)
        {
            // not exactly thrilled about using Tag
            if (rb.Tag == null)
            {
                return null;
            }

            int? value = null;
            if (rb.Tag is int intTag)
            {
                value = intTag;
            }
            else if (Int32.TryParse(rb.Tag.ToString(), out var result))
            {
                value = result;
            }

            return value;
        }

        private bool _processPropertyChange = true;

        // Handle PropertyChanged notifications from the source.
        protected void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_processPropertyChange) return;

            if (e.PropertyName.Equals(ValueMember))
            {
                SetRadioButtonValue();
            }
        }
    }
}
