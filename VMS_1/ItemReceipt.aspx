<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemReceipt.aspx.cs" Inherits="VMS_1.ItemReceipt" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Item Receipt Module</title>
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
        <h2 class="mt-4">Receipt Module</h2>

        <form id="receiptForm" runat="server">
            <div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading itemname">Item Name</th>
                            <th class="heading qty">Quantity</th>
                            <th class="heading denom">Denomination</th>
                            <th class="heading rcvdfrom">Received From</th>
                            <th class="heading ref">Reference/CRV No</th>
                            <th class="heading date">Date</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <select class="form-control itemname" name="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="qty" required pattern="^\d+(\.\d+)?$" />
                            </td>
                            <td>
                                <select class="form-control" name="denom" required>
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control" name="rcvdfrom" required onchange="checkReceivedFrom(this)">
                                    <option value="">Select </option>
                                    <option value="BV Yard">BV Yard</option>
                                    <option value="Local Purchase">Local Purchase</option>
                                    <option value="Other Ship">Other Ship</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="ref" required />
                            </td>
                            <td>
                                <input type="date" class="form-control" name="date" required />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-left">
                <asp:LinkButton ID="item" runat="server" Text="Go to Item Master" CssClass="btn btn-info" PostBackUrl="~/ItemMaster.aspx"></asp:LinkButton>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
            </div>
            <div>
                <h2 class="mt-4">Entered Data</h2>
            </div>
            <div class="form-group">
                <label for="monthYear">Select Month and Year:</label>
                <input type="month" id="monthYear" name="monthYear" class="form-control" onchange="filterData()" /><br />
            &nbsp;</div>
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.1/js/bootstrap.min.js"></script>
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
            newRow.innerHTML = `<td><select class="form-control itemname" name="itemname" required>
                                    <option value="">Select</option>
                                </select></td>
                                <td><input type="text" class="form-control" name="qty" required pattern="^\\d+(\\.\\d+)?$" /></td>
                                <td><select class="form-control" name="denom" required>
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select></td>
                                <td><select class="form-control" name="rcvdfrom" required onchange="checkReceivedFrom(this)">
                                    <option value="">Select </option>
                                    <option value="BV Yard">BV Yard</option>
                                    <option value="Local Purchase">Local Purchase</option>
                                    <option value="Other Ship">Other Ship</option>
                                    <option value="Others">Others</option>
                                </select></td>
                                <td><input type="text" class="form-control" name="ref" required /></td>
                                <td><input type="date" class="form-control" name="date" required /></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            loadItemNamesForRow(newRow);
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function loadItemNamesForRow(row) {
            fetch('ItemReceipt.aspx/GetItemNames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = row.querySelector('.itemname');
                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            option.textContent = item;
                            itemSelect.appendChild(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function checkReceivedFrom(selectElement) {
            if (selectElement.value === 'Others') {
                var parentTd = selectElement.parentNode;
                var othersInput = document.createElement('input');
                othersInput.type = 'text';
                othersInput.name = 'otherReceivedFrom';
                othersInput.className = 'form-control mt-2';
                othersInput.placeholder = 'Specify other source';
                parentTd.appendChild(othersInput);
            } else {
                var parentTd = selectElement.parentNode;
                var othersInput = parentTd.querySelector('input[name="otherReceivedFrom"]');
                if (othersInput) {
                    parentTd.removeChild(othersInput);
                }
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            var rows = document.querySelectorAll("#tableBody tr");
            rows.forEach(loadItemNamesForRow);
        });

        function filterData() {
            var monthYearInput = document.getElementById('monthYear');
            var monthYearValue = monthYearInput.value;

            if (monthYearValue) {
                var form = document.getElementById('receiptForm');
                var formData = new FormData(form);

                fetch('ItemReceipt.aspx/GetFilteredData', {
                    method: 'POST',
                    body: JSON.stringify({ monthYear: monthYearValue }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d && data.d.length) {
                            var gridView = document.getElementById('<%= GridView1.ClientID %>');
                            gridView.innerHTML = '';

                            var table = document.createElement('table');
                            table.className = 'table table-bordered table-striped';

                            var thead = document.createElement('thead');
                            var theadRow = document.createElement('tr');
                            ['Item Name', 'Quantity', 'Denomination', 'Received From', 'Reference No', 'Date'].forEach(function (heading) {
                                var th = document.createElement('th');
                                th.textContent = heading;
                                theadRow.appendChild(th);
                            });
                            thead.appendChild(theadRow);
                            table.appendChild(thead);

                            var tbody = document.createElement('tbody');
                            data.d.forEach(function (row) {
                                var tr = document.createElement('tr');
                                row.forEach(function (cell) {
                                    var td = document.createElement('td');
                                    td.textContent = cell;
                                    tr.appendChild(td);
                                });
                                tbody.appendChild(tr);
                            });
                            table.appendChild(tbody);

                            gridView.appendChild(table);
                        }
                    })
                    .catch(error => {
                        console.error('Error fetching filtered data:', error);
                    });
            }
        }
    </script>
</body>
</html>
