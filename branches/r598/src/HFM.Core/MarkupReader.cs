/*
 * HFM.NET - Markup Reader Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Linq;

using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Markup;
using HFM.Core.Serializers;

namespace HFM.Core
{
   public sealed class MarkupReader
   {
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      private readonly IProteinDictionary _proteinDictionary;   

      public MarkupReader(IPreferenceSet prefs, IProteinDictionary proteinDictionary)
      {
         _prefs = prefs;
         _proteinDictionary = proteinDictionary;
      }

      public IEnumerable<SlotData> Read(string filePath)
      {
         var serializer = new XmlFileSerializer<SlotSummary>();
         return serializer.Deserialize(filePath).Slots;
      }

      public SlotModel Read(string filePath, string name, ClientSettings settings)
      {
         var slots = new List<SlotData>();
         var serializer = new XmlFileSerializer<SlotSummary>();
         
         try
         {
            slots = serializer.Deserialize(filePath).Slots;
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }

         var slotData = slots.FirstOrDefault(x => x.GridData.Name == name);
         if (slotData != null)
         {
            var slot = AutoMapper.Mapper.Map<SlotData, SlotModel>(slotData);
            Protein protein = _proteinDictionary.GetProteinOrDownload(slotData.UnitInfo.ProjectID);

            // build unit info logic
            var unitInfoLogic = ServiceLocator.Resolve<UnitInfoLogic>();
            unitInfoLogic.CurrentProtein = protein;
            unitInfoLogic.UnitInfoData = slotData.UnitInfo;
            slot.Prefs = _prefs;
            slot.Settings = settings;
            slot.UnitInfoLogic = unitInfoLogic;
            return slot;
         }

         return new SlotModel { Prefs = _prefs, Settings = settings };
      }
   }
}
