
using System;
using System.Globalization;
using System.Windows.Forms;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Forms.Models;

namespace HFM.Forms
{
    public class FahClientSettingsPresenter : IDisposable
    {
        public ILogger Logger { get; }
        public FahClientSettingsModel Model { get; }
        public MessageBoxPresenter MessageBox { get; }

        public FahClientSettingsPresenter(ILogger logger, FahClientSettingsModel model, MessageBoxPresenter messageBox)
        {
            Logger = logger ?? NullLogger.Instance;
            Model = model;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        public IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            ConnectIfModelHasNoError();

            Dialog = new FahClientSettingsDialog(this);
            return Dialog.ShowDialog(owner);
        }

        protected void ConnectIfModelHasNoError()
        {
            if (!Model.Error)
            {
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }
        }

        public void ConnectClicked()
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Dialog, ex.Message, Core.Application.NameAndVersion);
            }
        }

        public FahClientConnection Connection { get; private set; }

        private void Connect()
        {
            Connection?.Dispose();
            Connection = Model.CreateConnection();
            Connection.Open();
            if (!String.IsNullOrWhiteSpace(Model.Password))
            {
                Connection.CreateCommand("auth " + Model.Password).Execute();
            }

            Model.ConnectEnabled = !Connection.Connected;
            if (Connection.Connected)
            {
                Connection.CreateCommand("slot-info").Execute();
                var reader = Connection.CreateReader();
                if (reader.Read())
                {
                    var slotCollection = SlotCollection.Load(reader.Message.MessageText);
                    foreach (var slot in slotCollection)
                    {
                        Connection.CreateCommand(String.Format(CultureInfo.InvariantCulture, FahClientMessages.DefaultSlotOptions, slot.ID)).Execute();
                        if (reader.Read())
                        {
                            var slotOptions = SlotOptions.Load(reader.Message.MessageText);
                            if (slotOptions[Options.MachineID] != null)
                            {
                                slot.SlotOptions = slotOptions;
                            }
                        }
                    }

                    Model.RefreshSlots(slotCollection);
                }
            }
        }

        public void OKClicked()
        {
            if (Model.ValidateAcceptance())
            {
                Dialog.DialogResult = DialogResult.OK;
                Dialog.Close();
            }
            else
            {
                MessageBox.ShowError(Dialog,
                    "There are validation errors.  Please correct the yellow highlighted fields.",
                    Core.Application.NameAndVersion);
            }
        }

        public void Dispose()
        {
            Dialog?.Dispose();
            Connection?.Dispose();
        }
    }
}
