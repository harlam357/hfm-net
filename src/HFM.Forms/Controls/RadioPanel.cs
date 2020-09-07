
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
                if (value != null)
                {
                    _valueMember = value;
                    if (_dataSource != null)
                    {
                        SetRadioButtonBinding();
                    }
                }
            }
        }

        private object _dataSource;
        private EventInfo _eventInfo;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                if (value == null) { throw new ArgumentNullException("DataSource"); }

                if (_eventInfo != null)
                {
                    _eventInfo.RemoveEventHandler(_dataSource, _propertyChangedEventHandler);
                }

                // Set new binding source.
                _dataSource = value;

                // Does this property's object support INotifyPropertyChanged? If so, listen to property changes so that we can update our binding if the
                // source changes.
                Type propertyChangedInterface = _dataSource.GetType().GetInterface(nameof(INotifyPropertyChanged));
                if (propertyChangedInterface != null)
                {
                    _eventInfo = propertyChangedInterface.GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
                    if (_eventInfo != null) // which would be weird if it did...
                    {
                        _eventInfo.AddEventHandler(_dataSource, _propertyChangedEventHandler);
                    }
                }

                if (!String.IsNullOrEmpty(_valueMember))
                {
                    SetRadioButtonBinding();
                }

                _dataSource = value;
            }
        }

        private PropertyInfo _propertyInfo;

        /// <summary>
        /// Set up the binding to the property.
        /// </summary>
        private void SetRadioButtonBinding()
        {
            if (_dataSource == null)
            {
                throw new InvalidOperationException("m_objDataSource is null. This shouldn't happen here.");
            }

            _propertyInfo = _dataSource.GetType().GetProperty(_valueMember);
            if (_propertyInfo == null)
            {
                throw new ArgumentException("Could not find " + _valueMember + " on binding object " + _dataSource.GetType().Name);
            }

            SetRadioButtonValue();
        }

        private void SetRadioButtonValue()
        {
            if (_propertyInfo == null)
            {
                throw new InvalidOperationException("_pi cannot be null.");
            }

            object o = _propertyInfo.GetValue(_dataSource, null);

            string strInt = ((int)o).ToString();

            if (Int32.TryParse(strInt, out var i))
            {
                // TODO: I'm not exactly thrilled about using Tag. There ought to be an easier, faster, more intuitive way to do this. 
                // Until I figure it out, though, Tag will do.
                foreach (Control c in Controls)
                {
                    if (c is RadioButton rb)
                    {
                        if (rb.Tag == null)
                        {
                            throw new InvalidOperationException("RadioButton " + rb.Name +
                                                                " does not have its Tag property set to a valid enum integer value.");
                        }

                        if (!Int32.TryParse(rb.Tag.ToString(), out var setting))
                        {
                            throw new InvalidOperationException("RadioButton " + rb.Name +
                                                                " does not have its Tag property set to a valid enum integer value.");
                        }

                        if (setting == i)
                        {
                            rb.Checked = true;
                        }
                    }
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_dataSource != null)
            {
                if (sender is RadioButton rbSender)
                {
                    // Fire the RadioButtonChanged event.
                    OnRadioSelectionChanged(rbSender);

                    if (rbSender.Tag == null)
                    {
                        throw new InvalidOperationException("RadioButton " + rbSender.Name +
                                                            " does not have its Tag property set to a valid enum integer value.");
                    }

                    if (!Int32.TryParse(rbSender.Tag.ToString(), out var setting))
                    {
                        throw new InvalidOperationException("RadioButton " + rbSender.Name +
                                                            " does not have its Tag property set to a valid enum integer value.");
                    }

                    PropertyInfo pi = _dataSource.GetType().GetProperty(_valueMember);
                    if (pi != null)
                    {
                        // Convert the int into its corresponding enum. pi.PropertyType represents the enum type.
                        object parsedEnum;
                        try
                        {
                            parsedEnum = Enum.Parse(pi.PropertyType, setting.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Could not convert RadioButton.Tag value into an enum.", ex);
                        }

                        // Stop listening to property changes while we change the property - otherwise, stack overflow.
                        _processPropertyChange = false;

                        pi.SetValue(_dataSource, parsedEnum, null);

                        _processPropertyChange = true;
                    }
                }
            }
        }

        public event EventHandler<RadioSelectionChangedEventArgs> RadioSelectionChanged;

        protected void OnRadioSelectionChanged(RadioButton rbSender)
        {
            RadioSelectionChanged?.Invoke(rbSender, new RadioSelectionChangedEventArgs(rbSender));
        }

        private bool _processPropertyChange = true;

        // Handle PropertyChanged notifications from the source.
        protected void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_processPropertyChange)
            {
                if (e.PropertyName.Equals(_valueMember))
                {
                    SetRadioButtonValue();
                }
            }
        }
    }

    /// <summary>
    /// An EventArgs class for radio button selection changed events. 
    /// </summary>
    public class RadioSelectionChangedEventArgs : EventArgs
    {
        public RadioSelectionChangedEventArgs(RadioButton rb)
        {
            RadioButtonClicked = rb;
        }

        public RadioButton RadioButtonClicked { get; }
    }
}
