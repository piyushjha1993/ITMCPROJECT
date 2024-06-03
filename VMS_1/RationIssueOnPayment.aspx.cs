using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace VMS_1
{
    public partial class RationIssueOnPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                //LoadGridView();
                GetItems();
            }
        }

        [WebMethod]
        public static List<object> GetItems()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM RationScale";
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
            var items = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new
                {
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemId"].ToString(),
                });
            }

            return items;
        }

        [WebMethod]
        public static string GetItemDataByItemId(string Id)
        {
            List<string> itemsData = new List<string>();

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM RationScale WHERE ItemId = @ItemId";
            string itemNameQuery = "SELECT ItemName FROM RationScale WHERE ItemId = @ItemId";
            string referenceQuery = "SELECT DISTINCT ReferenceNo FROM RationScale WHERE ItemName = @ItemName";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemId", Id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemsData.Add(reader["Rate"].ToString());
                        }
                    }
                }

                string itemName;
                using (SqlCommand itemNameCmd = new SqlCommand(itemNameQuery, conn))
                {
                    itemNameCmd.Parameters.AddWithValue("@ItemId", Id);
                    itemName = itemNameCmd.ExecuteScalar()?.ToString();
                }

                if (!string.IsNullOrEmpty(itemName))
                {
                    // Use the ItemName to get the ReferenceNos
                    using (SqlCommand referenceCmd = new SqlCommand(referenceQuery, conn))
                    {
                        referenceCmd.Parameters.AddWithValue("@ItemName", itemName);
                        using (SqlDataReader referenceReader = referenceCmd.ExecuteReader())
                        {
                            while (referenceReader.Read())
                            {
                                itemsData.Add(referenceReader["ReferenceNo"].ToString());
                            }
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(itemsData);
        }


    }
}