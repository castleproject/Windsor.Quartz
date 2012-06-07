using System;
using Quartz;
using Castle.MicroKernel;

namespace Castle.Facilities.QuartzIntegration {

    /// <summary>
    /// JobListener that will release Jobs out of the Kernel
    /// </summary>
    [CLSCompliant(false)]
    public class ReleasingJobListener : IJobListener
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// JobListener that will release Jobs out of the Kernel
        /// </summary>
        /// <param name="kernel">Windsor Kernel</param>
        public ReleasingJobListener(IKernel kernel)
        {
            this._kernel = kernel;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> when a <see cref="T:Quartz.IJobDetail"/>
        ///             was about to be executed (an associated <see cref="T:Quartz.ITrigger"/>
        ///             has occurred), but a <see cref="T:Quartz.ITriggerListener"/> vetoed it's 
        ///             execution.
        /// </summary>
        /// <seealso cref="M:Quartz.IJobListener.JobToBeExecuted(Quartz.IJobExecutionContext)"/>
        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            _kernel.ReleaseComponent(context.JobInstance);
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> when a <see cref="T:Quartz.IJobDetail"/>
        ///             is about to be executed (an associated <see cref="T:Quartz.ITrigger"/>
        ///             has occurred).
        /// <para>
        /// This method will not be invoked if the execution of the Job was vetoed
        ///             by a <see cref="T:Quartz.ITriggerListener"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="M:Quartz.IJobListener.JobExecutionVetoed(Quartz.IJobExecutionContext)"/>
        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> after a <see cref="T:Quartz.IJobDetail"/>
        ///             has been executed, and be for the associated <see cref="T:Quartz.Spi.IOperableTrigger"/>'s
        ///             <see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)"/> method has been called.
        /// </summary>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            _kernel.ReleaseComponent(context.JobInstance);
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener"/>.
        /// </summary>
        public string Name
        {
            get { return GetType().FullName; }
        }
    }
}
