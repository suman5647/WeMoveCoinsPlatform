<%@ Page Language="VB" AutoEventWireup="false" Inherits="WMC.Admin.Web.ToBeDeleted" Codebehind="ToBeDeleted.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" />
        <br />
    <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal" PageSize="50">
            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                <asp:BoundField DataField="Number" HeaderText="Order Number" SortExpression="Number" ReadOnly="True" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" ReadOnly="True" />
                <asp:BoundField DataField="Code" HeaderText="Currency" SortExpression="Code" ReadOnly="True" />
                <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" ReadOnly="True" Visible="False" />
                <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" />
                <asp:TemplateField HeaderText="Approve" Visible="False">
                    <ItemTemplate>
                        <asp:Button ID="ButtonApprove" runat="server" BackColor="#66FF66" BorderStyle="Solid" CommandName="Approve" Text="Approve" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cancel">
                    <ItemTemplate>
                        <asp:Button ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="KYC Declined">
                    <ItemTemplate>
                        <asp:Button ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                    <EditItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
                <asp:BoundField DataField="OrderStatus" HeaderText="OrderStatus" SortExpression="OrderStatus" />
                <asp:TemplateField HeaderText="Note" SortExpression="Note">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBoxNote" runat="server" Height="175px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="377px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField EditText="Edit Note" ShowEditButton="True" />
            </Columns>
            <FooterStyle BackColor="White" ForeColor="#333333" />
            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="White" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#487575" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#275353" />
        </asp:GridView>
        <br />
        <br />
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Rate, [Order].Note, [Order].Status, [Order].SiteId, OrderStatus.Text AS OrderStatus, [Order].TransactionHash, [Order].ExtRef FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
