using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public enum ScrollDirection
    {
        Left,
        Right
    }

    public class LcdLabel : Label
    {
        private const int BOUNDARY_SLOWDOWN_FACTOR = 3;
        private const string FONT_NAME = "Digital-7";

        private Timer blinkTimer, scrollTimer;
        private ScrollDirection scrollDirection;
        private string wholeText;

        private string lastText;
        public string LastText
        {
            get { return this.lastText != null ? this.lastText : ""; }
        }

        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = new System.Drawing.Font(FONT_NAME, value.Size, value.Style, value.Unit);
            }
        }

        public string WholeText
        {
            get { return this.wholeText != null ? this.wholeText : ""; }
        }
        private int startIndex, endIndex;
        
        public int MaxWidth { get; set; }
        
        private int scrollInterval;
        public int ScrollInterval
        {
            get { return this.scrollInterval; }
            set { scrollTimer.Interval = value; scrollInterval = value; }
        }
        
        public int BlinkInterval
        {
            get { return blinkTimer.Interval; }
            set { blinkTimer.Interval = value; }
        }
        
        public bool Blinking
        {
            get { return blinkTimer.Enabled; }
            set { blinkTimer.Enabled = value; this.Visible = true; }
        }

        public LcdLabel()
            : base()
        {
            this.MaxWidth = -1;
            this.blinkTimer = new Timer();
            this.blinkTimer.Interval = 250;
            this.blinkTimer.Enabled = false;
            this.scrollTimer = new Timer();
            this.scrollTimer.Interval = 1000;
            this.scrollTimer.Enabled = false;
            this.scrollTimer.Tick += new EventHandler(scrollTimer_Tick);
            blinkTimer.Tick += new EventHandler(blinkTimer_Tick);
            base.Font = new System.Drawing.Font("Digital-7", 27.75F, System.Drawing.FontStyle.Regular);
        }

        void blinkTimer_Tick(object sender, EventArgs e)
        {
            Visible = !Visible;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                scrollTimer.Interval = this.scrollInterval;
                this.lastText = this.WholeText;
                base.Text = value;
                if (this.MaxWidth > 0 && base.ClientSize.Width > this.MaxWidth)
                {
                    startIndex = 0;
                    endIndex = value.Length;
                    while (base.ClientSize.Width > this.MaxWidth && base.Text.Length > 0)
                    {
                        base.Text = value.Substring(startIndex, --endIndex);
                    }
                    this.scrollDirection = ScrollDirection.Right;
                    scrollTimer.Enabled = true;
                }
                else
                {
                    this.scrollTimer.Enabled = false;
                }
                wholeText = value;
            }
        }

        public void Revert()
        {
            this.Text = this.lastText;
            this.Blinking = false;
        }

        void scrollTimer_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            if (scrollDirection == ScrollDirection.Right)
            {
                if (endIndex++ == WholeText.Length)
                {
                    t.Interval = this.scrollInterval * BOUNDARY_SLOWDOWN_FACTOR;
                    scrollDirection = ScrollDirection.Left;
                }
                else
                {
                    t.Interval = this.scrollInterval;
                }
                base.Text = WholeText.Substring(++startIndex, endIndex-startIndex-1);
            }
            else if (scrollDirection == ScrollDirection.Left)
            {
                if (--startIndex == 0)
                {
                    t.Interval = this.scrollInterval * BOUNDARY_SLOWDOWN_FACTOR;
                    scrollDirection = ScrollDirection.Right;
                }
                else
                {
                    t.Interval = this.scrollInterval;
                }
                base.Text = WholeText.Substring(startIndex, (--endIndex) - startIndex - 1);
            }
        }
    }
}
