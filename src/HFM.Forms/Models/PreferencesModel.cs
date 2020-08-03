
using System;
using System.Collections.Generic;
using System.Linq;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class PreferencesModel : ViewModelBase
    {
        public PreferencesModel(IPreferenceSet preferences, IAutoRunConfiguration autoRunConfiguration)
        {
            Preferences = preferences;
            ClientsModel = new ClientsModel(preferences);
            OptionsModel = new OptionsModel(preferences, autoRunConfiguration);
            WebGenerationModel = new WebGenerationModel(preferences);
            WebVisualStylesModel = new WebVisualStylesModel(preferences);
            ReportingModel = new ReportingModel(preferences);
            WebProxyModel = new WebProxyModel(preferences);
        }

        public IPreferenceSet Preferences { get; }
        public ClientsModel ClientsModel { get; set; }
        public OptionsModel OptionsModel { get; set; }
        public WebGenerationModel WebGenerationModel { get; set; }
        public WebVisualStylesModel WebVisualStylesModel { get; set; }
        public ReportingModel ReportingModel { get; set; }
        public WebProxyModel WebProxyModel { get; set; }

        public override string Error
        {
            get
            {
                var errors = EnumerateErrors().Where(x => !String.IsNullOrEmpty(x));
                return String.Join(Environment.NewLine, errors);
            }
        }

        private IEnumerable<string> EnumerateErrors()
        {
            yield return ClientsModel.Error;
            yield return OptionsModel.Error;
            yield return WebGenerationModel.Error;
            yield return WebVisualStylesModel.Error;
            yield return ReportingModel.Error;
            yield return WebProxyModel.Error;
        }

        public override bool ValidateAcceptance()
        {
            OnPropertyChanged(String.Empty);
            ClientsModel.ValidateAcceptance();
            OptionsModel.ValidateAcceptance();
            WebGenerationModel.ValidateAcceptance();
            WebVisualStylesModel.ValidateAcceptance();
            ReportingModel.ValidateAcceptance();
            WebProxyModel.ValidateAcceptance();
            return !HasError;
        }

        public override void Load()
        {
            ClientsModel.Load();
            OptionsModel.Load();
            WebGenerationModel.Load();
            WebVisualStylesModel.Load();
            ReportingModel.Load();
            WebProxyModel.Load();
        }

        public override void Save()
        {
            ClientsModel.Save();
            OptionsModel.Save();
            WebGenerationModel.Save();
            WebVisualStylesModel.Save();
            ReportingModel.Save();
            WebProxyModel.Save();

            Preferences.Save();
            OptionsModel.SaveAutoRunConfiguration();
        }
    }
}
