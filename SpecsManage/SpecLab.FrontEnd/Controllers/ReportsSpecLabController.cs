using System.Globalization;
using System.IO;
using System.Reflection;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Report;
using SpecLab.Business.Services;
using SpecLab.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [SpecLabAuthorize(UserRightCode.R00800)]
    public class ReportsSpecLabController : Controller
    {
        private const int ExportHistoryDefaultRange = 30;

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ReportService _reportService = new ReportService();

        string labID = "A";// cau hinh tieu de trang in

        public ActionResult Index()
        {
            ReportsModel model = new ReportsModel()
            {
                ExportHistoryDefaultStartDate = DateTime.Today.AddDays(-ExportHistoryDefaultRange)
                    .ToString(CommonConstant.DateFormatDisplay),
                ExportHistoryDefaultEndDate = DateTime.Today
                    .ToString(CommonConstant.DateFormatDisplay)
            };
            return View(model);
        }

        private List<GridColumn> StorageStatisticsImportGridColumns()
        {
            List<GridColumn> gridColumns = new List<GridColumn>(new GridColumn[]
            {
                new GridColumn()
                {
                    HeaderName = "STT", 
                    DataSourceName = "{{index}}", 
                    Ratio = 5, 
                    DataAlign = XStringFormats.Center
                }, 
                new GridColumn()
                {
                    HeaderName = "Mã lưu", 
                    DataSourceName = "StorageDisplay", 
                    Ratio = 15
                }, 
                new GridColumn()
                {
                    HeaderName = "Vị trí", 
                    DataSourceName = "LocationNumDisplay", 
                    Ratio = 10, 
                    DataAlign = new XStringFormat()
                    {
                        LineAlignment = XLineAlignment.Center, 
                        Alignment = XStringAlignment.Far
                    }
                }, 
                new GridColumn()
                {
                    HeaderName = "Mã số mẫu", 
                    DataSourceName = "TubeId", 
                    Ratio = 50
                }, 
                new GridColumn()
                {
                    HeaderName = "Tình trạng", 
                    DataSourceName = "ConditionDisplay", 
                    Ratio = 20
                }, 
            });

            return gridColumns;
        }

        private List<GridColumn> ExportHistoryImportGridColumns()
        {
            List<GridColumn> gridColumns = new List<GridColumn>(new GridColumn[]
            {
                new GridColumn()
                {
                    HeaderName = "STT", 
                    DataSourceName = "{{index}}", 
                    Ratio = 5, 
                    DataAlign = XStringFormats.Center
                }, 
                new GridColumn()
                {
                    HeaderName = "Mã số mẫu", 
                    DataSourceName = "TubeId", 
                    Ratio = 13
                }, 
                new GridColumn()
                {
                    HeaderName = "Loại mẫu", 
                    DataSourceName = "SampleTypeDisplay", 
                    Ratio = 10
                }, 
                new GridColumn()
                {
                    HeaderName = "Người xuất", 
                    DataSourceName = "ExportUserId", 
                    Ratio = 10
                }, 
                new GridColumn()
                {
                    HeaderName = "Ngày xuất", 
                    DataSourceName = "ExportDateDisplay", 
                    Ratio = 12
                }, 
                new GridColumn()
                {
                    HeaderName = "Người nhập lại", 
                    DataSourceName = "UpdateUserId", 
                    Ratio = 10, 
                }, 
                new GridColumn()
                {
                    HeaderName = "Ngày nhập lại", 
                    DataSourceName = "UpdateDateDisplay", 
                    Ratio = 12
                }, 
                new GridColumn()
                {
                    HeaderName = "Mục đích sử dụng", 
                    DataSourceName = "ExportReason", 
                    Ratio = 15
                }, 
                new GridColumn()
                {
                    HeaderName = "Vị trí lưu", 
                    DataSourceName = "StorageDisplay", 
                    Ratio = 13
                },
            });

            return gridColumns;
        }

        private void ExportHistoryDrawReportSummary(PagingSetup pagingSetup, PdfDocument document,
            DateTime startDate, DateTime endDate)
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            ReportUtils.DrawParagraphLine(pagingSetup, document, new XFont("Tahoma", 12, XFontStyle.Bold, options),
                XParagraphAlignment.Center, pagingSetup.RegularLineHeight, 
                string.Format("Từ ngày: {0} - Đến ngày: {1}", 
                    startDate.ToString(CommonConstant.DateFormatDisplay), 
                    endDate.ToString(CommonConstant.DateFormatDisplay)));

            ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);
        }

        private List<GridColumn> SampleStatisticsImportGridColumns()
        {
            List<GridColumn> gridColumns = new List<GridColumn>(new GridColumn[]
            {
                new GridColumn()
                {
                    HeaderName = "STT", 
                    DataSourceName = "{{index}}", 
                    Ratio = 5, 
                    DataAlign = XStringFormats.Center
                }, 
                new GridColumn()
                {
                    HeaderName = "Mã số mẫu", 
                    DataSourceName = "TubeId", 
                    Ratio = 13
                }, 
                new GridColumn()
                {
                    HeaderName = "Loại mẫu", 
                    DataSourceName = "SampleTypeDisplay", 
                    Ratio = 8
                }, 
                new GridColumn()
                {
                    HeaderName = "Họ tên", 
                    DataSourceName = "FullName", 
                    Ratio = 17
                }, 
                new GridColumn()
                {
                    HeaderName = "GT", 
                    DataSourceName = "SexDisplay", 
                    Ratio = 5
                }, 
                new GridColumn()
                {
                    HeaderName = "NS", 
                    DataSourceName = "YearOfBirthDisplay", 
                    Ratio = 5, 
                    DataAlign = new XStringFormat()
                    {
                        LineAlignment = XLineAlignment.Center, 
                        Alignment = XStringAlignment.Far
                    }
                }, 
                new GridColumn()
                {
                    HeaderName = "Nguồn", 
                    DataSourceName = "Source", 
                    Ratio = 5
                }, 
                new GridColumn()
                {
                    HeaderName = "Vị trí lưu", 
                    DataSourceName = "StorageDisplay", 
                    Ratio = 13
                }, 
                new GridColumn()
                {
                    HeaderName = "Rã đông", 
                    DataSourceName = "ExportNumerDisplay", 
                    Ratio = 7, 
                    DataAlign = new XStringFormat()
                    {
                        LineAlignment = XLineAlignment.Center, 
                        Alignment = XStringAlignment.Far
                    }
                }, 
                new GridColumn()
                {
                    HeaderName = "Tình trạng", 
                    DataSourceName = "ConditionDisplay", 
                    Ratio = 9
                }, 
                new GridColumn()
                {
                    HeaderName = "Người nhập", 
                    DataSourceName = "ImportUserId", 
                    Ratio = 13
                }, 
            });

            return gridColumns;
        }

        [HttpGet]
        public FileContentResult ExportHistory(string startDate, string endDate)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                DateTime startDateValue = DateTime.Today.AddDays(-ExportHistoryDefaultRange);
                DateTime endDateValue = DateTime.Today;
                
                if (!string.IsNullOrEmpty(startDate))
                {
                    startDateValue = DateTime.ParseExact(startDate,
                        CommonConstant.UniversalDateFormat, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    endDateValue = DateTime.ParseExact(endDate, 
                        CommonConstant.UniversalDateFormat, CultureInfo.InvariantCulture);
                }

                var reportDetailList = _reportService.GetExportHistory(new ReportService.GetExportHistoryParam()
                                                                           {
                                                                               EndDate = endDateValue,
                                                                               StartDate = startDateValue
                                                                           });

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                pagingSetup.PageOrientation = PageOrientation.Landscape;
                pagingSetup.PageSize = PageSize.A4;
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);

                ReportUtils.DrawReportHeader(pagingSetup, document, "Thống kê mẫu đã xuất",labID);
                ExportHistoryDrawReportSummary(pagingSetup, document, startDateValue, endDateValue);

                var gridTable = new ReportGridTable<ReportExportHistoryInfo>()
                {
                    Columns = ExportHistoryImportGridColumns(),
                    Datas = reportDetailList
                };

                ReportUtils.DrawGrid(gridTable, pagingSetup, document);
                ReportUtils.DrawReportFooter(pagingSetup, document, labID);
                document.Save(stream);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
            }
            stream.Flush();
            return File(stream.ToArray(), "application/pdf");
        }

        [HttpGet]
        public FileContentResult SampleStatistics()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var reportDetailList = _reportService.GetSampleStatistics();

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                pagingSetup.PageOrientation = PageOrientation.Landscape;
                pagingSetup.PageSize = PageSize.A4;
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);
                PdfPage page = document.Pages[0];
                ReportUtils.DrawReportHeader(pagingSetup, document, "Thống kê tổng hợp", labID);
                ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

                var gridTable = new ReportGridTable<ReportSampleStatisticInfo>()
                {
                    Columns = SampleStatisticsImportGridColumns(),
                    Datas = reportDetailList
                };

                ReportUtils.DrawGrid(gridTable, pagingSetup, document);
                ReportUtils.DrawReportFooter(pagingSetup, document, labID);
                document.Save(stream);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
            }
            stream.Flush();
            return File(stream.ToArray(), "application/pdf");
        }

        [HttpGet]
        public FileContentResult StorageStatistics()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var reportDetailList = _reportService.GetStorageStatistics();

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);
                PdfPage page = document.Pages[0];
                
                ReportUtils.DrawReportHeader(pagingSetup, document, "Thống kê vị trí đang sử dụng",labID);
                ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

                var gridTable = new ReportGridTable<ReportStorageStatisticsInfo>()
                {
                    Columns = StorageStatisticsImportGridColumns(),
                    Datas = reportDetailList
                };

                ReportUtils.DrawGrid(gridTable, pagingSetup, document);
                ReportUtils.DrawReportFooter(pagingSetup, document, labID);
                document.Save(stream);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
            }
            stream.Flush();
            return File(stream.ToArray(), "application/pdf");
        }
    }
}
