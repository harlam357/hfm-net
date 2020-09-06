/*
 * View and DataGridView save state code based on code by Ron Dunant.
 * http://www.codeproject.com/KB/grid/PersistentDataGridView.aspx
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.ScheduledTasks;
using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms
{
    public sealed class MainPresenter : FormPresenter<MainModel>
    {
        public MainModel Model { get; }
        public ILogger Logger { get; }
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public MessageBoxPresenter MessageBox { get; }
        public ClientConfiguration ClientConfiguration { get; }
        public IProteinService ProteinService { get; }
        public UserStatsDataModel UserStatsDataModel { get; }
        private IPreferences Preferences { get; }
        public MainGridModel GridModel { get; }

        private readonly ClientSettingsManager _settingsManager;

        public MainPresenter(MainModel model, ILogger logger, IServiceScopeFactory serviceScopeFactory, MessageBoxPresenter messageBox,
                             ClientConfiguration clientConfiguration, IProteinService proteinService, EocStatsScheduledTask eocStatsScheduledTask)
            : base(model)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            ServiceScopeFactory = serviceScopeFactory;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ClientConfiguration = clientConfiguration;
            ProteinService = proteinService ?? NullProteinService.Instance;

            UserStatsDataModel = new UserStatsDataModel(Form, Model.Preferences, eocStatsScheduledTask);
            Preferences = Model.Preferences;
            GridModel = new MainGridModel(Model.Preferences, Form, clientConfiguration);
            GridModel.Load();

            GridModel.AfterResetBindings += (s, e) =>
            {
                // run asynchronously so binding operation can finish
                Form.BeginInvoke(new Action(() => Model.GridModelSelectedSlotChanged(s, e)), null);
            };
            GridModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MainGridModel.SelectedSlot):
                        // run asynchronously so binding operation can finish
                        Form.BeginInvoke(new Action(() => Model.GridModelSelectedSlotChanged(s, e)), null);
                        break;
                }
            };

            _settingsManager = new ClientSettingsManager();

            ClientConfiguration.ClientConfigurationChanged += (s, e) => AutoSaveConfig();
        }

        private IMainView _view;

        public override IWin32Form Form
        {
            get
            {
                if (base.Form is null)
                {
                    ModelBase.Load();

                    base.Form = OnCreateForm();
                    base.Form.Closed += OnClosed;

                    _view = (IMainView)base.Form;
                }
                return base.Form;
            }
            protected set => throw new InvalidOperationException("Use MainForm property.");
        }

        public override void Show() => throw new InvalidOperationException("Use MainForm property.");

        protected override IWin32Form OnCreateForm() => new MainForm(this);

        private string _openFile;

        public void Initialize(string openFile)
        {
            _openFile = openFile;
        }

        #region View Handling Methods

        public void ViewShown()
        {
            if (Preferences.Get<bool>(Preference.RunMinimized))
            {
                Form.WindowState = FormWindowState.Minimized;
            }

            if (!String.IsNullOrEmpty(_openFile))
            {
                LoadConfigFile(_openFile);
            }
            else if (Preferences.Get<bool>(Preference.UseDefaultConfigFile))
            {
                var fileName = Preferences.Get<string>(Preference.DefaultConfigFile);
                if (!String.IsNullOrEmpty(fileName))
                {
                    LoadConfigFile(fileName);
                }
            }
        }

        public void CheckForUpdateOnStartup(IApplicationUpdateService service)
        {
            if (Preferences.Get<bool>(Preference.StartupCheckForUpdate))
            {
                CheckForUpdateInternal(service);
            }
        }

        private void CheckForUpdate(IApplicationUpdateService service)
        {
            var result = CheckForUpdateInternal(service);
            if (result.HasValue && !result.Value)
            {
                string text = $"{Core.Application.NameAndVersion} is already up-to-date.";
                MessageBox.ShowInformation(Form, text, Core.Application.NameAndVersion);
            }
        }

        private readonly object _checkForUpdateLock = new object();
        private ApplicationUpdateModel _applicationUpdateModel;

        private bool? CheckForUpdateInternal(IApplicationUpdateService service)
        {
            if (!Monitor.TryEnter(_checkForUpdateLock))
            {
                return null;
            }
            try
            {
                string url = Properties.Settings.Default.UpdateUrl;
                var update = service.GetApplicationUpdate(url);

                if (update is null) return false;
                if (!update.VersionIsGreaterThan(Core.Application.VersionNumber)) return false;

                _applicationUpdateModel = new ApplicationUpdateModel(update);
                using (var presenter = new ApplicationUpdatePresenter(_applicationUpdateModel, Logger, Preferences, MessageBox))
                {
                    if (presenter.ShowDialog(Form) == DialogResult.OK)
                    {
                        if (_applicationUpdateModel.SelectedUpdateFileIsReadyToBeExecuted)
                        {
                            string text = String.Format(CultureInfo.CurrentCulture,
                                "{0} will install the new version when you exit the application.", Core.Application.Name);
                            MessageBox.ShowInformation(Form, text, Core.Application.NameAndVersion);
                        }
                    }
                }
                return true;
            }
            finally
            {
                Monitor.Exit(_checkForUpdateLock);
            }
        }

        public bool ViewClosing()
        {
            if (!CheckForConfigurationChanges())
            {
                return true;
            }

            CheckForAndFireUpdateProcess(_applicationUpdateModel);

            return false;
        }

        private void CheckForAndFireUpdateProcess(ApplicationUpdateModel update)
        {
            if (update != null && update.SelectedUpdateFileIsReadyToBeExecuted)
            {
                string path = update.SelectedUpdateFileLocalFilePath;
                Logger.Info($"Firing update file '{path}'...");
                try
                {
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    string message = String.Format(CultureInfo.CurrentCulture,
                        "Update process failed to start with the following error:{0}{0}{1}", Environment.NewLine, ex.Message);
                    MessageBox.ShowError(Form, message, Core.Application.NameAndVersion);
                }
            }
        }

        #endregion

        #region Data Grid View Handling Methods

        public void DataGridViewSorted()
        {
            GridModel.ResetSelectedSlot();
        }

        #endregion

        #region File Handling Methods

        public void FileNewClick()
        {
            if (CheckForConfigurationChanges())
            {
                ClearConfiguration();
            }
        }

        public void FileOpenClick(FileDialogPresenter openFile)
        {
            if (CheckForConfigurationChanges())
            {
                openFile.DefaultExt = _settingsManager.FileExtension;
                openFile.Filter = _settingsManager.FileTypeFilters;
                openFile.FileName = _settingsManager.FileName;
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    ClearConfiguration();
                    LoadConfigFile(openFile.FileName, openFile.FilterIndex);
                }
            }
        }

        private void ClearConfiguration()
        {
            // clear the clients and UI
            _settingsManager.ClearFileName();
            ClientConfiguration.Clear();
        }

        private void LoadConfigFile(string filePath, int filterIndex = 1)
        {
            Debug.Assert(filePath != null);

            try
            {
                // Read the config file
                ClientConfiguration.Load(_settingsManager.Read(filePath, filterIndex));

                if (ClientConfiguration.Count == 0)
                {
                    MessageBox.ShowError(Form, "No client configurations were loaded from the given config file.", Core.Application.NameAndVersion);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, String.Format(CultureInfo.CurrentCulture,
                   "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
            }
        }

        private void AutoSaveConfig()
        {
            if (Preferences.Get<bool>(Preference.AutoSaveConfig) &&
                ClientConfiguration.IsDirty)
            {
                FileSaveClick();
            }
        }

        public void FileSaveClick()
        {
            if (ClientConfiguration.Count == 0)
            {
                return;
            }

            if (String.IsNullOrEmpty(_settingsManager.FileName))
            {
                // TODO: Fix dependency on DefaultFileDialogPresenter
                using (var saveFile = DefaultFileDialogPresenter.SaveFile())
                {
                    FileSaveAsClick(saveFile);
                }
            }
            else
            {
                WriteClientConfiguration(_settingsManager.FileName, _settingsManager.FilterIndex);
            }
        }

        public void FileSaveAsClick(FileDialogPresenter saveFile)
        {
            if (ClientConfiguration.Count == 0)
            {
                return;
            }

            saveFile.DefaultExt = _settingsManager.FileExtension;
            saveFile.Filter = _settingsManager.FileTypeFilters;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                WriteClientConfiguration(saveFile.FileName, saveFile.FilterIndex);
            }
        }

        private void WriteClientConfiguration(string fileName, int filterIndex)
        {
            try
            {
                var clients = ClientConfiguration.GetClients();
                _settingsManager.Write(clients.Select(x => x.Settings), fileName, filterIndex);
                ClientConfiguration.IsDirty = false;

                ApplyClientIdentifierToBenchmarks(clients);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, String.Format(CultureInfo.CurrentCulture,
                    "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
            }
        }

        private static void ApplyClientIdentifierToBenchmarks(ICollection<IClient> clients)
        {
            var benchmarkService = clients.FirstOrDefault()?.BenchmarkService;
            if (benchmarkService is null)
            {
                return;
            }

            foreach (var benchmarkClientIdentifier in benchmarkService.GetClientIdentifiers())
            {
                var clientIdentifier = clients.Select(x => (ClientIdentifier?)x.Settings.ClientIdentifier)
                    .FirstOrDefault(x => x.Value.Equals(benchmarkClientIdentifier) ||
                                         ClientIdentifier.ProteinBenchmarkEqualityComparer.Equals(x.Value, benchmarkClientIdentifier));

                if (clientIdentifier.HasValue)
                {
                    benchmarkService.UpdateClientIdentifier(clientIdentifier.Value);
                }
            }
        }

        private bool CheckForConfigurationChanges()
        {
            if (ClientConfiguration.Count != 0 && ClientConfiguration.IsDirty)
            {
                DialogResult result = MessageBox.AskYesNoCancelQuestion(Form,
                   String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes{0}No - Continue and do not save the changes{0}Cancel - Do not continue", Environment.NewLine),
                   Core.Application.NameAndVersion);

                switch (result)
                {
                    case DialogResult.Yes:
                        FileSaveClick();
                        return !ClientConfiguration.IsDirty;
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
                return false;
            }

            return true;
        }

        #endregion

        #region Edit Menu Handling Methods

        public void EditPreferencesClick()
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                using (var presenter = scope.ServiceProvider.GetRequiredService<PreferencesPresenter>())
                {
                    presenter.ShowDialog(Form);
                    // TODO: Invalidate View by mutating the Model
                    _view.DataGridView.Invalidate();
                }
            }
        }

        #endregion

        #region Help Menu Handling Methods

        public void ShowHfmLogFile(LocalProcessService localProcess)
        {
            string path = Path.Combine(Preferences.Get<string>(Preference.ApplicationDataFolderPath), Core.Logging.Logger.LogFileName);
            string errorMessage = String.Format(CultureInfo.CurrentCulture,
                "An error occurred while attempting to open the HFM log file.{0}{0}Please check the log file viewer defined in the preferences.",
                Environment.NewLine);

            string fileName = Preferences.Get<string>(Preference.LogFileViewer);
            string arguments = WrapString.InQuotes(path);
            localProcess.StartAndNotifyError(fileName, arguments, errorMessage, Logger, MessageBox);
        }

        public void ShowHfmDataFiles(LocalProcessService localProcess)
        {
            string path = Preferences.Get<string>(Preference.ApplicationDataFolderPath);
            string errorMessage = String.Format(CultureInfo.CurrentCulture,
                "An error occurred while attempting to open '{0}'.{1}{1}Please check the current file explorer defined in the preferences.",
                path, Environment.NewLine);

            string fileName = Preferences.Get<string>(Preference.FileExplorer);
            string arguments = WrapString.InQuotes(path);
            localProcess.StartAndNotifyError(fileName, arguments, errorMessage, Logger, MessageBox);
        }

        public void ShowHfmGoogleGroup(LocalProcessService localProcess)
        {
            const string errorMessage = "An error occurred while attempting to open the HFM.NET Google Group.";
            localProcess.StartAndNotifyError(Core.Application.SupportForumUrl, errorMessage, Logger, MessageBox);
        }

        public void CheckForUpdateClick(IApplicationUpdateService service)
        {
            CheckForUpdate(service);
        }

        #endregion

        #region Clients Menu Handling Methods

        internal void ClientsAddClick()
        {
            using (var dialog = new FahClientSettingsPresenter(new FahClientSettingsModel(), Logger, MessageBox))
            {
                while (dialog.ShowDialog(Form) == DialogResult.OK)
                {
                    dialog.Model.Save();
                    var settings = dialog.Model.ClientSettings;
                    //if (_clientDictionary.ContainsKey(settings.Name))
                    //{
                    //   string message = String.Format(CultureInfo.CurrentCulture, "Client name '{0}' already exists.", settings.Name);
                    //   _messageBoxView.ShowError(_view, Core.Application.NameAndVersion, message);
                    //   continue;
                    //}
                    // perform the add
                    try
                    {
                        ClientConfiguration.Add(settings);
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Logger.Error(ex.Message, ex);
                        MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
                    }
                }
            }
        }

        public void ClientsEditClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (GridModel.SelectedSlot == null) return;

            EditFahClient();
        }

        private void EditFahClient()
        {
            Debug.Assert(GridModel.SelectedSlot != null);
            IClient client = ClientConfiguration.Get(GridModel.SelectedSlot.Settings.Name);
            ClientSettings originalSettings = client.Settings;
            Debug.Assert(originalSettings.ClientType == ClientType.FahClient);

            var model = new FahClientSettingsModel(originalSettings);
            model.Load();
            using (var dialog = new FahClientSettingsPresenter(model, Logger, MessageBox))
            {
                while (dialog.ShowDialog(Form) == DialogResult.OK)
                {
                    dialog.Model.Save();
                    var newSettings = dialog.Model.ClientSettings;
                    // perform the edit
                    try
                    {
                        ClientConfiguration.Edit(originalSettings.Name, newSettings);
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Logger.Error(ex.Message, ex);
                        MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
                    }
                }
            }
        }

        public void ClientsDeleteClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (GridModel.SelectedSlot == null) return;

            ClientConfiguration.Remove(GridModel.SelectedSlot.Settings.Name);
        }

        public void ClientsRefreshSelectedClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (GridModel.SelectedSlot == null) return;

            var client = ClientConfiguration.Get(GridModel.SelectedSlot.Settings.Name);
            Task.Run(client.Retrieve);
        }

        public void ClientsRefreshAllClick()
        {
            ClientConfiguration.ScheduledTasks.RetrieveAll();
        }

        public void ClientsViewCachedLogClick(LocalProcessService localProcess)
        {
            // Check for SelectedSlot, and get out if not found
            if (GridModel.SelectedSlot == null) return;

            string path = Path.Combine(Preferences.Get<string>(Preference.CacheDirectory), GridModel.SelectedSlot.Settings.ClientLogFileName);
            if (File.Exists(path))
            {
                string errorMessage = String.Format(CultureInfo.CurrentCulture,
                    "An error occurred while attempting to open the client log file.{0}{0}Please check the current log file viewer defined in the preferences.",
                    Environment.NewLine);

                string fileName = Preferences.Get<string>(Preference.LogFileViewer);
                string arguments = WrapString.InQuotes(path);
                localProcess.StartAndNotifyError(fileName, arguments, errorMessage, Logger, MessageBox);
            }
            else
            {
                string message = String.Format(CultureInfo.CurrentCulture, "The log file for '{0}' does not exist.", GridModel.SelectedSlot.Settings.Name);
                MessageBox.ShowInformation(Form, message, Core.Application.NameAndVersion);
            }
        }

        #endregion

        #region Grid Context Menu Handling Methods

        internal void ClientsFoldSlotClick()
        {
            if (GridModel.SelectedSlot == null) return;

            if (ClientConfiguration.Get(GridModel.SelectedSlot.Settings.Name) is IFahClient client)
            {
                client.Fold(GridModel.SelectedSlot.SlotID);
            }
        }

        internal void ClientsPauseSlotClick()
        {
            if (GridModel.SelectedSlot == null) return;

            if (ClientConfiguration.Get(GridModel.SelectedSlot.Settings.Name) is IFahClient client)
            {
                client.Pause(GridModel.SelectedSlot.SlotID);
            }
        }

        internal void ClientsFinishSlotClick()
        {
            if (GridModel.SelectedSlot == null) return;

            if (ClientConfiguration.Get(GridModel.SelectedSlot.Settings.Name) is IFahClient client)
            {
                client.Finish(GridModel.SelectedSlot.SlotID);
            }
        }

        public void CopyPRCGToClipboardClicked()
        {
            if (GridModel.SelectedSlot == null) return;

            string projectString = GridModel.SelectedSlot.WorkUnitModel.WorkUnit.ToProjectString();

            // TODO: Replace ClipboardWrapper.SetText() with abstraction
            ClipboardWrapper.SetText(projectString);
        }

        #endregion

        #region View Menu Handling Methods

        private MessagesPresenter _messagesPresenter;

        public void ViewMessagesClick()
        {
            try
            {
                if (_messagesPresenter is null)
                {
                    var scope = ServiceScopeFactory.CreateScope();
                    _messagesPresenter = scope.ServiceProvider.GetRequiredService<MessagesPresenter>();
                    _messagesPresenter.Closed += (sender, args) =>
                    {
                        scope.Dispose();
                        _messagesPresenter = null;
                    };
                }

                _messagesPresenter?.Show();
            }
            catch (Exception)
            {
                _messagesPresenter?.Dispose();
                _messagesPresenter = null;
                throw;
            }
        }

        public void ShowHideLogWindow()
        {
            Model.FormLogWindowVisible = !Model.FormLogWindowVisible;
        }

        public void ShowHideQueue()
        {
            Model.QueueWindowVisible = !Model.QueueWindowVisible;
        }

        public void ViewToggleDateTimeClick()
        {
            var style = Preferences.Get<TimeFormatting>(Preference.TimeFormatting);
            Preferences.Set(Preference.TimeFormatting, style == TimeFormatting.None
                                    ? TimeFormatting.Format1
                                    : TimeFormatting.None);
            Preferences.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewToggleCompletedCountStyleClick()
        {
            var style = Preferences.Get<UnitTotalsType>(Preference.UnitTotals);
            Preferences.Set(Preference.UnitTotals, style == UnitTotalsType.All
                                    ? UnitTotalsType.ClientStart
                                    : UnitTotalsType.All);
            Preferences.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewToggleVersionInformationClick()
        {
            Preferences.Set(Preference.DisplayVersions, !Preferences.Get<bool>(Preference.DisplayVersions));
            Preferences.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewCycleBonusCalculationClick()
        {
            var calculationType = Preferences.Get<BonusCalculation>(Preference.BonusCalculation);
            int typeIndex = 0;
            // None is always LAST entry
            if (calculationType != BonusCalculation.None)
            {
                typeIndex = (int)calculationType;
                typeIndex++;
            }

            calculationType = (BonusCalculation)typeIndex;
            Preferences.Set(Preference.BonusCalculation, calculationType);
            Preferences.Save();

            _view.DataGridView.Invalidate();
        }

        public void ViewCycleCalculationClick()
        {
            var calculationType = Preferences.Get<PPDCalculation>(Preference.PPDCalculation);
            int typeIndex = 0;
            // EffectiveRate is always LAST entry
            if (calculationType != PPDCalculation.EffectiveRate)
            {
                typeIndex = (int)calculationType;
                typeIndex++;
            }

            calculationType = (PPDCalculation)typeIndex;
            Preferences.Set(Preference.PPDCalculation, calculationType);
            Preferences.Save();

            _view.DataGridView.Invalidate();
        }

        #endregion

        #region Tools Menu Handling Methods

        public void ToolsDownloadProjectsClick(IProteinService proteinService)
        {
            try
            {
                IEnumerable<ProteinChange> result = null;
                using (var dialog = new ProgressDialog((progress, token) => result = proteinService.Refresh(progress), false))
                {
                    dialog.Text = Core.Application.NameAndVersion;
                    dialog.ShowDialog(Form);
                    if (dialog.Exception != null)
                    {
                        Logger.Error(dialog.Exception.Message, dialog.Exception);
                        MessageBox.ShowError(dialog.Exception.Message, Core.Application.NameAndVersion);
                    }
                }

                if (result != null)
                {
                    var proteinChanges = result.Where(x => x.Action != ProteinChangeAction.None).ToList();
                    if (proteinChanges.Count > 0)
                    {
                        if (ClientConfiguration.Count > 0)
                        {
                            ClientConfiguration.ScheduledTasks.RetrieveAll();
                        }
                        using (var dialog = new ProteinChangesDialog(proteinChanges))
                        {
                            dialog.ShowDialog(Form);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.ShowError(ex.Message, Core.Application.NameAndVersion);
            }
        }

        public void ToolsBenchmarksClick()
        {
            int projectID = 0;

            // Check for SelectedSlot, and if found... load its ProjectID.
            if (GridModel.SelectedSlot != null)
            {
                projectID = GridModel.SelectedSlot.WorkUnitModel.WorkUnit.ProjectID;
            }

            var scope = ServiceScopeFactory.CreateScope();
            var presenter = scope.ServiceProvider.GetRequiredService<BenchmarksPresenter>();
            presenter.Closed += (s, e) => scope.Dispose();
            presenter.Model.DefaultProjectID = projectID;
            presenter.Show();
        }

        private IFormPresenter _historyPresenter;

        public void ToolsHistoryClick()
        {
            try
            {
                if (_historyPresenter is null)
                {
                    var scope = ServiceScopeFactory.CreateScope();
                    _historyPresenter = scope.ServiceProvider.GetRequiredService<WorkUnitHistoryPresenter>();
                    _historyPresenter.Closed += (sender, args) =>
                    {
                        scope.Dispose();
                        _historyPresenter = null;
                    };
                }

                _historyPresenter?.Show();
            }
            catch (Exception)
            {
                _historyPresenter?.Dispose();
                _historyPresenter = null;
                throw;
            }
        }

        internal void ToolsPointsCalculatorClick()
        {
            var scope = ServiceScopeFactory.CreateScope();
            IWin32Form calculatorForm = scope.ServiceProvider.GetRequiredService<ProteinCalculatorForm>();
            calculatorForm.Closed += (s, e) => scope.Dispose();
            calculatorForm.Show();
        }

        #endregion

        #region Web Menu Handling Methods

        public void ShowEocUserPage(LocalProcessService localProcess)
        {
            string fileName = new Uri(String.Concat(EocStatsService.UserBaseUrl, Preferences.Get<int>(Preference.EocUserId))).AbsoluteUri;
            const string errorMessage = "An error occurred while attempting to open the EOC user stats page.";
            localProcess.StartAndNotifyError(fileName, errorMessage, Logger, MessageBox);
        }

        public void ShowStanfordUserPage(LocalProcessService localProcess)
        {
            string fileName = new Uri(String.Concat(FahUrl.UserBaseUrl, Preferences.Get<string>(Preference.StanfordId))).AbsoluteUri;
            const string errorMessage = "An error occurred while attempting to open the FAH user stats page.";
            localProcess.StartAndNotifyError(fileName, errorMessage, Logger, MessageBox);
        }

        public void ShowEocTeamPage(LocalProcessService localProcess)
        {
            string fileName = new Uri(String.Concat(EocStatsService.TeamBaseUrl, Preferences.Get<int>(Preference.TeamId))).AbsoluteUri;
            const string errorMessage = "An error occurred while attempting to open the EOC team stats page.";
            localProcess.StartAndNotifyError(fileName, errorMessage, Logger, MessageBox);
        }

        public void RefreshUserStatsData()
        {
            UserStatsDataModel.Refresh();
        }

        public void ShowHfmGitHub(LocalProcessService localProcess)
        {
            const string errorMessage = "An error occurred while attempting to open the HFM.NET GitHub project.";
            localProcess.StartAndNotifyError(Core.Application.ProjectSiteUrl, errorMessage, Logger, MessageBox);
        }

        #endregion

        #region System Tray Icon Handling Methods

        public void NotifyIconDoubleClick()
        {
            if (Form.WindowState == FormWindowState.Minimized)
            {
                Form.WindowState = Model.OriginalWindowState;
            }
            else
            {
                Form.WindowState = FormWindowState.Minimized;
            }
        }

        public void NotifyIconRestoreClick()
        {
            if (Form.WindowState == FormWindowState.Minimized)
            {
                Form.WindowState = Model.OriginalWindowState;
            }
            else if (Form.WindowState == FormWindowState.Maximized)
            {
                Form.WindowState = FormWindowState.Normal;
            }
        }

        public void NotifyIconMinimizeClick()
        {
            if (Form.WindowState != FormWindowState.Minimized)
            {
                Form.WindowState = FormWindowState.Minimized;
            }
        }

        public void NotifyIconMaximizeClick()
        {
            if (Form.WindowState != FormWindowState.Maximized)
            {
                Form.WindowState = FormWindowState.Maximized;
            }
        }

        #endregion

        public void AboutClicked()
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                using (var dialog = scope.ServiceProvider.GetRequiredService<AboutDialog>())
                {
                    dialog.ShowDialog(Form);
                }
            }
        }

        #region Other Handling Methods

        public void SetUserStatsDataViewStyle(bool showTeamStats)
        {
            UserStatsDataModel.SetViewStyle(showTeamStats);
        }

        #endregion
    }
}
