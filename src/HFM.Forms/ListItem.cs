
using System;

namespace HFM.Forms
{
    public struct ListItem
    {
        public ListItem(string display, object value)
        {
            DisplayMember = display ?? throw new ArgumentNullException(nameof(display));
            ValueMember = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string DisplayMember { get; set; }

        public object ValueMember { get; set; }
    }
}
