
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IClient
   {
      ClientSettings Settings { get; set; }

      IDictionary<int, SlotModel> Slots { get; }
   }
}
