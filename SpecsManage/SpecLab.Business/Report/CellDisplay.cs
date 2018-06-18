using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;

namespace SpecLab.Business.Report
{
    public class CellDisplay
    {
        public string DisplayText { get; set; }

        public double Ratio { get; set; }

        public double PaddingLeft { get; set; }

        public XStringFormat CellAlign { get; set; }

        public CellDisplay()
        {
            CellAlign = XStringFormats.Default;
        }
    }
}
