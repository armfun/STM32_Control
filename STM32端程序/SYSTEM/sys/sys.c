/*****************************************************************
程序名:系统时钟初始化
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:
*****************************************************************/
//V1.2修改说明
//修正了中断中调用出现死循环的错误
//防止延时不准确,采用do while结构!

//V1.3修改说明
//增加了对UCOSII延时的支持.
//如果使用ucosII,delay_init会自动设置SYSTICK的值,使之与ucos的TICKS_PER_SEC对应.
//delay_ms和delay_us也进行了针对ucos的改造.
//delay_us可以在ucos下使用,而且准确度很高,更重要的是没有占用额外的定时器.
//delay_ms在ucos下,可以当成OSTimeDly来用,在未启动ucos时,它采用delay_us实现,从而准确延时
//可以用来初始化外设,在启动了ucos之后delay_ms根据延时的长短,选择OSTimeDly实现或者delay_us实现.

//V1.4修改说明 20110929
//修改了使用ucos,但是ucos未启动的时候,delay_ms中中断无法响应的bug.
//V1.5修改说明 20120902
//在delay_us加入ucos上锁，防止由于ucos打断delay_us的执行，可能导致的延时不准。
//V1.4修改说明
//把NVIC KO了,没有使用任何库文件!
//加入了JTAG_Set函数
//V1.5 20120322
//增加void INTX_DISABLE(void)和void INTX_ENABLE(void)两个函数
//V1.6 20120412
//1,增加MSR_MSP函数												    
//2,修改VECT_TAB_RAM的默认偏移,设置为0X6000.
//V1.7 20120818
//1,添加ucos支持配置宏SYSTEM_SUPPORT_UCOS
//2,修改了注释
//3,去掉了不常用函数BKP_Write
#include "sys.h"

void NVIC_Configuration(void)
{

    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);	//设置NVIC中断分组2:2位抢占优先级，2位响应优先级

}
