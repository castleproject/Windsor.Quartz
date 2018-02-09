using Castle.MicroKernel;
using Quartz;
using Quartz.Spi;

namespace Castle.Facilities.Quartz
{
    /// <summary>
    ///     Creates a Quartz job with Windsor
    /// </summary>
    /// <seealso cref="Quartz.Spi.IJobFactory" />
    /// <inheritdoc />
    public class WindsorJobFactory : IJobFactory
    {
        private readonly IKernel _kernel;

        /// <summary>
        ///     Creates a Quartz job with Windsor
        /// </summary>
        /// <param name="kernel">Windsor Kernel</param>
        public WindsorJobFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        ///     Resolve a Job by it's name
        /// </summary>
        public bool ResolveByJobName { get; set; }

        /// <summary>
        ///     Called by the scheduler at the time of the trigger firing, in order to
        ///     produce a <see cref="T:Quartz.IJob" /> instance on which to call Execute.
        /// </summary>
        /// <remarks>
        ///     It should be extremely rare for this method to throw an exception -
        ///     basically only the the case where there is no way at all to instantiate
        ///     and prepare the Job for execution.  When the exception is thrown, the
        ///     Scheduler will move all triggers associated with the Job into the
        ///     <see cref="F:Quartz.TriggerState.Error" /> state, which will require human
        ///     intervention (e.g. an application restart after fixing whatever
        ///     configuration problem led to the issue wih instantiating the Job.
        /// </remarks>
        /// <param name="bundle">
        ///     The TriggerFiredBundle from which the <see cref="T:Quartz.IJobDetail" />
        ///     and other info relating to the trigger firing can be obtained.
        /// </param>
        /// <param name="scheduler">a handle to the scheduler that is about to execute the job</param>
        /// <throws>SchedulerException if there is a problem instantiating the Job. </throws>
        /// <returns>
        ///     the newly instantiated Job
        /// </returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) (ResolveByJobName
                ? _kernel.Resolve(bundle.JobDetail.Key.ToString(), typeof(IJob))
                : _kernel.Resolve(bundle.JobDetail.JobType));
        }

        /// <summary>
        ///     Allows the job factory to destroy/cleanup the job if needed.
        /// </summary>
        /// <param name="job"></param>
        public void ReturnJob(IJob job)
        {
            _kernel.ReleaseComponent(job);
        }
    }
}