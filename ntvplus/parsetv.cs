using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemClass;

namespace ntvplus
{
    public class parsetv
    {
        /// <summary>
        /// Load ntv plus channels
        /// </summary>
        /// <returns></returns>
        public List<ChannelItem> channel_load()
        {
            List<ChannelItem> lch = new List<ItemClass.ChannelItem>();
            string[,] ch;
            string[] cc = new string[] { "Кино", "Спорт", "Познавательное", "HD", "Музыка", "Новости", "Юмор", "Детям", "Эфирные канали", "Другое", "Сериалы", "Эротика" };
            HtmlWeb hwObject = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmldocObject = hwObject.Load("http://www.ntvplus.ru/tv/lite.xl");
            int ij = 0;
            //
            foreach (HtmlNode link in htmldocObject.DocumentNode.SelectNodes("//select[@name='channel']/optgroup"))
            {
                string[] str = link.InnerHtml.Split('<');
                foreach (string st in str)
                {
                    if (st.Length != 0)
                    {
                        string[] ss = st.Split('>');
                        string name = ss[1];
                        string[] s = ss[0].Split('"');
                        string code = s[1];
                        bool isd = true;
                        if (isd)
                        {
                            ItemClass.ChannelItem ci = new ItemClass.ChannelItem();
                            ci.Id = code;
                            ci.Name = name;
                            ci.Category = cc[ij];
                            lch.Add(ci);
                        }
                    }
                }
                ij++;
            }
            return lch;
        }

        /// <summary>
        /// Load date
        /// </summary>
        /// <returns></returns>
        public List<DateItem> date_load()
        {
            List<DateItem> ldt = new List<DateItem>();
            HtmlWeb hwObject = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmldocObject = hwObject.Load("http://www.ntvplus.ru/tv/lite.xl");
            foreach (HtmlNode link in htmldocObject.DocumentNode.SelectNodes("//select[@name='day']"))
            {
                string[] dts = link.InnerHtml.ToString().Split('<');
                for (int i = 0; i < dts.Length; i++)
                {
                    string[] d = dts[i].Split('>');
                    if (d.Length != 1)
                    {
                        string ddet = d[1];
                        string[] c = d[0].Split('"');
                        if (c.Length != 1)
                        {
                            DateItem di = new DateItem();
                            string dat = c[1];
                            di.DateInt = dat;
                            di.DateLong = ddet;
                            ldt.Add(di);
                        }
                    }
                }
            }
            return ldt;
        }

        /// <summary>
        /// Get information of action on channel by date
        /// </summary>
        /// <param name="ch">channel id</param>
        /// <param name="date">date</param>
        /// <returns></returns>
        public List<ProgramItem> ActionByChDt(string ch, string date)
        {
            List<ProgramItem> items = new List<ProgramItem>();
            Boolean iscurrent = false;
            HtmlWeb hwObject = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmldocObject = hwObject.Load("http://www.ntvplus.ru/tv/lite.xl?channel=" + ch + "&day=" + date);
            try
            {
                foreach (HtmlNode links in htmldocObject.DocumentNode.SelectNodes("//div[@class='row2']"))
                {
                    HtmlAgilityPack.HtmlDocument htmlDocuments = new HtmlAgilityPack.HtmlDocument();
                    htmlDocuments.LoadHtml(links.InnerHtml);
                    try
                    {
                        foreach (HtmlNode link in htmlDocuments.DocumentNode.SelectNodes("//div[@class='program current']"))
                        {
                            HtmlAgilityPack.HtmlDocument htmls = new HtmlAgilityPack.HtmlDocument();
                            htmls.LoadHtml(link.InnerHtml);
                            ProgramItem it = new ProgramItem();
                            iscurrent = true;
                            foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='time']"))
                            {
                                it.Start = lnk.InnerText;
                            }
                            foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='content']"))
                            {
                                it.Name = lnk.InnerText;
                            }
                            it.Discription = null;
                            it.Dateint = date;
                            items.Add(it);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) {}

            try
            {
                foreach (HtmlNode links in htmldocObject.DocumentNode.SelectNodes("//div[@class='row2']"))
                {
                    HtmlAgilityPack.HtmlDocument htmlDocuments = new HtmlAgilityPack.HtmlDocument();
                    htmlDocuments.LoadHtml(links.InnerHtml);
                    if (iscurrent)
                    {
                        try
                        {
                            foreach (HtmlNode link in htmlDocuments.DocumentNode.SelectNodes("//div[@class='program']"))
                            {
                                HtmlAgilityPack.HtmlDocument htmls = new HtmlAgilityPack.HtmlDocument();
                                htmls.LoadHtml(link.InnerHtml);
                                ProgramItem it = new ProgramItem();
                                foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='time']"))
                                {
                                    it.Start = lnk.InnerText;
                                }
                                foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='content']"))
                                {
                                    it.Name = lnk.InnerText;
                                }
                                it.Discription = null;
                                it.Dateint = date;
                                items.Add(it);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception) { }

            try
            {
                foreach (HtmlNode links in htmldocObject.DocumentNode.SelectNodes("//div[@class='row2 lastrow']"))
                {
                    HtmlAgilityPack.HtmlDocument htmlDocuments = new HtmlAgilityPack.HtmlDocument();
                    htmlDocuments.LoadHtml(links.InnerHtml);
                    if (!iscurrent)
                    {
                        try
                        {
                            foreach (HtmlNode link in htmlDocuments.DocumentNode.SelectNodes("//div[@class='program current']"))
                            {
                                HtmlAgilityPack.HtmlDocument htmls = new HtmlAgilityPack.HtmlDocument();
                                htmls.LoadHtml(link.InnerHtml);
                                ProgramItem it = new ProgramItem();
                                foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='time']"))
                                {
                                    it.Start = lnk.InnerText;
                                }
                                foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='content']"))
                                {
                                    it.Name = lnk.InnerText;
                                }
                                it.Discription = null;
                                it.Dateint = date;
                                items.Add(it);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception) { }

            try
            {
                foreach (HtmlNode links in htmldocObject.DocumentNode.SelectNodes("//div[@class='row2 lastrow']"))
                {
                    HtmlAgilityPack.HtmlDocument htmlDocuments = new HtmlAgilityPack.HtmlDocument();
                    htmlDocuments.LoadHtml(links.InnerHtml);
                    try
                    {
                        foreach (HtmlNode link in htmlDocuments.DocumentNode.SelectNodes("//div[@class='program']"))
                        {
                            HtmlAgilityPack.HtmlDocument htmls = new HtmlAgilityPack.HtmlDocument();
                            htmls.LoadHtml(link.InnerHtml);
                            ProgramItem it = new ProgramItem();
                            foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='time']"))
                            {
                                it.Start = lnk.InnerText;
                            }
                            foreach (HtmlNode lnk in htmls.DocumentNode.SelectNodes("//div[@class='content']"))
                            {
                                it.Name = lnk.InnerText;
                            }
                            it.Discription = null;
                            it.Dateint = date;
                            items.Add(it);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }


            return AnalyzeProgramItem(items);
        }

        /// <summary>
        /// Prepeare program to reading
        /// </summary>
        /// <param name="progitem"></param>
        /// <returns></returns>
        private List<ProgramItem> AnalyzeProgramItem(List<ProgramItem> progitem)
        {
            bool isnext = false;
            
            for (int i = 1; i < progitem.Count; i++)
            {

                string[] p1 = progitem[i - 1].Start.Split(':');
                string[] p2 = progitem[i].Start.Split(':');
                if (isnext)
                {
                    string[] d = progitem[i].Dateint.Split('.');
                    progitem[i].Dateint = (Convert.ToInt32(d[0]) + 1).ToString() + "." + d[1] + "." + d[2];
                }
                else
                    if (Convert.ToInt32(p2[0]) < Convert.ToInt32(p1[0]))
                    {
                        isnext = true;
                        Console.WriteLine("true");
                        string[] d = progitem[i].Dateint.Split('.');
                        progitem[i].Dateint = (Convert.ToInt32(d[0])+1).ToString() + "." + d[1] + "." + d[2];
                    }
            }

            for (int i = 0; i < progitem.Count - 1; i++)
            {
                string[] p1 = progitem[i].Start.Split(':');
                string[] p2 = progitem[i+1].Start.Split(':');
                if (progitem[i].Dateint == progitem[i + 1].Dateint)
                {
                    progitem[i].End = (Convert.ToInt32(p2[0]) * 60 + Convert.ToInt32(p2[1]) - Convert.ToInt32(p1[0]) * 60 - Convert.ToInt32(p1[1])).ToString();
                }
                else
                {
                    int dur = 24 * 60 + Convert.ToInt32(p2[1]) - Convert.ToInt32(p1[0]) * 60 - Convert.ToInt32(p1[1]);
                    if (dur<0)
                    {
                        dur = Convert.ToInt32(p1[0]) * 60 - Convert.ToInt32(p1[1]);
                    }
                    progitem[i].End = dur.ToString();
                }
            }

            return progitem;
        }
    }
}
