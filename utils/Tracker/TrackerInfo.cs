using System;
using System.Collections.Generic;
using System.Timers;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace Utils
{
    public class TrackerInfo
    {
        private static readonly HttpClient client = new HttpClient();

        private static Timer aTimer;

        string AZURE_POST_FUNCTION = "YOURURLHERE";

        public string Date;
        public Dictionary<string, List<string>> ListInfo;
        public string ProjectTitle;
        public string RevitVersion;
        public string UserMachine;
        public string UserName;
        public string ToolName;
        public float ToolTime;
        public float ToolVersion;
        public float Seconds;


        public TrackerInfo()
        {
            Seconds = 0; ToolTime = 0;
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UserName = Environment.UserName;
            UserMachine = Environment.MachineName;
            ListInfo = new Dictionary<string, List<string>>();         
            SetTimer();
        }

        public async void PostInfo(TrackerInfo obj)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                //FormUrlEncodedContent content = new FormUrlEncodedContent(dix);

                var response = await client.PostAsync(AZURE_POST_FUNCTION, content);
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetTimer()
        {
            // Create a timer with a one second interval.
            aTimer = new Timer(1000);
            // Hook up the Elapsed event for the timer.            
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public void StartTimer()
        {
            aTimer.Start();
        }

        public void StopTimer()
        {
            ToolTime += Seconds;
            aTimer.Stop();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Seconds = e.SignalTime.Second;
        }

    }
}
