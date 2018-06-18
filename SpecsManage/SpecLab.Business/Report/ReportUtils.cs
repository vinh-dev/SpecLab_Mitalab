using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Services;

namespace SpecLab.Business.Report
{
    public static class ReportUtils
    {
        public static PdfDocument CreateDefaultPdfDocument(PagingSetup pagingSetup)
        {
            PdfDocument document = new PdfDocument { Language = "vi-vn" };
            PdfPage page = document.AddPage();
            page.Size = pagingSetup.PageSize;
            page.Orientation = pagingSetup.PageOrientation;

            return document;
        }

        public static PagingSetup CreateDefaultPagingSetup()
        {
            PagingSetup pagingSetup = new PagingSetup();
            return pagingSetup;
        }

        public static void DrawGridLine(List<CellDisplay> listCellDisplay,
            PagingSetup pagingSetup, PdfDocument document, XFont font,
            double lineHeight)
        {
            PdfPage currentPage = document.Pages[document.Pages.Count - 1];
            using (var gfx = XGraphics.FromPdfPage(currentPage))
            {
                var regularPen = new XPen(XColor.FromArgb(255, 0, 0, 0));
                double lineWidth = currentPage.Width - pagingSetup.MarginLeft - pagingSetup.MarginRight;

                double offsetRect = pagingSetup.MarginLeft;
                for (int i = 0; i < listCellDisplay.Count; i++)
                {
                    double rectWidth = lineWidth * listCellDisplay[i].Ratio / 100;
                    var gridRect = new XRect(offsetRect, pagingSetup.CurrentOffset, rectWidth, lineHeight);
                    gfx.DrawRectangle(regularPen, gridRect);
                    if (listCellDisplay[i].DisplayText != null)
                    {
                        gridRect = new XRect(offsetRect + listCellDisplay[i].PaddingLeft, pagingSetup.CurrentOffset, rectWidth - 2 * listCellDisplay[i].PaddingLeft, lineHeight);
                        gfx.DrawString(listCellDisplay[i].DisplayText, font, XBrushes.Black,
                            gridRect, listCellDisplay[i].CellAlign);
                    }

                    offsetRect += rectWidth;
                }
            }
            IncreaseLineHeight(pagingSetup, document, lineHeight);
        }

        public static void DrawParagraphOffset(PagingSetup pagingSetup, PdfPage page,
            XFont font, XParagraphAlignment paragraphAlignment, double offsetTop,
            double lineHeight, string textDisplay)
        {
            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                double lineWidth = page.Width - pagingSetup.MarginLeft - pagingSetup.MarginRight;

                var tf = new XTextFormatter(gfx)
                {
                    Alignment = paragraphAlignment
                };
                tf.DrawString(textDisplay, font, XBrushes.Black,
                    new XRect(pagingSetup.MarginLeft, offsetTop, lineWidth, pagingSetup.RegularLineHeight));
            }
        }

        public static void DrawParagraphLine(PagingSetup pagingSetup, PdfDocument document,
            XFont font, XParagraphAlignment paragraphAlignment,
            double lineHeight, string textDisplay)
        {
            PdfPage page = document.Pages[document.Pages.Count - 1];
            DrawParagraphOffset(pagingSetup, page, font, paragraphAlignment, pagingSetup.CurrentOffset, lineHeight,
                                textDisplay);
            IncreaseLineHeight(pagingSetup, document, lineHeight);
        }

        public static void DrawReportFooter(PagingSetup pagingSetup, PdfDocument document, string LabID)
        {
            for (int i = 0; i < document.Pages.Count; i++)
            {
                PdfPage page = document.Pages[i];

                double lineWidth = page.Width - pagingSetup.MarginLeft - pagingSetup.MarginRight;
                double bottomLine = page.Height - pagingSetup.MarginTop;
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    var regularPen = new XPen(XColor.FromArgb(255, 0, 0, 0));
                    gfx.DrawLine(regularPen, pagingSetup.MarginLeft, bottomLine - 5, pagingSetup.MarginLeft + lineWidth, bottomLine - 5);
                }

                DrawParagraphOffset(pagingSetup, page, PagingSetup.CreateDefaultPDFSmallRegularFont(),
                              XParagraphAlignment.Right,
                              page.Height - pagingSetup.MarginTop,
                              pagingSetup.SmallLineHeight,
                              string.Format("Trang {0}/{1}", i + 1, document.Pages.Count));

                string labName = HeaderService.GetHeader(LabID).Lab == null ? "" : HeaderService.GetHeader(LabID).Lab.Trim();
                if (labName != "")
                {
                    DrawParagraphOffset(pagingSetup, page, PagingSetup.CreateDefaultPDFSmallRegularFont(),
                             XParagraphAlignment.Left,
                             page.Height - pagingSetup.MarginTop,
                             pagingSetup.SmallLineHeight,
                             labName);
                }

            }
        }


        public static void DrawReportHeader(PagingSetup pagingSetup, PdfDocument document, string reportHeader, string LabID)
        {
            //DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFSmallBoldFont(),
            //                  XParagraphAlignment.Center, pagingSetup.SmallLineHeight, "VIỆN VỆ SINH DỊCH TỄ");

            string labName = HeaderService.GetHeader(LabID).Lab == null ? "" : HeaderService.GetHeader(LabID).Lab.Trim();
            if (labName != "")
            {
                DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFSmallBoldFont(),
                            XParagraphAlignment.Center, pagingSetup.SmallLineHeight, labName);
            }

            string address = HeaderService.GetHeader(LabID).Address == null ? "" : HeaderService.GetHeader(LabID).Address.Trim();
            if (address != "")
            {
                DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFSmallRegularFont(),
                              XParagraphAlignment.Center, pagingSetup.SmallLineHeight, address);
            }

            string phone = HeaderService.GetHeader(LabID).Phone == null ? "" : HeaderService.GetHeader(LabID).Phone.Trim();
            if (phone != "")
            {
                DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFSmallRegularFont(),
                             XParagraphAlignment.Center, pagingSetup.SmallLineHeight, phone);
            }

            IncreaseLineHeight(pagingSetup, document, pagingSetup.BoldLineHeight);

            DrawParagraphLine(pagingSetup, document, pagingSetup.HeaderFont,
                              XParagraphAlignment.Center, pagingSetup.BoldLineHeight, reportHeader);
        }

        public static bool IsHeightInPage(PagingSetup pagingSetup, PdfDocument document, double lineHeight)
        {
            PdfPage currentPage = document.Pages[document.Pages.Count - 1];
            if (pagingSetup.CurrentOffset + lineHeight > currentPage.Height - pagingSetup.MarginBottom - pagingSetup.MarginTop)
            {
                return false;
            }

            return true;
        }

        public static void BreakPage(PagingSetup pagingSetup, PdfDocument document)
        {
            PdfPage lastPage = document.AddPage();
            lastPage.Size = pagingSetup.PageSize;
            lastPage.Orientation = pagingSetup.PageOrientation;
            pagingSetup.CurrentOffset = pagingSetup.MarginTop;
        }

        public static PdfPage IncreaseLineHeight(PagingSetup pagingSetup, PdfDocument document, double lineHeight)
        {
            PdfPage currentPage = document.Pages[document.Pages.Count - 1];
            if (!IsHeightInPage(pagingSetup, document, lineHeight))
            {
                BreakPage(pagingSetup, document);
            }
            else
            {
                pagingSetup.CurrentOffset += lineHeight;
            }

            return currentPage;
        }

        /// <summary>
        /// Last position in last page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="pagingSetup"></param>
        /// <param name="beginDrawTop"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static void DrawGrid<T>(ReportGridTable<T> table, PagingSetup pagingSetup, PdfDocument document)
        {
            List<CellDisplay> listHeaderCellDisplay = table.HeaderCellDisplay;
            DrawGridLine(listHeaderCellDisplay, pagingSetup, document,
                pagingSetup.GridHeaderFont, pagingSetup.RegularLineHeight);

            for (int i = 0; i < table.Datas.Count; i++)
            {
                List<CellDisplay> listCellDisplay = table.GetDataDisplay(i);
                DrawGridLine(listCellDisplay, pagingSetup, document,
                    pagingSetup.GridContentFont, pagingSetup.RegularLineHeight);
            }
        }
    }
}
