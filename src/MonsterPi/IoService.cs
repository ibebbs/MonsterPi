using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace MonsterPi
{
    internal class IoService
    {
        internal static readonly IoService Instance = new IoService();

        private const int DvrGpio = 26;
        private const int DvdGpio = 20;
        private const int TvGpio = 21;

        public IoService()
        {
            Gpio = GpioController.GetDefault();

            if (Gpio != null)
            {
                HasGpio = true;

                DvrRelayPin = Gpio.OpenPin(DvrGpio);
                DvrRelayPin.Write(GpioPinValue.Low);
                DvrRelayPin.SetDriveMode(GpioPinDriveMode.Output);

                DvdRelayPin = Gpio.OpenPin(DvdGpio);
                DvdRelayPin.Write(GpioPinValue.Low);
                DvdRelayPin.SetDriveMode(GpioPinDriveMode.Output);

                TvRelayPin = Gpio.OpenPin(TvGpio);
                TvRelayPin.Write(GpioPinValue.Low);
                TvRelayPin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        public GpioController Gpio { get; private set; }
        public bool HasGpio { get; private set; }
        public GpioPin DvrRelayPin { get; private set; }
        public GpioPin DvdRelayPin { get; private set; }
        public GpioPin TvRelayPin { get; private set; }
    }
}
