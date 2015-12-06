using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.IO;




namespace TiRoRiN_Restarter
{
    
    class Program
    {

        static string last_restart = "00:00";
        static string last_save = "00";
        
        static void OnTimerTick(object sender, ElapsedEventArgs e)
        {

            if (DateTime.Now.Minute.ToString() == "20" || DateTime.Now.Minute.ToString() == "50")
            {
                if (last_save != DateTime.Now.ToString("hh:mm"))
                {
                    last_save = DateTime.Now.ToString("hh:mm");
                    Console.WriteLine(last_save + " Zálohuji všechny DB");
                    System.Diagnostics.Process.Start(@"D:\Server_Utils\Backup_DB.bat");
                }
            }
            if (DateTime.Now.ToString("hh:mm") == "06:00" || DateTime.Now.ToString("hh:mm") == "03:00" || DateTime.Now.ToString("hh:mm") == "09:00" || DateTime.Now.ToString("hh:mm") == "12:00" || DateTime.Now.ToString("hh:mm") == "0:01")
            {
                if (last_restart != DateTime.Now.ToString("hh:mm"))
                {
                last_restart = DateTime.Now.ToString("hh:mm");
                Console.WriteLine(last_restart + " Restartuji všechny servery");
                
                System.Diagnostics.Process.Start(@"D:\Server_Utils\restart_all.bat");
                }
                
            }
        }

        static void Main(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = new TimeSpan(0,0,10).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            timer.Enabled = true;

            //Console.WriteLine(DateTime.Now.Minute.ToString());
            Console.WriteLine("Startuji TiRoRiN Restarter. Veškerá aktivita se ukládá do logu včetně času.");
            Console.WriteLine("Log se nachází v D:\\Server_Utils\\scheduler_log.txt");
            FileStream filestream = new FileStream(@"D:\Server_Utils\scheduler_log.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
                Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " Startuji TiRoRiN_Restarter");
                while (true)
                {
                    Thread.Sleep(1000);
                }
            
        }
    }

}
