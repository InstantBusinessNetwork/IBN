#ifndef __RS2XML_DEFS__H__
#define __RS2XML_DEFS__H__

#include "RS2XML.h"

/*
	<chat cid="CUID"/>
*/
CR2XItem g_r2x_chat_cid[] =
{
	R2X_BODY_BEGIN(_T("chat"))
		R2X_ATTR(_T("cid"), _T("CUID"))
	R2X_BODY_END
};

/*
	<chat cid="CUID">
		<name></name>
		<descr></descr>
		<owner></owner>
		<time></time>
	</chat>
*/
//CUID, [NAME], [DESC], OWNER_ID, BEGIN_TIME
CR2XItem g_r2x_chat_full[] =
{
	R2X_BODY_BEGIN(_T("chat"))
		R2X_ATTR(_T("cid"), _T("CUID"))
		R2X_TAG(_T("name"), _T("NAME"))
		R2X_TAG(_T("descr"), _T("DESC"))
		R2X_TAG(_T("owner"), _T("OWNER_ID"))
		R2X_TAG(_T("time"), _T("BEGIN_TIME"))
	R2X_BODY_END
};

/*
	<user id="sender"/>
 */
CR2XItem g_r2x_user_id[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_ATTR(_T("id"), _T("sender"))
	R2X_BODY_END
};

CR2XItem g_r2x_user_only_id[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_ATTR(_T("id"), _T("user_id"))
	R2X_BODY_END
};


CR2XItem g_r2x_fromuser_id[] =
{
	R2X_BODY_BEGIN(_T("from_user"))
		R2X_ATTR(_T("id"), _T("from_user_id"))
	R2X_BODY_END
};
/*

<user id="recipient"/>
 */
CR2XItem g_r2x_sender[] =
{
	R2X_BODY_BEGIN(_T("sender"))
		R2X_ATTR(_T("id"), _T("sender"))
	R2X_BODY_END
};

/*<user id="recipient"/>
 */
CR2XItem g_r2x_recipient[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_ATTR(_T("id"), _T("recipient"))
	R2X_BODY_END
};

/*
<recipients"/>
*/
CR2XItem g_r2x_recipients[] =
{
	R2X_BODY_BEGIN(_T("recipients"))
		R2X_CHILD(g_r2x_recipient)
	R2X_BODY_END
};

/*
	<product id="product_id">
		<name>"product_name"</name>
	</product>
 */
CR2XItem g_r2x_product[] =
{
	R2X_BODY_BEGIN(_T("product"))
		R2X_ATTR(_T("id"), _T("product_id"))
		R2X_TAG(_T("name"), _T("product_name"))
	R2X_BODY_END
};

/*
	<promo id="pid">
		<user id="sender"/>
		<subject>"subject"</subject>
		<body>"promo_text"</body>
		<time>"send_time"</time>
		<product id="product_id">
			<name>"product_name"</name>
		</product>
	</promo>
 */
CR2XItem g_r2x_promo[] =
{
	R2X_BODY_BEGIN(_T("promo"))
		R2X_ATTR(_T("pid"), _T("pid"))
		R2X_CHILD(g_r2x_user_id)
		R2X_TAG(_T("subject"), _T("subject"))
		R2X_TAG(_T("body"), _T("promo_text"))
		R2X_TAG(_T("time"), _T("send_time"))
		R2X_CHILD(g_r2x_product)
	R2X_BODY_END
};

/*
	<message mid="mid">
		<user id="sender"/>
		<body>"mess_text"</body>
		<time>"send_time"</time>
	</message>
 */
CR2XItem g_r2x_message[] =
{
	R2X_BODY_BEGIN(_T("message"))
		R2X_ATTR(_T("mid"), _T("mid"))
		R2X_CHILD(g_r2x_user_id)
		R2X_CHILD(g_r2x_recipients)
		R2X_TAG(_T("body"), _T("mess_text"))
		R2X_TAG(_T("time"), _T("send_time"))
	R2X_BODY_END
};
/*
	<session>
		<sid/>
		<count/>
	</session>
 */
CR2XItem g_r2x_session[] =
{
	R2X_BODY_BEGIN(_T("session"))
		R2X_TAG(_T("sid"), _T("sid"))
		R2X_TAG(_T("count"), _T("mess_count"))
	R2X_BODY_END
};
/*
	<file fid="file_id">
		<user id="sender"/>
		<real_name>"file_name"</real_name>
		<time>"file_date"</time>
		<size>"file_size"</size>
		<description>"file_descr"</description>
	</file>
 */
CR2XItem g_r2x_file[] =
{
	R2X_BODY_BEGIN(_T("file"))
		R2X_ATTR(_T("fid"), _T("file_id"))
		R2X_CHILD(g_r2x_user_id)
		R2X_TAG(_T("real_name"), _T("file_name"))
		R2X_TAG(_T("time"), _T("file_date"))
		R2X_TAG(_T("size"), _T("file_size"))
		R2X_TAG(_T("body"), _T("file_descr"))
	R2X_BODY_END
};

/*
	<user>
		<address1>"address1"</address1>
		<address2>"address1"</address1>
		<city>"city"</city>
		<state>"state_province"</state>
		<postal_code>"postal_code"</postal_code>
	</user>
 */

CR2XItem g_r2x_userExtInfo[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_TAG(_T("address1"), _T("address1"))
		R2X_TAG(_T("address1"), _T("address1"))
		R2X_TAG(_T("city"), _T("city"))
		R2X_TAG(_T("state"), _T("state_province"))
		R2X_TAG(_T("postal_code"), _T("postal_code"))
	R2X_BODY_END
};

// USER INFO [19:03 - 16.11.2000]
/*
	<role id="role_id">
		<name>"role_desc"</name>
	</role>
 */
CR2XItem g_r2x_role[] =
{
	R2X_BODY_BEGIN(_T("role"))
		R2X_ATTR(_T("id"), _T("imgroup_id"))
		R2X_TAG(_T("name"), _T("imgroup_name"))
	R2X_BODY_END
};

/*
	<company id="company_id">
		<name>"company_name"</name>
	</company>
 */
CR2XItem g_r2x_company[] =
{
	R2X_BODY_BEGIN(_T("company"))
		R2X_ATTR(_T("id"), _T("company_id"))
		R2X_TAG(_T("name"), _T("company_name"))
	R2X_BODY_END
};

/*
	<user id="user_id">
		<nick_name>"login"</nick_name>
		<first_name>"first_name"</first_name>
		<last_name>"last_name"</last_name>
		<email>"eMail"</email>
		<status>"status"</status>
		<role id="role_id">
			<name>"role_desc"</name>
		</role>
		<company id="company_id">
			<name>"company_name"</name>
		</company>
	</user>
 */
CR2XItem g_r2x_userBasicInfo[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_ATTR(_T("id"), _T("user_id"))
		R2X_TAG(_T("nick_name"), _T("login"))
		R2X_TAG(_T("first_name"), _T("first_name"))
		R2X_TAG(_T("last_name"), _T("last_name"))
		R2X_TAG(_T("email"), _T("eMail"))
		R2X_TAG(_T("status"), _T("status"))
		R2X_TAG(_T("time"), _T("time"))
		R2X_CHILD(g_r2x_role)
		//R2X_CHILD(g_r2x_company)
	R2X_BODY_END
};

CR2XItem g_r2x_userFromBasicInfo[] =
{
	R2X_BODY_BEGIN(_T("from_user"))
		R2X_ATTR(_T("id"), _T("from_user_id"))
		R2X_TAG(_T("nick_name"), _T("from_login"))
		R2X_TAG(_T("first_name"), _T("from_first_name"))
		R2X_TAG(_T("last_name"), _T("from_last_name"))
		R2X_TAG(_T("email"), _T("from_eMail"))
		R2X_TAG(_T("status"), _T("from_status"))
		R2X_TAG(_T("time"), _T("time"))

		//R2X_CHILD(g_r2x_role)
		//R2X_CHILD(g_r2x_company)
	R2X_BODY_END
};

CR2XItem g_r2x_usershot[] =
{
	R2X_BODY_BEGIN(_T("user_shot"))
		R2X_ATTR(_T("id"), _T("user_id"))
	R2X_BODY_END
};

CR2XItem g_r2x_userLogInfo[] =
{
	R2X_BODY_BEGIN(_T("user"))
		R2X_TAG(_T("nick_name"), _T("login"))
		R2X_TAG(_T("first_name"), _T("first_name"))
		R2X_TAG(_T("last_name"), _T("last_name"))
	R2X_BODY_END
};
/*
	<adduser>
	<user id="user_id">
		<nick_name>"login"</nick_name>
		<first_name>"first_name"</first_name>
		<last_name>"last_name"</last_name>
		<email>"eMail"</email>
		<status>"status"</status>
		<role id="role_id">
			<name>"role_desc"</name>
		</role>
		<company id="company_id">
			<name>"company_name"</name>
		</company>
	</user>
	<message>"mess_text"</message>
	</adduser>
 */
CR2XItem g_r2x_adduser[] =
{
	R2X_BODY_BEGIN(_T("adduser"))
		R2X_CHILD(g_r2x_userBasicInfo)
		R2X_TAG(_T("body"), _T("mess_text"))
	R2X_BODY_END
};

/*
	<adduserr>
	<user id="user_id">
		<nick_name>"login"</nick_name>
		<first_name>"first_name"</first_name>
		<last_name>"last_name"</last_name>
		<email>"eMail"</email>
		<status>"status"</status>
		<role id="role_id">
			<name>"role_desc"</name>
		</role>
		<company id="company_id">
			<name>"company_name"</name>
		</company>
	</user>
	<result>"deny_flag"</result>
	</adduser>
 */
CR2XItem g_r2x_adduserr[] =
{
	R2X_BODY_BEGIN(_T("adduserr"))
		R2X_CHILD(g_r2x_userBasicInfo)
		R2X_TAG(_T("result"), _T("deny_flag"))
	R2X_BODY_END
};

CR2XItem g_r2x_chat_status[] =
{
	R2X_BODY_BEGIN(_T("chat_status"))
		R2X_CHILD(g_r2x_chat_cid)
		R2X_CHILD(g_r2x_userBasicInfo)
	R2X_BODY_END
};

CR2XItem g_r2x_chat_leave[] =
{
	R2X_BODY_BEGIN(_T("chat_leave"))
		R2X_CHILD(g_r2x_chat_cid)
		R2X_CHILD(g_r2x_userBasicInfo)
	R2X_BODY_END
};

CR2XItem g_r2x_chat_accept[] =
{
	R2X_BODY_BEGIN(_T("chat_accept"))
		R2X_CHILD(g_r2x_chat_cid)
		R2X_CHILD(g_r2x_userBasicInfo)
		R2X_TAG(_T("result"), _T("ACCEPTED"))
	R2X_BODY_END
};
CR2XItem g_r2x_chat_invite_broadcast[] =
{
	R2X_BODY_BEGIN(_T("chat_invite"))
		R2X_CHILD(g_r2x_chat_cid)
		R2X_TAG(_T("body"), _T("INVITE_TEXT"))
		R2X_CHILD(g_r2x_fromuser_id)
		R2X_CHILD(g_r2x_userBasicInfo)
		
	R2X_BODY_END
};

CR2XItem g_r2x_chat_invite[] =
{
	R2X_BODY_BEGIN(_T("chat_invite"))
		R2X_CHILD(g_r2x_chat_full)
		R2X_TAG(_T("body"), _T("INVITE_TEXT"))
		R2X_CHILD(g_r2x_userFromBasicInfo)
		R2X_CHILD(g_r2x_user_only_id)
	R2X_BODY_END
};
CR2XItem g_r2x_ChatMessLog[] = 
{
	R2X_BODY_BEGIN(_T("mess"))
		R2X_CHILD(g_r2x_chat_cid)
		R2X_CHILD(g_r2x_userBasicInfo)
		R2X_TAG(_T("body"), _T("mess_text"))
		R2X_TAG(_T("time"), _T("send_time"))
		R2X_TAG(_T("id"), _T("id"))
	R2X_BODY_END
};

CR2XItem g_r2x_chat_message[] =
{
	R2X_BODY_BEGIN(_T("chat_message"))
		R2X_ATTR(_T("id"), _T("CHAT_MESS_ID"))
		R2X_CHILD(g_r2x_user_only_id)
		R2X_CHILD(g_r2x_chat_cid)
		R2X_TAG(_T("body"), _T("mess_text"))
		R2X_TAG(_T("time"), _T("send_time"))
	R2X_BODY_END
};
#endif//__RS2XML_DEFS__H__