using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace STM32Core
{
    public class Decoder
    {
        BuffersReader Reader;
        public void InitData(byte[] data)
        {
            Reader = new BuffersReader(data);
        }

        public Operations? GetOperation()
        {
            int type;
            if (!Reader.ReadInt32(out type))
            {
                return null;
            }
            return (Operations)type;
        }

        public object Decode(Operations operation)
        {
            if (operation == Operations.GetDHT1x)
            {
                string message;
                if (Reader.ReadString(out message, System.Text.Encoding.UTF8))
                {
                    if (message.Contains("Error"))
                    {
                        return "NotFind DHT1x";
                    }
                    else if (message.Contains("Wrong"))
                    {
                        return "Wrong";
                    }
                    else if (message.Contains("Miss"))
                    {
                        return "Miss";
                    }
                    var dts = message.Split('%');
                    return new Humiture() { Humidity = float.Parse(dts[0]), Temperature = float.Parse(dts[1]) };
                }
            }
            if (operation == Operations.SetDigital || operation == Operations.GetDigitalAndAnalog || operation == Operations.SetPWM || 
                operation==Operations.IRLearn || operation==Operations.IRSend)
            {
                string message;
                if (Reader.ReadString(out message, System.Text.Encoding.UTF8))
                {
                    return message;
                }
            }
            return null;
        }
    }
}
