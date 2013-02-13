using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerTV.Resources.Class
{
    public class KeyWords
    {
        public List<Key> Keys { get; set; }
    }

    public class Key
    {
        public string KeyName { get; set; }
        public string Category { get; set; } 
    }
}
