using GridViewApplication.Dto.RestEmployee;
using GridViewApplication.Library;
using GridViewApplication.Services;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication.Pages
{
    public partial class UserPage : Page
    {
        private readonly EmployeeService _employeeService = new EmployeeService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.Cookies["sessionid"] != null)
            {
                try
                {
                    gvUser.PageSize = int.Parse(ddlPageSize.SelectedValue);
                    dataEmployee();
                    Util.CreateLog("Process Binding Data...");
                }
                catch (Exception ex)
                {
                    Util.CreateLog($"FAILED : {Util.getDetail(ex)}");
                    showAlert("Error", "System error! Please, Contact Administrator.", "error");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvUser.PageIndex = 0;
            dataEmployee();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvUser.PageSize = int.Parse(ddlPageSize.SelectedValue);
            gvUser.PageIndex = 0;
            dataEmployee();
        }

        protected void gvUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUser.PageIndex = e.NewPageIndex;
            dataEmployee();
        }

        protected void lbtPage_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int pageIndex = Convert.ToInt32(btn.CommandArgument);
            gvUser.PageIndex = pageIndex - 1;
            dataEmployee();
        }

        private void pageTable(int totalRecords, int currentPage)
        {
            int pageSize = gvUser.PageSize;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pages = generatePageList(currentPage, totalPages);
            Repeater1.DataSource = pages;
            Repeater1.DataBind();

            Label1.Text = $"Halaman ke-{currentPage}";
        }
        protected void btnCreateEmployee_Click(object sender, EventArgs e)
        {
            pnlCreateEmployee.Visible = true;
            dataDepartment();
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string token = Request.Cookies["sessionid"]?.Value;
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            DateTime hireDate = DateTime.Parse(txtHireDate.Text);
            Guid departmentId = Guid.Parse(ddlDepartmentCreate.SelectedValue);
            string createName = Session["Username"]?.ToString() ?? "";
            createName = string.IsNullOrWhiteSpace(createName)
                ? ""
                : char.ToUpper(createName[0]) + createName.Substring(1).ToLower();

            var employee = new rest_create_employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                HireDate = hireDate,
                DepartmentId = departmentId,
                CreateDate = DateTime.Now,
                CreateName = createName
            };

            var service = new EmployeeService();
            service.CreateEmployee(token, employee);

            resetForm();
            pnlCreateEmployee.Visible = false;
            dataEmployee();
            showAlert("Success", "Employee created successfully!", "success");
        }

        protected void btnCancelEmployee_Click(object sender, EventArgs e)
        {
            resetForm();
            pnlCreateEmployee.Visible = false;
        }
        private void resetForm()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtHireDate.Text = string.Empty;
            ddlDepartmentCreate.ClearSelection();
        }

        private void dataDepartment()
        {
            string token = Request.Cookies["sessionid"]?.Value;
            var departments = DepartmentService.GetDepartments(token);
            ddlDepartmentCreate.DataSource = departments;
            ddlDepartmentCreate.DataTextField = "DepartmentName";
            ddlDepartmentCreate.DataValueField = "DepartmentId";
            ddlDepartmentCreate.DataBind();
        }


        private void dataEmployee()
        {
            try
            {
                string token = Request.Cookies["sessionid"]?.Value;
                Util.CreateLog($"Token from cookie: {token}");

                int pageNumber = gvUser.PageIndex + 1;
                int pageSize = gvUser.PageSize;
                string sortColumn = ddlSortColumn.SelectedValue;
                string sortDirection = ddlSortDirection.SelectedValue;
                string searchTerm = txtSearch.Text.Trim();

                Util.CreateLog($"Params - Page: {pageNumber}, Size: {pageSize}, Sort: {sortColumn} {sortDirection}, Search: {searchTerm}");

                var (users, totalCount) = _employeeService.GetEmployees(token, pageNumber, pageSize, sortColumn, sortDirection, searchTerm);

                gvUser.VirtualItemCount = totalCount;
                gvUser.DataSource = users;
                gvUser.DataBind();

                pageTable(totalCount, pageNumber);
            }
            catch (Exception ex)
            {
                Util.CreateLog("FAILED in Data Employee: " + Util.getDetail(ex));
                throw;
            }
        }

        protected void gvUser_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string token = Request.Cookies["sessionid"].Value;
            Guid userId = Guid.Parse(gvUser.DataKeys[e.RowIndex].Value.ToString());
            _employeeService.DeleteEmployee(token, userId);
            dataEmployee();
            showAlert("Success", "Data deleted successfully!", "success");
        }

        protected void gvUser_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUser.EditIndex = e.NewEditIndex;
            dataEmployee();
        }

        protected void gvUser_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUser.EditIndex = -1;
            dataEmployee();
        }

        protected void gvUser_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string token = Request.Cookies["sessionid"].Value;
            string userId = Convert.ToString(gvUser.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvUser.Rows[e.RowIndex];

            TextBox textFirstName = (TextBox)row.Cells[1].Controls[0];
            TextBox textLastName = (TextBox)row.Cells[2].Controls[0];
            TextBox textEmail = (TextBox)row.FindControl("txtEmailEdit");
            DropDownList ddlDepartment = (DropDownList)row.FindControl("ddlDepartment");
            string selectedDepartmentId = ddlDepartment?.SelectedValue;

            if (!System.Text.RegularExpressions.Regex.IsMatch(textEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                showAlert("Validation Error", "Invalid email format.", "warning");
                return;
            }

            var updated = new rest_employee
            {
                FirstName = textFirstName.Text,
                LastName = textLastName.Text,
                Email = textEmail.Text,
                DepartmentId = selectedDepartmentId,
            };

            _employeeService.UpdateEmployee(token, userId, updated);
            gvUser.EditIndex = -1;
            dataEmployee();
            showAlert("Success", "Data updated successfully!", "success");
        }

        protected void gvUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow &&
                (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlDepartment");
                if (ddl != null)
                {
                    string token = Request.Cookies["sessionid"]?.Value;
                    var departments = DepartmentService.GetDepartments(token);
                    ddl.DataSource = departments;
                    ddl.DataTextField = "DepartmentName";
                    ddl.DataValueField = "DepartmentId";
                    ddl.DataBind();

                    string currentDeptId = DataBinder.Eval(e.Row.DataItem, "DepartmentId")?.ToString();
                    if (!string.IsNullOrEmpty(currentDeptId))
                        ddl.SelectedValue = currentDeptId;
                }
            }
        }
        private List<PageItem> generatePageList(int currentPage, int totalPages)
        {
            var pages = new List<PageItem>();

            for (int i = 1; i <= totalPages; i++)
            {
                pages.Add(new PageItem
                {
                    PageNumber = i,
                    Value = i,
                    IsActive = (i == currentPage)
                });
            }

            return pages;
        }

        public class PageItem
        {
            public int PageNumber { get; set; }
            public int Value { get; set; }
            public bool IsActive { get; set; }
        }

        private void showAlert(string title, string message, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert",
                $"Swal.fire('{title}', '{message}', '{type}')", true);
        }
    }
}
