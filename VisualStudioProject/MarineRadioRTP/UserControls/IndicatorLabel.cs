using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MarineRadioRTP
{
    class IndicatorLabel : Label
    {
        public IndicatorLabel()
        {
            base.BackColor = Color.Black;
            base.ForeColor = Color.FromArgb(214, 140, 35);
            base.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Bold);
            base.AutoSize = true;
        }
    }
}
