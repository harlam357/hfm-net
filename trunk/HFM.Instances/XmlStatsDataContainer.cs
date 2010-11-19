/*
 * HFM.NET - XML Stats Data Container
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public class XmlStatsDataContainer : IXmlStatsDataContainer
   {
      #region Constants
      
      private const string DataStoreFilename = "UserStatsCache.dat";
      private const string EocFoldingStatsNode = "EOC_Folding_Stats";
      private const string EocStatusNode = "status";
      private const string EocUpdateStatusNode = "Update_Status";
      
      #endregion
   
      #region Fields
      
      private XmlStatsData _data;
      /// <summary>
      /// User Stats Data
      /// </summary>
      public XmlStatsData Data
      {
         get { return _data; }
      }
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _prefs;
      
      #endregion
      
      #region Constructor
      
      public XmlStatsDataContainer(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }
      
      #endregion
      
      #region Methods
      
      /// <summary>
      /// Is it Time for a Stats Update?
      /// </summary>
      public bool TimeForUpdate()
      {
         return TimeForNextUpdate(Data.LastUpdated, DateTime.UtcNow, DateTime.Now.IsDaylightSavingTime());
      }

      /// <summary>
      /// Is it Time for a Stats Update?
      /// </summary>
      internal static bool TimeForNextUpdate(DateTime lastUpdated, DateTime utcNow, bool isDaylightSavingTime)
      {
         // No Last Updated Value
         if (lastUpdated.Equals(DateTime.MinValue))
         {
            return true;
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture,
            "{0} Current Time: {1} (UTC)", HfmTrace.FunctionName, utcNow));

         DateTime nextUpdateTime = GetNextUpdateTime(lastUpdated, isDaylightSavingTime);
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture,
            "{0} Next Update Time: {1} (UTC)", HfmTrace.FunctionName, nextUpdateTime));

         if (utcNow > nextUpdateTime)
         {
            return true;
         }

         return false;
      }

      internal static DateTime GetNextUpdateTime(DateTime lastUpdated, bool isDaylightSavingTime)
      {
         // What I really need to know is if it is Daylight Savings Time
         // in the Central Time Zone, not the local machines Time Zone.

         int offset = 0;
         if (isDaylightSavingTime)
         {
            offset = 1;
         }

         DateTime nextUpdateTime = lastUpdated.Date;

         int hours = 24;
         for (int i = 0; i < 9; i++)
         {
            if (lastUpdated.TimeOfDay >= TimeSpan.FromHours(hours - offset))
            {
               nextUpdateTime = nextUpdateTime.Add(TimeSpan.FromHours(hours + 3 - offset));
               break;
            }

            hours -= 3;
         }

         return nextUpdateTime;
      }

      /// <summary>
      /// Get Overall User Data from EOC XML
      /// </summary>
      /// <param name="forceRefresh">Force Refresh or allow to check for next update time</param>
      public void GetEocXmlData(bool forceRefresh)
      {
         // if Forced or Time For an Update
         if (forceRefresh || TimeForUpdate())
         {
            DateTime start = HfmTrace.ExecStart;

            try
            {
               #region Get the XML Document

               var xmlData = new XmlDocument();
               xmlData.Load(_prefs.EocUserXml);
               xmlData.RemoveChild(xmlData.ChildNodes[0]);

               XmlNode eocNode = GetXmlNode(xmlData, EocFoldingStatsNode);
               XmlNode statusNode = GetXmlNode(eocNode, EocStatusNode);
               string updateStatus = GetXmlNode(statusNode, EocUpdateStatusNode).InnerText;

               #endregion

               // Update get the new data
               var newStatsData = GetUserStatsData(eocNode);

               // if Forced, set Last Updated and Serialize or
               // if the new data is not equal to the previous data, we updated... otherwise, if the update 
               // status is current we should assume the data is current but did not change - Issue 67
               if (forceRefresh || !(Data.Equals(newStatsData)) || updateStatus == "Current")
               {
                  _data = newStatsData;
                  Write();
               }
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
            finally
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, start);
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture,
            "{0} Last EOC Stats Update: {1} (UTC)", HfmTrace.FunctionName, Data.LastUpdated));
      }

      /// <summary>
      /// Updates the data container
      /// </summary>
      /// <param name="eocNode">EOC Stats XmlNode</param>
      private static XmlStatsData GetUserStatsData(XmlNode eocNode)
      {
         XmlNode teamNode = eocNode.SelectSingleNode("team");
         XmlNode userNode = eocNode.SelectSingleNode("user");
         
         var statsData = new XmlStatsData(DateTime.UtcNow);
         statsData.UserTwentyFourHourAvgerage = Convert.ToInt64(GetXmlNode(userNode, "Points_24hr_Avg").InnerText);
         statsData.UserPointsToday = Convert.ToInt64(GetXmlNode(userNode, "Points_Today").InnerText);
         statsData.UserPointsWeek = Convert.ToInt64(GetXmlNode(userNode, "Points_Week").InnerText);
         statsData.UserPointsTotal = Convert.ToInt64(GetXmlNode(userNode, "Points").InnerText);
         statsData.UserWorkUnitsTotal = Convert.ToInt64(GetXmlNode(userNode, "WUs").InnerText);
         statsData.UserPointsUpdate = Convert.ToInt64(GetXmlNode(userNode, "Points_Update").InnerText);
         statsData.UserTeamRank = Convert.ToInt32(GetXmlNode(userNode, "Team_Rank").InnerText);
         statsData.UserOverallRank = Convert.ToInt32(GetXmlNode(userNode, "Overall_Rank").InnerText);
         statsData.UserChangeRankTwentyFourHours = Convert.ToInt32(GetXmlNode(userNode, "Change_Rank_24hr").InnerText);
         statsData.UserChangeRankSevenDays = Convert.ToInt32(GetXmlNode(userNode, "Change_Rank_7days").InnerText);
         statsData.TeamTwentyFourHourAvgerage = Convert.ToInt64(GetXmlNode(teamNode, "Points_24hr_Avg").InnerText);
         statsData.TeamPointsToday = Convert.ToInt64(GetXmlNode(teamNode, "Points_Today").InnerText);
         statsData.TeamPointsWeek = Convert.ToInt64(GetXmlNode(teamNode, "Points_Week").InnerText);
         statsData.TeamPointsTotal = Convert.ToInt64(GetXmlNode(teamNode, "Points").InnerText);
         statsData.TeamWorkUnitsTotal = Convert.ToInt64(GetXmlNode(teamNode, "WUs").InnerText);
         statsData.TeamPointsUpdate = Convert.ToInt64(GetXmlNode(teamNode, "Points_Update").InnerText);
         statsData.TeamRank = Convert.ToInt32(GetXmlNode(teamNode, "Rank").InnerText);
         statsData.TeamChangeRankTwentyFourHours = Convert.ToInt32(GetXmlNode(teamNode, "Change_Rank_24hr").InnerText);
         statsData.TeamChangeRankSevenDays = Convert.ToInt32(GetXmlNode(teamNode, "Change_Rank_7days").InnerText);

         return statsData;
      }

      private static XmlNode GetXmlNode(XmlNode xmlNode, string xpath)
      {
         XmlNode node = xmlNode.SelectSingleNode(xpath);
         if (node == null)
         {
            throw new XmlException(String.Format(CultureInfo.CurrentCulture,
               "Node '{0}' does not exist.", xpath));
         }

         return node;
      }
      
      #endregion
      
      #region Serialization Support
      
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         string filePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);
         _data = Deserialize(filePath) ?? new XmlStatsData();
      }

      /// <summary>
      /// Write Binary File
      /// </summary>
      public void Write()
      {
         Serialize(_data, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }
      
      private static readonly object SerializeLock = typeof(XmlStatsDataContainer);

      public static void Serialize(XmlStatsData data, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  ProtoBuf.Serializer.Serialize(fileStream, data);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static XmlStatsData Deserialize(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         XmlStatsData data = null;
         try
         {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               data = ProtoBuf.Serializer.Deserialize<XmlStatsData>(fileStream);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return data;
      }
      
      #endregion
   }
}
