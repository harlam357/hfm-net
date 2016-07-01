
using System.IO;

using Microsoft.Deployment.WindowsInstaller;

namespace HFM.Setup.CustomActions
{
   public class CustomActions
   {
      [CustomAction]
      public static ActionResult DeleteAppDataFolderFilesAction(Session session)
      {
         string appDataFolderPath = session["AppDataFolder"];
         string unitInfoCachePath = Path.Combine(appDataFolderPath, "HFM", "UnitInfoCache.dat");
         if (File.Exists(unitInfoCachePath))
         {
            session.Log("Deleting {0}", unitInfoCachePath);
            File.Delete(unitInfoCachePath);
         }
         return ActionResult.Success;
      }
   }
}
