#if !defined _DEFPIPE_H
#define _DEFPIPE_H

#define IMServerCommandsPipe "\\\\.\\pipe\\IMServerCommandsPipe"
#define IMServerLogPipe		 "\\\\.\\pipe\\IMServerLogPipe"
//#define IMServerCommandsPipe "\\\\SERVAK\\pipe\\IMServerCommandsPipe"
//#define IMServerLogPipe		 "\\\\SERVAK\\pipe\\IMServerLogPipe"

#define POOL_VALUE			 5
#define BUFFER_SIZE			 512

//  Commands Defination
#define IM_COMMNAD_TEMPLATE_1			"%s;%d;%s;" //"type;[ID];[SID,FID,PID]"
#define IM_COMMNAD_TEMPLATE_2			"%s;%d;%d;" //"type;[ID];[ID,Status]"
#define IM_COMMAND_LENGHT			19

#define IM_COMMAND_NEW_MESSAGE		"COMMAND_NEW_MESSAGE"
#define IM_COMMAND_NEW_PROMO		"COMMAND_NEW_PROMO__"
#define IM_COMMAND_NEW_FILE			"COMMAND_NEW_FILE___"
#define IM_COMMAND_USER_STATUS		"COMMAND_STATUS_____"
#define IM_COMMAND_ADD_USER			"COMMAND_ADD_USER___"

#define IM2_COMMNAD_TEMPLATE		"%s;%d;" //"type;Setings;"
#define IM2_COMMAND_LENGHT			21

#define IM2_COMMAND_WARNING_START	"COMMAND_WARNING_START"
#define IM2_COMMAND_WARNING_STOP	"COMMAND_WARNING_STOP_"
#define IM2_COMMAND_STAT_START		"COMMAND_STAT_START___"
#define IM2_COMMAND_STAT_STOP		"COMMAND_STAT_STOP____"

//  CallBack Defination
#define IM_CALLBACK_TYPE_1			1
#define IM_CALLBACK_TYPE_2			2
#define IM_CALLBACK_TYPE_3			3
#define IM_CALLBACK_LENGHT			16

#define IM_CALLBACK_TYPE_1_TEMPLATE			"%s;%d;%d;%s;" //"type;time;level;Message"
#define IM_CALLBACK_TYPE_2_TEMPLATE			"%s;%d;%d;%d;" //"type;time;c_type;value"
#define IM_CALLBACK_TYPE_3_TEMPLATE			"%s;%d;" //"type;c_type;value"

#define IM_CALLBACK_STRING_1	"CALLBACK_WARNING"
#define IM_CALLBACK_STRING_2	"CALLBACK_STAT___"
#define IM_CALLBACK_STRING_3	"CALLBACK_STATE__"

#endif