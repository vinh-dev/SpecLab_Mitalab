using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace SpecLab.Business.Report
{
    public class PagingSetup
    {
        public double MarginTop { get; set; }
        public double MarginBottom { get; set; }
        public double MarginLeft { get; set; }
        public double MarginRight { get; set; }
        public double RegularLineHeight { get; set; }
        public double BoldLineHeight { get; set; }
        public double SmallLineHeight { get; set; }
        public double CurrentOffset { get; set; }

        public PageSize PageSize { get; set; }

        public PageOrientation PageOrientation { get; set; }

        public XFont RegularFont { get; set; }

        public XFont HeaderFont { get; set; }

        public XFont PageHeaderFont { get; set; }

        public XFont GridHeaderFont { get; set; }

        public XFont GridContentFont { get; set; }
        
        public PagingSetup()
        {
            MarginTop = 30;
            MarginBottom = 30;
            MarginLeft = 30;
            MarginRight = 30;
            BoldLineHeight = 30;
            RegularLineHeight = 20;
            SmallLineHeight = 15;

            CurrentOffset = MarginTop + 0;
            PageSize = PageSize.A4;
            PageOrientation = PageOrientation.Portrait;
            RegularFont = CreateDefaultPDFRegularFont();
            HeaderFont = CreateDefaultPDFHeaderFont();
            GridHeaderFont = CreateDefaultPDFGridHeaderFont();
            GridContentFont = CreateDefaultPDFGridContentFont();
            PageHeaderFont = CreateDefaultPDFSmallBoldFont();
        }

        public static XFont CreateDefaultPDFSmallBoldFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var regularFont = new XFont("Tahoma", 8, XFontStyle.Bold, options);

            return regularFont;
        }

        public static XFont CreateDefaultPDFSmallRegularFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var regularFont = new XFont("Tahoma", 8, XFontStyle.Regular, options);

            return regularFont;
        }

        public static XFont CreateDefaultPDFRegularFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var regularFont = new XFont("Tahoma", 10, XFontStyle.Regular, options);

            return regularFont;
        }

        public static XFont CreateDefaultPDFRegularBoldFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var regularFont = new XFont("Tahoma", 10, XFontStyle.Bold, options);

            return regularFont;
        }

        public static XFont CreateDefaultPDFHeaderFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var headerFont = new XFont("Tahoma", 24, XFontStyle.Bold, options);

            return headerFont;
        }

        public static XFont CreateDefaultPDFGridHeaderFont()
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var headerFont = new XFont("Tahoma", 10, XFontStyle.Bold, options);

            return headerFont;
        }

        public static XFont CreateDefaultPDFGridContentFont() 
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var headerFont = new XFont("Tahoma", 8, XFontStyle.Regular, options);

            return headerFont;
        }
    }
}
