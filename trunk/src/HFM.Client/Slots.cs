
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class Slots : Message, IList<Slot>
   {
      private readonly List<Slot> _slots;

      private Slots()
      {
         _slots = new List<Slot>();
      }

      public static Slots Parse(Message message)
      {
         var jsonArray = JArray.Parse(message.Value);
         var slots = new Slots();
         foreach (var token in jsonArray)
         {
            if (!token.HasValues)
            {
               continue;
            }

            var slot = new Slot();
            foreach (var prop in JObject.Parse(token.ToString()).Properties())
            {
               if (prop.Name.Equals("options"))
               {
                  var optionsValue = prop.ToString();
                  // have to strip off "options" portion of the JSON
                  slot.Options = Options.Parse(optionsValue.Substring(optionsValue.IndexOf('{')), message);
               }
               else
               {
                  FahClient.SetObjectProperty(slot, TypeDescriptor.GetProperties(slot), prop);
               }
            }
            slots.Add(slot);
         }
         slots.SetMessageValues(message);
         return slots;
      }

      #region IList<Slot> Members

      public int IndexOf(Slot item)
      {
         return _slots.IndexOf(item);
      }

      public void Insert(int index, Slot item)
      {
         _slots.Insert(index, item);
      }

      public void RemoveAt(int index)
      {
         _slots.RemoveAt(index);
      }

      public Slot this[int index]
      {
         get { return _slots[index]; }
         set { _slots[index] = value; }
      }

      #endregion

      #region ICollection<Slot> Members

      public void Add(Slot item)
      {
         _slots.Add(item);
      }

      public void Clear()
      {
         _slots.Clear();
      }

      public bool Contains(Slot item)
      {
         return _slots.Contains(item);
      }

      public void CopyTo(Slot[] array, int arrayIndex)
      {
         _slots.CopyTo(array, arrayIndex);
      }

      public int Count
      {
         get { return _slots.Count; }
      }

      bool ICollection<Slot>.IsReadOnly
      {
         get { return ((ICollection<Slot>)_slots).IsReadOnly; }
      }

      public bool Remove(Slot item)
      {
         return _slots.Remove(item);
      }

      #endregion

      #region IEnumerable<Slot> Members

      public IEnumerator<Slot> GetEnumerator()
      {
         return _slots.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }

   public class Slot
   {
      internal Slot()
      {
         
      }

      [MessageProperty("id")]
      public int ID { get; set; }

      [MessageProperty("status")]
      public string Status { get; set; }

      [MessageProperty("description")]
      public string Description { get; set; }

      public Options Options { get; set; }
   }
}
