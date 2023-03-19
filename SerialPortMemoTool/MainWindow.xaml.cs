using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SerialPortMemoTool
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        DateTime timestamp = new DateTime();

        List<SerialportData> datas = new List<SerialportData>();
        SerialportDataXml sdx = new SerialportDataXml();

        private BackgroundWorker bgwDriveDetector;

        public MainWindow()
        {
            InitializeComponent();

            bgwDriveDetector = new BackgroundWorker();
            bgwDriveDetector.DoWork += bgwDriveDetector_DoWork;
            bgwDriveDetector.RunWorkerAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timestamp = DateTime.Now;

            sdx.ReadXml(datas);
            dgridMain.ItemsSource = datas;

            SearchSerialPortNameStart();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            SearchSerialPortNameStart();
        }

        private void SearchSerialPortNameStart()
        {
            List<SerialportData> reg_datas = new List<SerialportData>();
            String pattern = String.Format("^VID_.*PID_.*");
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            //comports.Add((string)rk6.GetValue("PortName"));
                            var obj = rk6.GetValue("PortName");

                            if (obj != null)
                            {
                                string portname = obj.ToString();
                                if (portname.Length != 0)
                                {
                                    txtDebug.Text += s + ":" + portname + Environment.NewLine;

                                    SerialportData sd = new SerialportData(portname, s);
                                    reg_datas.Add(sd);

                                }
                            }
                        }
                    }
                }
            }

            List<SerialportData> new_datas = new List<SerialportData>();

            foreach (var regdata in reg_datas)
            {

                SerialportData serialportData = regdata;//new SerialportData();
                
                //serialportData.PortName = data.PortName;
                //serialportData.PID = data.PID;
                //serialportData.VID = data.VID;
                //serialportData.MI = data.MI;

                foreach (var data in datas)
                {
                    if (regdata.PortName == data.PortName)
                    {
                        serialportData.LastConnectTime = data.LastConnectTime;
                        serialportData.AlertTimeDay = data.AlertTimeDay;
                    }
                }


                new_datas.Add(serialportData);
            }

            datas.Clear();
            datas = new_datas;
            dgridMain.ItemsSource = datas;
        }

        private void btnTestSerialName_Click(object sender, RoutedEventArgs e)
        {
            GetAllSerialPortName();
        }


        bool g_start_insert = false;
        private async void GetAllSerialPortName()
        {
            foreach (String s in SerialPort.GetPortNames())
            {
                await Dispatcher.BeginInvoke(new Action(() => { txtDebug.Text += s + Environment.NewLine; }));

                foreach (var item in dgridMain.Items)
                {
                    if (item is SerialportData)
                    {
                        SerialportData target = ((SerialportData)item);
                        if (String.Equals(target.PortName, s))
                        {
                            target.LastConnectTime = DateTime.Now.ToString();
                            await Dispatcher.BeginInvoke(new Action(() => {
                                dgridMain.Items.Refresh();
                            }));
                        }
                    }
                }
            }

            sdx.WriteXml(datas);
            g_start_insert = false;
        }

        private void btnReadXml_Click(object sender, RoutedEventArgs e)
        {
            datas.Clear();

            sdx.ReadXml(datas);

            dgridMain.ItemsSource = datas;

            dgridMain.Items.Refresh();
        }

        private void btnWriteXml_Click(object sender, RoutedEventArgs e)
        {
            sdx.WriteXml(datas);
        }


        //https://stackoverflow.com/questions/620144/detecting-usb-drive-insertion-and-removal-using-windows-service-and-c-sharp/19435744#19435744
        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            if (!g_start_insert)
            {
                //string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
                GetAllSerialPortName();
                g_start_insert = true;
            }
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            //string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
            //GetAllSerialPortName();
        }

        void bgwDriveDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            var insertQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            var insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += DeviceInsertedEvent;
            insertWatcher.Start();

            var removeQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            var removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += DeviceRemovedEvent;
            removeWatcher.Start();
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetVisualChild<T>(v);
                else
                    break;
            }
            return child;
        }

        private void MenuItemStart_Click(object sender, RoutedEventArgs e)
        {
            SearchSerialPortNameStart();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItemSaveAlertTimeDay_Click(object sender, RoutedEventArgs e)
        {
            sdx.WriteXml(datas);
        }
    }

    //https://support.microsoft.com/ja-jp/topic/wpf-%E3%81%AE-datagrid-%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC%E3%83%AB%E3%81%AB%E3%81%8A%E3%81%84%E3%81%A6-%E5%85%A5%E5%8A%9B%E3%81%95%E3%82%8C%E3%81%9F%E5%80%A4%E3%81%AE%E6%A4%9C%E8%A8%BC%E3%82%92%E8%A1%8C%E3%81%A3%E3%81%9F%E5%A0%B4%E5%90%88-%E6%A4%9C%E8%A8%BC%E3%82%A8%E3%83%A9%E3%83%BC%E3%82%92%E7%A4%BA%E3%81%99%E8%B5%A4%E6%9E%A0%E3%81%8C%E3%81%9A%E3%82%8C%E3%81%A6%E8%A1%A8%E7%A4%BA%E3%81%95%E3%82%8C%E3%82%8B-f337de29-113d-bbdc-b607-49c01656e1b0
    public class NullCheckValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;
            if (String.IsNullOrEmpty(str))
                return new ValidationResult(false, "error");
            else
                return new ValidationResult(true, null);
        }
    }

}
