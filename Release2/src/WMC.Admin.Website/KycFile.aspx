<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="KycFile.aspx.vb" Inherits="KycFile" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .auto-style1 {
            height: 16px;
        }
        .modalBackground {
            background-color: rgba(128, 128, 128, 0.55);
            width: 700px;
            height: 700px;
        }
        .panelStyle
        {
            display: block; 
            top:auto ;
            padding: 10px;
            background: white;
            border: 1px solid rgb(0, 0, 0);
            border-radius:5px;
        }
        </style>
    <br />
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <br />
    <asp:GridView ID="GridViewUserToBeVerified" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycUser" EmptyDataText="No users are currently waiting to be approved" ForeColor="#333333" GridLines="None" Width="595px" PageSize="20">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField DataField="UserId" HeaderText="UserId" SortExpression="UserId" Visible="False" />
            <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
                <EditItemTemplate>
                    <asp:Label ID="LabelPhone" runat="server" Text='<%# Eval("Phone")%>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLinkPhone" runat="server" NavigateUrl='<%# Bind("Phone")%>' Target="_blank" Text='>'></asp:HyperLink>
                    <asp:Label ID="LabelOrderPhone" runat="server" Text='<%# Eval("Phone")%>'></asp:Label>                    
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField DataField="Fname" HeaderText="Fname" SortExpression="Fname" ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
            
            <asp:TemplateField HeaderText="KycNote" SortExpression="KycNote">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Height="147px" Text='<%# Bind("KycNote") %>' TextMode="MultiLine" Width="321px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("KycNote") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" />
        </Columns>
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
    </asp:GridView>
    <br />
   <%--<br />
    Details of user:<asp:GridView ID="GridViewUser" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="Fname" HeaderText="Fname" SortExpression="Fname" ReadOnly="True" />
            <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
            <asp:TemplateField HeaderText="KycNote" SortExpression="KycNote">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Height="163px" Text='<%# Bind("KycNote") %>' TextMode="MultiLine" Width="412px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:TextBox ID="Label1" runat="server" Text='<%# Bind("KycNote") %>' ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ButtonType="Button" />
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />s
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
    <br />--%>
    User Under EDD Approvel:<asp:GridView ID="GridViewUser" style="margin-top: 10px" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Id" EmptyDataText="No users are currently waiting to be approved for EDD" DataSourceID="SqlDataSourceUser" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
                <EditItemTemplate>
                    <asp:Label ID="LabelPhone" runat="server" Text='<%# Eval("Phone")%>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLinkPhone" runat="server" NavigateUrl='<%# Bind("Phone")%>' Target="_blank" Text='>'></asp:HyperLink>
                    <asp:Label ID="LabelOrderPhone" runat="server" Text='<%# Eval("Phone")%>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Fname" HeaderText="Fname" SortExpression="Fname" ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
            <asp:TemplateField HeaderText="EDD">
                <ItemTemplate>
                    <asp:Button ID="ButtonApproveUser" runat="server" CausesValidation="False" CommandName="ApproveUser" Text="Approve" CommandArgument="<%# Container.DataItemIndex %>"></asp:Button>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
    <br />
    <br />
    KYC Files:
    <asp:GridView ID="GridViewKycFile" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceKycFile" AllowSorting="True" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
                    <Columns>
                         <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSelectKyc" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                         <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButtonSelect" runat="server" CausesValidation="False" CommandArgument="<%# Container.DataItemIndex %>" CommandName="Select" Text="Show"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField ShowHeader="False">
                             <EditItemTemplate>
                                 <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>
                                 &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                             </EditItemTemplate>
                             <ItemTemplate>
                                 <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="ButtonApproveNow" runat="server" CausesValidation="False" CommandName="ApproveNow" Text="Approve"></asp:Button>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Type" SortExpression="Type">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownListKycType0" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" SelectedValue='<%# Bind("Type") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelKycType" runat="server" Text='<%# Bind("Text") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OriginalFilename" HeaderText="OriginalFilename" SortExpression="OriginalFilename" ReadOnly="True" />
                        <asp:BoundField DataField="Uploaded" HeaderText="Uploaded" SortExpression="Uploaded" ReadOnly="True" />

                        <asp:TemplateField HeaderText="Rejected" SortExpression="Rejected">
                            <EditItemTemplate>
                                <asp:Button ID="ButtonReject" runat="server" CommandName="Reject" Text="Reject" />
                                <asp:Button ID="ButtonUndoReject" runat="server" CommandName="UndoReject" Text="X" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelReject" runat="server" Text='<%# Bind("Rejected") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Approved" SortExpression="Approved">
                            <EditItemTemplate>
                                <asp:Button ID="ButtonApprove" runat="server" CommandName="Approve" Text="Approve" />
                                <asp:Button ID="ButtonUndoApprove" runat="server" CommandName="UndoApprove" Text="X" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelApprove" runat="server" Text='<%# Bind("Approved") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ApprovedBy" HeaderText="ApprovedBy" SortExpression="ApprovedBy" />
                        <asp:TemplateField HeaderText="Obsolete" SortExpression="Obsolete" Visible="False">
                            <EditItemTemplate>
                                <asp:Button ID="ButtonObsolete" runat="server" CommandName="Obsolete" Text="Make Obsolete" />
                                <asp:Button ID="ButtonUndoObsolete" runat="server" CommandName="UndoObsolete" Text="X" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelObsolete" runat="server" Text='<%# Bind("Obsolete") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Note" SortExpression="Note">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Height="85px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="206px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Note") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <RowStyle ForeColor="#000066" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
    <br />
                <asp:Button ID="ButtonDeleteKyc" runat="server" Text="Deleted Selected" />
    <%--<asp:GridView ID="GridViewKycFile" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceKycFile" AllowPaging="True" AllowSorting="True" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="Show Image"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ButtonType="Button" />
            <asp:TemplateField HeaderText="Type" SortExpression="Type">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownListKycType" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" SelectedValue='<%# Bind("Type") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Text") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UniqueFilename" HeaderText="UniqueFilename" SortExpression="UniqueFilename" ReadOnly="True" />
            <asp:BoundField DataField="Uploaded" HeaderText="Uploaded" SortExpression="Uploaded" ReadOnly="True" />

            <asp:TemplateField HeaderText="Rejected" SortExpression="Rejected">
                <EditItemTemplate>
                    <asp:Button ID="ButtonReject" runat="server" CommandName="Reject" Text="Reject" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelReject" runat="server" Text='<%# Bind("Rejected") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Approved" SortExpression="Approved">
                <EditItemTemplate>
                    <asp:Button ID="ButtonApprove" runat="server" CommandName="Approve" Text="Approve" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelApprove" runat="server" Text='<%# Bind("Approved") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Height="85px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="206px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <RowStyle ForeColor="#000066" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
    </asp:GridView>--%>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
            <asp:Panel ID="Popup" runat="server" Style="display: block;" CssClass="panelStyle">
                <h1>KYC File</h1>
                <asp:GridView ID="GridView1" runat="server" visible="false" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycImage" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Width="400px">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:ImageField DataImageUrlField="Id" DataImageUrlFormatString="ImageVB.aspx?ImageID={0}" HeaderText="Preview Image" ControlStyle-Width="400">
            </asp:ImageField>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
    <br />
    <br />
        <div>
        <asp:Literal ID="ltEmbed" Visible="false" runat="server" />
        </div>
      <br />
      <br />
     <asp:Button ID="btnCancel" runat="server" Text="Close" Style="float:right"  OnClick="btncancel_Click" CausesValidation="false" />
     </asp:Panel>
          <asp:ModalPopupExtender  ID="ModalPopupExtender1" runat="server" TargetControlID="btnfakecancel"
                PopupControlID="Popup" CancelControlID="btnfakecancel" BackgroundCssClass="modalBackground">
          </asp:ModalPopupExtender>
     <asp:Button ID="btnfakecancel" Style="display: none;" runat="server" />
    <br />
    <br />
<%--    <asp:GridView ID="GridView1" runat="server" visible="false" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycImage" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Width="700">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:ImageField DataImageUrlField="Id" DataImageUrlFormatString="ImageVB.aspx?ImageID={0}" HeaderText="Preview Image" ControlStyle-Width="900">
            </asp:ImageField>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
    <br />
    <br />
        <div>
        <asp:Literal ID="ltEmbed" Visible="false" runat="server" />
        </div>--%>
    <br />
    <br />
    <br />
    <asp:SqlDataSource ID="SqlDataSourceKycType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT * FROM [KycType]"></asp:SqlDataSource>
    <br />
    <asp:SqlDataSource ID="SqlDataSourceKycUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT KycFile.UserId, [User].Fname, [User].Id, [User].Email, [User].Phone, [User].KycNote FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL) AND (NOT (KycFile.UniqueFilename = N'EnforceKYC')) AND (KycFile.Rejected IS NULL) ORDER BY KycFile.UserId DESC" UpdateCommand="UPDATE [User] SET KycNote = @KycNote WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="KycNote" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceAdmin" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Fname, Id FROM [User] WHERE (RoleId = 2)"></asp:SqlDataSource>
    <br />
     <asp:SqlDataSource ID="SqlDataSourceUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT [User].Fname, [User].Id, [User].Email, [User].Phone, [User].KycNote FROM [User] WHERE [User].Tier=3 ORDER BY [User].Id DESC"> </asp:SqlDataSource>
    <br />
    <asp:SqlDataSource ID="SqlDataSourceKycFile" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [KycFile] WHERE [Id] = @Id" InsertCommand="INSERT INTO [KycFile] ([Note], [Uploaded], [Rejected], [RejectedBy], [Approved], [ApprovedBy]) VALUES (@Note, @Uploaded, @Rejected, @RejectedBy, @Approved, @ApprovedBy)" SelectCommand="SELECT KycFile.Id, KycFile.UserId, KycFile.Note, KycFile.Uploaded,NULL as [File], KycFile.Rejected, KycFile.RejectedBy, KycFile.Approved, KycFile.ApprovedBy, KycType.Text, KycFile.Obsolete, KycFile.OriginalFilename, KycFile.Type FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id WHERE (KycFile.UserId = @UserId) AND (KycFile.Approved IS NULL) AND (KycFile.Rejected IS NULL) AND (KycFile.UserId = @UserId)" UpdateCommand="UPDATE KycFile SET Note = @Note, Type = @Type WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="Uploaded" Type="DateTime" />
            <asp:Parameter Name="Rejected" Type="DateTime" />
            <asp:Parameter Name="RejectedBy" Type="Int32" />
            <asp:Parameter Name="Approved" Type="DateTime" />
            <asp:Parameter Name="ApprovedBy" Type="Int32" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewUserToBeVerified" Name="UserId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="Type" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
    <asp:SqlDataSource ID="SqlDataSourceKycImage" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [UniqueFilename], Id FROM KycFile WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewKycFile" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

