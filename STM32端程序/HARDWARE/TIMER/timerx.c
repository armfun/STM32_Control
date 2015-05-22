/*****************************************************************
程序名:PWM驱动程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#include "timerx.h"
#include "led.h"
#include "usart.h"
 
u32 uip_timer=0;//uip 计时器，每10ms增加1.

//定时器6中断服务程序	 
void TIM6_IRQHandler(void)
{ 	if (TIM_GetITStatus(TIM6, TIM_IT_Update) != RESET) //检查指定的TIM中断发生与否:TIM 中断源 
	{
  	uip_timer++;//uip计时器增加1	
	} 
		TIM_ClearITPendingBit(TIM6, TIM_IT_Update  );  //清除TIMx的中断待处理位:TIM 中断源 
				    		  			    	    
}
 
//基本定时器6中断初始化
//这里时钟选择为APB1的2倍，而APB1为36M
//arr：自动重装值。
//psc：时钟预分频数
//这里使用的是定时器3!
void TIM6_Int_Init(u16 arr,u16 psc)
{	
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM6, ENABLE); //时钟使能

	TIM_TimeBaseStructure.TIM_Period = arr; //设置在下一个更新事件装入活动的自动重装载寄存器周期的值	 计数到5000为500ms
	TIM_TimeBaseStructure.TIM_Prescaler =psc; //设置用来作为TIMx时钟频率除数的预分频值  10Khz的计数频率  
	TIM_TimeBaseStructure.TIM_ClockDivision = 0; //设置时钟分割:TDTS = Tck_tim
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;  //TIM向上计数模式
	TIM_TimeBaseInit(TIM6, &TIM_TimeBaseStructure); //根据TIM_TimeBaseInitStruct中指定的参数初始化TIMx的时间基数单位
 
	TIM_ITConfig( TIM6,TIM_IT_Update|TIM_IT_Trigger,ENABLE);//使能定时器6更新触发中断
 
	TIM_Cmd(TIM6, ENABLE);  //使能TIMx外设
 	
  NVIC_InitStructure.NVIC_IRQChannel = TIM6_IRQn;  //TIM3中断
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;  //先占优先级0级
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;  //从优先级3级
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; //IRQ通道被使能
	NVIC_Init(&NVIC_InitStructure);  //根据NVIC_InitStruct中指定的参数初始化外设NVIC寄存器 								 
}

//TIM1 PWM部分初始化 
//PWM输出初始化
//arr：自动重装值
//psc：时钟预分频数
void TIM1_PWM_Init(u16 arr,u16 psc)
{  
	GPIO_InitTypeDef GPIO_InitStructure;
	TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
	//TIM_OCInitTypeDef  TIM_OCInitStructure;
	TIM_OCInitTypeDef TIMOCInitStructure;

	 GPIO_DeInit(GPIOA);
	 GPIO_InitStructure.GPIO_Pin=GPIO_Pin_8|GPIO_Pin_9|GPIO_Pin_10|GPIO_Pin_11;
	 GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
	 GPIO_InitStructure.GPIO_Mode=GPIO_Mode_AF_PP ;//复用推挽输出
	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA|RCC_APB2Periph_AFIO, ENABLE);//使能端口时钟A
	 GPIO_Init(GPIOA, &GPIO_InitStructure);


	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_TIM1, ENABLE);
	 TIM_DeInit(TIM1);
	 TIM_InternalClockConfig(TIM1);
	 TIM_TimeBaseStructure.TIM_Period=arr;//ARR的值周期10K
	 TIM_TimeBaseStructure.TIM_Prescaler=psc;
	 TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up; //向上计数模式
	 TIM_TimeBaseInit(TIM1, &TIM_TimeBaseStructure);
	 
	 
	 TIMOCInitStructure.TIM_OCMode = TIM_OCMode_PWM1; //PWM模式1输出
	 TIMOCInitStructure.TIM_Pulse =0;//占空比=(CCRx/ARR)*100%
	 TIMOCInitStructure.TIM_OCPolarity = TIM_OCPolarity_High;//TIM输出比较极性高
	 TIMOCInitStructure.TIM_OutputState = TIM_OutputState_Enable;//使能输出状态
	 
	 TIM_OC1Init(TIM1, &TIMOCInitStructure);
	 TIM_OC2Init(TIM1, &TIMOCInitStructure);
	 TIM_OC3Init(TIM1, &TIMOCInitStructure);
	 TIM_OC4Init(TIM1, &TIMOCInitStructure);
	 TIM_CtrlPWMOutputs(TIM1,ENABLE);
	 TIM_Cmd(TIM1, ENABLE); //开启时钟
}
