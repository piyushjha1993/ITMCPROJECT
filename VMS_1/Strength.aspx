<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Strength.aspx.cs" Inherits="VMS_1.Strength" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Strength Module</title>
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
        <h2 class="mt-4">Strength Module</h2>
      
        <form id="strengthForm" runat="server">
              <div class="text-right">
      <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
      </div>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading date">Date</th>
                            <th class="heading veg-officer">Veg Officer</th>
                            <th class="heading non-veg-officer">Non-Veg Officer</th>
                            <th class="heading rik-officer">RIK Officer</th>
                            <th class="heading veg-staff">Veg Staff</th>
                            <th class="heading non-veg-staff">Non-Veg Staff</th>
                            <th class="heading rik-staff">RIK Staff</th>
                            <th class="heading non-entitled-officer">Non-Entitled Officer</th>
                            <th class="heading non-entitled-staff">Non-Entitled Staff</th>
                            <th class="heading civilian">Civilian</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td><input type="date" class="form-control" name="date" required /></td>
                            <td><input type="number" class="form-control" name="vegOfficer" required min="0" /></td>
                            <td><input type="number" class="form-control" name="nonVegOfficer" required min="0" /></td>
                            <td><input type="number" class="form-control" name="rikOfficer" required min="0" /></td>
                            <td><input type="number" class="form-control" name="vegStaff" required min="0" /></td>
                            <td><input type="number" class="form-control" name="nonVegStaff" required min="0" /></td>
                            <td><input type="number" class="form-control" name="rikStaff" required min="0" /></td>
                            <td><input type="number" class="form-control" name="nonEntitledOfficer" required min="0" /></td>
                            <td><input type="number" class="form-control" name="nonEntitledStaff" required min="0" /></td>
                            <td><input type="number" class="form-control" name="civilian" required min="0" /></td>
                            <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px"/>
                
            </div>
            <div>
                <h2 class="mt-4">Strength Data</h2>
                
            </div>
            <div>
                <asp:GridView ID="GridView2" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView></div>
            </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function setTheme(theme) {
            var gridView = document.getElementById("GridView2");

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
            newRow.innerHTML = `<td><input type="date" class="form-control" name="date" required /></td>
                                <td><input type="number" class="form-control" name="vegOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonVegOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="rikOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="vegStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonVegStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="rikStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonEntitledOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonEntitledStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="civilian" required min="0" /></td>
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
