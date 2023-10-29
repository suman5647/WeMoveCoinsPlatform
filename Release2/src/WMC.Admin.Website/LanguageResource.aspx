<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="LanguageResource.aspx.vb" Inherits="LanguageResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
        <br />
        <br />
        <br />
        <asp:DropDownList ID="DropDownListLanguage" runat="server" AutoPostBack="True" DataTextField="DisplayMember" DataValueField="ValueMember" AppendDataBoundItems="True" DataSourceID="SqlDataSourceLanguage">
        </asp:DropDownList>
        <br />
        <br />
        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceResources" AllowSorting="True" CellPadding="4" DataKeyNames="Id" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:CommandField ShowEditButton="True" />
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
                <asp:BoundField DataField="Key" HeaderText="Key" ReadOnly="True" SortExpression="Key" />
                <asp:TemplateField HeaderText="Value" SortExpression="Value">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" CssClass="verdana10" Text='<%# Bind("Value") %>' TextMode="MultiLine" Width="300px" Height="50px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Value") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Value_en" HeaderText="Value_en" ReadOnly="True" SortExpression="Value_en" />
            </Columns>
            <EditRowStyle BackColor="#99CCFF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSourceLanguage" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT (CASE WHEN COALESCE (Language , '') = '' THEN 'en' ELSE Language END) AS DisplayMember, Language AS ValueMember FROM LanguageResources"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceResources" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, [Key], Value, (SELECT Value FROM LanguageResources AS LanguageResources_1 WHERE (Language = N'') AND ([Key] = LanguageResources.[Key])) AS Value_en FROM LanguageResources WHERE (Language = @Language)" UpdateCommand="UPDATE LanguageResources SET Value = RTRIM(LTRIM(@Value)) WHERE (Id = @Id)">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListLanguage" Name="Language" PropertyName="SelectedValue" ConvertEmptyStringToNull="False" Size="2" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="Value" />
                <asp:Parameter Name="Id" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <br />
    </div>
</asp:Content>

