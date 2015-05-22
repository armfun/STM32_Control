/*****************************************************************
程序名:毕业设计主程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#include "led.h"
#include "delay.h"
#include "sys.h"
#include "usart.h"	 
#include "rtc.h" 
#include "adc.h"
#include "enc28j60.h"
#include "uip.h"
#include "uip_arp.h"
#include "tapdev.h"
#include "timer.h"
#include "timerx.h"
#include <math.h> 	
#include <string.h> 
#include "helper.h"
#include "dht11.h"
#include "IRRmote.h"

void uip_polling(void);
int OperationNum; 
u8 PacketBuf[200];
int k;
int IsDht1Init = 0;
u8 temperature;
u8 humidity;
u8 AddressCode=0;
u8 DataCode=0;
u8 LastDateCode=0;
#define BUF ((struct uip_eth_hdr *)&uip_buf[0])

///合并数据并标记为发送
void MergeAndSend(int OperationNum,char* Message)
{
	Merge(OperationNum,Message);
	for(k=0;k<PacketBuf[0];k++)
	{
		tcp_server_databuf[k] = PacketBuf[k];
	}
	tcp_server_sta|=1<<5;//标记有数据需要发送
}

 int main(void)
 {
	
	u8 tcp_server_tsta=0X00;
 	uip_ipaddr_t ipaddr;

	delay_init();	    	 //延时函数初始化	  
	NVIC_Configuration(); 	 //设置NVIC中断分组2:2位抢占优先级，2位响应优先级
	uart_init(9600);	 	//串口初始化为9600
 	LED_Init();			     //LED端口初始化
	DHT11_Init();				//Dht11初始化
	RTC_Init();				//初始化RTC
	Adc_Init();				//初始化ADC	  
	TIM1_PWM_Init(899,0);
	IRRemote_Init();			//红外接收初始化		 	

 	while(tapdev_init())	//初始化ENC28J60错误
	{								   
		delay_ms(200);
	};		
	uip_init();				//uIP初始化	  
 	uip_ipaddr(ipaddr, 192,168,1,16);	//设置本地设置IP地址
	uip_sethostaddr(ipaddr);					    
	uip_ipaddr(ipaddr, 192,168,1,1); 	//设置网关IP地址
	uip_setdraddr(ipaddr);						 
	uip_ipaddr(ipaddr, 255,255,255,0);	//设置网络掩码
	uip_setnetmask(ipaddr);

	uip_listen(HTONS(1200));			//监听1200端口,用于TCP Server

	while (1)
	{
		uip_polling();	//处理uip事件，必须插入到用户程序的循环体中  
		
		if(tcp_server_tsta!=tcp_server_sta)//TCP Server状态改变
		{															 

 			if(tcp_server_sta&(1<<6))	//收到新数据
			{
				if(CheckData(tcp_server_databuf))
				{
					OperationNum = GetOperation(tcp_server_databuf);
					tcp_server_sta&=~(1<<6);		//标记数据已经被处理
					//Clear(tcp_server_databuf);					
					switch(OperationNum)
					{
							//温湿度获取
							case 0:
									if(tcp_server_sta&(1<<7))	//连接还存在
									{
										if(DHT11_Read_Data()==1)
										{
											MergeAndSend(0,"dht1Wrong");
										}
										else
										{
											MergeDHT1();
											for(k=0;k<PacketBuf[0];k++)
											{
												tcp_server_databuf[k] = PacketBuf[k];
											}
											tcp_server_sta|=1<<5;//标记有数据需要发送
										}								
										}										
								break;
								//IO口输出
							case 1:
									if(tcp_server_sta&(1<<7))	//连接还存在
									{
										switch(GetDigitalNum(tcp_server_databuf))
										{
											case 1:
												LED1=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 2:
												LED2=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 3:
												LED3=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 4:
												LED4=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 5:
												LED5=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 6:
												LED6=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 7:
												LED5=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											case 8:
												LED6=GetDigitalLevel(tcp_server_databuf);
												MergeAndSend(1,"SetOK");
												break;
											default:
												break;
										}
									}
								break;
								//获取模拟端口和数字端口状态
							case 2:
									if(tcp_server_sta&(1<<7))
									{
										MergeDigitalAndAnalog();
										for(k=0;k<PacketBuf[0];k++)
										{
											tcp_server_databuf[k] = PacketBuf[k];
										}
										tcp_server_sta|=1<<5;//标记有数据需要发送
									}
									break;
									//脉冲宽度
							case 3:
									if(tcp_server_sta&(1<<7))
									{
										switch(GetPinNum(tcp_server_databuf))
										{
											case 1:												
													TIM_SetCompare1(TIM1,GetPWMData(tcp_server_databuf));											
													delay_ms(10);																							
												break;
											case 2:
													TIM_SetCompare2(TIM1,GetPWMData(tcp_server_databuf));
													delay_ms(10);
												break;
											case 3:
													TIM_SetCompare3(TIM1,GetPWMData(tcp_server_databuf));
													delay_ms(10);
												break;
											case 4:
													TIM_SetCompare4(TIM1,GetPWMData(tcp_server_databuf));
													delay_ms(10);
												break;
											default:
												break;											
										}
									}
									break;
									//红外接收
							case 4:
									if(tcp_server_sta&(1<<7))
									{
											DataCode=0;
											k=1000;
											while(k--)
											{
												IRRemote_Scan();
												if(DataCode)
												{
													LastDateCode=DataCode;
													MergeIR();
													for(k=0;k<PacketBuf[0];k++)
													{
														tcp_server_databuf[k] = PacketBuf[k];
													}
													tcp_server_sta|=1<<5;//标记有数据需要发送
													break;
												}
												delay_ms(10);
											}
											if(DataCode==0)
											{
												MergeAndSend(4,"Failed");
											}
											delay_ms(10);
									}
									break;
									//红外发送
							case 5:
									if(tcp_server_sta&(1<<7))
									{
											if(LastDateCode)
											{
												IRRmote_Send(AddressCode,LastDateCode);
												MergeAndSend(5,"Sent");
											}
											else
											{
												MergeAndSend(5,"Failed");
											}
											delay_ms(10);
									}
									break;
							default:
								break;
						}
				}
				else
				{
					MergeAndSend(0,"Miss");					
				}
			}							
			tcp_server_tsta=tcp_server_sta;
		}
		delay_ms(1);
	}  
} 
 
//uip事件处理函数
//必须将该函数插入用户主循环,循环调用.
void uip_polling(void)
{
	u8 i;
	static struct timer periodic_timer, arp_timer;
	static u8 timer_ok=0;	 
	if(timer_ok==0)//仅初始化一次
	{
		timer_ok = 1;
		timer_set(&periodic_timer,CLOCK_SECOND/2);  //创建1个0.5秒的定时器 
		timer_set(&arp_timer,CLOCK_SECOND*10);	   	//创建1个10秒的定时器 
	}				 
	uip_len=tapdev_read();	//从网络设备读取一个IP包,得到数据长度.uip_len在uip.c中定义
	if(uip_len>0) 			//有数据
	{   
		//处理IP数据包(只有校验通过的IP包才会被接收) 
		if(BUF->type == htons(UIP_ETHTYPE_IP))//是否是IP包? 
		{
			uip_arp_ipin();	//去除以太网头结构，更新ARP表
			uip_input();   	//IP包处理
			//当上面的函数执行后，如果需要发送数据，则全局变量 uip_len > 0
			//需要发送的数据在uip_buf, 长度是uip_len  (这是2个全局变量)		    
			if(uip_len>0)//需要回应数据
			{
				uip_arp_out();//加以太网头结构，在主动连接时可能要构造ARP请求
				tapdev_send();//发送数据到以太网
			}
		}else if (BUF->type==htons(UIP_ETHTYPE_ARP))//处理arp报文,是否是ARP请求包?
		{
			uip_arp_arpin();
 			//当上面的函数执行后，如果需要发送数据，则全局变量uip_len>0
			//需要发送的数据在uip_buf, 长度是uip_len(这是2个全局变量)
 			if(uip_len>0)tapdev_send();//需要发送数据,则通过tapdev_send发送	 
		}
	}else if(timer_expired(&periodic_timer))	//0.5秒定时器超时
	{
		timer_reset(&periodic_timer);		//复位0.5秒定时器 
		//轮流处理每个TCP连接, UIP_CONNS缺省是40个  
		for(i=0;i<UIP_CONNS;i++)
		{
			uip_periodic(i);	//处理TCP通信事件  
	 		//当上面的函数执行后，如果需要发送数据，则全局变量uip_len>0
			//需要发送的数据在uip_buf, 长度是uip_len (这是2个全局变量)
	 		if(uip_len>0)
			{
				uip_arp_out();//加以太网头结构，在主动连接时可能要构造ARP请求
				tapdev_send();//发送数据到以太网
			}
		}
#if UIP_UDP	//UIP_UDP 
		//轮流处理每个UDP连接, UIP_UDP_CONNS缺省是10个
		for(i=0;i<UIP_UDP_CONNS;i++)
		{
			uip_udp_periodic(i);	//处理UDP通信事件
	 		//当上面的函数执行后，如果需要发送数据，则全局变量uip_len>0
			//需要发送的数据在uip_buf, 长度是uip_len (这是2个全局变量)
			if(uip_len > 0)
			{
				uip_arp_out();//加以太网头结构，在主动连接时可能要构造ARP请求
				tapdev_send();//发送数据到以太网
			}
		}
#endif 
		//每隔10秒调用1次ARP定时器函数 用于定期ARP处理,ARP表10秒更新一次，旧的条目会被抛弃
		if(timer_expired(&arp_timer))
		{
			timer_reset(&arp_timer);
			uip_arp_timer();
		}
	}
}
