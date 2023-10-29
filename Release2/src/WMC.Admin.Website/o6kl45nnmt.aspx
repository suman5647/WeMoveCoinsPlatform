<%@ Page Language="VB" AutoEventWireup="false" CodeFile="o6kl45nnmt.aspx.vb" Inherits="o6kl45nnmt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

INPUT
{
    FONT: 10px verdana,tahoma,arial;
    /*COLOR: #cc6600;*/
    TEXT-DECORATION: none;
    }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="font-family: Arial, Helvetica, sans-serif; font-size: xx-large">
           <%-- <table>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtOTP" runat="server" CssClass="otptxt" Font-Size="XX-Large" Width="150px"></asp:TextBox></td>
                        <td>
                            <asp:Button ID="BtnUnlock" runat="server" Text="Unlock" Font-Size="XX-Large" Height="50px" /></td>
                        <td>
                                                       &nbsp;<asp:Label ID="Label2" runat="server" Font-Size="XX-Large"></asp:Label>&nbsp;&nbsp;&nbsp; <asp:Label ID="LabelBitGoUnlocked" runat="server" Text="Label" Font-Size="XX-Large"></asp:Label>
                        </td>
                    </tr>
                </table>--%>
            
            <asp:Label ID="LabelApprovePayout" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label"></asp:Label>

            <br />
            <asp:Label ID="LabelApproveBankPayment" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label"></asp:Label>

            <br />
            <br />

            <asp:Label ID="LabelRevenueYesterday" runat="server" CssClass="verdana13" Font-Bold="True" Text="Label"></asp:Label><br />
            <asp:Label ID="LabelRevenueToday" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label"></asp:Label>

            <asp:Label ID="Label1" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label" Font-Size="XX-Small">-</asp:Label>

            <br />
            <br />
                        <asp:Button ID="ButtonLockBitGo" runat="server" Text="LOCK!" BackColor="#FFFF66" CssClass="verdana13"/>

        </div>
    </form>
</body>
</html>
