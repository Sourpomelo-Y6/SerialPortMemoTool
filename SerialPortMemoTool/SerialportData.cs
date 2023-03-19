using System;
using System.Text.RegularExpressions;

namespace SerialPortMemoTool
{
    public class SerialportData
    {
        public string PortName { set; get; }

        int? _pid;
        public string PID
        {
            set
            {
                int i;
                _pid = int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out i) ? (int?)i : null;
            }

            get
            {
                return (_pid.HasValue) ? _pid.Value.ToString("X4") : "null";
            }
        }

        int? _vid;
        public string VID
        {
            set
            {
                int i;
                _vid = int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out i) ? (int?)i : null;
            }

            get
            {
                return (_vid.HasValue) ? _vid.Value.ToString("X4") : "null";
            }
        }

        int? _mi;
        public string MI
        {
            set
            {
                int i;
                _mi = int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out i) ? (int?)i : null;
            }

            get
            {
                return (_mi.HasValue) ? _mi.Value.ToString("X2") : "null";
            }
        }


        bool _dateset;
        DateTime lastconnect;
        public string LastConnectTime
        {
            set
            {
                try
                {
                    lastconnect = DateTime.Parse(value);
                    _dateset = true;
                }
                catch (FormatException)
                {
                    _dateset = false;
                }
            }
            get
            {
                if (_dateset) return lastconnect.ToString();
                else return "null";
            }
        }

        public bool IsTimeOut
        {
            get
            {
                if (LastConnectTime == "null") { return true; }
                var timeSpan = DateTime.Now - DateTime.Parse(LastConnectTime);
                return timeSpan.TotalDays > AlertTimeDay;
            }
        }

        public int AlertTimeDay { get; set; }

        public SerialportData(string portname, string s)
        {
            PortName = portname;
            {
                String pattern = String.Format("{0}_[0-9a-fA-F]+", nameof(VID));
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

                if (_rx.Match(s).Success)
                {
                    string test = _rx.Match(s).Value;
                    VID = test.Substring(4);
                }
            }
            {
                String pattern = String.Format("{0}_[0-9a-fA-F]+", nameof(PID));
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

                if (_rx.Match(s).Success)
                {
                    string test = _rx.Match(s).Value;
                    PID = test.Substring(4);
                }
            }
            {
                String pattern = String.Format("{0}_[0-9a-fA-F]+", nameof(MI));
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

                if (_rx.Match(s).Success)
                {
                    string test = _rx.Match(s).Value;
                    MI = test.Substring(3);
                }
            }

            _dateset = false;
        }

        public SerialportData() { }

        public SerialportData(SerialportDataXml.DataXml sd)
        {

            PortName = sd.PortName;
            VID = sd.VID;
            PID = sd.PID;
            MI = sd.MI;
            LastConnectTime = sd.LastConnectTime;
            AlertTimeDay = sd.AlertTimeDay;
        }
    }
}