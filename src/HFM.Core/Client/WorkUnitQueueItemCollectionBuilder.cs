﻿using System;
using System.Linq;

using HFM.Client.ObjectModel;

namespace HFM.Core.Client
{
    public class WorkUnitQueueItemCollectionBuilder
    {
        private readonly UnitCollection _units;
        private readonly SystemInfo _info;

        public WorkUnitQueueItemCollectionBuilder(UnitCollection units, SystemInfo info)
        {
            _units = units;
            _info = info;
        }

        public WorkUnitQueueItemCollection BuildForSlot(int slotID)
        {
            if (_units is null)
            {
                return null;
            }

            var collection = new WorkUnitQueueItemCollection();
            foreach (var unit in _units.Where(unit => unit.Slot == slotID))
            {
                var wui = new WorkUnitQueueItem(unit.ID.GetValueOrDefault());
                wui.ProjectID = unit.Project.GetValueOrDefault();
                wui.ProjectRun = unit.Run.GetValueOrDefault();
                wui.ProjectClone = unit.Clone.GetValueOrDefault();
                wui.ProjectGen = unit.Gen.GetValueOrDefault();
                wui.State = unit.State;
                wui.WaitingOn = unit.WaitingOn;
                wui.Attempts = unit.Attempts.GetValueOrDefault();
                wui.NextAttempt = unit.NextAttemptTimeSpan.GetValueOrDefault();
                wui.Assigned = unit.AssignedDateTime.GetValueOrDefault();
                wui.Timeout = unit.TimeoutDateTime.GetValueOrDefault();
                wui.Deadline = unit.DeadlineDateTime.GetValueOrDefault();
                wui.WorkServer = unit.WorkServer;
                wui.CollectionServer = unit.CollectionServer;
                wui.SlotID = slotID;

                collection.Add(wui);
                if (unit.State != null && unit.State.Equals("RUNNING", StringComparison.OrdinalIgnoreCase))
                {
                    collection.CurrentID = wui.ID;
                }
            }
            if (_info != null)
            {
                collection.OperatingSystem = _info.OS;
                // Memory Value is in Gigabytes - turn into Megabytes and truncate
                collection.Memory = (int)(_info.MemoryValue.GetValueOrDefault() * 1024);
                collection.CPUThreads = _info.CPUs.GetValueOrDefault();
            }

            if (collection.CurrentID == WorkUnitQueueItemCollection.NoID)
            {
                collection.CurrentID = collection.DefaultID;
            }

            return collection.Count > 0 ? collection : null;
        }
    }
}
