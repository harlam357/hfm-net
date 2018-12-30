
using System;
using System.Globalization;

namespace HFM.Log.Legacy
{
   /// <summary>
   /// Represents Folding@Home user data gathered from a <see cref="LogLine"/> object.
   /// </summary>
   public class ClientUserNameAndTeamData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ClientUserNameAndTeamData"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
      public ClientUserNameAndTeamData(ClientUserNameAndTeamData other)
      {
         if (other == null) return;

         FoldingID = other.FoldingID;
         Team = other.Team;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ClientUserNameAndTeamData"/> class.
      /// </summary>
      /// <param name="foldingID">The Folding ID (Username)</param>
      /// <param name="team">The team number.</param>
      public ClientUserNameAndTeamData(string foldingID, int team)
      {
         FoldingID = foldingID;
         Team = team;
      }

      /// <summary>
      /// Gets or sets the Folding ID (Username).
      /// </summary>
      public string FoldingID { get; set; }

      /// <summary>
      /// Gets or sets the team number.
      /// </summary>
      public int Team { get; set; }

      /// <summary>
      /// Returns a string that represents the current <see cref="ClientUserNameAndTeamData"/> object.
      /// </summary>
      /// <returns>A string that represents the current <see cref="ClientUserNameAndTeamData"/> object.</returns>
      public override string ToString()
      {
         return String.Format(CultureInfo.InvariantCulture, "{0} (Team {1})", FoldingID, Team);
      }
   }
}
