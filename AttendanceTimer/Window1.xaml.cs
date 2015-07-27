using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AttendanceTimer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        int hours;
        int minutes;
        int seconds;
        DateTime login_time;
        DateTime logout_time;
        DispatcherTimer timer;
        const double working_hours = 9.6;
        const double maximum_login_hour = 10.0;

        public Window1()
        {
            InitializeComponent(); 
            compute_time();
            do_timer();
        }

        private void compute_time()
        {
            date_now.Text = DateTime.Now.Date.Date.ToString();
            string query = "*[System/EventID=12]";
            EventLogQuery event_query = new EventLogQuery("System", PathType.LogName, query);
            try
            {
                EventLogReader reader = new EventLogReader(event_query);
                for (EventRecord eventdetail = reader.ReadEvent(); 
                     eventdetail != null; 
                     eventdetail = reader.ReadEvent())
                {
                    DateTime log_created = eventdetail.TimeCreated.Value;
                    if (log_created.Date.ToString() == DateTime.Now.Date.ToString())
                    {
                        double logout_hour = 0.0;
                        double login_hour = log_created.TimeOfDay.TotalHours;
                        if (login_hour > maximum_login_hour)
                        {
                            logout_hour = maximum_login_hour + working_hours;
                        }
                        else
                        {
                            logout_hour = login_hour + working_hours;
                        }
                        TimeSpan span = TimeSpan.FromHours(logout_hour);
                        login_time = eventdetail.TimeCreated.Value;
                        logout_time = DateTime.Today.Add(span);
                        login.Text = login_time.ToString("hh:mm tt");
                        logout.Text = logout_time.ToString("hh:mm tt");

                        double time_now = DateTime.Now.TimeOfDay.TotalHours;
                        double countdown_hour = logout_hour - time_now;
                        if (countdown_hour < 0)
                        {
                            countdown_hour = 0;
                        }
                        TimeSpan t = TimeSpan.FromHours(countdown_hour);
                        hours = t.Hours;
                        minutes = t.Minutes;
                        seconds = t.Seconds;
                    }
                }
            }
            catch (EventLogNotFoundException e)
            {
                Console.WriteLine("Error while readig event logs");
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {
            //hours = 0; minutes = 0; seconds = 0;
            if ((hours == 0) && (minutes == 0) && (seconds == 0))
            {
                hours_label.Content = "00";
                minutes_label.Content = "00";
                seconds_label.Content = "00";
                greetings.Content = "Go home NOW!";
                hours_label.Foreground = new SolidColorBrush(Colors.Red);
                minutes_label.Foreground = new SolidColorBrush(Colors.Red);
                seconds_label.Foreground = new SolidColorBrush(Colors.Red);
                colon1.Foreground = new SolidColorBrush(Colors.Red);
                colon2.Foreground = new SolidColorBrush(Colors.Red);
                greetings.Foreground = new SolidColorBrush(Colors.Red);
                timer.Stop();
            }
            else
            {
                if (seconds < 1)
                {
                    seconds = 59;
                    if (minutes == 0)
                    {
                        minutes = 59;
                        if (hours != 0)
                        {
                            hours -= 1;
                        }
                    }
                    else
                    {
                        minutes -= 1;
                    }
                }
                else
                {
                    seconds -= 1;
                }

                if (hours < 10)
                {
                    hours_label.Content = "0" + hours.ToString();
                }
                else
                {
                    hours_label.Content = hours.ToString();
                }

                if (minutes < 10)
                {
                    minutes_label.Content = "0" + minutes.ToString();
                }
                else
                {
                    minutes_label.Content = minutes.ToString();
                }

                if (seconds < 10)
                {
                    seconds_label.Content = "0" + seconds.ToString();
                }
                else
                {
                    seconds_label.Content = seconds.ToString();
                }
            }
        }

        private void do_timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_tick;
            timer.Start();
        }
    }
}
