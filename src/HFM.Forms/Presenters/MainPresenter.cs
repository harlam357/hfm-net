using System.Diagnostics;
using System.Globalization;

using HFM.Core;
using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.ScheduledTasks;
using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Services;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Presenters
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
        public SlotCollectionModel SlotsModel { get; }

        private readonly ClientSettingsManager _settingsManager;

        public MainPresenter(MainModel model,
                             ILogger logger,
                             IServiceScopeFactory serviceScopeFactory,
                             MessageBoxPresenter messageBox,
                             ClientConfiguration clientConfiguration,
                             IProteinService proteinService,
                             EocStatsScheduledTask eocStatsScheduledTask)
            : base(model)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            ServiceScopeFactory = serviceScopeFactory ?? NullServiceScopeFactory.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ClientConfiguration = clientConfiguration;
            ProteinService = proteinService ?? NullProteinService.Instance;

            UserStatsDataModel = new UserStatsDataModel(Form, Model.Preferences, eocStatsScheduledTask);
            Preferences = Model.Preferences;
            SlotsModel = new SlotCollectionModel(Form, Model.Preferences, clientConfiguration);
            SlotsModel.Load();

            SlotsModel.Reset += (s, e) =>
            {
                // run asynchronously so binding operation can finish
                Form.BeginInvoke(new Action(() =>
                {
                    Model.GridModelSelectedSlotChanged(e.SelectedSlot);
                    Model.GridModelSlotTotalsChanged(e.SlotTotals);
                }), null);
            };
            SlotsModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SlotCollectionModel.SelectedSlot):
                        // Create a local reference before handing off to BeginInvoke.
                        // This ensures that the BeginInvoke action uses the state of SlotsModel properties available now,
                        // not the state of SlotsModel properties when the BeginInvoke action is executed (at a later time).
                        var selectedSlot = SlotsModel.SelectedSlot;
                        // run asynchronously so binding operation can finish
                        Form.BeginInvoke(new Action(() => Model.GridModelSelectedSlotChanged(selectedSlot)), null);
                        break;
                }
            };

            _settingsManager = new ClientSettingsManager(Logger);

            ClientConfiguration.ClientConfigurationChanged += (s, e) => AutoSaveConfig();
        }

        public override IWin32Form Form
        {
            get
            {
                if (base.Form is null)
                {
                    ModelBase.Load();

                    base.Form = OnCreateForm();
                    base.Form.Closed += OnClosed;
                }
                return base.Form;
            }
        }

        public override void Show() => throw new InvalidOperationException("Use Form property.");

        protected override IWin32Form OnCreateForm() => new MainForm(this);

        public string ConfigFilePathFromArguments { get; set; }

        // View Handling Methods
        public void FormShown()
        {
            if (Preferences.Get<bool>(Preference.RunMinimized))
            {
                Form.WindowState = FormWindowState.Minimized;
            }

            if (!String.IsNullOrEmpty(ConfigFilePathFromArguments))
            {
                LoadConfigFile(ConfigFilePathFromArguments);
            }
            else if (Preferences.Get<bool>(Preference.UseDefaultConfigFile))
            {
                var filePath = Preferences.Get<string>(Preference.DefaultConfigFile);
                if (!String.IsNullOrEmpty(filePath))
                {
                    LoadConfigFile(filePath);
                }
            }
        }

        public void CheckForUpdateOnStartup(IApplicationUpdateService service, ApplicationUpdatePresenterFactory presenterFactory)
        {
            if (Preferences.Get<bool>(Preference.StartupCheckForUpdate))
            {
                CheckForUpdateInternal(service, presenterFactory);
            }
        }

        private void CheckForUpdate(IApplicationUpdateService service, ApplicationUpdatePresenterFactory presenterFactory)
        {
            var result = CheckForUpdateInternal(service, presenterFactory);
            if (result.HasValue && !result.Value)
            {
                string text = $"{Core.Application.NameAndVersion} is already up-to-date.";
                MessageBox.ShowInformation(Form, text, Core.Application.NameAndVersion);
            }
        }

        private readonly object _checkForUpdateLock = new object();
        private ApplicationUpdateModel _applicationUpdateModel;

        private bool? CheckForUpdateInternal(IApplicationUpdateService service, ApplicationUpdatePresenterFactory presenterFactory)
        {
            if (!Monitor.TryEnter(_checkForUpdateLock))
            {
                return null;
            }
            try
            {
                var uri = new Uri(Properties.Settings.Default.UpdateUrl);
                var update = service.GetApplicationUpdate(uri);

                if (update is null) return false;
                if (!update.VersionIsGreaterThan(Core.Application.VersionNumber)) return false;

                _applicationUpdateModel = new ApplicationUpdateModel(update);
                using (var presenter = presenterFactory.Create(_applicationUpdateModel))
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

        public bool FormClosing(ICollection<string> formColumns, LocalProcessService process)
        {
            Model.FormColumns.Reset(formColumns);

            if (!CheckForConfigurationChanges())
            {
                return true;
            }

            CheckForAndFireUpdateProcess(_applicationUpdateModel, process);

            return false;
        }

        private void CheckForAndFireUpdateProcess(ApplicationUpdateModel update, LocalProcessService process)
        {
            if (update != null && update.SelectedUpdateFileIsReadyToBeExecuted)
            {
                string path = update.SelectedUpdateFileLocalFilePath;
                Logger.Info($"Firing update file '{path}'...");
                try
                {
                    process.Start(path);
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

        // File Handling Methods
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
            if (Preferences.Get<bool>(Preference.AutoSaveConfig) && ClientConfiguration.IsDirty)
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, String.Format(CultureInfo.CurrentCulture,
                    "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
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

        // Help Menu Handling Methods
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
            string errorMessage = $"An error occurred while attempting to open the {Core.Application.Name} Google Group.";
            localProcess.StartAndNotifyError(Core.Application.SupportForumUrl, errorMessage, Logger, MessageBox);
        }

        public void CheckForUpdateClick(IApplicationUpdateService service, ApplicationUpdatePresenterFactory presenterFactory)
        {
            CheckForUpdate(service, presenterFactory);
        }

        // Clients Menu Handling Methods
        public void ClientsAddClick(FahClientSettingsPresenterFactory presenterFactory)
        {
            using (var dialog = presenterFactory.Create(new FahClientSettingsModel()))
            {
                while (dialog.ShowDialog(Form) == DialogResult.OK)
                {
                    var settings = dialog.Model.ClientSettings;
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

        public void ClientsEditClick(FahClientSettingsPresenterFactory presenterFactory)
        {
            var selectedSlot = SlotsModel.SelectedSlot;
            if (selectedSlot == null) return;

            var originalSettings = selectedSlot.Client.Settings;
            Debug.Assert(originalSettings.ClientType == ClientType.FahClient);

            var model = new FahClientSettingsModel(originalSettings);
            using (var dialog = presenterFactory.Create(model))
            {
                while (dialog.ShowDialog(Form) == DialogResult.OK)
                {
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
            if (SlotsModel.SelectedSlot == null) return;

            ClientConfiguration.Remove(SlotsModel.SelectedSlot.Client.Settings.Name);
        }

        public void ClientsRefreshSelectedClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (SlotsModel.SelectedSlot == null) return;

            Task.Run(SlotsModel.SelectedSlot.Client.Retrieve);
        }

        public void ClientsRefreshAllClick()
        {
            ClientConfiguration.ScheduledTasks.RetrieveAll();
        }

        public void ClientsViewCachedLogClick(LocalProcessService localProcess)
        {
            // Check for SelectedSlot, and get out if not found
            if (SlotsModel.SelectedSlot == null) return;

            string path = Path.Combine(Preferences.Get<string>(Preference.CacheDirectory), SlotsModel.SelectedSlot.Client.Settings.ClientLogFileName);
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
                string message = String.Format(CultureInfo.CurrentCulture, "The log file for '{0}' does not exist.", SlotsModel.SelectedSlot.Client.Settings.Name);
                MessageBox.ShowInformation(Form, message, Core.Application.NameAndVersion);
            }
        }

        // Grid Context Menu Handling Methods
        public void ClientsFoldSlotClick()
        {
            if (SlotsModel.SelectedSlot == null) return;

            if (SlotsModel.SelectedSlot.Client is IFahClientCommand client)
            {
                client.Fold(SlotsModel.SelectedSlot.SlotIdentifier.SlotID);
            }
        }

        public void ClientsPauseSlotClick()
        {
            if (SlotsModel.SelectedSlot == null) return;

            if (SlotsModel.SelectedSlot.Client is IFahClientCommand client)
            {
                client.Pause(SlotsModel.SelectedSlot.SlotIdentifier.SlotID);
            }
        }

        public void ClientsFinishSlotClick()
        {
            if (SlotsModel.SelectedSlot == null) return;

            if (SlotsModel.SelectedSlot.Client is IFahClientCommand client)
            {
                client.Finish(SlotsModel.SelectedSlot.SlotIdentifier.SlotID);
            }
        }

        public void CopyPRCGToClipboardClicked()
        {
            if (SlotsModel.SelectedSlot == null) return;

            string projectString = SlotsModel.SelectedSlot.WorkUnitModel.WorkUnit.ToProjectString();

            // TODO: Replace ClipboardWrapper.SetText() with abstraction
            ClipboardWrapper.SetText(projectString);
        }

        // View Menu Handling Methods
        public void ShowHideInactiveSlots()
        {
            SlotsModel.HideInactiveSlots = !SlotsModel.HideInactiveSlots;
        }

        private MessagesPresenter _messagesPresenter;

        public void ViewMessagesClick(Func<MessagesPresenter> presenterFactory)
        {
            try
            {
                if (_messagesPresenter is null)
                {
                    _messagesPresenter = presenterFactory();
                    _messagesPresenter.Closed += (s, e) => _messagesPresenter = null;
                }
                _messagesPresenter?.Show();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);

                _messagesPresenter?.Dispose();
                _messagesPresenter = null;
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
        }

        public void ViewToggleCompletedCountStyleClick()
        {
            var style = Preferences.Get<UnitTotalsType>(Preference.UnitTotals);
            Preferences.Set(Preference.UnitTotals, style == UnitTotalsType.All
                                    ? UnitTotalsType.ClientStart
                                    : UnitTotalsType.All);
            Preferences.Save();
        }

        public void ViewToggleVersionInformationClick()
        {
            Preferences.Set(Preference.DisplayVersions, !Preferences.Get<bool>(Preference.DisplayVersions));
            Preferences.Save();
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
        }

        // Tools Menu Handling Methods
        public void ToolsDownloadProjectsClick(IProteinService proteinService)
        {
            try
            {
                IEnumerable<ProteinChange> result = null;
                using (var dialog = new ProgressDialog((progress, _) =>
                {
                    result = proteinService.Refresh(progress);
                    return Task.CompletedTask;
                }, false))
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

        public async Task ToolsBenchmarksClick(BenchmarksPresenter presenter)
        {
            int projectID = 0;

            // Check for SelectedSlot, and if found... load its ProjectID.
            if (SlotsModel.SelectedSlot != null)
            {
                projectID = SlotsModel.SelectedSlot.WorkUnitModel.WorkUnit.ProjectID;
            }

            presenter.Model.DefaultProjectID = projectID;
            await presenter.ShowAsync().ConfigureAwait(true);
        }

        private IAsyncFormPresenter _historyPresenter;

        public async Task ToolsHistoryClick(Func<WorkUnitHistoryPresenter> presenterFactory)
        {
            try
            {
                if (_historyPresenter is null)
                {
                    _historyPresenter = presenterFactory();
                    _historyPresenter.Closed += (_, _) => _historyPresenter = null;
                }
                var task = _historyPresenter?.ShowAsync();
                if (task is not null)
                {
                    await task.ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);

                _historyPresenter?.Dispose();
                _historyPresenter = null;
            }
        }

        // Web Menu Handling Methods
        public void ShowEocUserPage(LocalProcessService localProcess)
        {
            string fileName = new Uri(String.Concat(EocStatsService.UserBaseUrl, Preferences.Get<int>(Preference.EocUserId))).AbsoluteUri;
            const string errorMessage = "An error occurred while attempting to open the EOC user stats page.";
            localProcess.StartAndNotifyError(fileName, errorMessage, Logger, MessageBox);
        }

        public async void ShowFahUserPage(LocalProcessService localProcess, FahUserService userService)
        {
            string name = Preferences.Get<string>(Preference.StanfordId);
            var fahUser = await userService.FindUserAndLogError(name, Logger).ConfigureAwait(true);

            string fileName = new Uri(String.Concat(FahUrl.UserBaseUrl, fahUser.ID)).AbsoluteUri;
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
            string errorMessage = $"An error occurred while attempting to open the {Core.Application.Name} GitHub project.";
            localProcess.StartAndNotifyError(Core.Application.ProjectSiteUrl, errorMessage, Logger, MessageBox);
        }

        // System Tray Icon Handling Methods
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

        // User Stats
        public void SetUserStatsDataViewStyle(bool showTeamStats)
        {
            UserStatsDataModel.SetViewStyle(showTeamStats);
        }
    }
}
