/*****************************************************************
程序名:TCP程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#include "sys.h"
#include "usart.h"	 		   
#include "uip.h"	    
#include "enc28j60.h"
#include "httpd.h"
#include "tcp.h"

//TCP应用接口函数(UIP_APPCALL)
//完成TCP Server服务
void tcp_appcall(void)
{	
  	
	switch(uip_conn->lport)
	{
		case HTONS(1200):
		    tcp_server_appcall(); 
			break;
		default:						  
		    break;
	}		    
}
//打印日志用
void uip_log(char *m)
{			    
	printf("uIP log:%s\r\n",m);
}

























