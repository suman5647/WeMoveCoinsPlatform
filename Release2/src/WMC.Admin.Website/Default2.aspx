<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default2.aspx.vb" Inherits="Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            Kragen account balance
            <span id="acbalance"></span>
        </div>


        <script type="text/javascript">
            // function getACBalance() {
            var request = $.get('KrakenHandler.ashx?query=balance');
            request.success(function (result) {
                var acBalanceElement = document.getElementById('acbalance');
                acBalanceElement.textContent = 'EUR: ' + Math.round(result.result.ZEUR).toLocaleString('de-DE') + ', BTC: ' + result.result.XXBT.toLocaleString('de-DE', { minimumFractionDigits: 4, maximumFractionDigits: 8 });
            });
        // }
        </script>
    </form>
</body>
</html>
