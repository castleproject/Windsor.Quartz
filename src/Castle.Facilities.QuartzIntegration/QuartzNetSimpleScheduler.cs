using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace Castle.Facilities.Quartz {

    /// <summary>
    ///     Light-weight job scheduler
    /// </summary>
    public class QuartzNetSimpleScheduler : IJobScheduler
    {
        private readonly IScheduler _scheduler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuartzNetSimpleScheduler" /> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        public QuartzNetSimpleScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        /// <summary>
        ///     Resumes triggers of a paused job
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ResumeJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeJob(jobKey, token);
        }

        /// <summary>
        ///     Deletes a job
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DeleteJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.DeleteJob(jobKey, token);
        }

        /// <summary>
        ///     Interrupts a running job
        /// </summary>
        public async Task<bool> Interrupt(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.Interrupt(jobKey, token);
        }

        /// <summary>
        ///     Gets the job status, assuming it has only one trigger
        /// </summary>
        public async Task<TriggerState> GetJobStatus(JobKey jobKey,
            CancellationToken token = default(CancellationToken))
        {
            var triggerKeyTask = await _scheduler.GetTriggersOfJob(jobKey, token);
            var triggerKey = triggerKeyTask.ToArray()[0].Key;

            return await _scheduler.GetTriggerState(triggerKey, token);
        }

        /// <summary>
        ///     Gets the job keys.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<JobKey[]> GetJobKeys(CancellationToken token = default(CancellationToken))
        {
            var jobGroupNames = await _scheduler.GetJobGroupNames(token);

            var task = Task.Run(() =>
            {
                var jobGroupNamesArray = jobGroupNames.ToArray();
                var jobGroupNamesLength = jobGroupNamesArray.Length;

                var tasks = new Task<IReadOnlyCollection<JobKey>>[jobGroupNamesLength];

                for (var index = 0; index < jobGroupNamesArray.Length; index++)
                {
                    var jobGroupName = jobGroupNamesArray[index];
                    tasks[index] = _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName), token);
                }

                Task.WaitAll(tasks, token);

                return tasks.SelectMany(x => x.Result).ToArray();
            }, token);

            return await task;
        }

        /// <summary>
        ///     Runs a job immediately
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task RunJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.TriggerJob(jobKey, token);
        }

        /// <summary>
        ///     Get all currently executing jobs
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<JobKey[]> GetExecutingJobs(CancellationToken token)
        {
            var executingJobs = await _scheduler.GetCurrentlyExecutingJobs(token);
            return await Task.Run(() => { return executingJobs.Select(j => j.JobDetail.Key).ToArray(); }, token);
        }

        /// <summary>
        ///     Pauses all triggers
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task PauseAll(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseAll(token);
        }

        /// <summary>
        ///     Resumes all triggers
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task ResumeAll(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeAll(token);
        }

        /// <summary>
        ///     Pauses a job's triggers
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task PauseJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseJob(jobKey, token);
        }
    }
}