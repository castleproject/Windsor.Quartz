using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Quartz;

namespace Castle.Facilities.Quartz {

    /// <summary>
    /// JobListener that will release Jobs out of the Kernel
    /// </summary>
    /// <seealso cref="IJobListener" />
    /// <inheritdoc />
    public class ReleasingJobListener : IJobListener
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// JobListener that will release Jobs out of the Kernel
        /// </summary>
        /// <param name="kernel">Windsor Kernel</param>
        public ReleasingJobListener(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// was about to be executed (an associated <see cref="T:Quartz.ITrigger" />
        /// has occurred), but a <see cref="T:Quartz.ITriggerListener" /> vetoed it's
        /// execution.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IJobListener.JobToBeExecuted(Quartz.IJobExecutionContext)" />
        public virtual async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => _kernel.ReleaseComponent(context.JobInstance), token);
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.IJobDetail" />
        /// is about to be executed (an associated <see cref="T:Quartz.ITrigger" />
        /// has occurred).
        /// <para>
        /// This method will not be invoked if the execution of the Job was vetoed
        /// by a <see cref="T:Quartz.ITriggerListener" />.
        /// </para>
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IJobListener.JobExecutionVetoed(Quartz.IJobExecutionContext)" />
        public virtual async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            await Task.FromResult<object>(null);
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler" /> after a <see cref="T:Quartz.IJobDetail" />
        /// has been executed, and be for the associated <see cref="T:Quartz.Spi.IOperableTrigger" />'s
        /// <see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)" /> method has been called.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="jobException">The job exception.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public virtual async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => _kernel.ReleaseComponent(context.JobInstance), token);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        public virtual string Name => GetType().FullName;
    }
}
