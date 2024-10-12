using OfficeOpenXml;

namespace API.Utils
{
    public class Utils
    {
        public static ExcelPackage createExcelPackage()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Test Report";
            package.Workbook.Properties.Author = "Tan";
            package.Workbook.Properties.Subject = "Test Report";
            package.Workbook.Properties.Keywords = "Testing";

            var worksheet = package.Workbook.Worksheets.Add("Employee");
            //First Add the headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Gender";
            worksheet.Cells[1, 4].Value = "Salary (in $)";
            //Add values
            var numberFormat = "#.##0";
            var dataCellStyleName = "TableNumber";
            var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            numStyle.Style.Numberformat.Format = numberFormat;

            worksheet.Cells[2, 1].Value = 1;
            worksheet.Cells[2, 2].Value = "Giap";
            worksheet.Cells[2, 3].Value = "Nam";
            worksheet.Cells[2, 4].Value = 50000;
            worksheet.Cells[2, 4].Style.Numberformat.Format = numberFormat;

            worksheet.Cells[3, 1].Value = 2;
            worksheet.Cells[3, 2].Value = "Tan";
            worksheet.Cells[3, 3].Value = "Nam";
            worksheet.Cells[3, 4].Value = 50000;
            worksheet.Cells[23, 4].Style.Numberformat.Format = numberFormat;

            worksheet.Cells[4, 1].Value = 3;
            worksheet.Cells[4, 2].Value = "Trang";
            worksheet.Cells[4, 3].Value = "Nu";
            worksheet.Cells[4, 4].Value = 50000;
            worksheet.Cells[4, 4].Style.Numberformat.Format = numberFormat;

            worksheet.Cells[5, 1].Value = 4;
            worksheet.Cells[5, 2].Value = "Tho";
            worksheet.Cells[5, 3].Value = "Nam";
            worksheet.Cells[5, 4].Value = 50000;
            worksheet.Cells[5, 4].Style.Numberformat.Format = numberFormat;

            var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: 5, toColumn: 4), "Data");
            tbl.ShowHeader = true;
            tbl.TableStyle = OfficeOpenXml.Table.TableStyles.Dark9;
            tbl.ShowTotal = true;
            tbl.Columns[3].DataCellStyleName = dataCellStyleName;
            tbl.Columns[3].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
            worksheet.Cells[5, 4].Style.Numberformat.Format = numberFormat;

            worksheet.Cells[1, 1, 5, 4].AutoFitColumns();
            return package;
        }

    }
}
