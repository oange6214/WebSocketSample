using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_ProcessEvent.helper
{
    /// <summary>
    /// 建立長期運行的物件
    /// </summary>
    public class Counter
    {
        public event EventHandler ThreadholdReached;

        private int total;
        private CounterStatus status;

        enum CounterStatus
        {
            STOP = 0,
            START = 1,
        }

        public Counter()
        {
            total = 0;
            status = CounterStatus.STOP;

            Run();
        }

        private void Run()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (status == CounterStatus.START)
                    {
                        this.Add(1);
                    }
                    string msg =
                    String.Format("{0} thread information\n", Task.CurrentId.ToString());

                    Console.WriteLine($"=====================");
                    Console.WriteLine($"{msg}");
                    Console.WriteLine($"=====================");


                    // 主動 拋出 異常
                    //if (total > 5)
                    //{
                    //    throw new ArgumentNullException("message");
                    //}


                    Thread.Sleep(1000);
                };
            });
        }

        public void StartRun()
        {
            status = CounterStatus.START;
        }

        public void StopRun()
        {
            status = CounterStatus.STOP;
        }

        // 給內部使用的方法
        private void Add(int x)
        {
            total += x;
            EventArgs eventArgs = new EventArgs();
            OnThresholdReached(eventArgs);
        }
        
        // 執行方法
        protected virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = ThreadholdReached;
            handler?.Invoke(this, e);
        }
    }
}
