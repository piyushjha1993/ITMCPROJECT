using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class IssueMaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack)
            {
                if (ViewState["DataTable1"] == null)
                {
                    ViewState["DataTable1"] = new DataTable();
                }
                GetItemCategories();
            }
        }

        //private void PopulateItemCategoriesDropdown()
        //{
        //    string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
        //    string query = "SELECT CategoryName FROM ItemCategories";

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connStr))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                string categoryName = reader["CategoryName"].ToString();
        //                ListItem item = new ListItem(categoryName);
        //                itemcategory.Items.Add(item);
        //            }

        //            reader.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblStatus.Text = "An error occurred while populating item categories dropdown: " + ex.Message;
        //    }
        //}

        [WebMethod]
        public static List<object> GetItemCategories()
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM Items";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            var categories = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new
                {
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemID"].ToString(),
                    ScaleAmount = row["RationScaleOfficer"].ToString()
                });
            }

            return categories;
        }

        [WebMethod]
        public static string GetItemNamesByCategory(string category)
        {
            List<string> itemNames = new List<string>();

            // Your SQL query to fetch item names based on the selected category
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM AlternateItem where ItemID = @Category";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", category);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemNames.Add(reader["AltItemName"].ToString());
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(itemNames); ;
        }


        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] itemcategory = Request.Form.GetValues("itemcategory");
                string[] itemname = Request.Form.GetValues("itemname");
                string[] enterstrength = Request.Form.GetValues("Strength");
                string[] qtyentitled = Request.Form.GetValues("entitledstrength");
                string[] qtyissued = Request.Form.GetValues("Qtyissued");
                string[] denomination = Request.Form.GetValues("denom");
                string[] role = Request.Form.GetValues("userrole");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int maxLength = Math.Max(date.Length, Math.Max(itemcategory.Length, Math.Max(itemname.Length, Math.Max(enterstrength.Length, Math.Max(qtyentitled.Length, Math.Max(qtyissued.Length, Math.Max(denomination.Length, role.Length)))))));
                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < maxLength; i++)
                    {
                        SqlCommand cmd = new SqlCommand("InsertIssue", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Date", i < date.Length ? date[i] : date[0]);
                        cmd.Parameters.AddWithValue("@ItemCategoryId", i < itemcategory.Length ? itemcategory[i] : itemcategory[0]);
                        cmd.Parameters.AddWithValue("@ItemName", itemname[i]);
                        cmd.Parameters.AddWithValue("@Strength", enterstrength[i]);
                        cmd.Parameters.AddWithValue("@QtyEntitled", i < qtyentitled.Length ? qtyentitled[i] : qtyentitled[0]);
                        cmd.Parameters.AddWithValue("@QtyIssued", qtyissued[i]);
                        cmd.Parameters.AddWithValue("@Denomination", denomination[i]);
                        cmd.Parameters.AddWithValue("@Role", i < role.Length ? role[i] : role[0]);

                        cmd.ExecuteNonQuery();
                    }
                }
                lblStatus.Text = "Data entered successfully.";

                // Refresh the GridView after data insertion
                BindGridView();

                // Bind the total GridView after data insertion
                BindTotalGridView((DataTable)ViewState["DataTable"]);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BindGridView()
        {
            try
            {
                string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string firstSubmittedDate = Request.Form.GetValues("date")[0];
                DateTime dateTime = DateTime.Parse(firstSubmittedDate);
                string monthFilter = dateTime.ToString("yyyy-MM");

                string query = "SELECT * FROM IssueMaster WHERE CONVERT(VARCHAR(7), Date, 120) = @Month";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Month", monthFilter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Store DataTable in ViewState
                    ViewState["DataTable"] = dt;


                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                lblStatus.Text = "An error occurred while fetching data: " + ex.Message;
            }
        }

        private void BindTotalGridView(DataTable dt)
        {
            // Calculate totals for each column except "Date"
            DataRow totalRow = dt.NewRow();
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(int) && column.ColumnName != "Date")
                {
                    totalRow[column.ColumnName] = dt.Compute($"SUM([{column.ColumnName}])", "");
                }
            }

            // Add the total row to the DataTable
            dt.Rows.Add(totalRow);

            // Bind the totals to the second GridView
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

    }
}
