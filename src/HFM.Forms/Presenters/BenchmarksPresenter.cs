using System;
using System.Globalization;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public class BenchmarksPresenter : FormPresenter<BenchmarksModel>
    {
        public BenchmarksModel Model { get; }
        public ILogger Logger { get; }
        public MessageBoxPresenter MessageBox { get; }
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkService BenchmarkService { get; }
        public ClientConfiguration ClientConfiguration { get; }

        public BenchmarksPresenter(BenchmarksModel model, ILogger logger, MessageBoxPresenter messageBox,
            IProteinService proteinService, IProteinBenchmarkService benchmarkService, ClientConfiguration clientConfiguration) : base(model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ProteinService = proteinService ?? NullProteinService.Instance;
            BenchmarkService = benchmarkService;
            ClientConfiguration = clientConfiguration;
        }

        protected override IWin32Form OnCreateForm()
        {
            return new BenchmarksForm(this);
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
                string text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, Core.Application.NameAndVersion);
                MessageBox.ShowError(text, Core.Application.NameAndVersion);
            }
        }
    }
}
