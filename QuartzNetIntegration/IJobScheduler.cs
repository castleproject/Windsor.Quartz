using System.Collections.Generic;
using Quartz;

namespace Castle.Facilities.QuartzIntegration {

    /// <summary>
    /// Light-weight job scheduler
    /// </summary>
    public interface IJobScheduler {

        /// <summary>
        /// Get all known jobs
        /// </summary>
        ICollection<JobKey> GetJobKeys();

        /// <summary>
        /// Runs a job immediately
        /// </summary>
        void RunJob(JobKey jobKey);

        /// <summary>
        /// Get all currently executing jobs
        /// </summary>
        ICollection<JobKey> GetExecutingJobs();

        /// <summary>
        /// Pauses all triggers
        /// </summary>
        void PauseAll();

        /// <summary>
        /// Resumes all triggers
        /// </summary>
        void ResumeAll();

        /// <summary>
        /// Pauses a job's triggers
        /// </summary>
        void PauseJob(JobKey jobKey);

        /// <summary>
        /// Resumes triggers of a paused job
        /// </summary>
        void ResumeJob(JobKey jobKey);

        /// <summary>
        /// Deletes a job
        /// </summary>
        bool DeleteJob(JobKey jobKey);

        /// <summary>
        /// Interrupts a running job
        /// </summary>
        bool Interrupt(JobKey jobKey);

        /// <summary>
        /// Gets the job status, assuming it has only one trigger
        /// </summary>
        TriggerState GetJobStatus(JobKey jobKey);
    }
}