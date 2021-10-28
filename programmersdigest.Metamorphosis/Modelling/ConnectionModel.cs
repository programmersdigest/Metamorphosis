using Newtonsoft.Json;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class ConnectionModel
    {
        public string Signal { get; }
        public string Trigger { get; }

        [JsonIgnore] internal string Sender { get; }
        [JsonIgnore] internal string SignalName { get; }
        [JsonIgnore] internal string Receiver { get; }
        [JsonIgnore] internal string TriggerName { get; }

        public ConnectionModel(string signal, string trigger)
        {
            Signal = signal;
            var separatorIndex = signal.LastIndexOf('.');
            Sender = signal[..separatorIndex];
            SignalName = signal[(separatorIndex + 1)..];

            Trigger = trigger;
            separatorIndex = trigger.LastIndexOf('.');
            Receiver = trigger[..separatorIndex];
            TriggerName = trigger[(separatorIndex + 1)..];
        }
    }
}