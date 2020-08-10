using System.Collections.Generic;
using System.Linq;

using HFM.Core;

namespace HFM.Forms.Models
{
    public class ApplicationUpdateModel : ViewModelBase
    {
        public ApplicationUpdate Update { get; }

        public ICollection<ListItem> UpdateFilesList { get; }

        public ApplicationUpdateModel(ApplicationUpdate update)
        {
            Update = update;
            UpdateFilesList = BuildUpdateFilesList(update);
        }

        private static ICollection<ListItem> BuildUpdateFilesList(ApplicationUpdate update)
        {
            var list = update?.UpdateFiles?
                .Select(x => new ListItem(x.Description, x))
                .ToList();
            return list ?? new List<ListItem>();
        }

        private ApplicationUpdateFile _selectedUpdateFile;

        public ApplicationUpdateFile SelectedUpdateFile
        {
            get => _selectedUpdateFile;
            set
            {
                if (_selectedUpdateFile != value)
                {
                    _selectedUpdateFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedUpdateFileLocalFilePath;

        public string SelectedUpdateFileLocalFilePath
        {
            get => _selectedUpdateFileLocalFilePath;
            set
            {
                if (_selectedUpdateFileLocalFilePath != value)
                {
                    _selectedUpdateFileLocalFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool SelectedUpdateFileIsReadyToBeExecuted { get; set; }

        private bool _downloadInProgress;

        public bool DownloadInProgress
        {
            get => _downloadInProgress;
            set
            {
                if (_downloadInProgress != value)
                {
                    _downloadInProgress = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
