using System;
using System.Threading.Tasks;
using Quartz;

namespace SampleApp {
    public class SampleJob : IJob, IDisposable
    {
        /// <summary>
        ///     Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        ///     fires that is associated with the <see cref="T:Quartz.IJob" />.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The implementation may wish to set a  result object on the
        ///     JobExecutionContext before this method exits.  The result itself
        ///     is meaningless to Quartz, but may be informative to
        ///     <see cref="T:Quartz.IJobListener" />s or
        ///     <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
        ///     execution.
        /// </remarks>
        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => { Console.WriteLine("Hello world!"); });
        }

        public void Dispose()
        {
            Console.WriteLine("disposing...");
        }
    }
}