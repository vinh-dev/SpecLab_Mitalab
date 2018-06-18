using System.Globalization;
using System.IO;
using System.Reflection;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using SpecLab.Business;
using SpecLab.Business.BusinessEnum;
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
using SpecLab.FrontEnd.Models.SpecImport;
using SpecLab.Business.BusinessObjects;
using log4net;

namespace SpecLab.FrontEnd.Controllers
{
    [Authorize]
    public class SpecImportController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TubeSampleService _tubeSampleService = new TubeSampleService();

        private StorageService _storageService = new StorageService();

        private LabconnService _labconnService = new LabconnService();
        private string labID="A";

        [SpecLabAuthorize(UserRightCode.R00700)]
        public ActionResult Index()
        {
            SpecImportSearchModel model = new SpecImportSearchModel()
            {
                EndSearchDate = DateTime.Today,
                StartSearchDate = DateTime.Today.AddDays(-SpecLabWebConstant.DefaultSearchDateDayRange)
            };

            return View(model);
        }

        [SpecLabAuthorize(UserRightCode.R00701)]
        public ActionResult ShowCreate()
        {
            var loginUser = SessionUtils.LoginUserInfo;
            SpecImportCreateModel model = new SpecImportCreateModel()
            {
                SexSelectItems = WebUtils.GetSampleSexSelectList(),
                SampleTypeList = WebUtils.GetSampleTypeSelectList(),
            };
            return View("Create", model);
        }

        [SpecLabAuthorize(UserRightCode.R00700)]
        public JsonResult Search(ImportSearchCriteria criteria)
        {
            ImportListResult model = new ImportListResult();
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

                var listData = this._tubeSampleService.GetImportList(new TubeSampleService.GetImportListParams()
                {
                    ImportId = criteria.ImportId,
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

        [SpecLabAuthorize(UserRightCode.R00700)]
        public JsonResult Detail(SelectedImportId criteria)
        {
            ImportInfo model = new ImportInfo();
            try
            {
                model.ImportNote = _tubeSampleService.GetImportInfo(criteria.ImportId);
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

        [HttpPost]
        [SpecLabAuthorize(UserRightCode.R00701)]
        public JsonResult SearchLabconn(LabConnSearchInfo searchInfo)
        {
            LabConnSearchList model = new LabConnSearchList();
            try
            {
                DateTime? dateSearch = null;
                if(!string.IsNullOrEmpty(searchInfo.DateSearch))
                {
                    dateSearch = DateTime.ParseExact(searchInfo.DateSearch, 
                        CommonConstant.DateFormatDisplay, CultureInfo.InvariantCulture);
                }

                var criteria = new LabconnService.ListPatientCriteria()
                {
                    PatientName = searchInfo.PatientName,
                    SID = searchInfo.SID,
                    DateSearch = dateSearch,
                    Sequence = searchInfo.Sequence
                };

                int rowCountReturn = this._labconnService.GetCountPatientId(criteria);

                if(rowCountReturn <= CommonConstant.MaxRowsReturn)
                {
                    var searchResult = this._labconnService.GetListPatientId(criteria);
                    model.SampleInfos.AddRange(searchResult);
                } 
                else
                {
                    throw new BusinessException(ErrorCode.ExceedMaxRowsReturn, CommonConstant.MaxRowsReturn);
                }
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

        [HttpPost]
        [SpecLabAuthorize(UserRightCode.R00701)]
        public JsonResult Import(SampleSpecInfo sampleSpec)
        {
            CommonModelResult model = new CommonModelResult();
            try
            {
                sampleSpec.UserInput = SessionUtils.LoginUserInfo.UserId;
                sampleSpec.DateInput = DateTime.Now;
                _tubeSampleService.ImportSample(sampleSpec);
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

        public List<GridColumn> ImportGridColumns()
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
                    HeaderName = "Mã ống", 
                    DataSourceName = "TubeId", 
                    Ratio = 25
                }, 
                new GridColumn()
                {
                    HeaderName = "Loại mẫu", 
                    DataSourceName = "SampleTypeDisplay", 
                    Ratio = 20
                }, 
                new GridColumn()
                {
                    HeaderName = "Ngày nhập", 
                    DataSourceName = "ImportDateDisplay", 
                    Ratio = 15, 
                    DataAlign = XStringFormats.Center
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
                    HeaderName = "Vị trí lưu", 
                    DataSourceName = "StorageDisplay", 
                    Ratio = 25
                }, 
            });

            return gridColumns;
        }

        private void DrawReportSummary(PagingSetup pagingSetup, PdfDocument document,
            SampleSpecInfo sampleSpecInfo)
        {
            ReportUtils.IncreaseLineHeight(pagingSetup, document, pagingSetup.RegularLineHeight);

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Mã bệnh phẩm: {0}", sampleSpecInfo.SampleSpecId));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Tên bệnh nhân: {0}", sampleSpecInfo.PatientName));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Giới tính: {0}", sampleSpecInfo.SexDisplay));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Năm sinh: {0}", sampleSpecInfo.YearOfBirth));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Nguồn: {0}", sampleSpecInfo.LocationId));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Thời gian nhập: {0}", sampleSpecInfo.DateInputDisplay));

            ReportUtils.DrawParagraphLine(pagingSetup, document, PagingSetup.CreateDefaultPDFRegularFont(),
                XParagraphAlignment.Left, pagingSetup.RegularLineHeight,
                string.Format("Danh sách nhập"));
        }

        [HttpGet]
        [SpecLabAuthorize(UserRightCode.R00700)]
        public FileContentResult Print(string id)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var sampleSpec = _tubeSampleService.GetSampleSpec(id);
                var importNote = _tubeSampleService.GetImportInfo(id);
                //var historyData = _tubeSampleService.GetImportHistory(id);

                PagingSetup pagingSetup = ReportUtils.CreateDefaultPagingSetup();
                PdfDocument document = ReportUtils.CreateDefaultPdfDocument(pagingSetup);
                PdfPage page = document.Pages[0];
                ReportUtils.DrawReportHeader(pagingSetup, document, "Thông tin phiếu nhập", labID);
                DrawReportSummary(pagingSetup, document, sampleSpec);

                var gridTable = new ReportGridTable<ImportNoteDetail>()
                {
                    Columns = ImportGridColumns(),
                    Datas = importNote.ImportNoteDetails
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
