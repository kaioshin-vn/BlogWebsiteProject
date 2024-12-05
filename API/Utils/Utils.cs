using BlogWebsite.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace API.Utils
{
    public class Utils
    {
        public static ExcelPackage createExcelPackage(ApplicationDbContext _context, int year)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Test Report";
            package.Workbook.Properties.Author = "Tan";
            package.Workbook.Properties.Subject = "Test Report";
            package.Workbook.Properties.Keywords = "Testing";

            var worksheet = package.Workbook.Worksheets.Add("Tài khoản");
            //First Add the headers
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "ID";
            worksheet.Cells[1, 3].Value = "Tên đăng nhập";
            worksheet.Cells[1, 4].Value = "Tên tài khoản";
            worksheet.Cells[1, 5].Value = "Email";
            worksheet.Cells[1, 6].Value = "Ngày tạo tài khoản";
            //Add values
            var numberFormat = "#.##0";
            var dataCellStyleName = "TableNumber";
            var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            numStyle.Style.Numberformat.Format = numberFormat;

            var listUser = _context.Users.Where(a => a.CreateTime.Value.Year == year);

            var count = 1;
            foreach (var item in listUser)
            {
                count++;
                worksheet.Cells[count, 1].Value = count - 1;
                worksheet.Cells[count, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[count, 2].Value = item.Id;
                worksheet.Cells[count, 3].Value = item.FullName;
                worksheet.Cells[count, 4].Value = item.UserName;
                worksheet.Cells[count, 5].Value = item.Email;
                worksheet.Cells[count, 6].Value = item.CreateTime.Value.ToString("dd/MM/yyyy");
            }


            var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: count, toColumn: 6), "Data");
            tbl.ShowHeader = true;
            tbl.TableStyle = OfficeOpenXml.Table.TableStyles.Dark9;
            tbl.ShowTotal = true;

            worksheet.Cells[1, 1, count, 6].AutoFitColumns();


            ///////////////////////////////
            var worksheet1 = package.Workbook.Worksheets.Add("Bài viết");
            //First Add the headers
            worksheet1.Cells[1, 1].Value = "STT";
            worksheet1.Cells[1, 2].Value = "ID";
            worksheet1.Cells[1, 3].Value = "Người đăng";
            worksheet1.Cells[1, 4].Value = "Tiêu đề";
            worksheet1.Cells[1, 5].Value = "Like";
            worksheet1.Cells[1, 6].Value = "Ngày tạo";

            var listPost = _context.Posts.Include(a => a.User).Where(a => a.CreateDate.Value.Year == year && a.IsDeleted == false);

            count = 1;
            foreach (var item in listPost)
            {
                count++;
                worksheet1.Cells[count, 1].Value = count - 1;
                worksheet1.Cells[count, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet1.Cells[count, 2].Value = item.Id;
                worksheet1.Cells[count, 3].Value = item.User.FullName;
                worksheet1.Cells[count, 4].Value = item.Title;
                worksheet1.Cells[count, 5].Value = item.Like.Split('|').Count().ToString();
                worksheet1.Cells[count, 6].Value = item.CreateDate.Value.ToString("dd/MM/yyyy");
            }


            var tbl1 = worksheet1.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: count, toColumn: 6), "Data1");
            tbl1.ShowHeader = true;
            tbl1.TableStyle = OfficeOpenXml.Table.TableStyles.Dark9;
            tbl1.ShowTotal = true;

            worksheet1.Cells[1, 1, count, 6].AutoFitColumns();

            ///////////////////
            var worksheet2 = package.Workbook.Worksheets.Add("Nhóm");
            //First Add the headers
            worksheet2.Cells[1, 1].Value = "STT";
            worksheet2.Cells[1, 2].Value = "ID";
            worksheet2.Cells[1, 3].Value = "Tên nhóm";
            worksheet2.Cells[1, 4].Value = "Số lượng thành viên";
            worksheet2.Cells[1, 5].Value = "Ngày tạo";

            var listGr = _context.Groups.Include(a => a.MemberGroups).Where(a => a.DateTime.Year == year && a.isDeleted == false);

            count = 1;
            foreach (var item in listGr)
            {
                count++;
                worksheet2.Cells[count, 1].Value = count - 1;
                worksheet2.Cells[count, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet2.Cells[count, 2].Value = item.IdGroup;
                worksheet2.Cells[count, 3].Value = item.Name;
                worksheet2.Cells[count, 4].Value = item.MemberGroups.Count.ToString();
                worksheet2.Cells[count, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet2.Cells[count, 5].Value = item.DateTime.ToString("dd/MM/yyyy");
            }


            var tbl2 = worksheet2.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: count, toColumn: 5), "Data2");
            tbl2.ShowHeader = true;
            tbl2.TableStyle = OfficeOpenXml.Table.TableStyles.Dark9;
            tbl2.ShowTotal = true;

            worksheet2.Cells[1, 1, count, 5].AutoFitColumns();

            return package;
        }

    }
}
