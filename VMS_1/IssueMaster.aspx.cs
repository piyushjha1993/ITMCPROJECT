using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class IssueMaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack)
            {
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
            string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
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
            string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
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


        //private void PopulateItemNamesDropdown(string selectedCategory)
        //{
        //    string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
        //    string query = "SELECT ItemName FROM Items WHERE CategoryID = (SELECT CategoryID FROM ItemCategories WHERE CategoryName = @CategoryName)";

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connStr))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@CategoryName", selectedCategory);
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                string itemName = reader["ItemName"].ToString();
        //                ListItem item = new ListItem(itemName);
        //                itemname.Items.Add(item);
        //            }

        //            reader.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblStatus.Text = "An error occurred while populating item names dropdown: " + ex.Message;
        //    }
        //}

        //protected void itemcategory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    itemname.Items.Clear();
        //    PopulateItemNamesDropdown(itemcategory.SelectedItem.Text);
        //}

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";

                string[] date = Request.Form.GetValues("date");
                string[] itemcategory = Request.Form.GetValues("itemcategory");
                string[] itemname = Request.Form.GetValues("itemname");
                string[] enterstrength = Request.Form.GetValues("Strength");
                string[] qtyentitled = Request.Form.GetValues("val");
                string[] qtyissued = Request.Form.GetValues("Qtyissued");
                string[] denomination = Request.Form.GetValues("denom");
                string[] role = Request.Form.GetValues("userrole");
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
