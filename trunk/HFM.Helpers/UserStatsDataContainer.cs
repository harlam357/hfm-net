/*
 * HFM.NET - User Stats Data Container
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Helpers
{
   public class UserStatsDataContainer
   {
      private DateTime _LastUpdated = DateTime.MinValue;
      
      public DateTime LastUpdated
      {
         get { return _LastUpdated; }
         set { _LastUpdated = value; }
      }
   
      private long _User24hrAvg;

      public long User24hrAvg
      {
         get { return _User24hrAvg; }
         set 
         {
            if (_User24hrAvg != value)
            {
               _User24hrAvg = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }
      
      private long _UserPointsToday;

	   public long UserPointsToday
	   {
		   get { return _UserPointsToday;}
		   set 
		   {
		      if (_UserPointsToday != value)
		      {
		         _UserPointsToday = value;
               LastUpdated = DateTime.UtcNow;
		      }
		   }
	   }
	   
	   private long _UserPointsWeek;

	   public long UserPointsWeek
	   {
		   get { return _UserPointsWeek;}
		   set 
		   {
		      if (_UserPointsWeek != value)
		      {
		         _UserPointsWeek = value;
               LastUpdated = DateTime.UtcNow;
		      }
		   }
	   }
	   
	   private long _UserPointsTotal;

	   public long UserPointsTotal
	   {
		   get { return _UserPointsTotal;}
		   set 
		   { 
		      if (_UserPointsTotal != value)
		      {
		         _UserPointsTotal = value;
               LastUpdated = DateTime.UtcNow;
		      }
		   }
	   }
	
	   private long _UserWUsTotal;

	   public long UserWUsTotal
	   {
		   get { return _UserWUsTotal;}
		   set 
		   { 
		      if (_UserWUsTotal != value)
		      {
		         _UserWUsTotal = value;
               LastUpdated = DateTime.UtcNow;
		      }
		   }
	   }

      public bool TimeForUpdate()
      {
         if (LastUpdated.Equals(DateTime.MinValue))
         {
            return true;
         }

         DateTime CurrentTime = DateTime.UtcNow;
         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Current Time: {1} (UTC)", Debug.FunctionName, CurrentTime));
         DateTime NextUpdateTime = GetNextUpdateTime(LastUpdated);
         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Next Update Time: {1} (UTC)", Debug.FunctionName, NextUpdateTime));

         if (CurrentTime > NextUpdateTime)
         {
            return true;
         }

         return false;
      }

      private static DateTime GetNextUpdateTime(DateTime LastUpdated)
      {
         // What I really need to know is if it is Daylight Savings Time
         // in the Central Time Zone, not the local machines Time Zone.
         
         int offset = 0;
         if (DateTime.Now.IsDaylightSavingTime())
         {
            offset = 1;
         }

         DateTime nextUpdateTime = DateTime.UtcNow.Date;

         int hours = 21;
         for (int i = 0; i < 8; i++)
         {
            if (LastUpdated.TimeOfDay >= TimeSpan.FromHours(hours - offset))
            {
               nextUpdateTime = nextUpdateTime.Add(TimeSpan.FromHours(hours + 3 - offset));
               break;
            }

            hours -= 3;
         }

         return nextUpdateTime;
      }
   }
}
