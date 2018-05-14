using Quartz;
using Quartz.Impl.Matchers;

namespace Castle.Facilities.Quartz
{
    public class JobListener
    {
        public JobListener(IJobListener listener)
        {
            Listener = listener;
            Matchers = new IMatcher<JobKey>[]
            {
                EverythingMatcher<JobKey>.AllJobs()
            };
        }

        public JobListener(IJobListener listener, IMatcher<JobKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }

        public IJobListener Listener { get; set; }
        public IMatcher<JobKey>[] Matchers { get; set; }
    }
}
