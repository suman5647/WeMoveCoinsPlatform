<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web.SystemExceptions" CodeBehind="SystemException.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <asp:Button ID="ButtonDeleteAll" runat="server" Text="Delete all exceptions" />
    <br />
    <br />
    <asp:GridView ID="GridViewException" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceException" ForeColor="#333333" GridLines="None" EmptyDataText="Yeaa - no exception" AllowPaging="True" PageSize="100">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:TemplateField HeaderText="Message" SortExpression="Message">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Message") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Message") %>' Width="900px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
        </Columns>
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
    <br />
    <asp:SqlDataSource ID="SqlDataSourceException" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.Id, AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE (AuditTrailStatus.Text = N'Application Error') ORDER BY AuditTrail.Id DESC"></asp:SqlDataSource>
</asp:Content>