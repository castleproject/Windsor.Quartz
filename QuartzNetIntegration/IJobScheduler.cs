using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Castle.Facilities.QuartzIntegration {

    /// <summary>
    /// Light-weight job scheduler
    /// </summary>
    public interface IJobScheduler
    {

        /// <summary>
        /// Get all known jobs
        /// </summary>
        Task<JobKey[]> GetJobKeys(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Runs a job immediately
        /// </summary>
        Task RunJob(JobKey jobKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Get all currently executing jobs
        /// </summary>
        Task<JobKey[]> GetExecutingJobs(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Pauses all triggers
        /// </summary>
        Task PauseAll(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Resumes all triggers
        /// </summary>
        Task ResumeAll(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Pauses a job's triggers
        /// </summary>
        Task PauseJob(JobKey jobKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Resumes triggers of a paused job
        /// </summary>
        Task ResumeJob(JobKey jobKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Deletes a job
        /// </summary>
        Task<bool> DeleteJob(JobKey jobKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Interrupts a running job
        /// </summary>
        Task<bool> Interrupt(JobKey jobKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets the job status, assuming it has only one trigger
        /// </summary>
        Task<TriggerState> GetJobStatus(JobKey jobKey, CancellationToken token = default(CancellationToken));
    }
}