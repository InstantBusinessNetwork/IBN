<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PmboxRegistration.aspx.cs" Inherits="Mediachase.Ibn.WebTrial.PmboxRegistration" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" type="text/css" href="http://pmbox.ru/styles/general.css"/>
    <link rel="stylesheet" type="text/css" href="http://pmbox.ru/styles/examples.css"/>
    <link href="http://pmbox.ru/js/xfade/xfade2.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript">
       function onfocus_ctrl()
       {
        var ctrl = document.getElementById("<%=portalPassword.ClientID %>");
        if(ctrl != null)
            ctrl.focus();
       }
       function CheckDomain()
		{
		    var ret = false;
			var obj = document.getElementById("<%= portalDomain.ClientID %>");
			if(obj.value=="")
			{
				alert('Не указан домен.');
				return;
			}
			var objbutton = document.getElementById('btnCheck');
			var objimg = document.getElementById('imgLoading');
			var req = window.XMLHttpRequest? 
				new XMLHttpRequest() : 
				new ActiveXObject("Microsoft.XMLHTTP");
			req.onreadystatechange = function() 
			{
				if (req.readyState != 4 ) return ;
				if (req.readyState == 4)
				{
				  var objimg = document.getElementById('imgLoading');
				  objimg.style.display = "none";
					if (req.status == 200)
					{
					  //alert(req.responseText.toString());
					  if(req.responseText.toString()=="0")
					  {
					    ret = false;
					    var objimgfree = document.getElementById('imgFree');
					    var objimgbusy = document.getElementById('imgBusy');
					    objimgfree.style.display = "";
					    objimgbusy.style.display = "none";
					  }
					  else
					  {
					    ret = true;
					    var objimgfree = document.getElementById('imgFree');
					    var objimgbusy = document.getElementById('imgBusy');
					    objimgfree.style.display = "none";
					    objimgbusy.style.display = "";
					    obj.value = "";
					  }
					}
					else
						alert("There was a problem retrieving the XML data:\n" + req.statusText);
				}
			}
			objbutton.style.display = "none";
			objimg.style.display = "";
			var objimgfree = document.getElementById('imgFree');
	    var objimgbusy = document.getElementById('imgBusy');
	    objimgfree.style.display = "none";
	    objimgbusy.style.display = "none";
			var dt = new Date();
			var sID = dt.getMinutes() + "_" + dt.getSeconds() + "_" + dt.getMilliseconds();
			req.open("GET", 'CheckDomain.aspx?name='+obj.value+'&sID='+sID, true);
			req.send(null);
			
		return ret;
		}
		
		function ChangeText()
		{
		  var objimgfree = document.getElementById('imgFree');
	    var objimgbusy = document.getElementById('imgBusy');
	    objimgfree.style.display = "none";
	    objimgbusy.style.display = "none";
	    var objbutton = document.getElementById('btnCheck');
	    objbutton.style.display = "";
		}
    </script>
</head>
<body class="pmreg_body">
    <center>
    <form id="form1" runat="server">
    <div style="height: 458px; width: 535px;" class="pmreg_div">
        <table style="font-size: 13px; font-family: Tahoma;" class="pmreg_table">
            <tr>
                <td colspan="4"><h1>Регистрация триал-портала сроком на 14 дней. Бесплатно.</h1></td>
            </tr>
            <tr>
                <td colspan="4">
                    <table>
                        <tr>
                            <td>Домен</td>
                            <td>
                                http://<asp:TextBox runat="server" ID="portalDomain" onkeydown="ChangeText()"></asp:TextBox>.<asp:Label runat="server" ID="lblParentDomain" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" ControlToValidate="portalDomain"></asp:RequiredFieldValidator>
                                <asp:regularexpressionvalidator ID="Regularexpressionvalidator1" runat="server" ControlToValidate="portalDomain" ErrorMessage="*" ValidationExpression="[\dA-Za-z]+(-*)[\dA-Za-z]+"></asp:regularexpressionvalidator>
                            </td>
                            <td>
                                <input id="btnCheck" value="Проверить" type="button" onclick="CheckDomain()" style="width: 150px;" />
                                
                                <div id="imgLoading" style="display: none; width: 150px;" >
									<img align="absmiddle" src="layouts/loading6.gif" />
                                </div>
                                
                                <span id="imgFree" style="display:none;"><img title='Доменное имя свободно!' align="absmiddle" src="layouts/free-g.gif" />&nbsp;Доменное имя свободно</span>
				                <span id="imgBusy"  style="display:none;"><img title='Доменное имя занято!' align="absmiddle" src="layouts/busy.gif" />&nbsp;Доменное имя занято</span>				                
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="font-size: 10px;">Укажите адрес, который вы хотели бы использовать для входа в IBN Портал.</td>
                        </tr>
                        <tr>
                            <td colspan="3" style="font-size: 10px;">Например http://mycompany.mediachase.net</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td style="color: Red;"><asp:Label runat="server" ID="lblErrorDomainMessage" Text=""></asp:Label><br /></td></tr>
            <tr>
                <td>Имя входа<label style="font-size: 10px;">&nbsp;(например admin)</label></td>
                <td>
                    <div style="position: relative; padding-top: 23px;">
                        <asp:TextBox runat="server" ID="portalLogin" style="position: absolute; z-index: 10; left: 0px; top: 0px;"></asp:TextBox>
                        <asp:TextBox runat="server" ID="portalName2" style="position: absolute; left: 0px; top: 0px;z-index: 0;" onfocus="onfocus_ctrl()"></asp:TextBox>
                    </div>
                </td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="portalLogin"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td>Пароль</td>
                <td><asp:TextBox runat="server" ID="portalPassword" TextMode="Password"></asp:TextBox></td>
                <td><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ControlToValidate="portalPassword"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td>Пароль еще раз</td>
                <td><asp:TextBox runat="server" ID="portalPassword2" TextMode="Password"></asp:TextBox></td>
                <td><asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="portalPassword2" ControlToValidate="portalPassword" ErrorMessage="*" EnableClientScript="true"></asp:CompareValidator></td>
            </tr>
            <tr>
                <td>Ваше имя</td>
                <td><asp:TextBox runat="server" ID="firstName"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Ваша фамилия</td>
                <td><asp:TextBox runat="server" ID="secondName"></asp:TextBox></td>
            </tr>
            <tr>
                <td>E-мail</td>
                <td><asp:TextBox runat="server" ID="portalEmail"></asp:TextBox></td>
                <td>
					<asp:requiredfieldvalidator id="txtEmailRFValidator" runat="server" controltovalidate="portalEmail" errormessage="*" />
					<asp:regularexpressionvalidator id="RegularExpressionValidator2" runat="server" ErrorMessage="Wrong Email" ControlToValidate="portalEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>Телефон для связи</td>
                <td><asp:TextBox runat="server" ID="portalPhone"></asp:TextBox></td>
            </tr>
            <tr>
                <td><br /></td>
            </tr>
            <tr>
                <td colspan="4" align="right">
				<center><br />
					<asp:Button ID="Register_Btn" runat="server" Text="Создать мой IBN портал" OnClick="Register_Click" />
					<asp:Label runat="server" ID="lblPendingMessage" Text="" ForeColor="Green" Font-Bold="true"/>
				</center>
				</td>
            </tr>
        </table>
        
    </div>
    </form>
    </center>
        <script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("UA-490257-27");
pageTracker._trackPageview();
} catch(err) {}</script>
          
</body>
</html>
