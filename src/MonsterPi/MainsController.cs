using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using Devkoes.Restup.WebServer.Rest.Models.Contracts;
using System;
using Windows.Devices.Gpio;

namespace MonsterPi
{
    public enum Socket
    {
        Dvr,
        Dvd,
        Tv
    }

    public enum PowerState
    {
        On,
        Off
    }

    public sealed class SocketState
    {
        public string Socket { get; set; }

        public string State { get; set; }
    }

    [RestController(InstanceCreationType.Singleton)]
    public sealed class MainsController
    {
        private static bool TryGetGpioPin(string value, out Socket socket, out GpioPin pin)
        {
            if (Enum.TryParse(value, true, out socket))
            {
                switch (socket)
                {
                    case Socket.Dvr:
                        pin = IoService.Instance.DvrRelayPin;
                        return true;
                    case Socket.Dvd:
                        pin = IoService.Instance.DvdRelayPin;
                        return true;
                    case Socket.Tv:
                        pin = IoService.Instance.TvRelayPin;
                        return true;
                    default:
                        pin = null;
                        return false;
                }
            }
            else
            {
                pin = null;
                return false;
            }
        }

        private static bool TryGetGpioPinValue(string state, out PowerState powerState, out GpioPinValue pinValue)
        {
            if (Enum.TryParse(state, true, out powerState))
            {
                switch (powerState)
                {
                    case PowerState.Off:
                        pinValue = GpioPinValue.High;
                        return true;
                    case PowerState.On:
                        pinValue = GpioPinValue.Low;
                        return true;
                    default:
                        pinValue = GpioPinValue.Low;
                        return false;
                }
            }
            else
            {
                pinValue = GpioPinValue.Low;
                return false;
            }
        }

        [UriFormat("/socket/{socketName}")]
        public IGetResponse GetSocketState(string socketName)
        {
            if (IoService.Instance.HasGpio)
            {
                Socket socket;
                GpioPin pin;

                if (TryGetGpioPin(socketName, out socket, out pin))
                {
                    GpioPinValue pinValue = pin.Read();

                    return new GetResponse(GetResponse.ResponseStatus.OK, new SocketState { Socket = socket.ToString(), State = (pinValue == GpioPinValue.High) ? PowerState.Off.ToString() : PowerState.On.ToString() });
                }
                else
                {
                    return new GetResponse(GetResponse.ResponseStatus.NotFound, $"Socket {socket} not found");
                }
            }
            else
            {
                return new GetResponse(GetResponse.ResponseStatus.NotFound, $"GPIO not found");
            }
        }

        [UriFormat("/socket/{socketName}?state={stateName}")]
        public IPutResponse SetSocketState(string socketName, string stateName)
        {
            if (IoService.Instance.HasGpio)
            {
                Socket socket;
                GpioPin pin;

                if (TryGetGpioPin(socketName, out socket, out pin))
                {
                    PowerState state;
                    GpioPinValue pinValue;

                    if (TryGetGpioPinValue(stateName, out state, out pinValue))
                    {
                        pin.Write(pinValue);

                        return new PutResponse(PutResponse.ResponseStatus.OK, new SocketState { Socket = socket.ToString(), State = state.ToString() });
                    }
                    else
                    {
                        return new PutResponse(PutResponse.ResponseStatus.NotFound, $"Invalid state: {state}");
                    }
                }
                else
                {
                    return new PutResponse(PutResponse.ResponseStatus.NotFound, $"Socket {socket} not found");
                }
            }
            else
            {
                return new PutResponse(PutResponse.ResponseStatus.NotFound, $"GPIO not found");
            }
        }
    }
}
