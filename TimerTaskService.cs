using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    /// <summary>
    /// ��ʱ���������
    ///</summary>
    public class TimerTaskService:IDisposable
    {

        #region  ��ʱ����ʵ����Ա

        private TimerInfo timerInfo;  //��ʱ��Ϣ

        private Action TimerTaskDelegateFun = null; //ִ�о��������ί�з���

        private Action<object> ParmTimerTaskDelegateFun = null; //��������ִ�о��������ί�з���
        private object parm = null; //����

        private Task TaskService;

        CancellationTokenSource ct = new CancellationTokenSource();

        private bool isStart = false;
        TimeSpan _timerSpanDay = new TimeSpan(0, 0, 1);
        TimeSpan _timerSpanMil= new TimeSpan(0, 0, 0, 0, 1);
        /// <summary>
        ///  ��һ��ִ��ʱ������
        /// </summary>
        private TimeSpan timeSpan = new TimeSpan(100);

        private DateTime nextRunTime;
        
        /// <summary>
        /// ���ݶ�ʱ��Ϣ���춨ʱ�������
        /// </summary>
        /// <param name="_timer"></param>
        private TimerTaskService(TimerInfo _timer)
        {
            timerInfo = _timer;
        }

        /// <summary>
        /// ���ݶ�ʱ��Ϣ��ִ�о��������ί�з������춨ʱ�������
        /// </summary>
        /// <param name="_timer">��ʱ��Ϣ</param>
        /// <param name="trd">ִ�о��������ί�з���</param>
        private TimerTaskService(TimerInfo _timer, Action trd)
        {
            timerInfo = _timer;
            TimerTaskDelegateFun = trd;
        }

        /// <summary>
        /// ���ݶ�ʱ��Ϣ��ִ�о��������ί�з������춨ʱ�������
        /// </summary>
        /// <param name="_timer">��ʱ��Ϣ</param>
        /// <param name="ptrd">������ִ�о��������ί�з���</param>
        private TimerTaskService(TimerInfo _timer, Action<object> ptrd)
        {
            timerInfo = _timer;
            ParmTimerTaskDelegateFun = ptrd;
        }

        /// <summary>
        /// ���ò���
        /// </summary>
        /// <param name="_parm"></param>
        private void setParm(object _parm)
        {
            parm = _parm;
        }


        /// <summary>
        /// ������ʱ����
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
        /// ��鶨ʱ��
        /// </summary>
        private void Run(CancellationTokenSource ct)
        {
            //Console.WriteLine("Run");
            if(!isStart)
            {
                isStart = true;
                //�����´�ִ��ʱ��
                getNextRunTime();
                
            }
            while (isStart)
            {

                //����ʱ��ȶԷ�ʽ
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
                ////������ּ���ʱ��С������
                //if (timeSpan < _timerSpanMil)
                //{
                //    Console.WriteLine("break;");
                //    break;
                //}
                ////Console.WriteLine("Sleep(timeSpan);");
                ////����.׼������һ������
                //Sleep(timeSpan,ct);
                ////Thread.Sleep(timeSpan);
                ////����������ڼ�ֹͣ���˳�
                ////Console.WriteLine("if (!isStart) break;");
                //if (!isStart) break;
                //if (ct.IsCancellationRequested)
                //{
                //    Console.WriteLine("ct.IsCancellationRequested");
                //    break;
                //}

                //���������߷�ʽ




                Task task = null;
                //����ִ�д�����
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
                //���¼����´�ִ��ʱ��
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
                    Console.WriteLine("����ȡ���ˡ���");
                    break;
                }
            }
            
            //Thread.CurrentThread.Join(timeSpan);
            Thread.Sleep(timeSpan);
            Console.WriteLine("�������.׼��ִ��");
        }

        /// <summary>
        /// ������һ��ִ������ʱ��
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


        #region ������ʱ����̬����
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="_trd"></param>
        /// <returns></returns>
        public static TimerTaskService CreateTimerTaskService(TimerInfo info, Action _trd)
        {
            TimerTaskService tus = new TimerTaskService(info, _trd);
            //���������߳�
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

            //���������߳�
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
    /// �ƻ�������������
    /// </summary>
    public class TimerInfo
    {
        private TimerType timerType;
        /// <summary>
        /// ���ѡ��Every����,����ÿ��ĺ�����Խ���ִ��,ֻ��ִ�бȵ�ǰС�ĵ�λ.
        /// </summary>
        public TimerType TimerType
        {
            get { return timerType; }
            set { timerType = value; }
        }
        private int year =0;
        /// <summary>
        /// ��
        /// </summary>
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        private int month = 0;
        /// <summary>
        /// ��
        /// </summary>
        public int Month
        {
            get { return month; }
            set { month = value; }
        }
        private int day = 0;
        /// <summary>
        /// ��
        /// </summary>
        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        private int hour = 0;
        /// <summary>
        /// Сʱ
        /// </summary>
        public int Hour
        {
            get { return hour; }
            set { hour = value; }
        }
        private int minute = 0;
        /// <summary>
        /// ����
        /// </summary>
        public int Minute
        {
            get { return minute; }
            set { minute = value; }
        }
        private int second = 0;
        /// <summary>
        /// ��
        /// </summary>
        public int Second
        {
            get { return second; }
            set { second = value; }
        }
        private int millisecond = 0;
        /// <summary>
        /// ����
        /// </summary>
        public int Millisecond
        {
            get { return millisecond; }
            set { millisecond = value; }
        }
    }    
    /// <summary>
    /// �ƻ���������
    /// </summary>
    public enum TimerType
    {
        /// <summary>
        /// ��
        /// </summary>
        EverySecond,
        /// <summary>
        /// ����
        /// </summary>
        EveryMinute,
        /// <summary>
        /// Сʱ
        /// </summary>
        EveryHour,
        /// <summary>
        /// ��
        /// </summary>
        EveryDay,
        /// <summary>
        /// ��
        /// </summary>
        EveryWeek,
        /// <summary>
        /// ��
        /// </summary>
        EveryMonth,
        /// <summary>
        /// 
        /// </summary>
        EveryYear,
        /// <summary>
        /// �ƶ�����,ִ��һ�κ�ֹͣ
        /// </summary>
        DesDate,
        /// <summary>
        /// ����һ��ֹͣ�� ����ѭ��ʱ��
        /// </summary>
        LoopStop,
        /// <summary>
        /// ����һ����ʼʱ����ѭ��ʱ��
        /// </summary>
        LoopStart,
        /// <summary>
        /// ÿһ��ʱ��
        /// </summary>
        

    }

}
