using System;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public class BenchmarksPresenter : FormPresenter<BenchmarksModel>
    {
        public BenchmarksModel Model { get; }
        public ILogger Logger { get; }
        public MessageBoxPresenter MessageBox { get; }

        public BenchmarksPresenter(BenchmarksModel model, ILogger logger, MessageBoxPresenter messageBox) : base(model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        protected override IWin32Form OnCreateForm()
        {
            return new BenchmarksForm(this);
        }

        public void DeleteSlotClicked()
        {
            string text = String.Format(CultureInfo.CurrentCulture,
                "Are you sure you want to delete {0}?", Model.SelectedSlotIdentifier.Value);

            if (MessageBox.AskYesNoQuestion(Form, text, Core.Application.NameAndVersion) == DialogResult.Yes)
            {
                Model.RemoveSlot(Model.SelectedSlotIdentifier.Value);
            }
        }

        public void DeleteProjectClicked()
        {
            string text = String.Format(CultureInfo.CurrentCulture,
                "Are you sure you want to delete {0} - Project {1}?", Model.SelectedSlotIdentifier.Value, Model.SelectedSlotProject.Value);

            if (MessageBox.AskYesNoQuestion(Form, text, Core.Application.NameAndVersion) == DialogResult.Yes)
            {
                Model.RemoveProject(Model.SelectedSlotIdentifier.Value, Model.SelectedSlotProject.Value);
            }
        }

        public void RefreshMinimumFrameTimeClicked()
        {
            string text = String.Format(CultureInfo.CurrentCulture,
                "Are you sure you want to refresh {0} - Project {1} minimum frame time?", Model.SelectedSlotIdentifier.Value, Model.SelectedSlotProject.Value);

            if (MessageBox.AskYesNoQuestion(Form, text, Core.Application.NameAndVersion) == DialogResult.Yes)
            {
                Model.BenchmarkService.UpdateMinimumFrameTime(Model.SelectedSlotIdentifier.Value, Model.SelectedSlotProject.Value);
                Model.RunReports();
            }
        }

        public void DescriptionLinkClicked(LocalProcessService localProcess)
        {
            try
            {
                localProcess.Start(Model.DescriptionUrl);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                string text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "description");
                MessageBox.ShowError(text, Core.Application.NameAndVersion);
            }
        }
    }
}
