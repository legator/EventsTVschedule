using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerTV.Resources.Class
{
    public class Raketa
    {
        public string channel_number { get; set; }
        public List<Program> program { get; set; }
    }

    public class Program
    {
        public string ut_start { get; set; }
        public string ut_stop { get; set; }
        public string pid { get; set; }
        public string title { get; set; }
    }
}
