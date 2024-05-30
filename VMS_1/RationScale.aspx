<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RationScale.aspx.cs" Inherits="VMS_1.RationScale" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Item Master</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            transition: background-color 0.3s, color 0.3s;
            position: relative;
            background-color: #3498db; 
            color: #000; 
        }

        .dark-theme {
            color: #fff; 
        }

        .form-control {
            width: 100%; 
            max-width: none; 
        }

        .table {
            color: #000; 
        }

        .table.dark-theme {
            color: #fff; 
        }

        .theme-selector {
            position: fixed;
            top: 10px;
            right: 10px;
            z-index: 9999;
            background-color: #fff;
            padding: 5px 10px;
            border-radius: 5px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }

        .theme-selector label {
            margin-right: 5px;
        }

        .theme-selector select {
            padding: 5px;
            border-radius: 4px;
        }

        .container {
            margin-top: 50px;
        }

        .dark-theme-text {
            color: #fff;
        }

        .heading {
            color: #000;
        }

        .heading.dark-theme {
            color: #fff;
        }
    </style>
</head>
<body>
    <div class="theme-selector">
        <label for="ddlTheme">Choose Theme:</label>
        <select id="ddlTheme" onchange="setTheme(this.value)">
            <option value="blue">Blue</option>
            <option value="dark">Dark</option>
        </select>
    </div>
    <div class="container">
        <h1 class="mt-4">Victualling Management System</h1>
    </div>
    <div class="container">
        <h2 class="mt-4">Item Master</h2>
      
        <form id="itemMasterForm" runat="server">
              <div class="text-right">
      <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
      </div>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Item Name</th>
                            <th class="heading">Rate</th>
                            <th class="heading">Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td><input type="text" class="form-control" name="itemName" required /></td>
<td><input type="text" class="form-control" name="rate" required pattern="^\d+(\.\d+)?$" />Default Value 0</td>

                            <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>

                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px"/>
            </div>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

            <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-striped">
</asp:GridView>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function setTheme(theme) {
            var gridView = document.getElementById("GridView1");

            if (theme === "blue") {
                document.body.style.backgroundColor = '#3498db';
                document.body.classList.remove('dark-theme');
                gridView.classList.remove('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.remove('dark-theme');
                });
                document.querySelector('.table').classList.remove('dark-theme');
            } else if (theme === "dark") {
                document.body.style.backgroundColor = '#333';
                document.body.classList.add('dark-theme');
                gridView.classList.add('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.add('dark-theme');
                });
                document.querySelector('.table').classList.add('dark-theme');
            }
        }

        function addRow() {
            var tableBody = document.getElementById("tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="text" class="form-control" name="itemName" required /></td>
    <td><input type="number" class="form-control" name="rate" required min="0" step="0.01" />Default Value 0</td>
    <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
        }


        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }
    </script>
</body>
</html>
