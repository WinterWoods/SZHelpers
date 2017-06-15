using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    /// <summary>
    /// 定时任务服务类
    ///</summary>
    public class TimerTaskService:IDisposable
    {

        #region  定时任务实例成员

        private TimerInfo timerInfo;  //定时信息

        private Action TimerTaskDelegateFun = null; //执行具体任务的委托方法

        private Action<object> ParmTimerTaskDelegateFun = null; //带参数的执行具体任务的委托方法
        private object parm = null; //参数

        private Task TaskService;

        CancellationTokenSource ct = new CancellationTokenSource();

        private bool isStart = false;
        TimeSpan _timerSpanDay = new TimeSpan(0, 0, 1);
        TimeSpan _timerSpanMil= new TimeSpan(0, 0, 0, 0, 1);
        /// <summary>
        ///  下一次执行时间休眠
        /// </summary>
        private TimeSpan timeSpan = new TimeSpan(100);

        private DateTime nextRunTime;
        
        /// <summary>
        /// 根据定时信息构造定时任务服务
        /// </summary>
        /// <param name="_timer"></param>
        private TimerTaskService(TimerInfo _timer)
        {
            timerInfo = _timer;
        }

        /// <summary>
        /// 根据定时信息和执行具体任务的委托方法构造定时任务服务
        /// </summary>
        /// <param name="_timer">定时信息</param>
        /// <param name="trd">执行具体任务的委托方法</param>
        private TimerTaskService(TimerInfo _timer, Action trd)
        {
            timerInfo = _timer;
            TimerTaskDelegateFun = trd;
        }

        /// <summary>
        /// 根据定时信息和执行具体任务的委托方法构造定时任务服务
        /// </summary>
        /// <param name="_timer">定时信息</param>
        /// <param name="ptrd">带参数执行具体任务的委托方法</param>
        private TimerTaskService(TimerInfo _timer, Action<object> ptrd)
        {
            timerInfo = _timer;
            ParmTimerTaskDelegateFun = ptrd;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="_parm"></param>
        private void setParm(object _parm)
        {
            parm = _parm;
        }


        /// <summary>
        /// 启动定时任务
        /// </summary>
        public void Start()
        {
            TaskService.Start();
        }
        public void Stop()
        {
            isStart = false;
            ct.Cancel();
            
        }

        /// <summary>
        /// 检查定时器
        /// </summary>
        private void Run(CancellationTokenSource ct)
        {
            //Console.WriteLine("Run");
            if(!isStart)
            {
                isStart = true;
                //计算下次执行时间
                getNextRunTime();
                
            }
            while (isStart)
            {

                //采用时间比对方式
                if(DateTime.Now<nextRunTime)
                {
                    Task.Delay(_timerSpanMil).Wait();
                    //Thread.Sleep(_timerSpanMil);
                    
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                    continue;
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }


                ////Console.WriteLine("timeSpan < _timerSpanMil");
                ////如果发现计算时间小于跳过
                //if (timeSpan < _timerSpanMil)
                //{
                //    Console.WriteLine("break;");
                //    break;
                //}
                ////Console.WriteLine("Sleep(timeSpan);");
                ////休眠.准备到下一次运行
                //Sleep(timeSpan,ct);
                ////Thread.Sleep(timeSpan);
                ////如果在运行期间停止则退出
                ////Console.WriteLine("if (!isStart) break;");
                //if (!isStart) break;
                //if (ct.IsCancellationRequested)
                //{
                //    Console.WriteLine("ct.IsCancellationRequested");
                //    break;
                //}

                //不采用休眠方式




                Task task = null;
                //调用执行处理方法
               // Console.WriteLine("if (TimerTaskDelegateFun != null)");
                if (TimerTaskDelegateFun != null)
                {
                    //Console.WriteLine("task = new Task(TimerTaskDelegateFun);");
                    task = new Task(TimerTaskDelegateFun);
                }
                else if (ParmTimerTaskDelegateFun != null)
                {
                    //Console.WriteLine("task = new Task(ParmTimerTaskDelegateFun, parm);");
                    task = new Task(ParmTimerTaskDelegateFun, parm);
                }
                else
                {
                    //Console.WriteLine("break;");
                    break;
                }
               // Console.WriteLine("task.Start();");
                task.Start();
                //Console.WriteLine("if (timerInfo.TimerType == TimerType.LoopStop)");
                if (timerInfo.TimerType == TimerType.LoopStop)
                {
                    //Console.WriteLine("task.Wait();");
                    task.Wait();
                }
                else if (timerInfo.TimerType == TimerType.DesDate)
                {
                   // Console.WriteLine("task.Wait(); break;");
                    task.Wait();
                    break;
                }
                //Console.WriteLine("getNextRunTime();");
                //重新计算下次执行时间
                getNextRunTime();
            }
        }
        private void Sleep(TimeSpan timeSpan, CancellationTokenSource ct)
        {
            while(timeSpan>_timerSpanDay)
            {
                if (!ct.IsCancellationRequested)
                {
                    timeSpan = timeSpan - _timerSpanDay;
                    Thread.Sleep(_timerSpanDay);
                }
                else
                {
                    Console.WriteLine("任务取消了……");
                    break;
                }
            }
            
            //Thread.CurrentThread.Join(timeSpan);
            Thread.Sleep(timeSpan);
            Console.WriteLine("休眠完成.准备执行");
        }

        /// <summary>
        /// 计算下一次执行休眠时间
        /// </summary>
        /// <returns></returns>
        private void getNextRunTime()
        {
            DateTime DateTimeNow = DateTime.Now;
            if (timerInfo.TimerType == TimerType.DesDate)
            {
                nextRunTime=new DateTime(timerInfo.Year, timerInfo.Month,timerInfo.Day,timerInfo.Hour,timerInfo.Minute,timerInfo.Second,timerInfo.Millisecond);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if(timerInfo.TimerType==TimerType.EveryYear)
            {
                nextRunTime=new DateTime(DateTimeNow.Year,timerInfo.Month,timerInfo.Day,timerInfo.Hour,timerInfo.Minute,timerInfo.Second,timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime = nextRunTime.AddYears(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if (timerInfo.TimerType == TimerType.EveryMonth)
            {
                nextRunTime = new DateTime(DateTimeNow.Year, DateTimeNow.Month, timerInfo.Day, timerInfo.Hour, timerInfo.Minute, timerInfo.Second, timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime = nextRunTime.AddMonths(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if (timerInfo.TimerType == TimerType.EveryDay)
            {
                nextRunTime = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day, timerInfo.Hour, timerInfo.Minute, timerInfo.Second, timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime = nextRunTime.AddDays(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if (timerInfo.TimerType == TimerType.EveryHour)
            {
                nextRunTime = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day, DateTimeNow.Hour, timerInfo.Minute, timerInfo.Second, timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime = nextRunTime.AddHours(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if (timerInfo.TimerType == TimerType.EveryMinute)
            {
                nextRunTime = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day, DateTimeNow.Hour, DateTimeNow.Minute, timerInfo.Second,timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime = nextRunTime.AddMinutes(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else if (timerInfo.TimerType == TimerType.EverySecond)
            {
                nextRunTime = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day, DateTimeNow.Hour, DateTimeNow.Minute, DateTimeNow.Second, timerInfo.Millisecond);
                while ((nextRunTime - DateTimeNow) < _timerSpanMil)
                nextRunTime=nextRunTime.AddSeconds(1);
                timeSpan = nextRunTime - DateTimeNow;
            }
            else 
            {
               nextRunTime =DateTimeNow.AddYears(timerInfo.Year).AddMonths(timerInfo.Month).AddDays(timerInfo.Day).AddHours(timerInfo.Hour).AddMinutes(timerInfo.Minute).AddSeconds(timerInfo.Second).AddMilliseconds(timerInfo.Millisecond);
               timeSpan = nextRunTime - DateTimeNow;
            }
            
        }


        #endregion


        #region 创建定时任务静态方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="_trd"></param>
        /// <returns></returns>
        public static TimerTaskService CreateTimerTaskService(TimerInfo info, Action _trd)
        {
            TimerTaskService tus = new TimerTaskService(info, _trd);
            //创建启动线程
            tus.TaskService = new Task(() => tus.Run(tus.ct));
            return tus;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="_ptrd"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public static TimerTaskService CreateTimerTaskService(TimerInfo info, Action<object> _ptrd, object parm)
        {
            TimerTaskService tus = new TimerTaskService(info, _ptrd);
            tus.setParm(parm);

            //创建启动线程
            tus.TaskService = new Task(() => tus.Run(tus.ct));
            return tus;
        }
        #endregion

        public void Dispose()
        {
            isStart = false;
            ct.Cancel();
            
        }
    }
    /// <summary>
    /// 计划任务配置属性
    /// </summary>
    public class TimerInfo
    {
        private TimerType timerType;
        /// <summary>
        /// 如果选择Every类型,会在每年的后边属性进行执行,只会执行比当前小的单位.
        /// </summary>
        public TimerType TimerType
        {
            get { return timerType; }
            set { timerType = value; }
        }
        private int year =0;
        /// <summary>
        /// 年
        /// </summary>
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        private int month = 0;
        /// <summary>
        /// 月
        /// </summary>
        public int Month
        {
            get { return month; }
            set { month = value; }
        }
        private int day = 0;
        /// <summary>
        /// 天
        /// </summary>
        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        private int hour = 0;
        /// <summary>
        /// 小时
        /// </summary>
        public int Hour
        {
            get { return hour; }
            set { hour = value; }
        }
        private int minute = 0;
        /// <summary>
        /// 分钟
        /// </summary>
        public int Minute
        {
            get { return minute; }
            set { minute = value; }
        }
        private int second = 0;
        /// <summary>
        /// 秒
        /// </summary>
        public int Second
        {
            get { return second; }
            set { second = value; }
        }
        private int millisecond = 0;
        /// <summary>
        /// 毫秒
        /// </summary>
        public int Millisecond
        {
            get { return millisecond; }
            set { millisecond = value; }
        }
    }    
    /// <summary>
    /// 计划任务类型
    /// </summary>
    public enum TimerType
    {
        /// <summary>
        /// 秒
        /// </summary>
        EverySecond,
        /// <summary>
        /// 分钟
        /// </summary>
        EveryMinute,
        /// <summary>
        /// 小时
        /// </summary>
        EveryHour,
        /// <summary>
        /// 天
        /// </summary>
        EveryDay,
        /// <summary>
        /// 周
        /// </summary>
        EveryWeek,
        /// <summary>
        /// 月
        /// </summary>
        EveryMonth,
        /// <summary>
        /// 
        /// </summary>
        EveryYear,
        /// <summary>
        /// 制定日期,执行一次后停止
        /// </summary>
        DesDate,
        /// <summary>
        /// 在上一个停止后 加上循环时间
        /// </summary>
        LoopStop,
        /// <summary>
        /// 在上一个开始时加上循环时间
        /// </summary>
        LoopStart,
        /// <summary>
        /// 每一个时间
        /// </summary>
        

    }

}
