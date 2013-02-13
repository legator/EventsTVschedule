using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SchedulerTV.Resources.Class
{
    class AsyncThread
    {
        public static void InvokeAsync(Action action)
        {
            ThreadStart threadStart = () => action();
            var thread = new Thread(threadStart);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
