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
using SpecLab.FrontEnd.Models.SpecExport;
using log4net;
using System.Globalization;

namespace SpecLab.FrontEnd.Controllers
{
    [Authorize]
    public class SpecExportController : Controller
    {

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TubeSampleService _tubeSampleService = new TubeSampleService();

        private ExportService _exportService = new ExportService();

        private UserService _userService = new UserService();

        private StorageService _storageService = new StorageService();

        private const string DefaultExportId = "EX{0:yyyy}{1:000}";

        private string labID = "A";

        [SpecLabAuthorize(UserRightCode.R00500)]
        public ActionResult Index()
        {
            SpecExportSearchModel model = new SpecExportSearchModel()
            {
                EndSearchDate = DateTime.Today,
                StartSearchDate = DateTime.Today.AddDays(-SpecLabWebConstant.DefaultSearchDateDayRange)
            };
            return View(model);
        }

        [SpecLabAuthorize(UserRightCode.R00501)]
        public ActionResult ShowCreate()
        {
            var loginUser = SessionUtils.LoginUserInfo;
            var startYear = new DateTime(DateTime.Today.Year, 1, 1);
            var countExport = _exportService.GetNumberExport(new ExportService.GetNumberExportParams()
            {
                FromDate = startYear
            });
            var lstUser = this._userService.GetAllShortUserInfo();
            lstUser.Insert(0, new ShortUserInfo() { UserId = "", FullName = "" });
            SpecExportCreateModel model = new SpecExportCreateModel()
            {
                CurrentUser = new ShortUserInfo()
                {
                    UserId = loginUser.UserId,
                    FullName = loginUser.FullName
                },
                ExportId = string.Format(DefaultExportId, DateTime.Now, countExport + 1),
                UserList = lstUser,

                StatusAllowList = new List<int>(new int[] { (int)TubeSampleStatus.Good }),
                HasRightOverride = SessionUtils.LoginUserInfo.Rights.Exists(code => code == UserRightCode.R00502)

            };
            return View("Create", model);
        }


        [SpecLabAuthorize(UserRightCode.R00501)]
        [HttpPost]
        public JsonResult IncreateEX()
        {
            var loginUser = SessionUtils.LoginUserInfo;
            var startYear = new DateTime(DateTime.Today.Year, 1, 1);
            var countExport = _exportService.GetNumberExport(new ExportService.GetNumberExportParams()
            {
                FromDate = startYear
            });
            var lstUser=this._userService.GetAllShortUserInfo();
            lstUser.Insert(0, new ShortUserInfo() { UserId ="", FullName=""});
            SpecExportCreateModel model = new SpecExportCreateModel()
            {
                CurrentUser = new ShortUserInfo()
                {
                    UserId = loginUser.UserId,
                    FullName = loginUser.FullName
                },
                ExportId = string.Format(DefaultExportId, DateTime.Now, countExport + 1),
                UserList = lstUser,
                
                StatusAllowList = new List<int>(new int[] { (int)TubeSampleStatus.Good }),
                HasRightOverride = SessionUtils.LoginUserInfo.Rights.Exists(code => code == UserRightCode.R00502)

            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [SpecLabAuthorize(UserRightCode.R00500)]
        public JsonResult GetTubeExportCount(ExportCountRequest request)
        {
            ExportCountResponse model = new ExportCountResponse();
            try
            {
                var result = _exportService.GetTubeExportCount(new ExportService.GetTubeExportCountParams()
                {
                    TubeIds = request.ListTubeId
                });

                model.CountDetails.AddRange(result);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(model);
        }

        [SpecLabAuthorize(UserRightCode.R00501)]
        public JsonResult Create(ExportSampleRequest exportNote)
        {
            CommonModelResult model = new CommonModelResult();
            try
            {
                var tubeCountList = _exportService.GetTubeExportCount(
                    new ExportService.GetTubeExportCountParams()
                    {
                        TubeIds = exportNote.TubeExportIds
                    });


                this._tubeSampleService.ExportSamples(new TubeSampleService.ExportSampleParam()
                {
                    ExportDate = DateTime.Now,
                    ExportId = exportNote.ExportId,
                    ExportForUserId = exportNote.ExportForUserId,
                    ExportUserId = SessionUtils.LoginUserInfo.UserId,
                    ExportReason = exportNote.ExportReason,
                    TubeExportIds = exportNote.TubeExportIds
                });
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<GridColumn> ExportGridColumns()
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
                    HeaderName = "Mã bệnh phẩm",
                    DataSourceName = "SampleSpecId",
                    Ratio = 20
                },
                new GridColumn()
                {
                    HeaderName = "Mã ống",
                    DataSourceName = "TubeId",
                    Ratio = 20
                },
                new GridColumn()
                {
                    HeaderName = "Thể tích",
                    DataSourceName = "VolumeDisplay",
                    Ratio = 10,
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
                    Ratio = 15
                },
                new GridColumn()
                {
                    HeaderName = "Vị trí lưu",
                    DataSourceName = "StorageDisplay",
                    Ratio = 30
                },
            });

            return gridColumns;
        }

        private void DrawReportHeaderSummary(PagingSetup pagingSetup, PdfDocument document, ExportNote exportNote)
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                new XFont("Tahoma", 12, XFontStyle.Bold, options),
                XParagraphAlignment.Center, pagingSetup.RegularLineHeight,
                string.Format("Mã xuất: {0}", exportNote.ExportId));

            ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                "Danh sách mẫu xuất: ");
        }

        private void DrawReportFooterSummary(PagingSetup pagingSetup, PdfDocument document,
            ExportNote exportNote)
        {
            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Left,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Mục đích sử dụng: {0}", exportNote.ExportReason));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Left,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Ngày xuất mẫu: {0}", exportNote.ExportDateDisplay));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Left,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Ngày in: {0}",
                                                        DateTime.Now.ToString(CommonConstant.DateTimeFormatDisplay)));
        }

        private void DrawReportFooter(PagingSetup pagingSetup, PdfDocument document,
            string userExportName, string userExportTo)
        {
            double signatureHeaderHeight = pagingSetup.RegularLineHeight;
            double signatureContainerHeight = 100;
            double signatureContentName = pagingSetup.RegularLineHeight;
            double footerGridHeight = signatureHeaderHeight + signatureContainerHeight + signatureContentName;

            if (!ReportUtils.IsHeightInPage(pagingSetup, document, footerGridHeight))
            {
                ReportUtils.BreakPage(pagingSetup, document);
            }

            ReportUtils.DrawGridLine(new List<CellDisplay>()
            {
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = "Người xuất mẫu", CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = "Người nhận mẫu", CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
            }, pagingSetup, document, pagingSetup.GridHeaderFont, signatureHeaderHeight);

            ReportUtils.DrawGridLine(new List<CellDisplay>()
            {
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = "", CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = "", CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
            }, pagingSetup, document, pagingSetup.RegularFont, signatureContainerHeight);

            ReportUtils.DrawGridLine(new List<CellDisplay>()
            {
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = userExportName, CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
                new CellDisplay()
                {
                    Ratio = 50, DisplayText = userExportTo, CellAlign = XStringFormats.Center, PaddingLeft = 10
                },
            }, pagingSetup, document, pagingSetup.RegularFont, signatureContentName);
        }

        [SpecLabAuthorize(UserRightCode.R00500)]
        [HttpGet]
        public FileContentResult Print(string id)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var exportInfo = _tubeSampleService.GetExportInfo(id);
                var exportUserInfo = _userService.GetLoginUserInfo(exportInfo.ExportUserId);
                var exportForUserInfo = _userService.GetLoginUserInfo(exportInfo.ExportForUserId);

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);
                ReportUtils.DrawReportHeader(pagingSetup, document, "PHIẾU XUẤT MẪU", labID);
                DrawReportHeaderSummary(pagingSetup, document, exportInfo);

                var gridTable = new ReportGridTable<ExportNoteDetail>()
                {
                    Columns = ExportGridColumns(),
                    Datas = exportInfo.ExportNoteDetails
                };

                ReportUtils.DrawGrid(gridTable, pagingSetup, document);

                ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

                DrawReportFooterSummary(pagingSetup, document, exportInfo);

                DrawReportFooter(pagingSetup, document,
                                 exportUserInfo == null ? exportInfo.ExportUserId : exportUserInfo.FullName,
                                 exportForUserInfo == null ? exportInfo.ExportForUserId : exportForUserInfo.FullName);

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

        [SpecLabAuthorize(UserRightCode.R00500)]
        public JsonResult Search(ExportSearchCriteria criteria)
        {
            ExportListResult model = new ExportListResult();
            try
            {
                DateTime? criteriaFromDate = null;
                if (!string.IsNullOrEmpty(criteria.StartDate))
                {
                    criteriaFromDate = DateTime.ParseExact(criteria.StartDate,
                        CommonConstant.DateFormatDisplay, CultureInfo.InvariantCulture);
                }

                DateTime? criteriaToDate = null;
                if (!string.IsNullOrEmpty(criteria.EndDate))
                {
                    criteriaToDate = DateTime.ParseExact(criteria.EndDate,
                        CommonConstant.DateFormatDisplay, CultureInfo.InvariantCulture);
                }

                var listData = this._tubeSampleService.GetExportList(new TubeSampleService.GetExportListParams()
                {
                    ExportId = criteria.ExportId,
                    StartDate = criteriaFromDate,
                    EndDate = criteriaToDate
                });
                model.ListData.AddRange(listData);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(model);
        }

        [SpecLabAuthorize(UserRightCode.R00500)]
        public JsonResult Detail(SelectedExportId criteria)
        {
            ExportInfo model = new ExportInfo();
            try
            {
                model.ExportNote = _tubeSampleService.GetExportInfo(criteria.ExportId);
            }
            catch (BusinessException exception)
            {
                _logger.DebugFormat("BusinessException: {0}-{1}", exception.ErrorCode, exception.Message);
                model.ErrorCode = exception.ErrorCode;
                model.ErrorDescription = exception.Message;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                model.ErrorCode = ErrorCode.InternalErrorException;
                model.ErrorDescription = CommonUtils.GetEnumDescription(ErrorCode.InternalErrorException);
            }

            return Json(model);
        }
    }
}
