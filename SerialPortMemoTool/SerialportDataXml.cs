using System.Collections.Generic;

namespace SerialPortMemoTool
{
    public class SerialportDataXml
    {
        public class DataXml
        {
            public string PortName;
            public string VID;
            public string PID;
            public string MI;
            public string LastConnectTime;
            public int AlertTimeDay;


            public DataXml(SerialportData sd)
            {
                PortName = sd.PortName;
                VID = sd.VID;
                PID = sd.PID;
                MI = sd.MI;
                LastConnectTime = sd.LastConnectTime;
                AlertTimeDay = sd.AlertTimeDay;
            }

            public DataXml() { }
        }

        public string filepath = "app.xml";

        [System.Xml.Serialization.XmlElementAttribute("datas")]
        public List<DataXml> datas { get; set; }

        public SerialportDataXml() { datas = new List<DataXml>(); }

        public void ReadXml(List<SerialportData> target)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(SerialportDataXml));

            try
            {
                using (var file = new System.IO.StreamReader(filepath))
                {
                    SerialportDataXml sdx = (SerialportDataXml)reader.Deserialize(file);

                    foreach (var data in sdx.datas)
                    {
                        target.Add(new SerialportData(data));
                    }
                }
            }
            catch
            {

            }
        }

        public void WriteXml(List<SerialportData> target)
        {
            System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(SerialportDataXml));

            SerialportDataXml sdx = new SerialportDataXml();

            foreach (var data in target)
            {
                sdx.datas.Add(new SerialportDataXml.DataXml(data));
            }

            try
            {
                using (var file = System.IO.File.Create(filepath))
                {
                    writer.Serialize(file, sdx);
                }
            }
            catch
            {

            }
        }

    }
}