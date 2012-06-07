using System.Collections.Generic;
using System.Linq;
using Quartz;
using Quartz.Impl.Matchers;

namespace Castle.Facilities.QuartzIntegration {

    /// <summary>
    /// Light-weight job scheduler
    /// </summary>
    public class QuartzNetSimpleScheduler : IJobScheduler {
        private readonly IScheduler _scheduler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduler"></param>
        public QuartzNetSimpleScheduler(IScheduler scheduler) {
            this._scheduler = scheduler;
        }

        /// <summary>
        /// Resumes triggers of a paused job
        /// </summary>
        public void ResumeJob(JobKey jobKey) {
            _scheduler.ResumeJob(jobKey);
        }

        public bool DeleteJob(JobKey jobKey)
        {
            return _scheduler.DeleteJob(jobKey);
        }

        /// <summary>
        /// Interrupts a running job
        /// </summary>
        public bool Interrupt(JobKey jobKey)
        {
            return _scheduler.Interrupt(jobKey);
        }

        /// <summary>
        /// Gets the job status, assuming it has only one trigger
        /// </summary>
        public TriggerState GetJobStatus(JobKey jobKey)
        {
            var triggerKey = _scheduler.GetTriggersOfJob(jobKey)[0].Key;
            return _scheduler.GetTriggerState(triggerKey);
        }

        public ICollection<JobKey> GetJobKeys()
        {
            return _scheduler.GetJobGroupNames()
                .SelectMany(jobGroupName => _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName)))
                .ToList();
        }

        /// <summary>
        /// Runs a job immediately
        /// </summary>
        public void RunJob(JobKey jobKey)
        {
            _scheduler.TriggerJob(jobKey);
        }

        public ICollection<JobKey> GetExecutingJobs()
        {
            return _scheduler.GetCurrentlyExecutingJobs().Select(j => j.JobDetail.Key).ToList();
        }

        /// <summary>
        /// Pauses all triggers
        /// </summary>
        public void PauseAll() {
            _scheduler.PauseAll();
        }

        /// <summary>
        /// Resumes all triggers
        /// </summary>
        public void ResumeAll() {
            _scheduler.ResumeAll();
        }

        /// <summary>
        /// Pauses a job's triggers
        /// </summary>
        public void PauseJob(JobKey jobKey)
        {
            _scheduler.PauseJob(jobKey);
        }
    }
}