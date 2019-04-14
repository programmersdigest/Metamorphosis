using Newtonsoft.Json;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class ConnectionModel
    {
        private string _signal;
        private string _trigger;

        public string Signal
        {
            get { return _signal; }
            set
            {
                _signal = value;

                var separatorIndex = value.LastIndexOf('.');
                Sender = value.Substring(0, separatorIndex);
                SignalName = value.Substring(separatorIndex + 1);
            }
        }
        public string Trigger
        {
            get { return _trigger; }
            set
            {
                _trigger = value;

                var separatorIndex = value.LastIndexOf('.');
                Receiver = value.Substring(0, separatorIndex);
                TriggerName = value.Substring(separatorIndex + 1);
            }
        }

        [JsonIgnore]
        public string Sender { get; set; }
        [JsonIgnore]
        public string SignalName { get; set; }
        [JsonIgnore]
        public string Receiver { get; set; }
        [JsonIgnore]
        public string TriggerName { get; set; }
    }
}