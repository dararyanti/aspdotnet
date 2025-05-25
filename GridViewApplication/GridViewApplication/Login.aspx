<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GridViewApplication.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Login</title>
    
    <!-- Bootstrap -->
    <link href="../Assets/Vendors/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Assets/Css/Font-Awesome-6.x/css/all.min.css" rel="stylesheet" />

    <!-- Swal New -->
    <script src="../Assets/Vendors/sweetalert2-11.21.0/package/dist/sweetalert2.all.min.js"></script>
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

    .swal2-popup {
        font-size: 14px !important;
    }

    .swal2-styled {
        padding: 10px 32px 10px 32px !important;
        margin: 20px 10px 0px 10px !important;
        width: 170px;
        height: 45px;
    }

    </style>
</head>
<body>
    <form id="formLogin" runat="server">
    <div class="container">
        <table class="form-table">
            <tr>
                <td>
                    <h3><b>User Login</b></h3>
                </td>
                <td>
                    &nbsp;</td>
            </tr>            
            <tr>
                <td>
                    <label for="username">Username:</label></td>
                <td>
                    <asp:TextBox ID="txtUsername" runat="server" Text="admin" required></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="Password">Password:</label></td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" type="password" Text="admin123" required></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btn" runat="server" Text="Submit" OnClick="btn_Click" CssClass="btn-submit" />
                </td>
            </tr>
        </table>    
    </div>
    </form>
</body>
</html>
