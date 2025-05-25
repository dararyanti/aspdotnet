<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPage.aspx.cs" Inherits="GridViewApplication.Pages.UserPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User</title>

    <!-- Bootstrap -->
    <link href="../Assets/Vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Assets/Css/Font-Awesome-6.x/css/all.min.css" rel="stylesheet" />

    <!-- Swal New -->
    <script src="../Assets/Vendors/sweetalert2-11.21.0/package/dist/sweetalert2.all.min.js"></script>
    
    <script>
        function showAlert(title, message, type) {
            Swal.fire(title, message, type);
        }
    </script>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f1f1f1;
            background-image: url("https://www.transparenttextures.com/patterns/arches.png");
            padding: 20px;
        }
        .container {
            max-width: 360px;
            margin: auto;
            background-color: #fff;
            padding: 15px;
            border-radius: 5px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .form-table {
            width: 100%;
        }
        .form-table td {
            padding: 6px;
        }
        .form-table input[type="text"],
        .form-table input[type="email"],
        .form-table input[type="number"] {
            width: 100%;
            padding: 4px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        .form-table label {
            font-weight: bold;
        }
        .form-table .btn-submit {
            background-color: #007BFF;
            color: #fff;
            border: none;
            padding: 6px 10px;
            border-radius: 4px;
            cursor: pointer;
        }
        .form-table .btn-submit:hover {
            background-color: #0056b3;
        }
        .grid-view {
            margin-top: 30px;
        }
        .grid-view .fa-trash {
            color: #cc0000;
            cursor: pointer;
        }
        .grid-view table {
            width: 100%;
            border-collapse: collapse;
        }
        .grid-view th,
        .grid-view td {
            padding: 10px;
            border: 1px solid #ccc;
            text-align: left;
        }
        .grid-view th {
            background-color: #007BFF;
            color: #fff;
        }
        input::-webkit-outer-spin-button,
        input::-webkit-inner-spin-button {
            -webkit-appearance: none;
            margin: 0;
        }
        .edit-icon,
        .delete-icon,
        .update-icon,
        .cancel-icon {
            margin-right: 10px;
            text-decoration: none;
            outline: none !important;
        }
        .no-outline:focus {
            outline: none !important;
        }
        .no-highlight {
            background-color: transparent;
            border: none;
            font-size: 16PX;
        }
        .no-highlight:focus {
            border: none;
            outline: none !important;
        }
        .action-column {
            width: 95px;
        }
        .action-column a {
            margin-right: 5px;
        }
        .pagination .page-item {
            list-style: none;
            margin-right: 5px;
        }
        .pagination .page-link {
            background-color: #fff;
            color: #000000;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            text-align: center;
            text-decoration: none;
            border: 1px solid #ccc;
        }

        .pagination .page-link:hover {
            background-color: #ccc;
            color: #000000;
        }

        .pagination .page-link,
        .form-select.page-link {
            background-color: #fff;
            color: #000000;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            text-align: center;
            text-decoration: none;
        }

        .pagination .page-link:hover,
        .form-select.page-link:hover {
            background-color: #ccc;
            color: #000000;
        }

        .pagination .page-item.active .page-link {
            background-color: #0056b3;
            color: #fff;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h4><b>Employee List</b></h4>
        <asp:TextBox ID="txtSearch" runat="server" Placeholder="Search..." />
        <asp:DropDownList ID="ddlSortColumn" runat="server">
            <asp:ListItem Text="First Name" Value="first_name" />
            <asp:ListItem Text="Last Name" Value="last_name" />
            <asp:ListItem Text="Email" Value="email" />
            <asp:ListItem Text="Hire Date" Value="hire_date" />
            <asp:ListItem Text="Department Name" Value="department_name" />
            <asp:ListItem Text="Created Name" Value="create_name" />
            <asp:ListItem Text="Created Data" Value="create_date" />
        </asp:DropDownList>
        <asp:DropDownList ID="ddlSortDirection" runat="server">
            <asp:ListItem Text="Ascending" Value="asc" />
            <asp:ListItem Text="Descending" Value="desc" />
        </asp:DropDownList>
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
       <div style="margin-top: 20px; margin-bottom: 10px;">
            <asp:Button ID="btnCreateEmployee" runat="server" Text="Create Employee" CssClass="btn btn-primary" OnClick="btnCreateEmployee_Click" />
        </div>

        <asp:Panel ID="pnlCreateEmployee" runat="server" Visible="false" CssClass="container">
            <h5><b>Create New Employee</b></h5>
            <table class="form-table">
                <tr>
                    <td><label>First Name:</label></td>
                    <td><asp:TextBox ID="txtFirstName" runat="server" /></td>
                </tr>
                <tr>
                    <td><label>Last Name:</label></td>
                    <td><asp:TextBox ID="txtLastName" runat="server" /></td>
                </tr>
                <tr>
                    <td><label>Email:</label></td>
                    <td><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" /></td>
                    <asp:RegularExpressionValidator 
                        ID="revEmail" 
                        runat="server" 
                        ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Invalid email format"
                        ForeColor="Red"
                        Display="Dynamic" />
                </tr>
                <tr>
                    <td><label>Hire Date:</label></td>
                    <td><asp:TextBox ID="txtHireDate" runat="server" TextMode="Date" /></td>
                </tr>
                <tr>
                    <td><label>Department:</label></td>
                    <td><asp:DropDownList ID="ddlDepartmentCreate" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right;">
                        <asp:Button ID="btnCancelEmployee" runat="server" Text="Cancel" OnClick="btnCancelEmployee_Click" CausesValidation="false" />
                        <asp:Button ID="btnSaveEmployee" runat="server" Text="Save" CssClass="btn-submit" OnClick="btnSaveEmployee_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>



        <div class="grid-view">
        <asp:GridView ID="gvUser" runat="server" AutoGenerateColumns="False" GridLines="Both" DataKeyNames="EmployeeId" 
            OnPageIndexChanging="gvUser_PageIndexChanging" 
            OnRowCancelingEdit="gvUser_RowCancelingEdit" 
            OnRowDeleting="gvUser_RowDeleting" 
            OnRowEditing="gvUser_RowEditing" 
            OnRowUpdating="gvUser_RowUpdating"
            OnRowDataBound="gvUser_RowDataBound"
                AllowPaging="true"
                ShowFooter="false"
                PagerSettings-Visible="false"
                PageSize="5">

                 <Columns>
                     <asp:TemplateField HeaderText="Id" Visible="false">
                         <ItemTemplate>
                             <asp:Label ID="lblId" runat="server" Text='<%# Eval("EmployeeId") %>'></asp:Label>
                         </ItemTemplate>
                         <EditItemTemplate>
                             <asp:TextBox ID="txtId" runat="server" Text='<%# Eval("EmployeeId") %>' ReadOnly="true" CssClass="no-highlight"></asp:TextBox>
                         </EditItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="FirstName" HeaderText="First Name" ItemStyle-BorderWidth="1px" ItemStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" HeaderStyle-BorderStyle="Solid" />
                     <asp:BoundField DataField="LastName" HeaderText="Last Name" ItemStyle-BorderWidth="1px" ItemStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" HeaderStyle-BorderStyle="Solid" />
                     
                     <asp:TemplateField HeaderText="Email">
                        <ItemTemplate>
                            <%# Eval("Email") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEmailEdit" runat="server" Text='<%# Bind("Email") %>' />
                            <asp:RegularExpressionValidator 
                                ID="revEmailEdit" 
                                runat="server" 
                                ControlToValidate="txtEmailEdit"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Invalid email format"
                                ForeColor="Red"
                                Display="Dynamic" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                     <asp:BoundField DataField="HireDate" HeaderText="Hire Date" ItemStyle-BorderWidth="1px" ItemStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" HeaderStyle-BorderStyle="Solid" ReadOnly="true"/>
                      <asp:TemplateField HeaderText="Department">
                        <ItemTemplate>
                            <%# Eval("DepartmentName") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlDepartment" runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                     <asp:BoundField DataField="CreateName" HeaderText="Created Name" ItemStyle-BorderWidth="1px" ItemStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" HeaderStyle-BorderStyle="Solid" ReadOnly="true"/>
                     <asp:BoundField DataField="CreateDate" HeaderText="Created Date" ItemStyle-BorderWidth="1px" ItemStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="1px" HeaderStyle-BorderStyle="Solid" ReadOnly="true"/>

                     

                     <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="action-column">
                         <ItemTemplate>
                             <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="fa fa-pen-to-square edit-icon no-outline"></asp:LinkButton>
                             <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="fa fa-trash delete-icon"></asp:LinkButton>
                         </ItemTemplate>
                         <EditItemTemplate>
                             <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="fa fa-check update-icon"></asp:LinkButton>
                             <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="fa fa-xmark cancel-icon"></asp:LinkButton>
                         </EditItemTemplate>
                     </asp:TemplateField>
                 </Columns>
                 <PagerStyle HorizontalAlign="Center" />
                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" FirstPageText="First" LastPageText="Last" />
                 <SortedAscendingHeaderStyle CssClass="SortAsc" />
                 <SortedDescendingHeaderStyle CssClass="SortDesc" />
        </asp:GridView>
        </div>
        <div id="dgDataPaging">
            <div style="margin-top: 5px; display: flex; align-items: center;">
                <ul class="pagination" style="margin-top: -1px; margin-bottom: 0;">
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <li class='<%# (bool)Eval("IsActive") ? "page-item active" : "page-item" %>'>
                                <asp:LinkButton ID="lbtPage" runat="server"
                                    OnClick="lbtPage_Click"
                                    CssClass="page-link"
                                    Text='<%# Eval("PageNumber") %>'
                                    CommandArgument='<%# Eval("Value") %>'>
                                </asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                    <li class="page-item">
                        <asp:DropDownList ID="ddlPageSize" runat="server"
                            AutoPostBack="true"
                            CssClass="form-select page-link"
                            OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="50" Value="50" />
                        </asp:DropDownList>
                    </li>
                </ul>

                <div style="margin-left: auto;" id="divshowingdatapage">
                    <asp:Label ID="Label1" runat="server" Text="Halaman ke-1" CssClass="PageLabel"></asp:Label>
                </div>
            </div>
        </div>


    </form>
</body>
</html>