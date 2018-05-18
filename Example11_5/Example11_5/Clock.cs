using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Example11_5
{
    public class Clock : INotifyPropertyChanged
    {
        private int hour, min, sec;
        public event PropertyChangedEventHandler PropertyChanged;//属性值改变事件

        public Clock()
        {
            this.OnTimeTick(null,null);
            //使用定时器
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);//触发间隔0.1秒
            timer.Tick += OnTimeTick;//注册事件
            timer.Start();
        }

        public int Hour
        {
            set
            {
                if (value != hour)
                {
                    hour = value;
                    onPropertyChanged(new PropertyChangedEventArgs("Hour"));
                }
            }
            get
            {
                return hour;
            }
        }

        public int Minute
        {
            set
            {
                if (value != min)
                {
                    min = value;
                    onPropertyChanged(new PropertyChangedEventArgs("Minute"));
                }
                
            }
            get { return min; }
        }

        public int Second
        {
            set
            {
                if (value != sec)
                {
                    sec = value;
                    onPropertyChanged(new PropertyChangedEventArgs("Second"));
                }
            }
            get { return sec; }
        }


        protected void onPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }

        /**
         * 获取当前的时间
         * */
        private void OnTimeTick(object sender, object args)
        {
            DateTime datetime = DateTime.Now;
            Hour = datetime.Hour;
            Minute = datetime.Minute;
            Second = datetime.Second;
        }
    }
}
