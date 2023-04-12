using ReTimerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using outRp.ReTimerEvent;
using System.Threading.Tasks;

namespace outRp.Utils
{
    public class ServerUtil
    {
        // ReTimer线程
        private static Thread reTimerWorker;

        public static void InitReTimerEvent()
        {
            reTimerWorker = new(OnReTimerConsumer);
            reTimerWorker.IsBackground = true;
            reTimerWorker.Start();
        }

        // ReTimer消费者
        public static void OnReTimerConsumer()
        {
            // 根据配置调高消费者, 最高可达每秒触发数十万个计时器. 
            const int maxConsumer = 150000;
            while (true)
            {
                Thread.Sleep(1000);
                for (int i = 0; i < maxConsumer / 2; i++)
                {
                    ReTimerLib.Model.Timer timer = ReTimer.Service.GetNoticeTimer();
                    if (timer == null) continue;
                    ReTimerEvents.OnReTimerEvent(timer);
                }
                for (int i = maxConsumer / 2; i < maxConsumer; i++)
                {
                    ReTimerLib.Model.Timer timer = ReTimer.Service.GetNoticeTimer();
                    if (timer == null) continue;
                    ReTimerEvents.OnReTimerEvent(timer);
                }
            }
        }
    }
}
