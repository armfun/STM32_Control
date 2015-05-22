/*****************************************************************
程序名:LED驱动程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#ifndef __LED_H
#define __LED_H	 
#include "sys.h"

//#define LED1 PBout(1)// PB5
#define LED0 PEout(5)// PE5
//#define LED2 PBout(2)
//#define LED3 PBout(3)
//#define LED4 PBout(4)
//#define LED5 PBout(5)
//#define LED6 PEout(0)
//#define LED7 PEout(1)
//#define LED8 PEout(2)
#define LED1 PFout(1)
#define LED2 PFout(2)
#define LED3 PFout(3)
#define LED4 PFout(4)
#define LED5 PFout(5)
#define LED6 PFout(6)
#define LED7 PFout(7)
#define LED8 PFout(8)

void LED_Init(void);//初始化

		 				    
#endif
