using GridViewApplication.Dto;
using GridViewApplication.Dto.RestLogin;
using GridViewApplication.Library;
using GridViewApplication.Repository;
using GridViewApplication.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication.Forms
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = Repository1.BindGridViewDataset();
                UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
                Util.CreateLog("Process Binding Data...");
            }
        }

        private void ShowAlert(string title, string message, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                $"Swal.fire('{title}', '{message}', '{type}')", true);
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            string email = Mail.Text;

            if (Repository1.IsEmailExist(email, 0))
            {
                ShowAlert("Error", "Email already exists. Please use a different email.", "error");
                Reset();
            }
            else
            {
                Repository1.InsertData(Name.Text, email, Phone.Text, Age.Text);
                DataTable dt = Repository1.BindGridViewDataset();
                UtilUi.BindGrid(this.Page, GvData, dt, RepeaterPage, lbPage);
                Reset();
                ShowAlert("Success", "Data inserted successfully!", "success");
            }
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

        private void Reset()
        {
            Name.Text = Mail.Text = Age.Text = Phone.Text = string.Empty;
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
