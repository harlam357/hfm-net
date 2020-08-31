using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public class BenchmarksModel : ViewModelBase, IBenchmarksReportSource
    {
        public IPreferences Preferences { get; }
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkService BenchmarkService { get; }
        public IEnumerable<BenchmarksReport> Reports { get; }

        public BenchmarksModel(IPreferences preferences, IProteinService proteinService,
            IProteinBenchmarkService benchmarkService, IEnumerable<BenchmarksReport> reports)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            ProteinService = proteinService ?? NullProteinService.Instance;
            BenchmarkService = benchmarkService ?? NullProteinBenchmarkService.Instance;
            Reports = reports ?? Array.Empty<BenchmarksReport>();

            SlotIdentifiers = new BindingSource();
            SlotIdentifiers.DataSource = new BindingList<ListItem> { RaiseListChangedEvents = false };
            SlotProjects = new BindingSource();
            SlotProjects.DataSource = new BindingList<ListItem> { RaiseListChangedEvents = false };
            SelectedSlotProjectListItems = new BindingSourceListItemCollection(SlotProjects);
        }

        public override void Load()
        {
            FormLocation = Preferences.Get<Point>(Preference.BenchmarksFormLocation);
            FormSize = Preferences.Get<Size>(Preference.BenchmarksFormSize);
            LoadGraphColors(Preferences.Get<List<Color>>(Preference.GraphColors) ?? new List<Color>());

            SetFirstGraphColor();
            RefreshSlotIdentifiers();
            SetFirstSlotIdentifier();
        }

        public override void Save()
        {
            Preferences.Set(Preference.BenchmarksFormLocation, FormLocation);
            Preferences.Set(Preference.BenchmarksFormSize, FormSize);
            Preferences.Set(Preference.GraphColors, GraphColors.Select(x => x.GetValue<ValueItem<Color>>().Value).ToList());
            Preferences.Save();
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        IReadOnlyList<Color> IBenchmarksReportSource.Colors => GraphColors.Select(x => x.GetValue<ValueItem<Color>>().Value).ToList();

        public BindingList<ListItem> GraphColors { get; } = new BindingList<ListItem>();

        public BindingSource SlotIdentifiers { get; }

        public BindingSource SlotProjects { get; }

        #region Protein

        private void SetProtein()
        {
            var projectID = SelectedSlotProject?.Value;
            if (projectID is null)
            {
                Protein = null;
                return;
            }

            Protein = ProteinService.Get(projectID.Value);
        }

        private Protein _protein;

        public Protein Protein
        {
            get => _protein;
            private set
            {
                if (_protein != value)
                {
                    _protein = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WorkUnitName));
                    OnPropertyChanged(nameof(Credit));
                    OnPropertyChanged(nameof(KFactor));
                    OnPropertyChanged(nameof(Frames));
                    OnPropertyChanged(nameof(NumberOfAtoms));
                    OnPropertyChanged(nameof(Core));
                    OnPropertyChanged(nameof(DescriptionUrl));
                    OnPropertyChanged(nameof(PreferredDays));
                    OnPropertyChanged(nameof(MaximumDays));
                    OnPropertyChanged(nameof(Contact));
                    OnPropertyChanged(nameof(ServerIP));
                }
            }
        }

        public string WorkUnitName => Protein?.WorkUnitName;

        public double? Credit => Protein?.Credit;

        public double? KFactor => Protein?.KFactor;

        public int? Frames => Protein?.Frames;

        public int? NumberOfAtoms => Protein?.NumberOfAtoms;

        public string Core => Protein?.Core;

        public string DescriptionUrl => Protein?.Description;

        public double? PreferredDays => Protein?.PreferredDays;

        public double? MaximumDays => Protein?.MaximumDays;

        public string Contact => Protein?.Contact;

        public string ServerIP => Protein?.ServerIP;

        #endregion

        // SelectedGraphColor
        private ValueItem<Color> _selectedGraphColorItem;

        public ValueItem<Color> SelectedGraphColorItem
        {
            get => _selectedGraphColorItem;
            set
            {
                _selectedGraphColorItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedGraphColor));
            }
        }

        public Color SelectedGraphColor => SelectedGraphColorItem?.Value ?? Color.Empty;

        private void LoadGraphColors(IEnumerable<Color> graphColors)
        {
            foreach (var color in graphColors)
            {
                GraphColors.Add(new ListItem(color.Name, new ValueItem<Color>(color)));
            }
        }

        private void SetFirstGraphColor()
        {
            if (GraphColors.Count > 0)
            {
                SelectedGraphColorItem = GraphColors.First().GetValue<ValueItem<Color>>();
            }
        }

        // SelectedSlotIdentifier
        SlotIdentifier? IBenchmarksReportSource.SlotIdentifier => SelectedSlotIdentifier?.Value;

        private ValueItem<SlotIdentifier> _selectedSlotIdentifier;

        public ValueItem<SlotIdentifier> SelectedSlotIdentifier
        {
            get => _selectedSlotIdentifier;
            set
            {
                if (_selectedSlotIdentifier != value)
                {
                    _selectedSlotIdentifier = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedSlotDeleteEnabled));

                    RefreshSlotProjects();
                    if (_selectedSlotIdentifier is null || SelectedSlotProject is null)
                    {
                        SetFirstSlotProject();
                    }
                }
            }
        }

        public bool SelectedSlotDeleteEnabled => SelectedSlotIdentifier != null && SelectedSlotIdentifier.Value != SlotIdentifier.AllSlots;

        private void RefreshSlotIdentifiers()
        {
            var slots = Enumerable.Repeat(SlotIdentifier.AllSlots, 1)
                .Concat(BenchmarkService.GetSlotIdentifiers().OrderBy(x => x.Name))
                .Select(x => new ListItem(x.ToString(), new ValueItem<SlotIdentifier>(x)));

            SlotIdentifiers.Clear();
            foreach (var slot in slots)
            {
                SlotIdentifiers.Add(slot);
            }
            SlotIdentifiers.ResetBindings(false);
        }

        internal IEnumerable<ValueItem<SlotIdentifier>> SlotIdentifierValueItems =>
            SlotIdentifiers.Cast<ListItem>().Select(x => x.GetValue<ValueItem<SlotIdentifier>>());

        private void SetFirstSlotIdentifier()
        {
            SelectedSlotIdentifier = SlotIdentifierValueItems.FirstOrDefault();
        }

        // SelectedSlotProject
        IReadOnlyCollection<int> IBenchmarksReportSource.Projects =>
            SelectedSlotProjectListItems.Select(x => x.GetValue<ValueItem<int>>().Value).ToList();

        public ValueItem<int> SelectedSlotProject { get; private set; }

        private ListItemCollection _selectedSlotProjectListItems;

        public ListItemCollection SelectedSlotProjectListItems
        {
            get => _selectedSlotProjectListItems;
            set
            {
                if (value != null)
                {
                    if (_selectedSlotProjectListItems != null)
                    {
                        _selectedSlotProjectListItems.CollectionChanged -= OnSelectedSlotProjectListItemsChanged;
                    }
                    _selectedSlotProjectListItems = value;
                    _selectedSlotProjectListItems.CollectionChanged += OnSelectedSlotProjectListItemsChanged;
                }
            }
        }

        protected virtual void OnSelectedSlotProjectListItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedSlotProjectListItems.Count > 0)
            {
                SelectedSlotProject = SelectedSlotProjectListItems.First().GetValue<ValueItem<int>>();
                RunReports();
            }
            else
            {
                SelectedSlotProject = null;
                ClearReports();
            }
            SetProtein();
        }

        private void RefreshSlotProjects()
        {
            SlotProjects.Clear();

            if (SelectedSlotIdentifier != null)
            {
                var projects = BenchmarkService.GetBenchmarkProjects(SelectedSlotIdentifier.Value)
                    .Select(x => new ListItem(x.ToString(), new ValueItem<int>(x)));

                foreach (var project in projects)
                {
                    SlotProjects.Add(project);
                }
            }

            SlotProjects.ResetBindings(false);
        }

        internal IEnumerable<ListItem> SlotProjectListItems => SlotProjects.Cast<ListItem>();

        private void SetFirstSlotProject()
        {
            if (SelectedSlotProjectListItems.Count > 0)
            {
                SelectedSlotProjectListItems.Clear();
            }
            var item = SlotProjectListItems.FirstOrDefault();
            if (!item.IsEmpty)
            {
                SelectedSlotProjectListItems.Add(item);
            }
        }

        /// <summary>
        /// Set before calling <see cref="Load"/> to default the selected project to this project ID.
        /// </summary>
        public int DefaultProjectID { get; set; }

        public void SetDefaultSlotProject()
        {
            var defaultSlotProject = SlotProjectListItems.FirstOrDefault(x => x.GetValue<ValueItem<int>>().Value == DefaultProjectID);
            if (!defaultSlotProject.IsEmpty)
            {
                if (SelectedSlotProjectListItems.Count > 0)
                {
                    SelectedSlotProjectListItems.Clear();
                }
                SelectedSlotProjectListItems.Add(defaultSlotProject);
            }
        }

        // Reports
        public void RunReports()
        {
            System.Diagnostics.Debug.WriteLine(nameof(RunReports));

            foreach (var report in Reports)
            {
                report.Generate(this);
                switch (report.Key)
                {
                    case TextBenchmarksReport.KeyName:
                        BenchmarkText = (IReadOnlyCollection<string>)report.Result;
                        break;
                    case FrameTimeZedGraphBenchmarksReport.KeyName:
                        FrameTimeGraphControl = (Control)report.Result;
                        break;
                    case ProductionZedGraphBenchmarksReport.KeyName:
                        ProductionGraphControl = (Control)report.Result;
                        break;
                    case ProjectComparisonZedGraphBenchmarksReport.KeyName:
                        ProjectComparisonGraphControl = (Control)report.Result;
                        break;
                }
            }
        }

        private void ClearReports()
        {
            BenchmarkText = null;
            FrameTimeGraphControl = null;
            ProductionGraphControl = null;
            ProjectComparisonGraphControl = null;
        }

        private IReadOnlyCollection<string> _benchmarkText;

        public IReadOnlyCollection<string> BenchmarkText
        {
            get => _benchmarkText;
            private set
            {
                if (_benchmarkText != value)
                {
                    _benchmarkText = value;
                    OnPropertyChanged();
                }
            }
        }

        private Control _frameTimeGraphControl;

        public Control FrameTimeGraphControl
        {
            get => _frameTimeGraphControl;
            set
            {
                if (_frameTimeGraphControl != value)
                {
                    _frameTimeGraphControl = value;
                    OnPropertyChanged();
                }
            }
        }

        private Control _productionGraphControl;

        public Control ProductionGraphControl
        {
            get => _productionGraphControl;
            set
            {
                if (_productionGraphControl != value)
                {
                    _productionGraphControl = value;
                    OnPropertyChanged();
                }
            }
        }

        private Control _projectComparisonGraphControl;

        public Control ProjectComparisonGraphControl
        {
            get => _projectComparisonGraphControl;
            set
            {
                if (_projectComparisonGraphControl != value)
                {
                    _projectComparisonGraphControl = value;
                    OnPropertyChanged();
                }
            }
        }

        // Benchmark Actions
        public void RemoveSlot(SlotIdentifier slotIdentifier)
        {
            BenchmarkService.RemoveAll(slotIdentifier);
            RefreshSlotIdentifiers();
        }

        public void RemoveProject(SlotIdentifier slotIdentifier, int projectID)
        {
            BenchmarkService.RemoveAll(slotIdentifier, projectID);
            RefreshSlotIdentifiers();
            RefreshSlotProjects();
        }

        // Color Actions
        public void MoveSelectedGraphColorUp()
        {
            int index = IndexOfSelectedGraphColor();
            if (index <= 0) return;

            var item = GraphColors[index];
            GraphColors.Insert(index - 1, item);
            GraphColors.RemoveAt(index + 1);
            SelectedGraphColorItem = item.GetValue<ValueItem<Color>>();
        }

        public void MoveSelectedGraphColorDown()
        {
            int index = IndexOfSelectedGraphColor();
            if (index == GraphColors.Count - 1) return;

            var item = GraphColors[index];
            GraphColors.RemoveAt(index);
            GraphColors.Insert(index + 1, item);
            SelectedGraphColorItem = item.GetValue<ValueItem<Color>>();
        }

        public bool AddGraphColor(Color color)
        {
            if (HasGraphColor(color)) return false;
            var colorItem = new ValueItem<Color>(color);
            GraphColors.Add(new ListItem(color.Name, colorItem));
            SelectedGraphColorItem = colorItem;
            return true;
        }

        private bool HasGraphColor(Color color)
        {
            return GraphColors.Select(x => x.GetValue<ValueItem<Color>>().Value).Contains(color);
        }

        public void DeleteSelectedGraphColor()
        {
            int index = IndexOfSelectedGraphColor();
            if (index == -1) return;

            GraphColors.RemoveAt(index);
            index = GetNextSelectedGraphColorIndex(index);
            if (index >= 0)
            {
                SelectedGraphColorItem = GraphColors[index].GetValue<ValueItem<Color>>();
            }
        }

        private int GetNextSelectedGraphColorIndex(int index)
        {
            if (GraphColors.Count == 0) return -1;
            if (index == 0) return index;
            if (index >= GraphColors.Count) return index - 1;
            return index;
        }

        private int IndexOfSelectedGraphColor()
        {
            int i = 0;
            foreach (var item in GraphColors)
            {
                if (item.GetValue<ValueItem<Color>>() == SelectedGraphColorItem)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}
