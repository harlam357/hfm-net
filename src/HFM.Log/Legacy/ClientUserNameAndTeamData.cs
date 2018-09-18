
using System;
using System.Globalization;

namespace HFM.Log.Legacy
{
   public class ClientUserNameAndTeamData
   {
      public ClientUserNameAndTeamData(string foldingID, int team)
      {
         FoldingID = foldingID;
         Team = team;
      }

      public string FoldingID { get; set; }

      public int Team { get; set; }

      public override string ToString()
      {
         return String.Format(CultureInfo.InvariantCulture, "{0} (Team {1})", FoldingID, Team);
      }
   }
}
