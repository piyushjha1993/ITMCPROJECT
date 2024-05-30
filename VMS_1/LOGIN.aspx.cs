using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace VMS_1
{
    public partial class LOGIN : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = UserName.Text;
            string password = Password.Text;

            if (ValidateUser(username, password))
            {
                // If user is valid, redirect to Home.aspx
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                // If user is not valid, display error message
                ErrorLabel.Visible = true;
                ErrorLabel.Text = "Invalid username or password";
                UserName.Focus();
            }
        }
        private bool ValidateUser(string username, string password)
        {
            string connectionString = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=SSPI";
            string query = "SELECT COUNT(*) FROM usermaster WHERE NudId = @Username AND Password = @Password";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                // Execute the query and retrieve the result
                int count = (int)cmd.ExecuteScalar();

                // If count is greater than 0, user is valid
                return count > 0;
            }
        }
    }
}

