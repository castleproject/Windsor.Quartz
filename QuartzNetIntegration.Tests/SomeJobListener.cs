using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
    /// <summary>
    /// Some job listener
    /// </summary>
    /// <seealso cref="Quartz.IJobListener" />
    public class SomeJobListener : IJobListener
    {
        /// <summary>
        /// Jobs to be executed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        /// <inheritdoc />
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        /// <summary>
        /// Jobs the execution vetoed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        /// <inheritdoc />
        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        /// <summary>
        /// Jobs the was executed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="jobException">The job exception.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="T:System.NotImplementedException"></exception>
        /// <inheritdoc />
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener" />.
        /// </summary>
        /// <inheritdoc />
        public string Name => GetType().AssemblyQualifiedName;
    }
}