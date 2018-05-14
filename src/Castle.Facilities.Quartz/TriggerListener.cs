using Quartz;
using Quartz.Impl.Matchers;

namespace Castle.Facilities.Quartz
{
    public class TriggerListener
    {
        public TriggerListener(ITriggerListener listener)
        {
            Listener = listener;
            Matchers = new IMatcher<TriggerKey>[]
            {
                EverythingMatcher<TriggerKey>.AllTriggers()
            };
        }

        public TriggerListener(ITriggerListener listener, IMatcher<TriggerKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }

        public ITriggerListener Listener { get; set; }
        public IMatcher<TriggerKey>[] Matchers { get; set; }
    }
}