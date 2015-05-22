/*****************************************************************
程序名:数据包处理程序
作者:杨小杰
完成日期:2013年10月16日10:01:12
程序说明:v1.0
*****************************************************************/
#include "helper.h" 
#include <string.h>
#include "adc.h"
#include "LED.h"
int j;
const u8 Pwd[6]={49,50,51,52,53,54};//此处存放密码:123456
extern u8 PacketBuf[200];
extern u8 AddressCode;
extern u8 DataCode;
int PacketLength;
int MessageLength;
u16 adcx;

u8 buf[5];
//校验数据包
int CheckData(u8 DataBuf[200])
{
	int DataLength = DataBuf[0];
	if(DataLength > 8)
	{
		if(DataBuf[DataLength-1]==10 && DataBuf[DataLength-2]==13)
		{
			if(CheckPwd(DataBuf,DataLength))
			{
				return 1;
			}
		}		
	}
	return 0;
}

//检查密码
int CheckPwd(u8 DataBuf[200],int DataLength)
{
	if(DataBuf[8]==6)
	{
		for(j=0;j<6;j++)
		{
			if(Pwd[j]!=DataBuf[12+j])
			{
				return 0;
			}
		}
		return 1;
	}
	return 0;	
}

//清空收发缓存区
void Clear(u8 DataBuf[200])
{
	for(j=0;j<200;j++)
	DataBuf[j]=0;
}

//获取操作数
int GetOperation(u8 DataBuf[200])
{
	return DataBuf[4];
}

//获取IO口
int GetDigitalNum(u8 DataBuf[200])
{
	return DataBuf[18];
}

//获取IO电平高低
int GetDigitalLevel(u8 DataBuf[200])
{
	return DataBuf[19];
}
//获取引脚号
int GetPinNum(u8 DataBuf[200])
{
	return DataBuf[18];
}

int GetPWMData(u8 DataBuf[200])
{
	return DataBuf[19];
}
//合并红外信号
void MergeIR(void)
{
	Clear(PacketBuf);
	PacketBuf[0]=0x19;
	PacketBuf[4]=0x04;
	PacketBuf[8]=0x0D;
	PacketBuf[1]=PacketBuf[2]=PacketBuf[3]=PacketBuf[5]=PacketBuf[6]=PacketBuf[7]=PacketBuf[9]=PacketBuf[10]=PacketBuf[11]=0;
	PacketBuf[12]='L';
	PacketBuf[13]='e';
	PacketBuf[14]='a';
	PacketBuf[15]='r';
	PacketBuf[16]='n';
	PacketBuf[17]='e';
	PacketBuf[18]='d';
	PacketBuf[19]=ToHex((int)AddressCode/100);
	PacketBuf[20]=ToHex((int)AddressCode%100/10);
	PacketBuf[21]=ToHex((int)AddressCode%10);
	PacketBuf[22]=ToHex((int)DataCode/100);
	PacketBuf[23]=ToHex((int)DataCode%100/10);
	PacketBuf[24]=ToHex((int)DataCode%10);
}

//合并温湿度传感器数据并生成数据包
void MergeDHT1(void)
{
	Clear(PacketBuf);
	PacketBuf[0]=0x15;
	PacketBuf[4]=0x00;
	PacketBuf[8]=0x09;
	PacketBuf[1]=PacketBuf[2]=PacketBuf[3]=PacketBuf[5]=PacketBuf[6]=PacketBuf[7]=PacketBuf[9]=PacketBuf[10]=PacketBuf[11]=0;
	PacketBuf[12]= ToHex(((int)buf[0]/10));
	PacketBuf[13]= ToHex(((int)buf[0]%10));
	PacketBuf[14]='.';
	PacketBuf[15]='0';
	PacketBuf[16]='%';
	PacketBuf[17]=ToHex(((int)buf[2]/10));
	PacketBuf[18]=ToHex(((int)buf[2]%10));
	PacketBuf[19]='.';
	PacketBuf[20]='0';
}

//合并数字信号和模拟信号
void MergeDigitalAndAnalog(void)
{
	Clear(PacketBuf);
	PacketBuf[0]=0x59;
	PacketBuf[4]=0x02;
	PacketBuf[8]=0x4D;
	PacketBuf[1]=PacketBuf[2]=PacketBuf[3]=PacketBuf[5]=PacketBuf[6]
	=PacketBuf[7]=PacketBuf[9]=PacketBuf[10]=PacketBuf[11]=0;
	PacketBuf[12]='{';
	PacketBuf[13]=PacketBuf[21]=PacketBuf[60]=PacketBuf[69]='"';
	PacketBuf[14]='A';
	PacketBuf[15]='n';
	PacketBuf[16]='a';
	PacketBuf[17]='l';
	PacketBuf[18]='o';
	PacketBuf[19]='g';
	PacketBuf[20]='s';
	PacketBuf[22]=PacketBuf[70]=':';
	PacketBuf[23]=PacketBuf[71]='[';
	PacketBuf[28]=PacketBuf[33]=PacketBuf[38]=PacketBuf[43]=PacketBuf[48]
	=PacketBuf[53]=PacketBuf[59]=PacketBuf[73]=PacketBuf[75]=PacketBuf[77]
	=PacketBuf[79]=PacketBuf[81]=PacketBuf[83]=PacketBuf[85]=',';
	PacketBuf[58]=PacketBuf[87]=']';
	PacketBuf[61]='D';
	PacketBuf[62]='i';
	PacketBuf[63]='g';
	PacketBuf[64]='i';
	PacketBuf[65]='t';
	PacketBuf[66]='a';
	PacketBuf[67]='l';
	PacketBuf[68]='s';
	PacketBuf[88]='}';
	GetAngolags(ADC_Channel_0,0);
	GetAngolags(ADC_Channel_1,1);
	GetAngolags(ADC_Channel_2,2);
	GetAngolags(ADC_Channel_3,3);
	GetAngolags(ADC_Channel_4,4);
	GetAngolags(ADC_Channel_5,5);
	GetAngolags(ADC_Channel_6,6);
	PacketBuf[72]=ToHex(LED1);
	PacketBuf[74]=ToHex(LED2);
	PacketBuf[76]=ToHex(LED3);
	PacketBuf[78]=ToHex(LED4);
	PacketBuf[80]=ToHex(LED5);
	PacketBuf[82]=ToHex(LED6);
	PacketBuf[84]=ToHex(LED7);
	PacketBuf[86]=ToHex(LED8);
}

//获取模拟端口数据
void GetAngolags(u8 ch,int chNum)
{
	adcx=Get_Adc_Average(ch,10);
	PacketBuf[24+5*chNum]=ToHex(adcx/1000);
	PacketBuf[25+5*chNum]=ToHex(adcx%1000/100);
	PacketBuf[26+5*chNum]=ToHex(adcx%100/10);
	PacketBuf[27+5*chNum]=ToHex(adcx%10);
}

//合并操作数和字符串生成数据包
void Merge(int OperationNum, char* Message)
{
	Clear(PacketBuf);
	MessageLength=strlen(Message);
	PacketLength=MessageLength+12;
	PacketBuf[0]=PacketLength;
	PacketBuf[4]=OperationNum;
	PacketBuf[8]=MessageLength;
	PacketBuf[1]=	PacketBuf[2]=PacketBuf[3]=PacketBuf[5]=PacketBuf[6]
	=PacketBuf[7]=PacketBuf[9]=PacketBuf[10]=PacketBuf[11]=0;
  for(j=0;j<MessageLength;j++)
	{
		PacketBuf[12+j]=Message[j];
	}
}

//转化为十六进制
char ToHex(int i)
{
	switch(i)
	{
		case 0:
			i='0';
			break;
		case 1:
			i='1';
			break;
		case 2:
			i='2';
			break;
		case 3:
			i='3';
			break;
		case 4:
			i='4';
			break;
		case 5:
			i='5';
			break;
		case 6:
			i='6';
			break;
		case 7:
			i='7';
			break;
		case 8:
			i='8';
			break;
		case 9:
			i='9';
			break;
		default:
			i='0';
			break;
	}
	return i;
}
