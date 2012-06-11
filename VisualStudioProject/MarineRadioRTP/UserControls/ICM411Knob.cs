using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MarineRadioRTP
{
    class ICM411Knob : PictureBox
    {
        private int knobIndex = 0;
        public event EventHandler<KnobChangeEventArgs> KnobChange;

        public ICM411Knob()
        {
            this.KnobIndex = 0;
            base.BackColor = Color.Transparent;
            base.MouseClick += new MouseEventHandler(ICM411Knob_MouseClick);
            base.Cursor = Cursors.Hand;
            base.Invalidate();
        }

        void ICM411Knob_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.KnobIndex++;
            }
            else
            {
                this.KnobIndex--;
            }
        }

        private int KnobIndex
        {
            get
            {
                return this.knobIndex;
            }
            set
            {
                Bitmap img = (Bitmap)global::MarineRadioRTP.Properties.Resources.ResourceManager.GetObject("ic411_vol_" + value);
                if (img != null)
                {
                    base.BackgroundImage = img;
                    if (KnobChange != null)
                        KnobChange(this, new KnobChangeEventArgs() { OldKnobIndex = this.knobIndex, NewKnobIndex = value });
                    this.knobIndex = value;
                }
            }
        }
    }
}
