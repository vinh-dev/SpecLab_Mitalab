using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;

namespace SpecLab.Business.Report
{
    public class GridColumn
    {
        public string HeaderName { get; set; }

        public string DataSourceName { get; set; }

        public double Ratio { get; set; }

        public double PaddingLeft { get; set; }

        public XStringFormat HeaderAlign { get; set; }

        public XStringFormat DataAlign { get; set; }

        public CellDisplay CellDisplay
        {
            get
            {
                return new CellDisplay()
                {
                    DisplayText = HeaderName,
                    Ratio = Ratio,
                    PaddingLeft = PaddingLeft,
                    CellAlign = HeaderAlign
                };
            }
        }

        public GridColumn()
        {
            PaddingLeft = 10;

            HeaderAlign = new XStringFormat()
            {
                LineAlignment = XLineAlignment.Center,
                Alignment = XStringAlignment.Center
            };

            DataAlign = new XStringFormat()
            {
                LineAlignment = XLineAlignment.Center,
                Alignment = XStringAlignment.Near
            };
        }
    }
}
