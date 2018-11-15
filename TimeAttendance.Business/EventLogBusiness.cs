using TimeAttendance.Model;
using TimeAttendance.Model.SearchCondition;
using TimeAttendance.Model.SearchResults;
using TimeAttendance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.Pdf;
using System.Web;
using System.Data.Entity.Core.Objects;
using TimeAttendance.Model.Repositories;

namespace TimeAttendance.Business
{
    public class EventLogBusiness
    {
        private static EventLogBusiness _eventLogBusiness;
        private TimeAttendanceEntities db;
        private AuthContext authContext = new AuthContext();

        public EventLogBusiness()
        {

        }

        public static EventLogBusiness GetInstance()
        {
            if (_eventLogBusiness == null)
            {
                _eventLogBusiness = new EventLogBusiness();
            }

            return _eventLogBusiness;
        }

        public SearchResultObject<UserEventLogSearchResult> SearchUserEventLog(UserEventLogSearchCondition searchCondition, string saveOption)
        {
            db = new TimeAttendanceEntities();
            SearchResultObject<UserEventLogSearchResult> searchResult = new SearchResultObject<UserEventLogSearchResult>();
            try
            {
                var listmodel = (from a in db.UserEventLog.AsNoTracking()
                                 join b in db.User.AsNoTracking() on a.UserId equals b.UserId into ab
                                 from abv in ab.DefaultIfEmpty()
                                 select new UserEventLogSearchResult()
                                 {
                                     UserEventLogId = a.UserEventLogId,
                                     UserId = a.UserId,
                                     Description = a.Description,
                                     LogType = a.LogType,
                                     LogTypeName = a.LogType == 0 ? "Truy cập hệ thống" : "Khai thác dữ liệu",
                                     Type = abv.Type,
                                     CreateDate = a.CreateDate,
                                     UserName = abv.Name,
                                     FullName = abv.FullName
                                 }).AsQueryable();

                if (!string.IsNullOrEmpty(searchCondition.UserName))
                {
                    listmodel = listmodel.Where(r => r.UserName != null && r.UserName.ToUpper().Contains(searchCondition.UserName.ToUpper()));
                }
                if (!string.IsNullOrEmpty(searchCondition.UserType))
                {
                    listmodel = listmodel.Where(r => r.Type != null && r.Type.Equals(searchCondition.UserType));
                }
                if (!string.IsNullOrEmpty(searchCondition.UserIdSearch))
                {
                    listmodel = listmodel.Where(r => r.UserId != null && r.UserId.Equals(searchCondition.UserIdSearch));
                }
                if (!string.IsNullOrEmpty(searchCondition.FullName))
                {
                    listmodel = listmodel.Where(r => r.FullName != null && r.FullName.ToUpper().Contains(searchCondition.FullName.ToUpper()));
                }
                if (!string.IsNullOrEmpty(searchCondition.Description))
                {
                    listmodel = listmodel.Where(r => r.Description.ToUpper().Contains(searchCondition.Description.ToUpper()));
                }
                if (searchCondition.LogType.HasValue)
                {
                    listmodel = listmodel.Where(r => r.LogType.HasValue && r.LogType.Value == searchCondition.LogType.Value);
                }
                if (searchCondition.LogDateFrom.HasValue)
                {
                    searchCondition.LogDateFrom = DateTimeUtils.ConvertDateFrom(searchCondition.LogDateFrom);
                    listmodel = listmodel.Where(r => r.CreateDate.HasValue && r.CreateDate >= searchCondition.LogDateFrom);
                }
                if (searchCondition.LogDateTo.HasValue)
                {
                    searchCondition.LogDateTo = DateTimeUtils.ConvertDateTo(searchCondition.LogDateTo);
                    listmodel = listmodel.Where(r => r.CreateDate.HasValue && r.CreateDate <= searchCondition.LogDateTo);
                }

                searchResult.TotalItem = listmodel.Select(r => r.UserEventLogId).Count();
                var listResult = SQLHelpper.OrderBy(listmodel, searchCondition.OrderBy, searchCondition.OrderType).Skip((searchCondition.PageNumber - 1) * searchCondition.PageSize)
                            .Take(searchCondition.PageSize)
                            .ToList();


                string pathFile = string.Empty;
                if ((saveOption.Equals("PDF") || saveOption.Equals("XLSX")) && searchResult.TotalItem > 0)
                {
                    if (searchResult.TotalItem > Constants.MAX_RETURN_DATA_ROW)
                    {
                        throw new BusinessException(ErrorMessage.ERR007);
                    }
                    else
                    {
                        pathFile = this.Export(saveOption, listmodel.ToList(), searchCondition);
                    }
                }

                searchResult.ListResult = listResult;
                searchResult.PathFile = pathFile;
            }
            catch (Exception ex)
            {
                throw new ErrorException(ErrorMessage.ERR001, ex.InnerException);
            }

            return searchResult;
        }

        private string Export(string saveOption, List<UserEventLogSearchResult> eventLogList, UserEventLogSearchCondition model)
        {
            string pathExport = string.Empty;
            // Khỏi tạo bảng excel
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            IWorkbook workbook = application.Workbooks.Open(HttpContext.Current.Server.MapPath("/Template/LichSuTruyCapSuDung.xlsx"));

            IWorksheet sheet = workbook.Worksheets[0];

            //Thay đổi title trong báo cáo
            // Utiliti.FillTitleExport(db, sheet);

            //DateTime dateNow = DateTime.Now;
            //IRange rangeDay = sheet.FindFirst("<Day>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            //rangeDay.Text = rangeDay.Text.Replace("<Day>", dateNow.Day.ToString());
            //IRange rangeMonth = sheet.FindFirst("<Month>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            //rangeMonth.Text = rangeMonth.Text.Replace("<Month>", dateNow.Month.ToString());
            //IRange rangeYear = sheet.FindFirst("<Year>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            //rangeYear.Text = rangeYear.Text.Replace("<Year>", dateNow.Year.ToString());

            IRange rangeDateFrom = sheet.FindFirst("<DateFrom>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            rangeDateFrom.Text = rangeDateFrom.Text.Replace("<DateFrom>", (model.LogDateFrom.HasValue ? model.LogDateFrom.Value.ToString("dd/MM/yyy") : "--/--/----"));
            IRange rangeDateTo = sheet.FindFirst("<DateTo>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            rangeDateTo.Text = rangeDateTo.Text.Replace("<DateTo>", (model.LogDateTo.HasValue ? model.LogDateTo.Value.ToString("dd/MM/yyy") : "--/--/----"));

            int total = eventLogList.Count;
            int index = 1;
            var listExport = (from a in eventLogList
                              select new
                              {
                                  Index = index++,
                                  a.LogTypeName,
                                 // a.UserType,
                                  a.Description,
                                  CreateDate = a.CreateDate.HasValue ? a.CreateDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : string.Empty,
                                  a.UserName
                              }).ToList();

            IRange iRangeData = sheet.FindFirst("<Data>", ExcelFindType.Text, ExcelFindOptions.MatchCase);
            sheet.ImportData(listExport, iRangeData.Row, iRangeData.Column, false);

            if (saveOption.Equals("PDF"))
            {
                sheet.Range[iRangeData.Row, 1, total + 4, 6].Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                sheet.Range[iRangeData.Row, 1, total + 4, 6].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                sheet.Range[iRangeData.Row, 1, total + 4, 6].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                sheet.Range[iRangeData.Row, 1, total + 4, 6].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                sheet.Range[iRangeData.Row, 1, total + 4, 6].Borders.Color = ExcelKnownColors.Black;

                pathExport = "/Template/Export/" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "LichSuTruyCapSuDung.pdf";
                //convert the sheet to pdf

                ExcelToPdfConverter converter = new ExcelToPdfConverter(sheet);

                PdfDocument pdfDocument = new PdfDocument();

                pdfDocument = converter.Convert();

                pdfDocument.Save(HttpContext.Current.Server.MapPath(pathExport));

                pdfDocument.Close();

                converter.Dispose();

                workbook.Close();
                excelEngine.Dispose();
            }
            else
            {
                pathExport = "/Template/Export/" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "LichSuTruyCapSuDung.xlsx";
                workbook.SaveAs(HttpContext.Current.Server.MapPath(pathExport));
            }

            return pathExport;
        }
    }
}
