<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="QuickPayException.aspx.vb" Inherits="QuickPayException" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
        <br /><br />
    <br />
        <asp:GridView ID="GridViewException" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" EmptyDataText="No orders found" AllowPaging="True" DataSourceID="SqlDataSourceQuickOrders">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                
                <asp:TemplateField HeaderText="OrderId" InsertVisible="False" SortExpression="OrderId">
                    <EditItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("OrderId") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLinkOrderLookup" runat="server" Target="_blank" NavigateUrl='<%# Eval("OrderId")%>' Text='<%# Bind("OrderId")%>'></asp:HyperLink>

                    </ItemTemplate>
                </asp:TemplateField>
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
    <asp:SqlDataSource ID="SqlDataSourceQuickOrders" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT(OrderId) FROM [AuditTrail] WHERE AuditTrailLevelId = @status AND Created >= @fromDate"  >
        
        <SelectParameters>
           <asp:Parameter Name="status" Type="Int64" />
           <asp:Parameter Name="fromDate" Type="DateTime" />
        </SelectParameters> 
    </asp:SqlDataSource>
        <br />
    </asp:Content>


