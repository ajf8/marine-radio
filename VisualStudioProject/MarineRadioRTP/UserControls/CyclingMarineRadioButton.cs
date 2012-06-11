using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public class CyclingMarineRadioButton : MarineRadioButton
    {
        public event EventHandler<CyclingMenuItem> MenuCycled;
        public event EventHandler CyclingMenuClick;

        private Stack<CyclingMenuItemCollection> cycleHandlerStack = new Stack<CyclingMenuItemCollection>();

        private int currentIndex = 0;
        public bool SelfCycling { get; set; }

        public CyclingMarineRadioButton()
        {
            cycleHandlerStack.Push(new CyclingMenuItemCollection());
            this.SelfCycling = false;
            base.MouseClick += new System.Windows.Forms.MouseEventHandler(CyclingMarineRadioButton_MouseClick);
        }

        public CyclingMenuItem CurrentItem
        {
            get
            {
                return CurrentCycleList[currentIndex];
            }
        }

        public void AddCycleItem(string name, MouseEventHandler handler, object tag)
        {
            if (!HasMenu)
                cycleHandlerStack.Push(new CyclingMenuItemCollection());
            CurrentCycleList.Add(new CyclingMenuItem(name, handler, tag));
        }

        public void AddCycleItem(string name, MouseEventHandler handler)
        {
            if (!HasMenu)
                cycleHandlerStack.Push(new CyclingMenuItemCollection());
            CurrentCycleList.AddCycleItem(new CyclingMenuItem(name, handler));
        }

        public void ClearCycleItems()
        {
            currentIndex = 0;
            cycleHandlerStack.Clear();
        }

        public void RollbackCycleHandlersBy(int maxRemove)
        {
            currentIndex = 0;
            int i = 0;
            while (i++ < maxRemove && HasMenu)
                cycleHandlerStack.Pop();
        }

        public void RollbackCycleHandlersTo(int newDepth)
        {
            currentIndex = 0;
            while (cycleHandlerStack.Count > newDepth)
                cycleHandlerStack.Pop();
        }

        public CyclingMenuItemCollection CurrentCycleList
        {
            get
            {
                return cycleHandlerStack.Peek();
            }
        }

        public void NewCycleHandlerSet()
        {
            currentIndex = 0;
            cycleHandlerStack.Push(new CyclingMenuItemCollection());
        }

        void CyclingMarineRadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && HasMenu && CurrentCycleList.Count > 0)
            {
                if (ConfSingleton.Instance.SoundEffects)
                    Win32.WinMM.PlayWavResource(global::MarineRadioRTP.Properties.Resources.buttonBeep);
                if (this.SelfCycling)
                {
                    CycleUp();
                }
                else if (CyclingMenuClick != null)
                {
                    CyclingMenuClick(this, new EventArgs());
                }
            }
        }

        public void CycleUp()
        {
            if (++currentIndex >= CurrentCycleList.Count)
                currentIndex = 0;
            if (CurrentCycleList.Count > 0 && MenuCycled != null)
                MenuCycled(this, CurrentCycleList[currentIndex]);
        }

        public void CycleDown()
        {
            if (--currentIndex < 0)
                currentIndex = CurrentCycleList.Count - 1;
            if (CurrentCycleList.Count > 0 && MenuCycled != null)
                MenuCycled(this, CurrentCycleList[currentIndex]);
        }

        public void ForceCycleEvent()
        {
            if (MenuCycled != null && HasMenuItems)
                MenuCycled(this, CurrentCycleList[currentIndex]);
        }

        public void Select(object sender, MouseEventArgs e)
        {
            if (HasMenu)
                CurrentCycleList[currentIndex].Handler.Invoke(sender, e);
        }

        public void RemoveMenuItem(string name)
        {
            if (HasMenu)
                CurrentCycleList.RemoveMenuItem(name);
        }

        public bool HasMenuItems
        {
            get { return HasMenu && CurrentCycleList.Count > 0; }
        }

        public bool HasMenu
        {
            get { return cycleHandlerStack.Count > 0; }
        }
    }
}