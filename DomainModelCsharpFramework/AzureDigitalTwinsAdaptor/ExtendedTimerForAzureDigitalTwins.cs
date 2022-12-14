using Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM;
using Kae.DomainModel.Csharp.Framework.ExternalEntities.ETMR;
using Kae.DomainModel.CSharp.Framework.Service.Event;
using Kae.StateMachine;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage.AzureDigitalTwins
{
    public class ExtendedTimerForAzureDigitalTwins : ETMRWrapper
    {
        private static readonly string timerServiceUrlKey = "timer-service-url";
        private static readonly string loggerKey = "logger";
        private static HttpClient httpClient = null;
        private string timerServiceUrl;
        private Logger logger;

        public ExtendedTimerForAzureDigitalTwins()
        {
            configurationKeys.Add(timerServiceUrlKey);
            configurationKeys.Add(loggerKey);
        }
        public override bool cancel(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            bool existed = false;
            var operation = new EventTimerOperation() { TimerId=timer_inst_ref.TimerId, Operation= EventTimerOperation.OperationType.cancel };
            var content = new StringContent(operation.Serialize());
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode== System.Net.HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerResponse>(reader.ReadToEnd());
                    existed = result.WaitForFire;
                }
                logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] has been caneled - {existed}");
            }
            else
            {
                logger.LogWarning($"\"Timer[{timer_inst_ref.TimerId}] cancel failed - {response.StatusCode.ToString()}");
            }
            return existed;
        }

        public override DateTime remaining_time(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            DateTime datetime = DateTime.Now;
            var operation = new EventTimerOperation() { TimerId = timer_inst_ref.TimerId, Operation = EventTimerOperation.OperationType.remaining_time };
            var content = new StringContent(operation.Serialize());
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerResponse>(reader.ReadToEnd());
                    if (result != null)
                    {
                        datetime = result.RemainingTime;
                    }
                    logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] remaining_time is {datetime.ToString("yyyyMMddTHHmmss")}");
                }
            }
            else
            {
                logger.LogWarning($"Timer[{timer_inst_ref.TimerId}] remaining_time failed - {response.StatusCode.ToString()}");
            }
            return datetime;
        }

        public override bool reset_time(ExternalEntities.TIM.Timer timer_inst_ref, DateTime datetime)
        {
            bool existed = false;
            var operation = new EventTimerOperation() { TimerId = timer_inst_ref.TimerId, Operation = EventTimerOperation.OperationType.reset_time };
            var content = new StringContent(operation.Serialize());
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode== System.Net.HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerResponse>(reader.ReadToEnd());
                    if (result != null)
                    {
                        existed = result.WaitForFire;
                    }
                }
                logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] has been reset - {existed}");
            }
            else
            {
                logger.LogWarning($"Timer[{timer_inst_ref.TimerId}] reset_time failed - {response.StatusCode.ToString()}");
            }
            return existed;
        }

        public override ExternalEntities.TIM.Timer start(DateTime datetime, EventData event_inst)
        {
            var newTimer = new TimerImpl(event_inst, datetime);
            var operation = new EventTimerOperation()
            {
                TimerId = newTimer.TimerId,
                Operation = EventTimerOperation.OperationType.start,
                FireTime = datetime,
                EventLabel = event_inst.EventName,
                DestinationIdentities = event_inst.GetReceiverIdentities(),
                Parameters = event_inst.GetSupplementalData()
            };
            var content =new StringContent(operation.Serialize());
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode== System.Net.HttpStatusCode.OK)
            {
                logger.LogInfo($"Timer[{newTimer.TimerId}] has been started.");
            }
            else
            {
                logger.LogWarning($"Timer[{newTimer.TimerId}] start failed - {response.StatusCode.ToString()}");
            }
            return newTimer;
        }

        public override ExternalEntities.TIM.Timer start_recuring(DateTime datetime, EventData event_inst)
        {
            var newTimer = new TimerImpl(event_inst, datetime);
            var operation = new EventTimerOperation()
            {
                TimerId = newTimer.TimerId,
                Operation = EventTimerOperation.OperationType.start_recurring,
                FireTime = datetime,
                EventLabel = event_inst.EventName,
                DestinationIdentities = event_inst.GetReceiverIdentities(),
                Parameters = event_inst.GetSupplementalData()
            };
            var content = new StringContent(operation.Serialize());
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInfo($"Timer[{newTimer.TimerId}] has been started recuring.");
            }
            else
            {
                logger.LogWarning($"Timer[{newTimer.TimerId}] start recuring failed - {response.StatusCode.ToString()}");
            }
            return newTimer;
        }

        protected override void InitializeImple()
        {
            httpClient = new HttpClient();
            timerServiceUrl = (string)this.configurations[timerServiceUrlKey];
            if (this.configurations.ContainsKey(loggerKey))
            {
                logger = (Logger)this.configurations[loggerKey];
            }
        }
    }

    public class TimerImpl : Timer
    {
        public string TimerIdOnService { get; set; }
        public TimerImpl(EventData eventData, DateTime fireTime) : base(eventData,0)
        {
            this.firingTime = fireTime;
            TimerIdOnService = Guid.NewGuid().ToString();
        }

        public override bool AddTime(long microseconds)
        {
            throw new NotImplementedException();
        }

        public override bool Cancel()
        {
            throw new NotImplementedException();
        }

        public override bool ResetTime(DateTime time)
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public void SetTime(DateTime fireTime)
        {
            this.firingTime = fireTime;
        }
    }
}
