using System;
using Quartz;

namespace SampleApp
{
    public class SampleSchedulerListener : ISchedulerListener
    {
        public void JobScheduled(ITrigger trigger)
        {
            Console.WriteLine(GetType().Name + ".JobScheduled");
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            Console.WriteLine(GetType().Name + ".JobUnscheduled");
        }

        public void TriggerFinalized(ITrigger trigger)
        {
            Console.WriteLine(GetType().Name + ".TriggerFinalized");
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            Console.WriteLine(GetType().Name + ".TriggerPaused");
        }

        public void TriggersPaused(string triggerGroup)
        {
            Console.WriteLine(GetType().Name + ".TriggersPaused");
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            Console.WriteLine(GetType().Name + ".TriggerResumed");
        }

        public void TriggersResumed(string triggerGroup)
        {
            Console.WriteLine(GetType().Name + ".TriggersResumed");
        }

        public void JobAdded(IJobDetail jobDetail)
        {
            Console.WriteLine(GetType().Name + ".JobAdded");
        }

        public void JobDeleted(JobKey jobKey)
        {
            Console.WriteLine(GetType().Name + ".JobDeleted");
        }

        public void JobPaused(JobKey jobKey)
        {
            Console.WriteLine(GetType().Name + ".JobPaused");
        }

        public void JobsPaused(string jobGroup)
        {
            Console.WriteLine(GetType().Name + ".JobsPaused");
        }

        public void JobResumed(JobKey jobKey)
        {
            Console.WriteLine(GetType().Name + ".JobResumed");
        }

        public void JobsResumed(string jobGroup)
        {
            Console.WriteLine(GetType().Name + ".JobsResumed");
        }

        public void SchedulerError(string msg, SchedulerException cause)
        {
            Console.WriteLine(GetType().Name + ".SchedulerError");
        }

        public void SchedulerInStandbyMode()
        {
            Console.WriteLine(GetType().Name + ".SchedulerInStandbyMode");
        }

        public void SchedulerStarted()
        {
            Console.WriteLine(GetType().Name + ".SchedulerStarted");
        }

        public void SchedulerStarting() {
            Console.WriteLine(GetType().Name + ".SchedulerStarting");
        }

        public void SchedulerShutdown()
        {
            Console.WriteLine(GetType().Name + ".SchedulerShutdown");
        }

        public void SchedulerShuttingdown()
        {
            Console.WriteLine(GetType().Name + ".SchedulerShuttingdown");
        }

        public void SchedulingDataCleared()
        {
            Console.WriteLine(GetType().Name + ".SchedulingDataCleared");
        }
    }
}