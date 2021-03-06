﻿using System;
using System.Drawing;
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
                MessageBox.ShowError(Form, text, Core.Application.NameAndVersion);
            }
        }

        public void AddGraphColorClicked(ColorDialogPresenter dialog)
        {
            if (dialog.ShowDialog(Form) == DialogResult.OK)
            {
                Color addColor = FindNearestKnown(dialog.Color);
                if (!Model.AddGraphColor(addColor))
                {
                    string text = String.Format(CultureInfo.CurrentCulture, "{0} is already a graph color.", addColor.Name);
                    MessageBox.ShowInformation(Form, text, Core.Application.NameAndVersion);
                }
            }
        }

        private static Color FindNearestKnown(Color c)
        {
            var best = new ColorName { Name = null };

            foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
            {
                var known = Color.FromName(colorName);
                int dist = Math.Abs(c.R - known.R) + Math.Abs(c.G - known.G) + Math.Abs(c.B - known.B);

                if (best.Name == null || dist < best.Distance)
                {
                    best.Color = known;
                    best.Name = colorName;
                    best.Distance = dist;
                }
            }

            return best.Color;
        }

        private struct ColorName
        {
            public Color Color { get; set; }
            public string Name { get; set; }
            public int Distance { get; set; }
        }

        public void DeleteGraphColorClicked()
        {
            if (Model.SelectedGraphColorItem is null)
            {
                MessageBox.ShowInformation(Form, "No Color Selected.", Core.Application.NameAndVersion);
                return;
            }

            if (Model.GraphColors.Count <= 3)
            {
                MessageBox.ShowInformation(Form, "Must have at least three colors.", Core.Application.NameAndVersion);
                return;
            }

            Model.DeleteSelectedGraphColor();
        }
    }
}
