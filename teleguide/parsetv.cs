using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.Xml;
using ItemClass;

namespace teleguide
{
    public class parsetv
    {
        private string xmlpath = "C:\\xmltv.xml";

        /// <summary>
        /// Load channels from file
        /// </summary>
        /// <returns></returns>
        public List<ChannelItem> channel_load()
        {
            List<ChannelItem> lch = new List<ChannelItem>(); 
            download_xml();

            StreamReader sr = new StreamReader(xmlpath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            sr.Close();

            foreach (XmlElement xe in doc.DocumentElement.SelectNodes("//channel"))
            {
                ChannelItem ch = new ChannelItem();
                string s = "<root>" + xe.InnerXml + "</root>";
                XmlDocument pdoc = new XmlDocument();
                pdoc.LoadXml(s);

                ch.Id = xe.GetAttribute("id");
                foreach (XmlElement lnks in pdoc.DocumentElement.SelectNodes("//display-name"))
                {
                    ch.Name = lnks.InnerText;
                }
                ch.Category = "ALL";
                lch.Add(ch);
            }
            return lch;
        }

        /// <summary>
        /// Get xml file
        /// </summary>
        private void download_xml()
        {
            WebClient webClient = new WebClient();
            byte[] receivedData = webClient.DownloadData("http://www.teleguide.info/download/new3/xmltv.xml.gz");
            WriteByte2File(xmlpath + ".gz",receivedData);

            byte[] file = File.ReadAllBytes(xmlpath + ".gz");
            byte[] decompressed = Decompress(file);
            WriteByte2File(xmlpath, decompressed);

        }

        /// <summary>
        /// Write byte[] to file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="b"></param>
        private void WriteByte2File(string path, byte[] b)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(b);
            bw.Close();
            fs.Close();
        }

        /// <summary>
        /// Create stream
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns></returns>
        private byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Convert string to DateTime
        /// </summary>
        /// <param name="dat">String which have DateTime</param>
        /// <returns></returns>
        private DateTime DateTimeConvert(string dat)
        { 
            DateTime dt = new DateTime();
            string str = dat.Split(' ')[0];
            string d = "";
            for (int i = 0; i < 4; i++)
            {
                d = d + str[i];
            }
            d = d + "-";
            for (int i = 4; i < 6; i++)
            {
                d = d + str[i];
            }
            d = d + "-";
            for (int i = 6; i < 8; i++)
            {
                d = d + str[i];
            }
            d = d + " ";
            for (int i = 8; i < 10; i++)
            {
                d = d + str[i];
            }
            d = d + ":";
            for (int i = 10; i < 12; i++)
            {
                d = d + str[i];
            }
            d = d + ":";
            for (int i = 12; i < 14; i++)
            {
                d = d + str[i];
            }

            dt = DateTime.ParseExact(d, "yyyy-MM-dd HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            return dt;
        }

        /// <summary>
        /// Get information of action on channel by date
        /// </summary>
        /// <param name="ch">channel id</param>
        /// <param name="date">date</param>
        /// <returns></returns>
        public List<ProgramItem> ActionByChDt(string ch, string date)
        {
            List<ProgramItem> progitem = new List<ProgramItem>();
            StreamReader sr = new StreamReader(xmlpath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            sr.Close();

            foreach (XmlElement xe in doc.DocumentElement.SelectNodes("//programme"))
            {
                if (xe.GetAttribute("channel") == ch)
                {
                    ProgramItem pitem = new ProgramItem();
                    XmlDocument pdoc = new XmlDocument();
                    pdoc.LoadXml("<root>" + xe.InnerXml+"</root>");

                    pitem.Start = DateTimeConvert(xe.GetAttribute("start")).ToString();
                    pitem.End = DateTimeConvert(xe.GetAttribute("stop")).ToString();

                    string dt = "";
                    for (int i = 6; i < 8; i++)
                    {
                        dt = dt + xe.GetAttribute("start")[i];
                    }
                    dt = dt + ".";
                    for (int i = 4; i < 6; i++)
                    {
                        dt = dt + xe.GetAttribute("start")[i];
                    }
                    dt = dt + ".";
                    for (int i = 0; i < 4; i++)
                    {
                        dt = dt + xe.GetAttribute("start")[i];
                    }
                    if (dt==date)
                    {
                        foreach (XmlElement x in pdoc.DocumentElement.SelectNodes("//title"))
                        {
                            pitem.Name = x.InnerText;
                        }
                        try
                        {
                            foreach (XmlElement x in pdoc.DocumentElement.SelectNodes("//desc"))
                            {
                                pitem.Discription = x.InnerText;
                            }
                        }catch(Exception){}
                        try
                        {
                            foreach (XmlElement x in pdoc.DocumentElement.SelectNodes("//category"))
                            {
                                pitem.Category = x.InnerText;
                            }
                        }catch(Exception){}
                    }
                    progitem.Add(pitem);
                }
            }
            return progitem;
        }
    }
}
