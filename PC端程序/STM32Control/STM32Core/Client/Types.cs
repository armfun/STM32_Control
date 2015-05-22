using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Core
{
    public class Humiture
    {
        public float Temperature { set; get; }
        public float Humidity { set; get; }
    }

    public class GPIOs
    {
        public int[] Analogs { set; get; }
        public byte[] Digitals { set; get; }
    }

    public enum Operations
    {
        GetDHT1x,
        SetDigital,
        GetDigitalAndAnalog,
        SetPWM,
        IRLearn,
        IRSend
    }

    public enum DigitalLevel
    {
        LOW,
        HIGH
    }
}
