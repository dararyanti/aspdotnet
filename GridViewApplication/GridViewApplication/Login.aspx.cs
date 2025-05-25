using GridViewApplication.Dto;
using GridViewApplication.Dto.RestLogin;
using GridViewApplication.Library;
using GridViewApplication.Services;
using System;
using System.Web;
using System.Web.UI;

namespace GridViewApplication
{
    public partial class Login : Page
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
                login.username = txtUsername.Text;
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
                Session["Username"] = login.username;

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
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}