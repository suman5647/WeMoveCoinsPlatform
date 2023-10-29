<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="SystemException.aspx.vb" Inherits="SystemExceptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
        <br /><br />
        <asp:Button ID="ButtonDeleteAll" runat="server" Text="Delete all exceptions" Visible="False" />
    <br />
    Level:
    <asp:DropDownList ID="DropDownListAuditLevel" runat="server" Height="22px" Width="193px" DataSourceID="SqlDataSourceLevel" DataTextField="Text" DataValueField="Id">       
    </asp:DropDownList>
    &nbsp;&nbsp; Status:&nbsp;
    <asp:DropDownList ID="DropDownListAuditStatus" runat="server" Height="22px" Width="193px" DataSourceID="SqlDataSourceStatus" DataTextField="Text" DataValueField="Id">    
    </asp:DropDownList>
    &nbsp;&nbsp;
    <asp:Button ID="ButtonSearch" runat="server" Text="Search" />
    <br /><br />
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
        <asp:SqlDataSource ID="SqlDataSourceException" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT TOP 1000 Id, OrderId, Status, Message, Created, AuditTrailLevelId FROM AuditTrail WHERE (AuditTrailLevelId = @AuditTrailLevelId) AND (Status = @Status) AND Created >= @fromDate ORDER BY Id DESC">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListAuditLevel" Name="AuditTrailLevelId" PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="DropDownListAuditStatus" Name="Status" PropertyName="SelectedValue" />
                <asp:Parameter Name="fromDate" Type="DateTime" />
            </SelectParameters>
    </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceLevel" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Id], [Text] FROM [AuditTrailLevel]">
    </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Id], [Text] FROM [AuditTrailStatus]">
    </asp:SqlDataSource>
    </asp:Content>

