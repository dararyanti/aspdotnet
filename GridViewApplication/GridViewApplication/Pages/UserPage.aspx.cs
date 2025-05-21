using GridViewApplication.Dto.RestEmployee;
using GridViewApplication.Library;
using GridViewApplication.Repository;
using GridViewApplication.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication.Pages
{
    public partial class UserPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.Cookies["sessionid"] != null)
                    {
                        GvUser_DataTable();
                    }
                }
                catch (Exception ex)
                {
                    Util.CreateLog(string.Format("FAILED : {0} - {1}", EmployeeService.errorMsg, Util.getDetail(ex)));
                    ShowAlert("Error", "System error! Please, Contact Administrator.", "error");
                    return;
                }

                DataTable dt = Repository1.BindGridViewDataset();
                UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
                Util.CreateLog("Process Binding Data...");
            }
        }

        private void GvUser_DataTable()
        {
            string token = Request.Cookies["sessionid"].Value;
            Util.CreateLog(string.Format("Token : {0}", token));
            List<rest_employee> respose = EmployeeService.getListEmployee(token, null, null, null, "true", "1", "10");
            if (!string.IsNullOrEmpty(EmployeeService.errorMsg))
            {
                ShowAlert("Error", EmployeeService.errorMsg, "error");
                EmployeeService.errorMsg = string.Empty;
                return;
            }
            gvUser.DataSource = respose;
            gvUser.DataBind();
        }

        protected void GvUser_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string token = Request.Cookies["sessionid"].Value;
            Util.CreateLog(string.Format("Token : {0}", token));
            string userId = Convert.ToString(gvUser.DataKeys[e.RowIndex].Value);
            Console.WriteLine("userId", userId);
            EmployeeService.deleteEmployee(token, userId);
            GvUser_DataTable();
            ShowAlert("Success", "Data deleted successfully!", "success");
        }

        protected void GvUser_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUser.EditIndex = e.NewEditIndex;
            string token = Request.Cookies["sessionid"].Value;
            GvUser_DataTable();
        }

        protected void GvUser_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GvUser_CancelEdit();
        }

        private void GvUser_CancelEdit()
        {
            gvUser.EditIndex = -1;
            GvUser_DataTable();
        }

        protected void GvUser_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string token = Request.Cookies["sessionid"].Value;
            string userId = Convert.ToString(gvUser.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvUser.Rows[e.RowIndex];
            TextBox textName = (TextBox)row.Cells[2].Controls[0];
            TextBox textDept = (TextBox)row.Cells[3].Controls[0];

            DropDownList ddlPosition = (DropDownList)row.FindControl("ddlPosition");
            string selectedPositionId = ddlPosition?.SelectedValue;

            EmployeeService.updateEmployee(token, userId, textName.Text, textDept.Text, selectedPositionId);
            GvUser_CancelEdit();
            ShowAlert("Success", "Data updated successfully!", "success");
            
        }

        protected void gvUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlPosition");
                if (ddl != null)
                {
                    string token = Request.Cookies["sessionid"]?.Value;
                    var positions = PositionService.GetPositions(token);
                    ddl.DataSource = positions;
                    ddl.DataTextField = "positionName";
                    ddl.DataValueField = "id";
                    ddl.DataBind();

                    string currentPosId = DataBinder.Eval(e.Row.DataItem, "position.id")?.ToString();
                    if (!string.IsNullOrEmpty(currentPosId))
                        ddl.SelectedValue = currentPosId;
                }
            }
        }


        //====================================================GVDATA=============================================

        private void ShowAlert(string title, string message, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                $"Swal.fire('{title}', '{message}', '{type}')", true);
        }
        protected void GvData_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(GvData.DataKeys[e.RowIndex].Value);
            Repository1.DeleteData(userId);
            DataTable dt = Repository1.BindGridViewDataset();
            UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
            ShowAlert("Success", "Data deleted successfully!", "success");
        }

        protected void GvData_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GvData.EditIndex = e.NewEditIndex;
            DataTable dt = Repository1.BindGridViewDataset();
            UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
        }

        protected void GvData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            CancelEditing();
        }

        protected void Gvdata_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int userId = Convert.ToInt32(GvData.DataKeys[e.RowIndex].Value);
            GridViewRow row = GvData.Rows[e.RowIndex];
            TextBox textName = (TextBox)row.Cells[1].Controls[0];
            TextBox textMail = (TextBox)row.Cells[2].Controls[0];
            TextBox textPhone = (TextBox)row.Cells[3].Controls[0];
            TextBox textAge = (TextBox)row.Cells[4].Controls[0];

            if (Repository1.IsEmailExist(textMail.Text, userId))
            {
                ShowAlert("Error", "Email already exists. Please use a different email.", "error");
                return;
            }

            Repository1.UpdateData(userId, textName.Text, textMail.Text, textPhone.Text, textAge.Text);
            CancelEditing();
            ShowAlert("Success", "Data updated successfully!", "success");
        }

        private void CancelEditing()
        {
            GvData.EditIndex = -1;
            DataTable dt = Repository1.BindGridViewDataset();
            UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
        }

        protected void GvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvData.PageIndex = e.NewPageIndex;
            DataTable dt = Repository1.BindGridViewDataset();
            UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
        }

        protected void lbtPage_Click(object sender, EventArgs e)
        {
            int MaxPageNumberDisplayed = 10;

            LinkButton lb = (LinkButton)sender;
            int SelPageNumber = 0;
            if (lb.Text != "<<" && lb.Text != ">>")
            {
                SelPageNumber = int.Parse(lb.Text);
                SelPageNumber = SelPageNumber - 1;
            }
            else
            {
                int PageNumberIndex = 1;
                if (lb.Text == "<<")
                {
                    PageNumberIndex = GvData.PageIndex;
                    SelPageNumber = PageNumberIndex - MaxPageNumberDisplayed;
                    if (SelPageNumber < 0)
                    {
                        SelPageNumber = 0;
                    }
                }
                else if (lb.Text == ">>")
                {
                    PageNumberIndex = GvData.PageIndex;
                    SelPageNumber = PageNumberIndex + MaxPageNumberDisplayed;
                }
            }
            GvData.PageIndex = SelPageNumber;
            DataTable dt = Repository1.BindGridViewDataset();
            UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
        }

    }
}