/*****************************************************************
程序名:红外驱动程序程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#ifndef __IRRMOTE_H
#define __IRRMOTE_H
#include "sys.h"

#define RDATA PBin(9)	 //红外数据输入脚

//红外遥控识别码(ID),每款遥控器的该值基本都不一样,但也有一样的.
#define REMOTE_ID 0      		   

extern u8 RmtCnt;	//按键按下的次数
extern u8 AddressCode;
extern u8 DataCode;
void IRRemote_Init(void);    //红外传感器接收头引脚初始化
void IRRemote_Scan(void);	
void IRRmote_Send(u8 addressCode,u8 dataCode);
void IRRmote_Send_Byte(u8 getdata);
#endif
