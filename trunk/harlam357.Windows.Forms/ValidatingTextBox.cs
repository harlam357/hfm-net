/*
 * Self Validating TextBox Control
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace harlam357.Windows.Forms
{
   public enum ValidationType
   {
      None = 0,
      Empty,
      Custom
   }
   public delegate bool CustomValidationEventHandler(object sender, CustomValidationEventArgs e);

   public partial class ValidatingTextBox : TextBox
   {
      public event CustomValidationEventHandler CustomValidation;

      #region Properties
      private ToolTip _ToolTip = null;
      public ToolTip ToolTip
      {
         get { return _ToolTip; }
         set { _ToolTip = value; }
      }

      private string _ToolTipText = String.Empty;
      public string ToolTipText
      {
         get { return _ToolTipText; }
         set { _ToolTipText = value; }
      }

      private ValidationType _ValidationType = ValidationType.None;
      public ValidationType ValidationType
      {
         get { return _ValidationType; }
         set { _ValidationType = value; }
      }

      private Color _ErrorColor = Color.Yellow;
      public Color ErrorColor
      {
         get { return _ErrorColor; }
         set { _ErrorColor = value; }
      }

      private bool _Error = false;
      public bool Error
      {
         get 
         {
            if (Enabled)
            {
               return _Error;
            }
            return false;
         }
         private set { _Error = value; }
      }
      
      private readonly List<Control> _CompanionControls = new List<Control>();
      public List<Control> CompanionControls
      {
         get { return _CompanionControls; }
      }
      #endregion

      #region Constructor
      public ValidatingTextBox()
      {
         InitializeComponent();

         DataBindings.CollectionChanged += DataBindings_CollectionChanged;
      }
      #endregion

      #region Event Handlers
      private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
      {
         if (Enabled && ReadOnly == false)
         {
            ValidateValue();
         }
      }

      protected override void OnValidating(CancelEventArgs e)
      {
         if (Enabled && ReadOnly == false)
         {
            ValidateValue();
            if (ToolTip != null)
            {
               if (Error && ToolTipText.Length != 0)
               {
                  ToolTip.RemoveAll();
                  ToolTip.Tag = Name;
                  ToolTip.Show(ToolTipText, this, 10, -20, 5000);
               }
               else
               {
                  ToolTip.RemoveAll();
               }
            }
         }
         base.OnValidating(e);
      }

      protected override void OnEnabledChanged(EventArgs e)
      {
         ReadOnly = !Enabled;
         CausesValidation = Enabled;
         
         if (Enabled)
         {
            ValidateValue();
         }
         else
         {
            if (ToolTip != null && ToolTip.Tag != null)
            {
               if (ToolTip.Tag.Equals(Name))
               {
                  ToolTip.RemoveAll();
               }
            }
         }
         base.OnEnabledChanged(e);
      }

      protected override void OnReadOnlyChanged(EventArgs e)
      {
         if (ReadOnly)
         {
            BackColor = SystemColors.Control;
         }
         else
         {
            BackColor = SystemColors.Window;
         }
         base.OnReadOnlyChanged(e);
      }
      #endregion

      #region Methods
      public void ValidateValue()
      {
         Color NewColor = SystemColors.Window;
         bool NewError = false;
         if (DoValidation() == false)
         {
            NewColor = ErrorColor;
            NewError = true;
         }

         BackColor = NewColor;
         Error = NewError;
         foreach (Control ctrl in CompanionControls)
         {
            ctrl.BackColor = NewColor;
            ValidatingTextBox txt = ctrl as ValidatingTextBox;
            if (txt != null)
            {
               txt.Error = NewError;
            }
         }
      }

      private bool DoValidation()
      {
         switch (ValidationType)
         {
            case ValidationType.None:
               return true;
            case ValidationType.Empty:
               if (Text.Length != 0)
               {
                  return true;
               }
               return false;
            case ValidationType.Custom:
               if (CustomValidation == null)
               {
                  // Revert to behavior of ValidationType.Empty
                  if (Text.Length != 0)
                  {
                     return true;
                  }
                  return false;
               }
               CustomValidationEventArgs e = new CustomValidationEventArgs(Text, ToolTipText);
               bool result = CustomValidation(this, e);
               Text = e.Text;
               ToolTipText = e.Message;
               return result;
            default:
               throw new NotImplementedException();
         }
      }
      #endregion
   }

   public class CustomValidationEventArgs : EventArgs
   {
      private string _Text;
      public string Text 
      { 
         get { return _Text; }
         set { _Text = value; }
      }

      private string _Message;
      public string Message
      {
         get { return _Message; }
         set { _Message = value; }
      }

      public CustomValidationEventArgs(string TextValue, string MessageValue)
      {
         Text = TextValue;
         Message = MessageValue;
      }
   }
}
