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
using SpecLab.FrontEnd.Models.SpecRemoval;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{

    [Authorize]
    public class SpecRemovalController : Controller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TubeSampleService _tubeSampleService = new TubeSampleService();

        private RemovalService _removalService = new RemovalService();

        private UserService _userService = new UserService();

        private const string DefaultRemovalId = "RE{0:yyyy}{1:000}";

        private string labID = "A";

        [SpecLabAuthorize(UserRightCode.R00400)]
        public ActionResult Index()
        {
            SpecRemovalSearchModel model = new SpecRemovalSearchModel()
            {
                EndSearchDate = DateTime.Today,
                StartSearchDate = DateTime.Today.AddDays(-SpecLabWebConstant.DefaultSearchDateDayRange)
            };

            return View(model);
        }

        [SpecLabAuthorize(UserRightCode.R00401)]
        public ActionResult ShowCreate()
        {
            var loginUser = SessionUtils.LoginUserInfo;
            var startYear = new DateTime(DateTime.Today.Year, 1, 1);
            var countRemoval = _removalService.GetNumberRemoval(new RemovalService.GetNumberRemovalParams()
            {
                FromDate = startYear
            });
            SpecRemovalCreateModel model = new SpecRemovalCreateModel()
            {
                CurrentUser = new ShortUserInfo()
                {
                    UserId = loginUser.UserId,
                    FullName = loginUser.FullName
                },
                RemovalId = string.Format(DefaultRemovalId, DateTime.Now, countRemoval + 1),
                StatusAllowList = new List<int>(new int[] { (int)TubeSampleStatus.Corrupt, (int)TubeSampleStatus.Good })
            };
            return View("Create", model);
        }

        [SpecLabAuthorize(UserRightCode.R00501)]
        [HttpPost]
        public JsonResult IncreateRE()
        {
            var loginUser = SessionUtils.LoginUserInfo;
            var startYear = new DateTime(DateTime.Today.Year, 1, 1);
            var countRemoval = _removalService.GetNumberRemoval(new RemovalService.GetNumberRemovalParams()
            {
                FromDate = startYear
            });
            SpecRemovalCreateModel model = new SpecRemovalCreateModel()
            {
                CurrentUser = new ShortUserInfo()
                {
                    UserId = loginUser.UserId,
                    FullName = loginUser.FullName
                },
                RemovalId = string.Format(DefaultRemovalId, DateTime.Now, countRemoval + 1),
                StatusAllowList = new List<int>(new int[] { (int)TubeSampleStatus.Corrupt, (int)TubeSampleStatus.Good })
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [SpecLabAuthorize(UserRightCode.R00400)]
        public JsonResult Search(RemovalSearchCriteria criteria)
        {
            RemovalListResult model = new RemovalListResult();
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

                var listData = this._tubeSampleService.GetRemovalList(new TubeSampleService.GetRemovalListParams()
                {
                    RemovalId = criteria.RemovalId,
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

        [SpecLabAuthorize(UserRightCode.R00400)]
        public JsonResult Detail(SelectedRemovalId criteria)
        {
            RemovalInfo model = new RemovalInfo();
            try
            {
                model.RemovalNote = _tubeSampleService.GetRemovalInfo(criteria.RemovalId);
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

        [SpecLabAuthorize(UserRightCode.R00401)]
        public JsonResult Create(RemovalSampleRequest removalNote)
        {
            CommonModelResult model = new CommonModelResult();
            try
            {
                this._tubeSampleService.RemovalSamples(new TubeSampleService.RemovalSampleParam()
                {
                    RemovalDate = DateTime.Now,
                    RemovalId = removalNote.RemovalId,
                    RemovalUserId = SessionUtils.LoginUserInfo.UserId,
                    RemovalReason = removalNote.RemovalReason,
                    TubeRemovalIds = removalNote.TubeRemovalIds
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

        private List<GridColumn> RemovalGridColumns()
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
                    Ratio = 17
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
                    Ratio = 13
                }, 
                new GridColumn()
                {
                    HeaderName = "Ngày nhập", 
                    DataSourceName = "InputDateDisplay", 
                    Ratio = 15, 
                    DataAlign = XStringFormats.Center
                }, 
                new GridColumn()
                {
                    HeaderName = "Vị trí lưu", 
                    DataSourceName = "StorageDisplay", 
                    Ratio = 20
                },
            });

            return gridColumns;
        }

        private void DrawReportSummary(PagingSetup pagingSetup, PdfDocument document,
            RemovalNote removalNote)
        {
            var options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            ReportUtils.DrawParagraphLine(pagingSetup, document, new XFont("Tahoma", 12, XFontStyle.Bold, options),
                XParagraphAlignment.Center, pagingSetup.RegularLineHeight,
                string.Format("Mã hủy: {0}", removalNote.RemovalId));

            ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                "Danh sách hủy: ");
        }

        private void DrawReportFooterSummary(PagingSetup pagingSetup, PdfDocument document,
            RemovalNote removalNote, string removalUser)
        {
            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Justify,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Người hủy: {0} Ngày hủy: {1}",
                                            removalUser, removalNote.RemovalDate.ToString(CommonConstant.DateFormatDisplay)));
            
            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Justify,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Lý do hủy: {0}", removalNote.RemovalReason));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Left,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Phương pháp hủy mẫu:.............................................................................................................................................."));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Justify,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Quản lý mẫu kiểm tra:..........................................................................................................Ngày.....tháng.....năm....."));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Justify,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Quản lý an toàn:..................................................................................................................Ngày.....tháng.....năm....."));

            ReportUtils.DrawParagraphLine(pagingSetup, document,
                                          pagingSetup.RegularFont, XParagraphAlignment.Justify,
                                          pagingSetup.RegularLineHeight,
                                          string.Format("Trưởng PTN phê duyệt:........................................................................................................Ngày.....tháng.....năm....."));
        }

        [SpecLabAuthorize(UserRightCode.R00400)]
        [HttpGet]
        public FileContentResult Print(string id)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var removalInfo = _tubeSampleService.GetRemovalInfo(id);
                var removalUserInfo = _userService.GetLoginUserInfo(removalInfo.RemovalUserId);

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);
                PdfPage page = document.Pages[0];
                ReportUtils.DrawReportHeader(pagingSetup, document, "Thông tin phiếu hủy", labID);
                DrawReportSummary(pagingSetup, document, removalInfo);

                var gridTable = new ReportGridTable<RemovalNoteDetail>()
                {
                    Columns = RemovalGridColumns(),
                    Datas = removalInfo.RemovalNoteDetails
                };

                ReportUtils.DrawGrid(gridTable, pagingSetup, document);
                ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

                DrawReportFooterSummary(pagingSetup, document, removalInfo,
                    removalUserInfo == null ? removalInfo.RemovalUserId : removalUserInfo.FullName);
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
