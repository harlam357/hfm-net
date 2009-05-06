using System;

using HFM.Proteins;

namespace HFM.Instances
{
   public class DisplayInstance
   {
      #region Members & Read Only Properties
      /// <summary>
      /// 
      /// </summary>
      private ClientStatus _Status;
      /// <summary>
      /// 
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
      }

      /// <summary>
      /// Private member holding the percentage progress of the unit
      /// </summary>
      private string _Progress;
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public string Progress
      {
         get { return _Progress; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _InstanceName;
      /// <summary>
      /// 
      /// </summary>
      public String Name
      {
         get { return _InstanceName; }
      }

      /// <summary>
      /// 
      /// </summary>
      private ClientType _ClientType;
      /// <summary>
      /// 
      /// </summary>
      public ClientType ClientType
      {
         get { return _ClientType; }
      }

      /// <summary>
      /// Private member holding the time per frame of the unit
      /// </summary>
      private TimeSpan _TimePerFrame;
      /// <summary>
      /// 
      /// </summary>
      public TimeSpan TPF
      {
         get { return _TimePerFrame; }
         set { _TimePerFrame = value; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD
      {
         get { return _PPD; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD_MHz;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD_MHz
      {
         get { return _PPD_MHz; }
      }

      /// <summary>
      /// Private variable holding the ETA
      /// </summary>
      private TimeSpan _ETA;
      /// <summary>
      /// ETA for this instance
      /// </summary>
      public TimeSpan ETA
      {
         get { return _ETA; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _Core;
      /// <summary>
      /// 
      /// </summary>
      public String Core
      {
         get { return _Core; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _CoreVersion;
      /// <summary>
      /// 
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _ProjectRunCloneGen;

      /// <summary>
      /// 
      /// </summary>
      public string ProjectRunCloneGen
      {
         get { return _ProjectRunCloneGen; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Credit;
      /// <summary>
      /// 
      /// </summary>
      public int Credit
      {
         get { return _Credit; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Complete;
      /// <summary>
      /// 
      /// </summary>
      public int Complete
      {
         get { return _Complete; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Failed;
      /// <summary>
      /// 
      /// </summary>
      public int Failed
      {
         get { return _Failed; }
      }

      /// <summary>
      /// Private member holding the download time of the unit
      /// </summary>
      private DateTime _DownloadTime;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
      }

      /// <summary>
      /// 
      /// </summary>
      private DateTime _Deadline;
	   /// <summary>
	   /// 
	   /// </summary>
      public DateTime Deadline
      {
         get { return _Deadline; }
      }

      public string Dummy
      {
         get { return String.Empty; }
      }
      #endregion
      
      public void Load(ClientInstance baseInstance)
      {
         _Status = baseInstance.Status;
         _Progress = String.Format("{0:00}%", baseInstance.UnitInfo.PercentComplete);
         _InstanceName = baseInstance.InstanceName;
         _ClientType = baseInstance.UnitInfo.TypeOfClient;
         _TimePerFrame = baseInstance.UnitInfo.TimePerFrame;
         _PPD = Math.Round(baseInstance.UnitInfo.PPD, 1);
         _PPD_MHz = Math.Round(baseInstance.UnitInfo.PPD / baseInstance.ClientProcessorMegahertz, 3);
         _ETA = baseInstance.UnitInfo.ETA;
         _Core = baseInstance.CurrentProtein.Core;
         _CoreVersion = baseInstance.UnitInfo.CoreVersion;
         _ProjectRunCloneGen = String.Format("P{0} (R{1}, C{2}, G{3})", baseInstance.UnitInfo.ProjectID,
                                                                        baseInstance.UnitInfo.ProjectRun,
                                                                        baseInstance.UnitInfo.ProjectClone,
                                                                        baseInstance.UnitInfo.ProjectGen);
         _Credit = baseInstance.CurrentProtein.Credit;
         _Complete = baseInstance.NumberOfCompletedUnitsSinceLastStart;
         _Failed = baseInstance.NumberOfFailedUnitsSinceLastStart;
         _DownloadTime = baseInstance.UnitInfo.DownloadTime;
         _Deadline = baseInstance.UnitInfo.DownloadTime.AddDays(baseInstance.CurrentProtein.PreferredDays);
      }
      
      public void UpdateName(string Key)
      {
         _InstanceName = Key;
      }
   }
}
