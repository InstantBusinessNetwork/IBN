<%@ Reference Control="~/Modules/DirectoryTreeView.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EmailBoxes.FileExtended" Codebehind="FileExtended.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="ibn" TagName="DirectoryTreeView" src="~/Modules/DirectoryTreeView.ascx" %>
<script language="javascript">
function ShowHelp()
{
	obj=document.getElementById("tbHelp");
	obj1=document.getElementById("imgHelp");
	obj2=document.getElementById("imgDenyHelp");
	if(obj)
	{
		if(obj.style.display=="none")
		{
			obj.style.display="";
			if(obj1 && obj2)
			{
				obj1.style.display="none";
				obj2.style.display="";
			}
		}
		else
		{
			obj.style.display="none";
			if(obj1 && obj2)
			{
				obj1.style.display="";
				obj2.style.display="none";
			}
		}
	}
}

function VerifyPattern(elId)
{
	obj=document.getElementById(elId);
	if(obj==null)return;
	//alert(obj.value);

	Pvalid = true;
	var errorText="";
	var arAT = new Array("UserName","From","Subject","Date.Now","Date.UtcNow","Date","Importance","MessageID","Sender","To","Header");
	var strToDo = obj.value;
	var vregExp = new RegExp("\\x5B([^\\x5D]*)\\x5D","igm");
	if(arr = strToDo.match(vregExp))
	{
		for(i=0;i<arr.length;i++)
		{
			var vregExp1 = /\x5B([^\x28\x5D]*)(\x28([^\x29]*)\x29)?\x5D/igm;
			//alert(i+":"+arr[i]);
			if(arr1=vregExp1.exec(arr[i]))
			{
				//alert(arr1.join("\r\n"));
				jj=0;
				for(j=0;j<arAT.length;j++)
				{
					if(arAT[j]==arr1[1])
					{
						jj=1;
						break
					}
				}
				if(jj==0)
				{
					Pvalid = false;
					if(errorText!="")errorText+="<br>";
					errorText+="<%= LocRM.GetString("tActiveTag") %> '"+arr1[1]+"' <%= LocRM.GetString("tNotSupported") %>.";
					//errorText+="Active tag '"+arr1[1]+"' is not supported";
				}
			}
		}
	}
	objTr=document.getElementById("trError");
	if(!Pvalid)
	{
		objTr.style.display="";
		objTd=document.getElementById("tdError");
		objTd.innerHTML=errorText;
		setTimeout("PattFocusElement()", 0);
	}
	else objTr.style.display="none";
	//alert(Pvalid);
	var elem=document.getElementById(gBtnSaveId);
	if(elem)
		elem.disabled=!Pvalid;
	return Pvalid;
}

function PattFocusElement()
{
    var elem=document.getElementById('<%= txtFolderPattern.ClientID %>');
    if(!elem)
		return;
    elem.focus();
}
</script>
<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
	<tr>
		<td colspan="2" style="PADDING-TOP:15px">
			<asp:CheckBox ID="cbSaveMessageBodyAsEml" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<asp:CheckBox ID="cbSaveMessageBodyAsMsg" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<asp:CheckBox ID="cbSaveMessageBodyAsMht" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<asp:CheckBox ID="cbUseExternal" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<asp:CheckBox ID="cbAutoDelete" Runat="server" CssClass="text" TextAlign="Right"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td width="120px" class="text" valign="top" style="padding-top:7px;">
			<%=LocRM.GetString("tFolder")%>:
		</td>
		<td nowrap>
			<ibn:DirectoryTreeView id="dtv" RequiredField="True" runat="server"></ibn:DirectoryTreeView>
		</td>
	</tr>
	<tr>
		<td width="120px" class="text">
			<%=LocRM.GetString("tFolderPattern")%>:
		</td>
		<td valign="top"><asp:TextBox CssClass="text" ID="txtFolderPattern" Runat=server Width="250px"></asp:TextBox><img border='0' width='16px' src='../layouts/images/icon-question.gif' height='16px' align='absmiddle' onclick="javascript:ShowHelp();" title='<%=LocRM.GetString("tShowHelp")%>' id="imgHelp" style="cursor:pointer;"><img border='0' width='16px' src='../layouts/images/scrollleft_hover.gif' height='16px' align='absmiddle' onclick="javascript:ShowHelp();" title='<%=LocRM.GetString("tHideHelp")%>' id="imgDenyHelp" style="display:none;cursor:pointer;"></td>
	</tr>
	<tr id="trError" style="display:none;">
		<td width="120px">&nbsp;</td>
		<td class="text" valign="top" style="color:red;" id="tdError"></td>
	</tr>
</table>
<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0" style="display:none;" id="tbHelp">
	<tr>
		<td colspan="2" class="text" valign="top"><%=LocRM.GetString("tHelpText")%></td>
	</tr>
</table>