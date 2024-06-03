using System;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Configuration;
using System.Web.Security;
using System.Web;
using System.Web.UI.WebControls;
using System.Threading;
using DocumentFormat.OpenXml.Bibliography;

namespace VMS_1
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private DataTable dtMonthStock;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridViewPresentStock();
            LoadGridViewPage2to7();
            LoadGridViewMonthStock();
            FilterDataByMonth(Convert.ToString(DateTime.Now.Month));
        }

        private void LoadGridViewMonthStock()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MonthEndStockMaster", conn);
                    dtMonthStock = new DataTable();
                    da.Fill(dtMonthStock);

                    GridViewMonthStock.DataSource = dtMonthStock;
                    GridViewMonthStock.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridViewMonthStock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int dateColumnIndex = 1;
                foreach (DataControlField field in GridViewMonthStock.Columns)
                {
                    if (field.HeaderText == "Date") // Replace "Date" with the actual header text of your date column
                    {
                        dateColumnIndex = GridViewMonthStock.Columns.IndexOf(field);
                        break;
                    }
                }

                if (dateColumnIndex >= 0)
                {
                    string dateText = e.Row.Cells[dateColumnIndex].Text;
                    if (!string.IsNullOrEmpty(dateText))
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(dateText, out dateValue))
                        {
                            e.Row.Cells[dateColumnIndex].Text = dateValue.ToString("MMMM yyyy");
                        }
                    }
                }

                int typeColumnIndex = 4; 
                string type = e.Row.Cells[typeColumnIndex].Text;
                if (type == "Issue")
                {
                    e.Row.CssClass = "issue-row";
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedMonth = ddlMonths.SelectedValue;

            DataTable filteredData = FilterDataByMonth(selectedMonth);

            GridViewMonthStock.DataSource = filteredData;
            GridViewMonthStock.DataBind();
        }

        private DataTable FilterDataByMonth(string month)
        {
            DataTable filteredData = dtMonthStock.Clone(); 
            foreach (DataRow row in dtMonthStock.Rows)
            {
                DateTime dateValue;
                if (DateTime.TryParse(row["Date"].ToString(), out dateValue))
                {
                    if (dateValue.Month == int.Parse(month))
                    {
                        filteredData.ImportRow(row);
                    }
                }
            }

            return filteredData;
        }


        private void LoadGridViewPresentStock()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM PresentStockMaster", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewPresentStock.DataSource = dt;
                    GridViewPresentStock.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        private void LoadGridViewPage2to7()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(@"SELECT 
                                                PS.*
                                            FROM 
                                                PresentStockMaster PS 
                                            LEFT JOIN 
                                                IssueMaster ISM ON PS.ItemName = ISM.ItemName", conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewP2to7.DataSource = dt;
                    GridViewP2to7.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void ExportPresentStockButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                DataTable dt = new DataTable();
                foreach (TableCell cell in GridViewPresentStock.HeaderRow.Cells)
                {
                    dt.Columns.Add(cell.Text);
                }
                foreach (GridViewRow row in GridViewPresentStock.Rows)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        dr[i] = row.Cells[i].Text;
                    }
                    dt.Rows.Add(dr);
                }

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PresentStock");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                    // Save the file
                    string fileName = $"PresentStock_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
                    FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                    excelPackage.SaveAs(excelFile);

                    // Provide download link
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    Response.TransmitFile(excelFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value; // Assuming monthYearPicker.Value holds the selected month and year in YYYY-MM format
            DataTable dt = FetchStrengthData(monthYear);
            object sumNonEntitledOfficer = dt.Compute("SUM(NonEntitled_Officer)", "");
            object sumNonEntitledSailor = dt.Compute("SUM(NonEntitled_Sailor)", "");
            object sumCivilian = dt.Compute("SUM(Civilian)", "");
            dt.Columns.Remove("NonEntitled_Officer");
            dt.Columns.Remove("NonEntitled_Sailor");
            dt.Columns.Remove("Civilian");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Export to Excel
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Strength Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;

                // Setting the column widths and header values
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[4, i + 1].Value = dt.Columns[i].ColumnName;
                    worksheet.Column(i + 1).Width = 15;
                }

                // Formatting for header rows
                ExcelRange headerRange = worksheet.Cells["A1:J1"];
                headerRange.Merge = true;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                headerRange.Value = "PAGE 13";

                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2:J2"].Style.Font.Bold = true;
                worksheet.Cells["A2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:J2"].Value = DateTime.Parse(monthYear).ToString("MMMM yyyy").ToUpper();

                worksheet.Cells["A3:J3"].Merge = true;
                worksheet.Cells["A3:J3"].Style.Font.Bold = true;
                worksheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:J3"].Value = "NUMBER VICTUALLED";

                worksheet.Cells["A4:J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A4:J4"].Style.Font.Bold = true;
                worksheet.Cells["A4"].Value = "Date";
                worksheet.Cells["B4:F4"].Merge = true;
                worksheet.Cells["B4:F4"].Value = "OFFICERS";
                worksheet.Cells["G4:J4"].Merge = true;
                worksheet.Cells["G4:J4"].Value = "SAILORS";

                worksheet.Cells["A5:J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A5:J5"].Style.Font.Bold = true;
                worksheet.Cells["A5"].Value = "";
                worksheet.Cells["B5"].Value = "S";
                worksheet.Cells["C5"].Value = "V";
                worksheet.Cells["D5"].Value = "RIK";
                worksheet.Cells["E5"].Value = "Hon.";
                worksheet.Cells["F5"].Value = "Total";
                worksheet.Cells["G5"].Value = "S";
                worksheet.Cells["H5"].Value = "V";
                worksheet.Cells["I5"].Value = "RIK";
                worksheet.Cells["J5"].Value = "Total";

                worksheet.Cells["A37:C37"].Merge = true;
                worksheet.Cells["A37:C37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A37:C37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A37:C37"].Value = "NON ENTITLED";
                worksheet.Cells["D37:E37"].Merge = true;
                worksheet.Cells["D37:E37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D37:E37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D37:E37"].Value = "OFFICERS";
                worksheet.Cells["G37:I37"].Merge = true;
                worksheet.Cells["G37:I37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G37:I37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["G37:I37"].Value = "SAILORS";
                worksheet.Cells["A38:I38"].Merge = true;
                worksheet.Cells["A38:I38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A38:I38"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A38:I38"].Value = "NON ENTITLED CIVILIANS";
                worksheet.Cells["A39"].Style.Font.Bold = true;
                worksheet.Cells["A39"].Value = "TOTAL";
                worksheet.Cells["F37"].Value = sumNonEntitledOfficer;
                worksheet.Cells["F37"].Style.Font.Bold = true;
                worksheet.Cells["F37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J37"].Value = sumNonEntitledSailor;
                worksheet.Cells["J37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J37"].Style.Font.Bold = true;
                worksheet.Cells["J38"].Value = sumCivilian;
                worksheet.Cells["J38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J38"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J38"].Style.Font.Bold = true;
                worksheet.Cells["B39"].Formula = string.Format("SUM(B6:B36)");
                worksheet.Cells["B39"].Style.Font.Bold = true;
                worksheet.Cells["B39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["C39"].Formula = string.Format("SUM(C6:C36)");
                worksheet.Cells["C39"].Style.Font.Bold = true;
                worksheet.Cells["C39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D39"].Formula = string.Format("SUM(D6:D36)");
                worksheet.Cells["D39"].Style.Font.Bold = true;
                worksheet.Cells["D39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["F39"].Formula = string.Format("SUM(F6:F36)");
                worksheet.Cells["F39"].Style.Font.Bold = true;
                worksheet.Cells["F39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["G39"].Formula = string.Format("SUM(G6:G36)");
                worksheet.Cells["G39"].Style.Font.Bold = true;
                worksheet.Cells["G39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["H39"].Formula = string.Format("SUM(H6:H36)");
                worksheet.Cells["H39"].Style.Font.Bold = true;
                worksheet.Cells["H39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["H39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["I39"].Formula = string.Format("SUM(I6:I36)");
                worksheet.Cells["I39"].Style.Font.Bold = true;
                worksheet.Cells["I39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["I39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J39"].Formula = string.Format("SUM(J6:J38)");
                worksheet.Cells["J39"].Style.Font.Bold = true;
                worksheet.Cells["J39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Set the row height
                for (int i = 1; i <= 39; i++)
                {
                    worksheet.Row(i).Height = 15;
                }

                // Formatting for data rows and header rows
                using (ExcelRange rng = worksheet.Cells["A1:J39"])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                int rowIndex = 6; // Starting row for data
                foreach (DataRow row in dt.Rows)
                {
                    DateTime dateValue = (DateTime)row["Date"];
                    worksheet.Cells[rowIndex, 1].Value = dateValue.ToString("dd MMM yyyy");
                    worksheet.Cells[rowIndex, 2].Value = row["Veg_Officer"];
                    worksheet.Cells[rowIndex, 3].Value = row["NonVeg_Officer"];
                    worksheet.Cells[rowIndex, 4].Value = row["RIK_Officer"];
                    worksheet.Cells[rowIndex, 6].Value = row["Total_day_officer"];
                    worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 7].Value = row["Veg_Sailor"];
                    worksheet.Cells[rowIndex, 8].Value = row["NonVeg_Sailor"];
                    worksheet.Cells[rowIndex, 9].Value = row["RIK_Sailor"];
                    worksheet.Cells[rowIndex, 10].Value = row["Total_day_sailor"];
                    worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                    rowIndex++;
                }

                // Save the file
                string fileName = $"Strength_{DateTime.Parse(monthYear).ToString("MMMM_yyyy")}.xlsx";
                FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                excelPackage.SaveAs(excelFile);

                // Provide download link
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(excelFile.FullName);
                Response.Flush();
                Response.End();
            }
        }

        private DataTable FetchStrengthData(string monthYear)
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT * FROM Strength WHERE ISDATE(CONVERT(VARCHAR(10), Date, 120)) = 1 AND CONVERT(VARCHAR(7), Date, 120) = @Month";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", monthYear);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }
    }
}
