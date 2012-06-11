using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class CyclingMenuItemCollection : List<CyclingMenuItem>
    {
        public void AddCycleItem(CyclingMenuItem newItem)
        {
            foreach (CyclingMenuItem item in this)
            {
                if (item.Name.Equals(newItem))
                {
                    return;
                }
            }
            base.Add(newItem);
        }

        public void RemoveMenuItem(string name)
        {
            CyclingMenuItem toRemove = null;
            foreach (CyclingMenuItem item in this)
            {
                if (item.Name.Equals(name))
                {
                    toRemove = item;
                    break;
                }
            }
            if (toRemove != null)
                this.Remove(toRemove);
        }
    }

}
