/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

namespace HFM.Log
{
   /// <summary>
   /// Represents the types of log lines that can be detected by the HFM.Log API.
   /// Once the type has been identified a parser may be assigned to find data in the line text.
   /// </summary>
   public struct LogLineType : IEquatable<LogLineType>
   {
      /// <summary>
      /// Line does not contain any information or indicate any condition of interest.
      /// </summary>
      public const int None = 0;
      /// <summary>
      /// Line is a log opening line containing client start date and time (FahClient and Legacy clients)
      /// </summary>
      public const int LogOpen = 1;
      /// <summary>
      /// Line is a log header line containing a long string of '#' characters (Legacy clients only)
      /// </summary>
      public const int LogHeader = 2;
      /// <summary>
      /// Line contains client version information (Legacy clients only)
      /// </summary>
      public const int ClientVersion = 3;
      /// <summary>
      /// Line indicates the client is sending work to server (FahClient and Legacy clients)
      /// </summary>
      public const int ClientSendWorkToServer = 4;
      /// <summary>
      /// Line contains client argument information (Legacy clients only)
      /// </summary>
      public const int ClientArguments = 5;
      /// <summary>
      /// Line contains user name and team information (Legacy clients only)
      /// </summary>
      public const int ClientUserNameAndTeam = 6;
      /// <summary>
      /// Line contains user ID information received from server (Legacy clients only)
      /// </summary>
      public const int ClientReceivedUserID = 7;
      /// <summary>
      /// Line contains user ID information stored locally by the client (Legacy clients only)
      /// </summary>
      public const int ClientUserID = 8;
      /// <summary>
      /// Line contains machine ID information (Legacy clients only)
      /// </summary>
      public const int ClientMachineID = 9;
      /// <summary>
      /// Line indicates the client is attempting to get a work packet (FahClient and Legacy clients)
      /// </summary>
      public const int ClientAttemptGetWorkPacket = 10;
      /// <summary>
      /// Line indicates the client has begun processing a work unit (Legacy clients only)
      /// </summary>
      public const int WorkUnitProcessing = 11;
      /// <summary>
      /// Line indicates the client is downloading a new core executable (Legacy clients only)
      /// </summary>
      public const int WorkUnitCoreDownload = 12;
      /// <summary>
      /// Line contains work unit index information (Legacy clients only)
      /// </summary>
      public const int WorkUnitIndex = 13;
      /// <summary>
      /// Line contains work unit queue index information (Legacy clients only)
      /// </summary>
      public const int WorkUnitQueueIndex = 14;
      /// <summary>
      /// Line indicates the client has begun working on a work unit (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitWorking = 15;
      /// <summary>
      /// Line contains an echo of the call to the core worker process, including the number of cpu threads (Legacy clients only)
      /// </summary>
      public const int WorkUnitCallingCore = 16;
      /// <summary>
      /// Line "*------------------------------*" indicates the client core process has begun working on a work unit (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitCoreStart = 17;
      /// <summary>
      /// Line contains core executable version information (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitCoreVersion = 18;
      /// <summary>
      /// Line indicates the client core process did not fail to start and is running (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitRunning = 19;
      /// <summary>
      /// Line contains work unit project information (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitProject = 20;
      /// <summary>
      /// Line contains work unit frame (progress) information (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitFrame = 21;
      /// <summary>
      /// Line indicates the work unit was paused by the client (Legacy clients only)
      /// </summary>
      public const int WorkUnitPaused = 22;
      /// <summary>
      /// Line indicates the work unit was paused by the client due to the host machine transitioning to battery power (Legacy clients only)
      /// </summary>
      public const int WorkUnitPausedForBattery = 23;
      /// <summary>
      /// Line indicates the work unit was resumed by the client due to the host machine transitioning from battery power (Legacy clients only)
      /// </summary>
      public const int WorkUnitResumeFromBattery = 24;
      /// <summary>
      /// Line contains client core process result string (FahClient and Legacy clients)
      /// </summary>
      public const int WorkUnitCoreShutdown = 25;
      /// <summary>
      /// Line contains the client echo of the core process result string (FahClient clients only)
      /// </summary>
      public const int WorkUnitCoreReturn = 26;
      /// <summary>
      /// Line indicates work unit processing is complete (FahClient clients only)
      /// </summary>
      public const int WorkUnitCleaningUp = 27;
      /// <summary>
      /// Line contains the total number of work units completed by the client (Legacy clients only)
      /// </summary>
      public const int ClientNumberOfUnitsCompleted = 28;
      /// <summary>
      /// Line indicates a client-core communications error (Legacy clients only)
      /// </summary>
      public const int ClientCoreCommunicationsError = 29;
      /// <summary>
      /// Line indicates a client-core communications error which caused the client to shutdown (Legacy clients only)
      /// </summary>
      public const int ClientCoreCommunicationsErrorShutdown = 30;
      /// <summary>
      /// Line indicates the client has encountered too many EARLY_UNIT_END results from client core processes and will pause activity for 24 hours (Legacy clients only)
      /// </summary>
      public const int ClientEuePauseState = 31;
      /// <summary>
      /// Line indicates the client has been shutdown (Legacy clients only)
      /// </summary>
      public const int ClientShutdown = 32;
      /// <summary>
      /// Line indicates the client detected too many failures to run the same work unit (FahClient clients only)
      /// </summary>
      public const int WorkUnitTooManyErrors = 33;

      private readonly int _value;

      private LogLineType(int value)
      {
         _value = value;
      }

      /// <summary>
      /// Indicates whether the current object is equal to another object of the same type.
      /// </summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
      public bool Equals(LogLineType other)
      {
         return _value == other._value;
      }

      /// <summary>
      /// Indicates whether this instance and a specified object are equal.
      /// </summary>
      /// <param name="obj">The object to compare with the current instance.</param>
      /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         return obj is LogLineType other && Equals(other);
      }

      /// <summary>
      /// Returns the hash code for this instance.
      /// </summary>
      /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
      public override int GetHashCode()
      {
         return _value;
      }

#pragma warning disable 1591
      public static bool operator ==(LogLineType left, LogLineType right)
      {
         return left.Equals(right);
      }

      public static bool operator !=(LogLineType left, LogLineType right)
      {
         return !left.Equals(right);
      }

      public static implicit operator int(LogLineType logLineType)
      {
         return logLineType._value;
      }

      public static implicit operator LogLineType(int value)
      {
         return new LogLineType(value);
      }
#pragma warning restore 1591
   }
}
