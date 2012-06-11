using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public class MarineRadioButton : Panel
    {
        private const int
            MOUSEDOWN_ALPHA = 120,
            MOUSEDOWN_RED = 155,
            MOUSEDOWN_GREEN = 255,
            MOUSEDOWN_BLUE = 155;

        private Color mouseDownColor, normalColor;
        private bool isHeld = false;
        private Stack<MouseEventHandler> handlerStack = new Stack<MouseEventHandler>();
        private Timer holdTimer;
        public bool IsHoldable { get; set; }

        public MarineRadioButton(Color mouseDownColor)
            : base()
        {
            this.IsHoldable = false;
            this.normalColor = base.BackColor = Color.Transparent;
            this.mouseDownColor = mouseDownColor;
            base.MouseLeave += new EventHandler(RadioButtonPanel_MouseLeave);
            base.MouseDown += new MouseEventHandler(RadioButtonPanel_MouseDown);
            base.MouseClick += new MouseEventHandler(MarineRadioButton_MouseClick);
            base.Cursor = Cursors.Hand;
            base.MouseUp += new MouseEventHandler(RadioButtonPanel_MouseUp);
            base.Invalidate();
        }

        public void AddTimedMouseClickHandler(int interval, EventHandler meh)
        {
            holdTimer = new Timer();
            holdTimer.Interval = interval;
            holdTimer.Tick += meh;
            holdTimer.Tick += StopTimer;
        }

        public void DisableMouseDownTimer()
        {
            holdTimer = null;
        }

        private void StopTimer(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;
            this.IsHeld = false;
        }

        private void MarineRadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            BeepNotification.Instance.Enabled = false;
            if (e.Button == MouseButtons.Left)
            {
                if (handlerStack.Count > 0 && !isHeld)
                {
                    if (ConfSingleton.Instance.SoundEffects)
                        Win32.WinMM.PlayWavResource(global::MarineRadioRTP.Properties.Resources.buttonBeep);
                    handlerStack.Peek().Invoke(this, e);
                }
                this.IsHeld = false;
            }
            else if (e.Button == MouseButtons.Right && this.IsHoldable)
            {
                this.IsHeld = !this.isHeld;
            }
        }

        public bool AddMouseClickHandler(MouseEventHandler meh)
        {
            if (handlerStack.Count < 1 || !handlerStack.Peek().Equals(meh))
            {
                handlerStack.Push(meh);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearMouseClickHandlers()
        {
            handlerStack.Clear();
        }

        public void RollbackHandlersBy(int maxRemove)
        {
            int i = 0;
            while (i++ < maxRemove && handlerStack.Count > 0)
                handlerStack.Pop();
        }

        public void RollbackHandlersTo(int newDepth)
        {
            while (handlerStack.Count > newDepth)
                handlerStack.Pop();
        }

        public MarineRadioButton()
            : this(Color.FromArgb(MOUSEDOWN_ALPHA, MOUSEDOWN_RED, MOUSEDOWN_GREEN, MOUSEDOWN_BLUE))
        {
        }

        public bool IsHeld
        {
            get
            {
                return this.isHeld;
            }
            set
            {
                this.isHeld = value;
                if (value)
                {
                    base.BackColor = this.mouseDownColor;
                }
                else
                {
                    base.BackColor = Color.Transparent;
                }
            }
        }

        /* Discard setting of this property so the designer
         * doesn't mess with the background set here. */
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                //base.BackColor = value;
            }
        }

        /* I'll use this instead. */
        public void SetBackColor(Color c)
        {
            base.BackColor = c;
        }

        void RadioButtonPanel_MouseLeave(object sender, EventArgs e)
        {
            RadioButtonPanel_MouseUp(sender, null);
        }

        void RadioButtonPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (holdTimer != null)
                holdTimer.Enabled = false;
            if (!this.isHeld)
                base.BackColor = this.normalColor;
        }

        void RadioButtonPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (holdTimer != null)
                holdTimer.Enabled = true;
            base.BackColor = this.mouseDownColor;
        }
    }
}
