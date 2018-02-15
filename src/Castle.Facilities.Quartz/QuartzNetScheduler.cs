using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace Castle.Facilities.Quartz
{
    /// <summary>
    ///     Quartz scheduler implementation
    /// </summary>
    /// <seealso cref="Quartz.IScheduler" />
    /// <seealso cref="Castle.Core.IStartable" />
    /// <seealso cref="System.IDisposable" />
    public class QuartzNetScheduler : IScheduler, IStartable, IDisposable
    {
        private readonly NameValueCollection _properties = new NameValueCollection();
        private readonly IScheduler _scheduler;

        /// <summary>
        ///     Constructs a Scheduler that uses Castle Windsor
        /// </summary>
        /// <param name="props">Properties</param>
        /// <param name="jobFactory">JobFactory</param>
        /// <param name="kernel">Castle MicroKernel</param>
        public QuartzNetScheduler(IDictionary<string, string> props, IJobFactory jobFactory, IKernel kernel)
        {
            //Create Scheduler
            _scheduler = CreateScheduler(props);

            //Set JobFactory
            JobFactory = jobFactory;

            //Listener: Release jobs
            _scheduler.ListenerManager.AddJobListener(new ReleasingJobListener(kernel));

            //Properties
            WaitForJobsToCompleteAtShutdown = true; // default
        }

        /// <summary>
        ///     Wait for Jobs to finish when shutdown is triggered
        /// </summary>
        public bool WaitForJobsToCompleteAtShutdown { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the name of the <see cref="T:Quartz.IScheduler" />.
        /// </summary>
        public string SchedulerName => _scheduler.SchedulerName;

        /// <inheritdoc />
        /// <summary>
        ///     Returns the instance Id of the <see cref="T:Quartz.IScheduler" />.
        /// </summary>
        public string SchedulerInstanceId => _scheduler.SchedulerInstanceId;

        /// <inheritdoc />
        /// <summary>
        ///     Returns the <see cref="T:Quartz.SchedulerContext" /> of the <see cref="T:Quartz.IScheduler" />.
        /// </summary>
        public SchedulerContext Context => _scheduler.Context;

        /// <summary>
        ///     Reports whether the <see cref="T:Quartz.IScheduler" /> is in stand-by mode.
        /// </summary>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.Standby" />
        /// <seealso cref="M:Quartz.IScheduler.Start" />
        public bool InStandbyMode => _scheduler.InStandbyMode;

        /// <summary>
        ///     Reports whether the <see cref="T:Quartz.IScheduler" /> has been Shutdown.
        /// </summary>
        /// <inheritdoc />
        public bool IsShutdown => _scheduler.IsShutdown;

        /// <summary>
        ///     Set the <see cref="P:Quartz.IScheduler.JobFactory" /> that will be responsible for producing
        ///     instances of <see cref="T:Quartz.IJob" /> classes.
        /// </summary>
        /// <remarks>
        ///     JobFactories may be of use to those wishing to have their application
        ///     produce <see cref="T:Quartz.IJob" /> instances via some special mechanism, such as to
        ///     give the opportunity for dependency injection.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="T:Quartz.Spi.IJobFactory" />
        public IJobFactory JobFactory
        {
            set => _scheduler.JobFactory = value;
        }

        /// <summary>
        ///     Get a reference to the scheduler's <see cref="T:Quartz.IListenerManager" />,
        ///     through which listeners may be registered.
        /// </summary>
        /// <inheritdoc />
        /// <seealso cref="P:Quartz.IScheduler.ListenerManager" />
        /// <seealso cref="T:Quartz.IJobListener" />
        /// <seealso cref="T:Quartz.ITriggerListener" />
        /// <seealso cref="T:Quartz.ISchedulerListener" />
        public IListenerManager ListenerManager => _scheduler.ListenerManager;

        /// <inheritdoc />
        /// <summary>
        ///     Whether the scheduler has been started.
        /// </summary>
        /// <remarks>
        ///     Note: This only reflects whether <see cref="M:Quartz.IScheduler.Start" /> has ever
        ///     been called on this Scheduler, so it will return <see langword="true" /> even
        ///     if the <see cref="T:Quartz.IScheduler" /> is currently in standby mode or has been
        ///     since shutdown.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.Start" />
        /// <seealso cref="P:Quartz.IScheduler.IsShutdown" />
        /// <seealso cref="P:Quartz.IScheduler.InStandbyMode" />
        public bool IsStarted => _scheduler.IsStarted;

        /// <inheritdoc />
        /// <summary>
        ///     returns true if the given JobGroup
        ///     is paused
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     <c>true</c> if [is job group paused] [the specified group name]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsJobGroupPaused(string groupName, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.IsJobGroupPaused(groupName, token);
        }

        /// <summary>
        ///     returns true if the given TriggerGroup
        ///     is paused
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<bool> IsTriggerGroupPaused(string groupName,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.IsTriggerGroupPaused(groupName, token);
        }

        /// <summary>
        ///     Get a <see cref="T:Quartz.SchedulerMetaData" /> object describing the settings
        ///     and capabilities of the scheduler instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Note that the data returned is an 'instantaneous' snap-shot, and that as
        ///     soon as it's returned, the meta data values may be different.
        /// </remarks>
        /// <inheritdoc />
        public async Task<SchedulerMetaData> GetMetaData(CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetMetaData(token);
        }

        /// <summary>
        ///     Return a list of <see cref="T:Quartz.IJobExecutionContext" /> objects that
        ///     represent all currently executing Jobs in this Scheduler instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         This method is not cluster aware.  That is, it will only return Jobs
        ///         currently executing in this Scheduler instance, not across the entire
        ///         cluster.
        ///     </para>
        ///     <para>
        ///         Note that the list returned is an 'instantaneous' snap-shot, and that as
        ///         soon as it's returned, the true list of executing jobs may be different.
        ///         Also please read the doc associated with <see cref="T:Quartz.IJobExecutionContext" />-
        ///         especially if you're using remoting.
        ///     </para>
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="T:Quartz.IJobExecutionContext" />
        public async Task<IReadOnlyCollection<IJobExecutionContext>> GetCurrentlyExecutingJobs(
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetCurrentlyExecutingJobs(token);
        }

        /// <summary>
        ///     Get the names of all known <see cref="T:Quartz.IJobDetail" /> groups.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<string>> GetJobGroupNames(
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetJobGroupNames(token);
        }

        /// <summary>
        ///     Get the names of all known <see cref="T:Quartz.ITrigger" /> groups.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<string>> GetTriggerGroupNames(
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetTriggerGroupNames(token);
        }

        /// <summary>
        ///     Get the names of all <see cref="T:Quartz.ITrigger" /> groups that are paused.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<string>> GetPausedTriggerGroups(
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetPausedTriggerGroups(token);
        }

        /// <summary>
        ///     Starts the <see cref="T:Quartz.IScheduler" />'s threads that fire <see cref="T:Quartz.ITrigger" />s.
        ///     When a scheduler is first created it is in "stand-by" mode, and will not
        ///     fire triggers.  The scheduler can also be put into stand-by mode by
        ///     calling the <see cref="M:Quartz.IScheduler.Standby" /> method.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The misfire/recovery process will be started, if it is the initial call
        ///     to this method on this scheduler instance.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.StartDelayed(System.TimeSpan)" />
        /// <seealso cref="M:Quartz.IScheduler.Standby" />
        /// <seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)" />
        public async Task Start(CancellationToken token)
        {
            await _scheduler.Start(token);
        }

        /// <summary>
        ///     Calls <see cref="M:Quartz.IScheduler.Start" /> after the indicated delay.
        ///     (This call does not block). This can be useful within applications that
        ///     have initializers that create the scheduler immediately, before the
        ///     resources needed by the executing jobs have been fully initialized.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.Start" />
        /// <seealso cref="M:Quartz.IScheduler.Standby" />
        /// <seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)" />
        public async Task StartDelayed(TimeSpan delay, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.StartDelayed(delay, token);
        }

        /// <summary>
        ///     Temporarily halts the <see cref="T:Quartz.IScheduler" />'s firing of <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         When <see cref="M:Quartz.IScheduler.Start" /> is called (to bring the scheduler out of
        ///         stand-by mode), trigger misfire instructions will NOT be applied
        ///         during the execution of the <see cref="M:Quartz.IScheduler.Start" /> method - any misfires
        ///         will be detected immediately afterward (by the <see cref="T:Quartz.Spi.IJobStore" />'s
        ///         normal process).
        ///     </para>
        ///     <para>
        ///         The scheduler is not destroyed, and can be re-started at any time.
        ///     </para>
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.Start" />
        /// <seealso cref="M:Quartz.IScheduler.PauseAll" />
        public async Task Standby(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.Standby(token);
        }

        /// <summary>
        ///     Halts the <see cref="T:Quartz.IScheduler" />'s firing of <see cref="T:Quartz.ITrigger" />s,
        ///     and cleans up all resources associated with the Scheduler. Equivalent to
        ///     <see cref="M:Quartz.IScheduler.Shutdown(System.Boolean)" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The scheduler cannot be re-started.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)" />
        public async Task Shutdown(CancellationToken token = default(CancellationToken))
        {
            await Stop(token);
        }

        /// <summary>
        ///     Halts the <see cref="T:Quartz.IScheduler" />'s firing of <see cref="T:Quartz.ITrigger" />s,
        ///     and cleans up all resources associated with the Scheduler.
        /// </summary>
        /// <param name="waitForJobsToComplete">
        ///     if <see langword="true" /> the scheduler will not allow this method
        ///     to return until all currently executing jobs have completed.
        /// </param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The scheduler cannot be re-started.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.Shutdown" />
        public async Task Shutdown(bool waitForJobsToComplete, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.Shutdown(waitForJobsToComplete, token);
        }

        /// <summary>
        ///     Add the given <see cref="T:Quartz.IJobDetail" /> to the
        ///     Scheduler, and associate the given <see cref="T:Quartz.ITrigger" /> with
        ///     it.
        /// </summary>
        /// <param name="jobDetail">The job detail.</param>
        /// <param name="trigger">The trigger.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If the given Trigger does not reference any <see cref="T:Quartz.IJob" />, then it
        ///     will be set to reference the Job passed with it into this method.
        /// </remarks>
        /// <inheritdoc />
        public async Task<DateTimeOffset> ScheduleJob(IJobDetail jobDetail, ITrigger trigger,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.ScheduleJob(jobDetail, trigger, token);
        }

        /// <summary>
        ///     Schedule the given <see cref="T:Quartz.ITrigger" /> with the
        ///     <see cref="T:Quartz.IJob" /> identified by the <see cref="T:Quartz.ITrigger" />'s settings.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<DateTimeOffset> ScheduleJob(ITrigger trigger,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.ScheduleJob(trigger, token);
        }

        //
        // Summary:
        //     Schedule the given job with the related set of triggers.
        //
        // Remarks:
        //     If any of the given job or triggers already exist (or more specifically, if the
        //     keys are not unique) and the replace parameter is not set to true then an exception
        //     will be thrown.
        /// <summary>
        ///     Schedules the job.
        /// </summary>
        /// <param name="jobDetail">The job detail.</param>
        /// <param name="triggersForJob">The triggers for job.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task ScheduleJob(IJobDetail jobDetail, IReadOnlyCollection<ITrigger> triggersForJob, bool replace,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ScheduleJob(jobDetail, triggersForJob, replace, token);
        }

        /// <summary>
        ///     Schedules the jobs.
        /// </summary>
        /// <param name="triggersAndJobs">The triggers and jobs.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task ScheduleJobs(IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> triggersAndJobs,
            bool replace, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ScheduleJobs(triggersAndJobs, replace, token);
        }


        /// <summary>
        ///     Remove the indicated <see cref="T:Quartz.ITrigger" /> from the scheduler.
        ///     <para>
        ///         If the related job does not have any other triggers, and the job is
        ///         not durable, then the job will also be deleted.
        ///     </para>
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<bool> UnscheduleJob(TriggerKey triggerKey,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.UnscheduleJob(triggerKey, token);
        }

        /// <summary>
        ///     Remove all of the indicated <see cref="T:Quartz.ITrigger" />s from the scheduler.
        /// </summary>
        /// <param name="triggerKeys">The trigger keys.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         If the related job does not have any other triggers, and the job is
        ///         not durable, then the job will also be deleted.
        ///     </para>
        ///     Note that while this bulk operation is likely more efficient than
        ///     invoking <see cref="M:Quartz.IScheduler.UnscheduleJob(Quartz.TriggerKey)" /> several
        ///     times, it may have the adverse affect of holding data locks for a
        ///     single long duration of time (rather than lots of small durations
        ///     of time).
        /// </remarks>
        /// <inheritdoc />
        public async Task<bool> UnscheduleJobs(IReadOnlyCollection<TriggerKey> triggerKeys,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.UnscheduleJobs(triggerKeys, token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Remove (delete) the <see cref="T:Quartz.ITrigger" /> with the
        ///     given key, and store the new given one - which must be associated
        ///     with the same job (the new trigger must have the job name &amp; group specified)
        ///     - however, the new trigger need not have the same name as the old trigger.
        /// </summary>
        /// <param name="triggerKey">The <see cref="T:Quartz.ITrigger" /> to be replaced.</param>
        /// <param name="newTrigger">The new <see cref="T:Quartz.ITrigger" /> to be stored.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     <see langword="null" /> if a <see cref="T:Quartz.ITrigger" /> with the given
        ///     name and group was not found and removed from the store (and the
        ///     new trigger is therefore not stored),  otherwise
        ///     the first fire time of the newly scheduled trigger.
        /// </returns>
        public async Task<DateTimeOffset?> RescheduleJob(TriggerKey triggerKey, ITrigger newTrigger,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.RescheduleJob(triggerKey, newTrigger, token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Add the given <see cref="T:Quartz.IJob" /> to the Scheduler - with no associated
        ///     <see cref="T:Quartz.ITrigger" />. The <see cref="T:Quartz.IJob" /> will be 'dormant' until
        ///     it is scheduled with a <see cref="T:Quartz.ITrigger" />, or
        ///     <see cref="M:Quartz.IScheduler.TriggerJob(Quartz.JobKey)" />
        ///     is called for it.
        /// </summary>
        /// <param name="jobDetail">The job detail.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The <see cref="T:Quartz.IJob" /> must by definition be 'durable', if it is not,
        ///     SchedulerException will be thrown.
        /// </remarks>
        public async Task AddJob(IJobDetail jobDetail, bool replace,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.AddJob(jobDetail, replace, token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds the job.
        /// </summary>
        /// <param name="jobDetail">The job detail.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <param name="storeNonDurableWhileAwaitingScheduling">
        ///     if set to <c>true</c> [store non durable while awaiting
        ///     scheduling].
        /// </param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task AddJob(IJobDetail jobDetail, bool replace, bool storeNonDurableWhileAwaitingScheduling,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.AddJob(jobDetail, replace, storeNonDurableWhileAwaitingScheduling, token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Delete the identified <see cref="T:Quartz.IJob" /> from the Scheduler - and any
        ///     associated <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if the Job was found and deleted.
        /// </returns>
        public async Task<bool> DeleteJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.DeleteJob(jobKey, token);
        }

        /// <summary>
        ///     Delete the identified jobs from the Scheduler - and any
        ///     associated <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        /// <param name="jobKeys">The job keys.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if all of the Jobs were found and deleted, false if
        ///     one or more were not deleted.
        /// </returns>
        /// <remarks>
        ///     Note that while this bulk operation is likely more efficient than
        ///     invoking <see cref="M:Quartz.IScheduler.DeleteJob(Quartz.JobKey)" /> several
        ///     times, it may have the adverse affect of holding data locks for a
        ///     single long duration of time (rather than lots of small durations
        ///     of time).
        /// </remarks>
        /// <inheritdoc />
        public async Task<bool> DeleteJobs(IReadOnlyCollection<JobKey> jobKeys,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.DeleteJobs(jobKeys, token);
        }

        /// <summary>
        ///     Trigger the identified <see cref="T:Quartz.IJobDetail" />
        ///     (Execute it now).
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task TriggerJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.TriggerJob(jobKey, token);
        }

        /// <summary>
        ///     Trigger the identified <see cref="T:Quartz.IJobDetail" /> (Execute it now).
        /// </summary>
        /// <param name="jobKey">The <see cref="T:Quartz.JobKey" /> of the <see cref="T:Quartz.IJob" /> to be executed.</param>
        /// <param name="data">
        ///     the (possibly <see langword="null" />) JobDataMap to be
        ///     associated with the trigger that fires the job immediately.
        /// </param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task TriggerJob(JobKey jobKey, JobDataMap data,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.TriggerJob(jobKey, data, token);
        }

        /// <summary>
        ///     Pause the <see cref="T:Quartz.IJobDetail" /> with the given
        ///     key - by pausing all of its current <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task PauseJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseJob(jobKey, token);
        }

        /// <summary>
        ///     Pause all of the <see cref="T:Quartz.IJobDetail" />s in the
        ///     matching groups - by pausing all of their <see cref="T:Quartz.ITrigger" />s.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         The Scheduler will "remember" that the groups are paused, and impose the
        ///         pause on any new jobs that are added to any of those groups until it is resumed.
        ///     </para>
        ///     <para>
        ///         NOTE: There is a limitation that only exactly matched groups
        ///         can be remembered as paused.  For example, if there are pre-existing
        ///         job in groups "aaa" and "bbb" and a matcher is given to pause
        ///         groups that start with "a" then the group "aaa" will be remembered
        ///         as paused and any subsequently added jobs in group "aaa" will be paused,
        ///         however if a job is added to group "axx" it will not be paused,
        ///         as "axx" wasn't known at the time the "group starts with a" matcher
        ///         was applied.  HOWEVER, if there are pre-existing groups "aaa" and
        ///         "bbb" and a matcher is given to pause the group "axx" (with a
        ///         group equals matcher) then no jobs will be paused, but it will be
        ///         remembered that group "axx" is paused and later when a job is added
        ///         in that group, it will become paused.
        ///     </para>
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.ResumeJobs(Quartz.Impl.Matchers.GroupMatcher{Quartz.JobKey})" />
        public async Task PauseJobs(GroupMatcher<JobKey> matcher, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseJobs(matcher, token);
        }

        /// <summary>
        ///     Pause the <see cref="T:Quartz.ITrigger" /> with the given key.
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task PauseTrigger(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseTrigger(triggerKey, token);
        }

        /// <summary>
        ///     Pause all of the <see cref="T:Quartz.ITrigger" />s in the groups matching.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         The Scheduler will "remember" all the groups paused, and impose the
        ///         pause on any new triggers that are added to any of those groups until it is resumed.
        ///     </para>
        ///     <para>
        ///         NOTE: There is a limitation that only exactly matched groups
        ///         can be remembered as paused.  For example, if there are pre-existing
        ///         triggers in groups "aaa" and "bbb" and a matcher is given to pause
        ///         groups that start with "a" then the group "aaa" will be remembered as
        ///         paused and any subsequently added triggers in that group be paused,
        ///         however if a trigger is added to group "axx" it will not be paused,
        ///         as "axx" wasn't known at the time the "group starts with a" matcher
        ///         was applied.  HOWEVER, if there are pre-existing groups "aaa" and
        ///         "bbb" and a matcher is given to pause the group "axx" (with a
        ///         group equals matcher) then no triggers will be paused, but it will be
        ///         remembered that group "axx" is paused and later when a trigger is added
        ///         in that group, it will become paused.
        ///     </para>
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.ResumeTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})" />
        public async Task PauseTriggers(GroupMatcher<TriggerKey> matcher,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseTriggers(matcher, token);
        }

        /// <summary>
        ///     Resume (un-pause) the <see cref="T:Quartz.IJobDetail" /> with
        ///     the given key.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If any of the <see cref="T:Quartz.IJob" />'s<see cref="T:Quartz.ITrigger" /> s missed one
        ///     or more fire-times, then the <see cref="T:Quartz.ITrigger" />'s misfire
        ///     instruction will be applied.
        /// </remarks>
        /// <inheritdoc />
        public async Task ResumeJob(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeJob(jobKey, token);
        }

        /// <summary>
        ///     Resume (un-pause) all of the <see cref="T:Quartz.IJobDetail" />s
        ///     in matching groups.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If any of the <see cref="T:Quartz.IJob" /> s had <see cref="T:Quartz.ITrigger" /> s that
        ///     missed one or more fire-times, then the <see cref="T:Quartz.ITrigger" />'s
        ///     misfire instruction will be applied.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.PauseJobs(Quartz.Impl.Matchers.GroupMatcher{Quartz.JobKey})" />
        public async Task ResumeJobs(GroupMatcher<JobKey> matcher, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeJobs(matcher, token);
        }

        /// <summary>
        ///     Resume (un-pause) the <see cref="T:Quartz.ITrigger" /> with the given
        ///     key.
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If the <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        ///     <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </remarks>
        /// <inheritdoc />
        public async Task ResumeTrigger(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeTrigger(triggerKey, token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Resume (un-pause) all of the <see cref="T:Quartz.ITrigger" />s in matching groups.
        /// </summary>
        /// <remarks>
        ///     If any <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        ///     <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})" />
        public async Task ResumeTriggers(GroupMatcher<TriggerKey> matcher,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeTriggers(matcher, token);
        }

        /// <summary>
        ///     Pause all triggers - similar to calling
        ///     <see cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})" />
        ///     on every group, however, after using this method <see cref="M:Quartz.IScheduler.ResumeAll" />
        ///     must be called to clear the scheduler's state of 'remembering' that all
        ///     new triggers will be paused as they are added.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     When <see cref="M:Quartz.IScheduler.ResumeAll" /> is called (to un-pause), trigger misfire
        ///     instructions WILL be applied.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IScheduler.ResumeAll" />
        /// <seealso cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})" />
        /// <seealso cref="M:Quartz.IScheduler.Standby" />
        public async Task PauseAll(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.PauseAll(token);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Resume (un-pause) all triggers - similar to calling
        ///     <see cref="M:Quartz.IScheduler.ResumeTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})" /> on every
        ///     group.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If any <see cref="T:Quartz.ITrigger" /> missed one or more fire-times, then the
        ///     <see cref="T:Quartz.ITrigger" />'s misfire instruction will be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.PauseAll" />
        public async Task ResumeAll(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.ResumeAll(token);
        }

        /// <summary>
        ///     Get the keys of all the <see cref="T:Quartz.IJobDetail" />s in the matching groups.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<JobKey>> GetJobKeys(GroupMatcher<JobKey> matcher,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetJobKeys(matcher, token);
        }

        /// <summary>
        ///     Get all <see cref="T:Quartz.ITrigger" /> s that are associated with the
        ///     identified <see cref="T:Quartz.IJobDetail" />.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The returned Trigger objects will be snap-shots of the actual stored
        ///     triggers.  If you wish to modify a trigger, you must re-store the
        ///     trigger afterward (e.g. see <see cref="M:Quartz.IScheduler.RescheduleJob(Quartz.TriggerKey,Quartz.ITrigger)" />).
        /// </remarks>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<ITrigger>> GetTriggersOfJob(JobKey jobKey,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetTriggersOfJob(jobKey, token);
        }

        /// <summary>
        ///     Get the names of all the <see cref="T:Quartz.ITrigger" />s in the given
        ///     groups.
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TriggerKey>> GetTriggerKeys(GroupMatcher<TriggerKey> matcher,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetTriggerKeys(matcher, token);
        }

        /// <summary>
        ///     Get the <see cref="T:Quartz.IJobDetail" /> for the <see cref="T:Quartz.IJob" />
        ///     instance with the given key .
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The returned JobDetail object will be a snap-shot of the actual stored
        ///     JobDetail.  If you wish to modify the JobDetail, you must re-store the
        ///     JobDetail afterward (e.g. see <see cref="M:Quartz.IScheduler.AddJob(Quartz.IJobDetail,System.Boolean)" />).
        /// </remarks>
        /// <inheritdoc />
        public async Task<IJobDetail> GetJobDetail(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetJobDetail(jobKey, token);
        }

        /// <summary>
        ///     Get the <see cref="T:Quartz.ITrigger" /> instance with the given key.
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The returned Trigger object will be a snap-shot of the actual stored
        ///     trigger.  If you wish to modify the trigger, you must re-store the
        ///     trigger afterward (e.g. see <see cref="M:Quartz.IScheduler.RescheduleJob(Quartz.TriggerKey,Quartz.ITrigger)" />).
        /// </remarks>
        /// <inheritdoc />
        public async Task<ITrigger> GetTrigger(TriggerKey triggerKey,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetTrigger(triggerKey, token);
        }

        /// <summary>
        ///     Get the current state of the identified <see cref="T:Quartz.ITrigger" />.
        /// </summary>
        /// <param name="triggerKey">The trigger key.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        /// <seealso cref="F:Quartz.TriggerState.Normal" />
        /// <seealso cref="F:Quartz.TriggerState.Paused" />
        /// <seealso cref="F:Quartz.TriggerState.Complete" />
        /// <seealso cref="F:Quartz.TriggerState.Blocked" />
        /// <seealso cref="F:Quartz.TriggerState.Error" />
        /// <seealso cref="F:Quartz.TriggerState.None" />
        public async Task<TriggerState> GetTriggerState(TriggerKey triggerKey,
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetTriggerState(triggerKey, token);
        }

        /// <summary>
        ///     Add (register) the given <see cref="T:Quartz.ICalendar" /> to the Scheduler.
        /// </summary>
        /// <param name="calName">Name of the calendar.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <param name="updateTriggers">
        ///     whether or not to update existing triggers that
        ///     referenced the already existing calendar so that they are 'correct'
        ///     based on the new trigger.
        /// </param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task AddCalendar(string calName, ICalendar calendar, bool replace, bool updateTriggers,
            CancellationToken token = default(CancellationToken))
        {
            await _scheduler.AddCalendar(calName, calendar, replace, updateTriggers, token);
        }

        /// <summary>
        ///     Delete the identified <see cref="T:Quartz.ICalendar" /> from the Scheduler.
        /// </summary>
        /// <param name="calName">Name of the calendar.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if the Calendar was found and deleted.
        /// </returns>
        /// <remarks>
        ///     If removal of the
        ///     <code>
        /// Calendar
        /// </code>
        ///     would result in
        ///     <see cref="T:Quartz.ITrigger" />s pointing to non-existent calendars, then a
        ///     <see cref="T:Quartz.SchedulerException" /> will be thrown.
        /// </remarks>
        /// <inheritdoc />
        public async Task<bool> DeleteCalendar(string calName, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.DeleteCalendar(calName, token);
        }

        /// <summary>
        ///     Get the <see cref="T:Quartz.ICalendar" /> instance with the given name.
        /// </summary>
        /// <param name="calName">Name of the cal.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<ICalendar> GetCalendar(string calName, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetCalendar(calName, token);
        }

        /// <summary>
        ///     Get the names of all registered <see cref="T:Quartz.ICalendar" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<string>> GetCalendarNames(
            CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.GetCalendarNames(token);
        }

        /// <summary>
        ///     Request the interruption, within this Scheduler instance, of all
        ///     currently executing instances of the identified <see cref="T:Quartz.IJob" />, which
        ///     must be an implementor of the <see cref="T:Quartz.IInterruptableJob" /> interface.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true is at least one instance of the identified job was found and interrupted.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If more than one instance of the identified job is currently executing,
        ///         the <see cref="M:Quartz.IInterruptableJob.Interrupt" /> method will be called on
        ///         each instance.  However, there is a limitation that in the case that
        ///         <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)" /> on one instances throws an exception, all
        ///         remaining  instances (that have not yet been interrupted) will not have
        ///         their <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)" /> method called.
        ///     </para>
        ///     <para>
        ///         If you wish to interrupt a specific instance of a job (when more than
        ///         one is executing) you can do so by calling
        ///         <see cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs" /> to obtain a handle
        ///         to the job instance, and then invoke <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)" /> on it
        ///         yourself.
        ///     </para>
        ///     <para>
        ///         This method is not cluster aware.  That is, it will only interrupt
        ///         instances of the identified InterruptableJob currently executing in this
        ///         Scheduler instance, not across the entire cluster.
        ///     </para>
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="T:Quartz.IInterruptableJob" />
        /// <seealso cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs" />
        public async Task<bool> Interrupt(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.Interrupt(jobKey, token);
        }

        /// <summary>
        ///     Request the interruption, within this Scheduler instance, of the
        ///     identified executing job instance, which
        ///     must be an implementor of the <see cref="T:Quartz.IInterruptableJob" /> interface.
        /// </summary>
        /// <param name="fireInstanceId">The fire instance identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if the identified job instance was found and interrupted.
        /// </returns>
        /// <remarks>
        ///     This method is not cluster aware.  That is, it will only interrupt
        ///     instances of the identified InterruptableJob currently executing in this
        ///     Scheduler instance, not across the entire cluster.
        /// </remarks>
        /// <inheritdoc />
        /// <seealso cref="M:Quartz.IInterruptableJob.Interrupt" />
        /// <seealso cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs" />
        /// <seealso cref="P:Quartz.IJobExecutionContext.FireInstanceId" />
        /// <seealso cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)" />
        public async Task<bool> Interrupt(string fireInstanceId, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.Interrupt(fireInstanceId, token);
        }

        /// <summary>
        ///     Determine whether a <see cref="T:Quartz.IJob" /> with the given identifier already
        ///     exists within the scheduler.
        /// </summary>
        /// <param name="jobKey">the identifier to check for</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if a Job exists with the given identifier
        /// </returns>
        /// <inheritdoc />
        public async Task<bool> CheckExists(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.CheckExists(jobKey, token);
        }

        /// <summary>
        ///     Determine whether a <see cref="T:Quartz.ITrigger" /> with the given identifier already
        ///     exists within the scheduler.
        /// </summary>
        /// <param name="triggerKey">the identifier to check for</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     true if a Trigger exists with the given identifier
        /// </returns>
        /// <inheritdoc />
        public async Task<bool> CheckExists(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            return await _scheduler.CheckExists(triggerKey, token);
        }

        /// <summary>
        ///     Clears (deletes!) all scheduling data - all <see cref="T:Quartz.IJob" />s, <see cref="T:Quartz.ITrigger" />s
        ///     <see cref="T:Quartz.ICalendar" />s.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task Clear(CancellationToken token = default(CancellationToken))
        {
            await _scheduler.Clear(token);
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        /// <inheritdoc />
        public void Start()
        {
            var startTask = Start(default(CancellationToken));
            startTask.Wait();
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        /// <inheritdoc />
        public void Stop()
        {
            var stopTask = Stop(default(CancellationToken));
            stopTask.Wait();
        }

        private IScheduler CreateScheduler(IDictionary<string, string> props)
        {
            //Set properties
            foreach (var prop in props.Keys)
                _properties[prop] = props[prop];

            //Create scheduler
            var factory = new StdSchedulerFactory(_properties);
            var getSchedulerTask = factory.GetScheduler();
            getSchedulerTask.Wait();

            return getSchedulerTask.Result;
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task Stop(CancellationToken token)
        {
            await _scheduler.Shutdown(WaitForJobsToCompleteAtShutdown, token);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task Dispose(CancellationToken token)
        {
            await Stop(token);
        }

        #region Methods that are here for backwards-compatibility (maybe they aren't needed anymore)

        /// <summary>
        ///     Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IJobListener[] SetGlobalJobListeners
        {
            set
            {
                if (value != null)
                    foreach (var jobListener in value)
                        _scheduler.ListenerManager.AddJobListener(jobListener);
            }
        }

        /// <summary>
        ///     Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public ITriggerListener[] SetGlobalTriggerListeners
        {
            set
            {
                if (value == null) return;

                foreach (var triggerListener in value)
                    _scheduler.ListenerManager.AddTriggerListener(triggerListener);
            }
        }

        /// <summary>
        ///     Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IDictionary JobListeners
        {
            set
            {
                if (value == null) return;

                foreach (DictionaryEntry jobListenerDictionaryEntry in value)
                {
                    if (!(jobListenerDictionaryEntry.Value is IList)) continue;

                    foreach (IJobListener jobListener in (IList)jobListenerDictionaryEntry.Value)
                        _scheduler.ListenerManager.AddJobListener(jobListener);
                }
            }
        }

        /// <summary>
        ///     Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IDictionary TriggerListeners
        {
            set
            {
                if (value == null) return;

                foreach (DictionaryEntry triggerListenerDictionaryEntry in value)
                {
                    if (!(triggerListenerDictionaryEntry.Value is IList)) continue;

                    foreach (ITriggerListener triggerListener in (IList)triggerListenerDictionaryEntry.Value)
                        _scheduler.ListenerManager.AddTriggerListener(triggerListener);
                }
            }
        }

        #endregion
    }
}