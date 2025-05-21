using GridViewApplication.Dto;
using GridViewApplication.Dto.RestLogin;
using GridViewApplication.Library;
using GridViewApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void ShowAlert(string title, string message, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                $"Swal.fire('{title}', '{message}', '{type}')", true);
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginDto login = new LoginDto();
                login.username = txtEMail.Text;
                login.password = txtPassword.Text;

                rest_response_login respose = LoginService.Login(login);
                if (!string.IsNullOrEmpty(LoginService.errorMsg))
                {
                    ShowAlert("Error", LoginService.errorMsg, "error");
                    LoginService.errorMsg = string.Empty;
                    return;
                }

                HttpCookie cookie = new HttpCookie("sessionid");
                cookie.Value = respose.jwtToken;
                cookie.Expires = DateTime.Now.AddHours(3);
                Response.SetCookie(cookie);

                RedirectPage();
            }
            catch (Exception ex)
            {
                Util.CreateLog(string.Format("FAILED : {0} - {1}", EmployeeService.errorMsg, Util.getDetail(ex)));
                ShowAlert("Error", "System error! Please, Contact Administrator.", "error");
                return;
            }
        }

        private void RedirectPage()
        {
            Response.Redirect("/Pages/UserPage.aspx",false);
        }
    }
}