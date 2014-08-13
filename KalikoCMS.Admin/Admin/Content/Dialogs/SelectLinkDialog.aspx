﻿<%@ Page Title="Select link" Language="C#" AutoEventWireup="true" CodeBehind="SelectLinkDialog.aspx.cs" Inherits="KalikoCMS.Admin.Content.Dialogs.SelectLinkDialog" MasterPageFile="Dialog.Master" %>
<%@ Import Namespace="KalikoCMS.PropertyType" %>
<%@ Register TagPrefix="cms" Namespace="KalikoCMS.Admin.WebControls" Assembly="KalikoCMS.Admin" %>


<asp:Content ContentPlaceHolderID="MainArea" runat="server">
  <asp:Literal runat="server" ID="PostbackResult" />   

  <ul id="link-tab" class="nav nav-tabs">
    <li class="active"><a href="#external" data-toggle="tab">External</a></li>
    <li><a href="#page" data-toggle="tab">Page</a></li>
    <li><a href="#file" data-toggle="tab">File</a></li>
  </ul>
  
  <div class="tab-content">
    <asp:HiddenField ID="SelectedTab" Value="#external" runat="server"/>
    <div class="tab-pane active" id="external">
      <asp:TextBox ID="ExternalUrl" CssClass="medium-input" runat="server" />
    </div>
    <div class="tab-pane" id="page">
      <asp:HiddenField runat="server" ID="LanguageId" />
      <asp:HiddenField runat="server" ID="PageId" />
      <div class="input-append">
        <asp:TextBox runat="server" CssClass="medium-input" ID="PageDisplayField" ReadOnly="True" /><asp:HyperLink ID="SelectPageButton" CssClass="btn" runat="server"><i class="icon-file-alt"></i></asp:HyperLink>
      </div>
    </div>
    <div class="tab-pane" id="file">
      <div class="input-append">
        <asp:HiddenField ID="FileUrl" runat="server" />
        <asp:TextBox runat="server" CssClass="medium-input" ID="FileDisplayField" ReadOnly="True" /><asp:HyperLink ID="SelectFileButton" CssClass="btn" runat="server"><i class="icon-file-alt"></i></asp:HyperLink>
      </div>
    </div>
  </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ButtonArea" runat="server">
  <cms:BootstrapButton id="SaveButton" Icon="icon-ok" Mode="Primary" Text="Select" runat="server"/>
  <button type="button" id="deselect-button" class="btn btn-danger"><i class="icon-trash icon-white"></i> No link</button>
  <button type="button" id="close-button" data-dismiss="modal" class="btn">Close</button>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptArea" runat="server">
    <script type="text/javascript">
      $(function () {
        $('#deselect-button').click(noLink);
        $('#close-button').click(abort);

        $('#link-tab a').on('shown.bs.tab', function (e) {
          $('#<%=SelectedTab.ClientID %>').val($(e.target).attr('href'));
        });

        $('#link-tab a[href=<%=ActiveTab %>]').tab('show');

        $('#<%=SelectFileButton.ClientID %>').click(function () {
          top.propertyEditor.file.openDialog($('#<%=FileUrl.ClientID %>'), $('#<%=FileDisplayField.ClientID %>'));
          return false;
        });

        $('#<%=SelectPageButton.ClientID %>').click(function () {
          top.propertyEditor.pageLink.openDialog($('#<%=LanguageId.ClientID %>'), $('#<%=PageId.ClientID %>'), $('#<%=PageDisplayField.ClientID %>'));
          return false;
        });

      });

      function noLink() {
        top.executeCallback('', '<%=LinkProperty.LinkType.Unknown %>');
        close();
      }
    </script>
</asp:Content>