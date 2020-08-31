using System;

using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Forms.Presenters;

namespace HFM.Forms.Internal
{
    internal static class LocalProcessServiceExtensions
    {
        internal static void StartAndNotifyError(this LocalProcessService localProcess, string fileName, string errorMessage, ILogger logger, MessageBoxPresenter messageBox)
        {
            try
            {
                localProcess.Start(fileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                messageBox.ShowError(errorMessage, Core.Application.NameAndVersion);
            }
        }

        internal static void StartAndNotifyError(this LocalProcessService localProcess, string fileName, string arguments, string errorMessage, ILogger logger, MessageBoxPresenter messageBox)
        {
            try
            {
                localProcess.Start(fileName, arguments);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                messageBox.ShowError(errorMessage, Core.Application.NameAndVersion);
            }
        }
    }
}
