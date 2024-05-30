<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="VMS_1.Dashboard" %>

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
        .auto-style1 {
            margin-top: 1.5rem;
            font-size: large;
            color: #FF3300;
            text-decoration: underline;
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
         <form id="form1" runat="server">
        <div class="container">
                 <h1 class="mt-4">Victualling Management System</h1>
        </div>
            <div class="container">
            <h2 class="mt-4">Dashboard</h2>
            <p class="mt-4">&nbsp;</p>
            <p class="mt-4">&nbsp;</p>
                <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" />
    </div>
    <asp:Button ID="ExportToExcelButton" runat="server" Text="Export to Excel" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" />
</div>
    <asp:Label ID="lblStatus" runat="server"></asp:Label>

  </div>
        </form>
    

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function addAlternativeItem() {
            var tableBody = document.getElementById("Table2");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="text" class="form-control" name="alternateitemname" /></td>
                                <td><input type="number" class="form-control" name = "equivalentofficerScale" required min="0" step="0.001"/></td>
                                <td><input type="number" class="form-control" name="equivalentsailorScale" required min="0" step="0.001"/></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
        }

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

        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }
    </script>
</body>
</html>