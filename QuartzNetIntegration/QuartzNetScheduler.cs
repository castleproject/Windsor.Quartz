using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Castle.Core;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Castle.MicroKernel;

namespace Castle.Facilities.QuartzIntegration {

    [CLSCompliant(false)]
    public class QuartzNetScheduler : IScheduler, IStartable, IDisposable
    {
        private readonly IScheduler _scheduler;
        private readonly NameValueCollection _properties = new NameValueCollection();

        /// <summary>
        /// Wait for Jobs to finish when shutdown is triggered
        /// </summary>
        public bool WaitForJobsToCompleteAtShutdown { get; set; }

        /// <summary>
        /// Constructs a Scheduler that uses Castle Windsor
        /// </summary>
        /// <param name="props">Properties</param>
        /// <param name="jobFactory">JobFactory</param>
        /// <param name="kernel">Castle MicroKernel</param>
        public QuartzNetScheduler(IDictionary<string, string> props, IJobFactory jobFactory, IKernel kernel)
        {
            foreach (var prop in props.Keys)
            {
                _properties[prop] = props[prop];
            }
            var sf = new StdSchedulerFactory(_properties);
            _scheduler = sf.GetScheduler();
            _scheduler.JobFactory = jobFactory;
            _scheduler.ListenerManager.AddJobListener(new ReleasingJobListener(kernel));
            WaitForJobsToCompleteAtShutdown = true; // default
        }

        #region Methods that are here for backwards-compatibility (maybe they aren't needed anymore)

        /// <summary>
        /// Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IJobListener[] SetGlobalJobListeners
        {
            set
            {
                foreach (var jobListener in value)
                {
                    _scheduler.ListenerManager.AddJobListener(jobListener);
                }
            }
        }

        /// <summary>
        /// Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public ITriggerListener[] SetGlobalTriggerListeners
        {
            set
            {
                foreach (var triggerListener in value)
                {
                    _scheduler.ListenerManager.AddTriggerListener(triggerListener);
                }
            }
        }

        /// <summary>
        /// Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IDictionary JobListeners
        {
            set
            {
                foreach (DictionaryEntry jl in value)
                {
                    foreach (IJobListener jobListener in jl.Value as IList)
                    {
                        _scheduler.ListenerManager.AddJobListener(jobListener);
                    }
                }
            }
        }

        /// <summary>
        /// Set by Castle via de configuration
        /// </summary>
        /// <remarks>This method is added for the backwards-compatibility with Quartz v1 and the QuartzFacility v1</remarks>
        public IDictionary TriggerListeners
        {
            set
            {
                foreach (DictionaryEntry tl in value)
                {
                    foreach (ITriggerListener triggerListener in tl.Value as IList)
                    {
                        _scheduler.ListenerManager.AddTriggerListener(triggerListener);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns the name of the <see cref="T:Quartz.IScheduler"/>.
        /// </summary>
        public string SchedulerName
        {
            get { return _scheduler.SchedulerName; }
        }

        /// <summary>
        /// Returns the instance Id of the <see cref="T:Quartz.IScheduler"/>.
        /// </summary>
        public string SchedulerInstanceId
        {
            get { return _scheduler.SchedulerInstanceId; }
        }

        /// <summary>
        /// Returns the <see cref="T:Quartz.SchedulerContext"/> of the <see cref="T:Quartz.IScheduler"/>.
        /// </summary>
        public SchedulerContext Context
        {
            get { return _scheduler.Context; }
        }

        /// <summary>
        /// Reports whether the <see cref="T:Quartz.IScheduler"/> is in stand-by mode.
        /// </summary>
        /// <seealso cref="M:Quartz.IScheduler.Standby"/><seealso cref="M:Quartz.IScheduler.Start"/>
        public bool InStandbyMode
        {
            get { return _scheduler.InStandbyMode; }
        }

        /// <summary>
        /// Reports whether the <see cref="T:Quartz.IScheduler"/> has been Shutdown.
        /// </summary>
        public bool IsShutdown
        {
            get { return _scheduler.IsShutdown; }
        }

        /// <summary>
        /// Set the <see cref="P:Quartz.IScheduler.JobFactory"/> that will be responsible for producing 
        ///             instances of <see cref="T:Quartz.IJob"/> classes.
        /// </summary>
        /// <remarks>
        /// JobFactories may be of use to those wishing to have their application
        ///             produce <see cref="T:Quartz.IJob"/> instances via some special mechanism, such as to
        ///             give the opportunity for dependency injection.
        /// </remarks>
        /// <seealso cref="T:Quartz.Spi.IJobFactory"/>
        public IJobFactory JobFactory
        {
            set { _scheduler.JobFactory = value; }
        }

        /// <summary>
        /// Get a reference to the scheduler's <see cref="T:Quartz.IListenerManager"/>,
        ///             through which listeners may be registered.
        /// </summary>
        /// <returns>
        /// the scheduler's <see cref="T:Quartz.IListenerManager"/>
        /// </returns>
        /// <seealso cref="P:Quartz.IScheduler.ListenerManager"/><seealso cref="T:Quartz.IJobListener"/><seealso cref="T:Quartz.ITriggerListener"/><seealso cref="T:Quartz.ISchedulerListener"/>
        public IListenerManager ListenerManager
        {
            get { return _scheduler.ListenerManager; }
        }

        /// <summary>
        /// Whether the scheduler has been started.  
        /// </summary>
        /// <remarks>
        /// Note: This only reflects whether <see cref="M:Quartz.IScheduler.Start"/> has ever
        ///             been called on this Scheduler, so it will return <see langword="true"/> even 
        ///             if the <see cref="T:Quartz.IScheduler"/> is currently in standby mode or has been 
        ///             since shutdown.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.Start"/><seealso cref="P:Quartz.IScheduler.IsShutdown"/><seealso cref="P:Quartz.IScheduler.InStandbyMode"/>
        public bool IsStarted
        {
            get { return _scheduler.IsStarted; }
        }

        /// <summary>
        /// returns true if the given JobGroup
        ///             is paused
        /// </summary>
        /// <param name="groupName"/>
        /// <returns/>
        public bool IsJobGroupPaused(string groupName)
        {
            return _scheduler.IsJobGroupPaused(groupName);
        }

        /// <summary>
        /// returns true if the given TriggerGroup
        ///             is paused
        /// </summary>
        /// <param name="groupName"/>
        /// <returns/>
        public bool IsTriggerGroupPaused(string groupName)
        {
            return _scheduler.IsTriggerGroupPaused(groupName);
        }

        /// <summary>
        /// Get a <see cref="T:Quartz.SchedulerMetaData"/> object describing the settings
        ///             and capabilities of the scheduler instance.
        /// </summary>
        /// <remarks>
        /// Note that the data returned is an 'instantaneous' snap-shot, and that as
        ///             soon as it's returned, the meta data values may be different.
        /// </remarks>
        public SchedulerMetaData GetMetaData()
        {
            return _scheduler.GetMetaData();
        }

        /// <summary>
        /// Return a list of <see cref="T:Quartz.IJobExecutionContext"/> objects that
        ///             represent all currently executing Jobs in this Scheduler instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is not cluster aware.  That is, it will only return Jobs
        ///             currently executing in this Scheduler instance, not across the entire
        ///             cluster.
        /// </para>
        /// <para>
        /// Note that the list returned is an 'instantaneous' snap-shot, and that as
        ///             soon as it's returned, the true list of executing jobs may be different.
        ///             Also please read the doc associated with <see cref="T:Quartz.IJobExecutionContext"/>-
        ///             especially if you're using remoting.
        /// </para>
        /// </remarks>
        /// <seealso cref="T:Quartz.IJobExecutionContext"/>
        public IList<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            return _scheduler.GetCurrentlyExecutingJobs();
        }

        /// <summary>
        /// Get the names of all known <see cref="T:Quartz.IJobDetail"/> groups.
        /// </summary>
        public IList<string> GetJobGroupNames()
        {
            return _scheduler.GetJobGroupNames();
        }

        /// <summary>
        /// Get the names of all known <see cref="T:Quartz.ITrigger"/> groups.
        /// </summary>
        public IList<string> GetTriggerGroupNames()
        {
            return _scheduler.GetTriggerGroupNames();
        }

        /// <summary>
        /// Get the names of all <see cref="T:Quartz.ITrigger"/> groups that are paused.
        /// </summary>
        public global::Quartz.Collection.ISet<string> GetPausedTriggerGroups()
        {
            return _scheduler.GetPausedTriggerGroups();
        }

        /// <summary>
        /// Starts the <see cref="T:Quartz.IScheduler"/>'s threads that fire <see cref="T:Quartz.ITrigger"/>s.
        ///             When a scheduler is first created it is in "stand-by" mode, and will not
        ///             fire triggers.  The scheduler can also be put into stand-by mode by
        ///             calling the <see cref="M:Quartz.IScheduler.Standby"/> method.
        /// </summary>
        /// <remarks>
        /// The misfire/recovery process will be started, if it is the initial call
        ///             to this method on this scheduler instance.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.StartDelayed(System.TimeSpan)"/><seealso cref="M:Quartz.IScheduler.Standby"/><seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)"/>
        public void Start()
        {
            _scheduler.Start();
        }

        /// <summary>
        /// Calls <see cref="M:Quartz.IScheduler.Start"/> after the indicated delay.
        ///             (This call does not block). This can be useful within applications that
        ///             have initializers that create the scheduler immediately, before the
        ///             resources needed by the executing jobs have been fully initialized.
        /// </summary>
        /// <seealso cref="M:Quartz.IScheduler.Start"/><seealso cref="M:Quartz.IScheduler.Standby"/><seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)"/>
        public void StartDelayed(TimeSpan delay)
        {
            _scheduler.StartDelayed(delay);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _scheduler.Shutdown(WaitForJobsToCompleteAtShutdown);
        }

        /// <summary>
        /// Temporarily halts the <see cref="T:Quartz.IScheduler"/>'s firing of <see cref="T:Quartz.ITrigger"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <see cref="M:Quartz.IScheduler.Start"/> is called (to bring the scheduler out of 
        ///             stand-by mode), trigger misfire instructions will NOT be applied
        ///             during the execution of the <see cref="M:Quartz.IScheduler.Start"/> method - any misfires 
        ///             will be detected immediately afterward (by the <see cref="T:Quartz.Spi.IJobStore"/>'s 
        ///             normal process).
        /// </para>
        /// <para>
        /// The scheduler is not destroyed, and can be re-started at any time.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.Start"/><seealso cref="M:Quartz.IScheduler.PauseAll"/>
        public void Standby()
        {
            _scheduler.Standby();
        }

        /// <summary>
        /// Halts the <see cref="T:Quartz.IScheduler"/>'s firing of <see cref="T:Quartz.ITrigger"/>s,
        ///             and cleans up all resources associated with the Scheduler. Equivalent to
        ///             <see cref="M:Quartz.IScheduler.Shutdown(System.Boolean)"/>.
        /// </summary>
        /// <remarks>
        /// The scheduler cannot be re-started.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.Shutdown(System.Boolean)"/>
        public void Shutdown()
        {
            Stop();
        }

        /// <summary>
        /// Halts the <see cref="T:Quartz.IScheduler"/>'s firing of <see cref="T:Quartz.ITrigger"/>s,
        ///             and cleans up all resources associated with the Scheduler. 
        /// </summary>
        /// <remarks>
        /// The scheduler cannot be re-started.
        /// </remarks>
        /// <param name="waitForJobsToComplete">if <see langword="true"/> the scheduler will not allow this method
        ///             to return until all currently executing jobs have completed.
        ///             </param><seealso cref="M:Quartz.IScheduler.Shutdown"/>
        public void Shutdown(bool waitForJobsToComplete)
        {
            _scheduler.Shutdown(waitForJobsToComplete);
        }

        /// <summary>
        /// Add the given <see cref="T:Quartz.IJobDetail"/> to the
        ///             Scheduler, and associate the given <see cref="T:Quartz.ITrigger"/> with
        ///             it.
        /// </summary>
        /// <remarks>
        /// If the given Trigger does not reference any <see cref="T:Quartz.IJob"/>, then it
        ///             will be set to reference the Job passed with it into this method.
        /// </remarks>
        public DateTimeOffset ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
        {
            return _scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// Schedule the given <see cref="T:Quartz.ITrigger"/> with the
        ///             <see cref="T:Quartz.IJob"/> identified by the <see cref="T:Quartz.ITrigger"/>'s settings.
        /// </summary>
        public DateTimeOffset ScheduleJob(ITrigger trigger)
        {
            return _scheduler.ScheduleJob(trigger);
        }

        /// <summary>
        /// Schedule all of the given jobs with the related set of triggers.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If any of the given jobs or triggers already exist (or more
        ///             specifically, if the keys are not unique) and the replace
        ///             parameter is not set to true then an exception will be thrown.
        /// </para>
        /// </remarks>
        public void ScheduleJob(IJobDetail jobDetail, Quartz.Collection.ISet<ITrigger> triggersForJob, bool replace)
        {
            _scheduler.ScheduleJob(jobDetail, triggersForJob, replace);
        }

        /// <summary>
        /// Remove the indicated <see cref="T:Quartz.ITrigger"/> from the scheduler.
        /// <para>
        /// If the related job does not have any other triggers, and the job is
        ///             not durable, then the job will also be deleted.
        /// </para>
        /// </summary>
        public bool UnscheduleJob(TriggerKey triggerKey)
        {
            return _scheduler.UnscheduleJob(triggerKey);
        }

        /// <summary>
        /// Remove all of the indicated <see cref="T:Quartz.ITrigger"/>s from the scheduler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the related job does not have any other triggers, and the job is
        ///             not durable, then the job will also be deleted.
        /// </para>
        ///             Note that while this bulk operation is likely more efficient than
        ///             invoking <see cref="M:Quartz.IScheduler.UnscheduleJob(Quartz.TriggerKey)"/> several
        ///             times, it may have the adverse affect of holding data locks for a
        ///             single long duration of time (rather than lots of small durations
        ///             of time).
        /// </remarks>
        public bool UnscheduleJobs(IList<TriggerKey> triggerKeys)
        {
            return _scheduler.UnscheduleJobs(triggerKeys);
        }

        /// <summary>
        /// Remove (delete) the <see cref="T:Quartz.ITrigger"/> with the
        ///             given key, and store the new given one - which must be associated
        ///             with the same job (the new trigger must have the job name &amp; group specified) 
        ///             - however, the new trigger need not have the same name as the old trigger.
        /// </summary>
        /// <param name="triggerKey">The <see cref="T:Quartz.ITrigger"/> to be replaced.</param><param name="newTrigger">The new <see cref="T:Quartz.ITrigger"/> to be stored.
        ///             </param>
        /// <returns>
        /// <see langword="null"/> if a <see cref="T:Quartz.ITrigger"/> with the given
        ///             name and group was not found and removed from the store (and the 
        ///             new trigger is therefore not stored),  otherwise
        ///             the first fire time of the newly scheduled trigger.
        /// </returns>
        public DateTimeOffset? RescheduleJob(TriggerKey triggerKey, ITrigger newTrigger)
        {
            return _scheduler.RescheduleJob(triggerKey, newTrigger);
        }

        /// <summary>
        /// Add the given <see cref="T:Quartz.IJob"/> to the Scheduler - with no associated
        ///             <see cref="T:Quartz.ITrigger"/>. The <see cref="T:Quartz.IJob"/> will be 'dormant' until
        ///             it is scheduled with a <see cref="T:Quartz.ITrigger"/>, or <see cref="M:Quartz.IScheduler.TriggerJob(Quartz.JobKey)"/>
        ///             is called for it.
        /// </summary>
        /// <remarks>
        /// The <see cref="T:Quartz.IJob"/> must by definition be 'durable', if it is not,
        ///             SchedulerException will be thrown.
        /// </remarks>
        public void AddJob(IJobDetail jobDetail, bool replace)
        {
            _scheduler.AddJob(jobDetail, replace);
        }

        /// <summary>
        /// Delete the identified <see cref="T:Quartz.IJob"/> from the Scheduler - and any
        ///             associated <see cref="T:Quartz.ITrigger"/>s.
        /// </summary>
        /// <returns>
        /// true if the Job was found and deleted.
        /// </returns>
        public bool DeleteJob(JobKey jobKey)
        {
            return _scheduler.DeleteJob(jobKey);
        }

        /// <summary>
        /// Delete the identified jobs from the Scheduler - and any
        ///             associated <see cref="T:Quartz.ITrigger"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that while this bulk operation is likely more efficient than
        ///             invoking <see cref="M:Quartz.IScheduler.DeleteJob(Quartz.JobKey)"/> several
        ///             times, it may have the adverse affect of holding data locks for a
        ///             single long duration of time (rather than lots of small durations
        ///             of time).
        /// </para>
        /// </remarks>
        /// <returns>
        /// true if all of the Jobs were found and deleted, false if
        ///             one or more were not deleted.
        /// </returns>
        public bool DeleteJobs(IList<JobKey> jobKeys)
        {
            return _scheduler.DeleteJobs(jobKeys);
        }

        /// <summary>
        /// Trigger the identified <see cref="T:Quartz.IJobDetail"/>
        ///             (Execute it now).
        /// </summary>
        public void TriggerJob(JobKey jobKey)
        {
            _scheduler.TriggerJob(jobKey);
        }

        /// <summary>
        /// Trigger the identified <see cref="T:Quartz.IJobDetail"/> (Execute it now).
        /// </summary>
        /// <param name="data">the (possibly <see langword="null"/>) JobDataMap to be
        ///             associated with the trigger that fires the job immediately.
        ///             </param><param name="jobKey">The <see cref="T:Quartz.JobKey"/> of the <see cref="T:Quartz.IJob"/> to be executed.
        ///             </param>
        public void TriggerJob(JobKey jobKey, JobDataMap data)
        {
            _scheduler.TriggerJob(jobKey, data);
        }

        /// <summary>
        /// Pause the <see cref="T:Quartz.IJobDetail"/> with the given
        ///             key - by pausing all of its current <see cref="T:Quartz.ITrigger"/>s.
        /// </summary>
        public void PauseJob(JobKey jobKey)
        {
            _scheduler.PauseJob(jobKey);
        }

        /// <summary>
        /// Pause all of the <see cref="T:Quartz.IJobDetail"/>s in the
        ///             matching groups - by pausing all of their <see cref="T:Quartz.ITrigger"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Scheduler will "remember" that the groups are paused, and impose the
        ///             pause on any new jobs that are added to any of those groups until it is resumed.
        /// </para>
        /// <para>
        /// NOTE: There is a limitation that only exactly matched groups
        ///             can be remembered as paused.  For example, if there are pre-existing
        ///             job in groups "aaa" and "bbb" and a matcher is given to pause
        ///             groups that start with "a" then the group "aaa" will be remembered
        ///             as paused and any subsequently added jobs in group "aaa" will be paused,
        ///             however if a job is added to group "axx" it will not be paused,
        ///             as "axx" wasn't known at the time the "group starts with a" matcher 
        ///             was applied.  HOWEVER, if there are pre-existing groups "aaa" and
        ///             "bbb" and a matcher is given to pause the group "axx" (with a
        ///             group equals matcher) then no jobs will be paused, but it will be 
        ///             remembered that group "axx" is paused and later when a job is added 
        ///             in that group, it will become paused.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.ResumeJobs(Quartz.Impl.Matchers.GroupMatcher{Quartz.JobKey})"/>
        public void PauseJobs(GroupMatcher<JobKey> matcher)
        {
            _scheduler.PauseJobs(matcher);
        }

        /// <summary>
        /// Pause the <see cref="T:Quartz.ITrigger"/> with the given key.
        /// </summary>
        public void PauseTrigger(TriggerKey triggerKey)
        {
            _scheduler.PauseTrigger(triggerKey);
        }

        /// <summary>
        /// Pause all of the <see cref="T:Quartz.ITrigger"/>s in the groups matching.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Scheduler will "remember" all the groups paused, and impose the
        ///             pause on any new triggers that are added to any of those groups until it is resumed.
        /// </para>
        /// <para>
        /// NOTE: There is a limitation that only exactly matched groups
        ///             can be remembered as paused.  For example, if there are pre-existing
        ///             triggers in groups "aaa" and "bbb" and a matcher is given to pause
        ///             groups that start with "a" then the group "aaa" will be remembered as
        ///             paused and any subsequently added triggers in that group be paused,
        ///             however if a trigger is added to group "axx" it will not be paused,
        ///             as "axx" wasn't known at the time the "group starts with a" matcher 
        ///             was applied.  HOWEVER, if there are pre-existing groups "aaa" and
        ///             "bbb" and a matcher is given to pause the group "axx" (with a
        ///             group equals matcher) then no triggers will be paused, but it will be 
        ///             remembered that group "axx" is paused and later when a trigger is added
        ///             in that group, it will become paused.
        /// </para>
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.ResumeTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})"/>
        public void PauseTriggers(GroupMatcher<TriggerKey> matcher)
        {
            _scheduler.PauseTriggers(matcher);
        }

        /// <summary>
        /// Resume (un-pause) the <see cref="T:Quartz.IJobDetail"/> with
        ///             the given key.
        /// </summary>
        /// <remarks>
        /// If any of the <see cref="T:Quartz.IJob"/>'s<see cref="T:Quartz.ITrigger"/> s missed one
        ///             or more fire-times, then the <see cref="T:Quartz.ITrigger"/>'s misfire
        ///             instruction will be applied.
        /// </remarks>
        public void ResumeJob(JobKey jobKey)
        {
            _scheduler.ResumeJob(jobKey);
        }

        /// <summary>
        /// Resume (un-pause) all of the <see cref="T:Quartz.IJobDetail"/>s
        ///             in matching groups.
        /// </summary>
        /// <remarks>
        /// If any of the <see cref="T:Quartz.IJob"/> s had <see cref="T:Quartz.ITrigger"/> s that
        ///             missed one or more fire-times, then the <see cref="T:Quartz.ITrigger"/>'s
        ///             misfire instruction will be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.PauseJobs(Quartz.Impl.Matchers.GroupMatcher{Quartz.JobKey})"/>
        public void ResumeJobs(GroupMatcher<JobKey> matcher)
        {
            _scheduler.ResumeJobs(matcher);
        }

        /// <summary>
        /// Resume (un-pause) the <see cref="T:Quartz.ITrigger"/> with the given
        ///             key.
        /// </summary>
        /// <remarks>
        /// If the <see cref="T:Quartz.ITrigger"/> missed one or more fire-times, then the
        ///             <see cref="T:Quartz.ITrigger"/>'s misfire instruction will be applied.
        /// </remarks>
        public void ResumeTrigger(TriggerKey triggerKey)
        {
            _scheduler.ResumeTrigger(triggerKey);
        }

        /// <summary>
        /// Resume (un-pause) all of the <see cref="T:Quartz.ITrigger"/>s in matching groups.
        /// </summary>
        /// <remarks>
        /// If any <see cref="T:Quartz.ITrigger"/> missed one or more fire-times, then the
        ///             <see cref="T:Quartz.ITrigger"/>'s misfire instruction will be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})"/>
        public void ResumeTriggers(GroupMatcher<TriggerKey> matcher)
        {
            _scheduler.ResumeTriggers(matcher);
        }

        /// <summary>
        /// Pause all triggers - similar to calling <see cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})"/>
        ///             on every group, however, after using this method <see cref="M:Quartz.IScheduler.ResumeAll"/> 
        ///             must be called to clear the scheduler's state of 'remembering' that all 
        ///             new triggers will be paused as they are added. 
        /// </summary>
        /// <remarks>
        /// When <see cref="M:Quartz.IScheduler.ResumeAll"/> is called (to un-pause), trigger misfire
        ///             instructions WILL be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.ResumeAll"/><seealso cref="M:Quartz.IScheduler.PauseTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})"/><seealso cref="M:Quartz.IScheduler.Standby"/>
        public void PauseAll()
        {
            _scheduler.PauseAll();
        }

        /// <summary>
        /// Resume (un-pause) all triggers - similar to calling 
        ///             <see cref="M:Quartz.IScheduler.ResumeTriggers(Quartz.Impl.Matchers.GroupMatcher{Quartz.TriggerKey})"/> on every group.
        /// </summary>
        /// <remarks>
        /// If any <see cref="T:Quartz.ITrigger"/> missed one or more fire-times, then the
        ///             <see cref="T:Quartz.ITrigger"/>'s misfire instruction will be applied.
        /// </remarks>
        /// <seealso cref="M:Quartz.IScheduler.PauseAll"/>
        public void ResumeAll()
        {
            _scheduler.ResumeAll();
        }

        /// <summary>
        /// Get the keys of all the <see cref="T:Quartz.IJobDetail"/>s in the matching groups.
        /// </summary>
        public global::Quartz.Collection.ISet<JobKey> GetJobKeys(GroupMatcher<JobKey> matcher)
        {
            return _scheduler.GetJobKeys(matcher);
        }

        /// <summary>
        /// Get all <see cref="T:Quartz.ITrigger"/> s that are associated with the
        ///             identified <see cref="T:Quartz.IJobDetail"/>.
        /// </summary>
        /// <remarks>
        /// The returned Trigger objects will be snap-shots of the actual stored
        ///             triggers.  If you wish to modify a trigger, you must re-store the
        ///             trigger afterward (e.g. see <see cref="M:Quartz.IScheduler.RescheduleJob(Quartz.TriggerKey,Quartz.ITrigger)"/>).
        /// </remarks>
        public IList<ITrigger> GetTriggersOfJob(JobKey jobKey)
        {
            return _scheduler.GetTriggersOfJob(jobKey);
        }

        /// <summary>
        /// Get the names of all the <see cref="T:Quartz.ITrigger"/>s in the given
        ///             groups.
        /// </summary>
        public global::Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(GroupMatcher<TriggerKey> matcher)
        {
            return _scheduler.GetTriggerKeys(matcher);
        }

        /// <summary>
        /// Get the <see cref="T:Quartz.IJobDetail"/> for the <see cref="T:Quartz.IJob"/>
        ///             instance with the given key .
        /// </summary>
        /// <remarks>
        /// The returned JobDetail object will be a snap-shot of the actual stored
        ///             JobDetail.  If you wish to modify the JobDetail, you must re-store the
        ///             JobDetail afterward (e.g. see <see cref="M:Quartz.IScheduler.AddJob(Quartz.IJobDetail,System.Boolean)"/>).
        /// </remarks>
        public IJobDetail GetJobDetail(JobKey jobKey)
        {
            return _scheduler.GetJobDetail(jobKey);
        }

        /// <summary>
        /// Get the <see cref="T:Quartz.ITrigger"/> instance with the given key.
        /// </summary>
        /// <remarks>
        /// The returned Trigger object will be a snap-shot of the actual stored
        ///             trigger.  If you wish to modify the trigger, you must re-store the
        ///             trigger afterward (e.g. see <see cref="M:Quartz.IScheduler.RescheduleJob(Quartz.TriggerKey,Quartz.ITrigger)"/>).
        /// </remarks>
        public ITrigger GetTrigger(TriggerKey triggerKey)
        {
            return _scheduler.GetTrigger(triggerKey);
        }

        /// <summary>
        /// Get the current state of the identified <see cref="T:Quartz.ITrigger"/>.
        /// </summary>
        /// <seealso cref="F:Quartz.TriggerState.Normal"/><seealso cref="F:Quartz.TriggerState.Paused"/><seealso cref="F:Quartz.TriggerState.Complete"/><seealso cref="F:Quartz.TriggerState.Blocked"/><seealso cref="F:Quartz.TriggerState.Error"/><seealso cref="F:Quartz.TriggerState.None"/>
        public TriggerState GetTriggerState(TriggerKey triggerKey)
        {
            return _scheduler.GetTriggerState(triggerKey);
        }

        /// <summary>
        /// Add (register) the given <see cref="T:Quartz.ICalendar"/> to the Scheduler.
        /// </summary>
        /// <param name="calName">Name of the calendar.</param><param name="calendar">The calendar.</param><param name="replace">if set to <c>true</c> [replace].</param><param name="updateTriggers">whether or not to update existing triggers that
        ///             referenced the already existing calendar so that they are 'correct'
        ///             based on the new trigger.</param>
        public void AddCalendar(string calName, ICalendar calendar, bool replace, bool updateTriggers)
        {
            _scheduler.AddCalendar(calName, calendar, replace, updateTriggers);
        }

        /// <summary>
        /// Delete the identified <see cref="T:Quartz.ICalendar"/> from the Scheduler.
        /// </summary>
        /// <remarks>
        /// If removal of the 
        /// <code>
        /// Calendar
        /// </code>
        ///  would result in
        ///             <see cref="T:Quartz.ITrigger"/>s pointing to non-existent calendars, then a
        ///             <see cref="T:Quartz.SchedulerException"/> will be thrown.
        /// </remarks>
        /// <param name="calName">Name of the calendar.</param>
        /// <returns>
        /// true if the Calendar was found and deleted.
        /// </returns>
        public bool DeleteCalendar(string calName)
        {
            return _scheduler.DeleteCalendar(calName);
        }

        /// <summary>
        /// Get the <see cref="T:Quartz.ICalendar"/> instance with the given name.
        /// </summary>
        public ICalendar GetCalendar(string calName)
        {
            return _scheduler.GetCalendar(calName);
        }

        /// <summary>
        /// Get the names of all registered <see cref="T:Quartz.ICalendar"/>.
        /// </summary>
        public IList<string> GetCalendarNames()
        {
            return _scheduler.GetCalendarNames();
        }

        /// <summary>
        /// Request the interruption, within this Scheduler instance, of all 
        ///             currently executing instances of the identified <see cref="T:Quartz.IJob"/>, which 
        ///             must be an implementor of the <see cref="T:Quartz.IInterruptableJob"/> interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If more than one instance of the identified job is currently executing,
        ///             the <see cref="M:Quartz.IInterruptableJob.Interrupt"/> method will be called on
        ///             each instance.  However, there is a limitation that in the case that  
        ///             <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)"/> on one instances throws an exception, all 
        ///             remaining  instances (that have not yet been interrupted) will not have 
        ///             their <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)"/> method called.
        /// </para>
        /// <para>
        /// If you wish to interrupt a specific instance of a job (when more than
        ///             one is executing) you can do so by calling 
        ///             <see cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs"/> to obtain a handle 
        ///             to the job instance, and then invoke <see cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)"/> on it
        ///             yourself.
        /// </para>
        /// <para>
        /// This method is not cluster aware.  That is, it will only interrupt 
        ///             instances of the identified InterruptableJob currently executing in this 
        ///             Scheduler instance, not across the entire cluster.
        /// </para>
        /// </remarks>
        /// <returns>
        /// true is at least one instance of the identified job was found and interrupted.
        /// </returns>
        /// <seealso cref="T:Quartz.IInterruptableJob"/><seealso cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs"/>
        public bool Interrupt(JobKey jobKey)
        {
            return _scheduler.Interrupt(jobKey);
        }

        /// <summary>
        /// Request the interruption, within this Scheduler instance, of the 
        ///             identified executing job instance, which 
        ///             must be an implementor of the <see cref="T:Quartz.IInterruptableJob"/> interface.
        /// </summary>
        /// <remarks>
        /// This method is not cluster aware.  That is, it will only interrupt 
        ///             instances of the identified InterruptableJob currently executing in this 
        ///             Scheduler instance, not across the entire cluster.
        /// </remarks>
        /// <seealso cref="M:Quartz.IInterruptableJob.Interrupt"/><seealso cref="M:Quartz.IScheduler.GetCurrentlyExecutingJobs"/><seealso cref="P:Quartz.IJobExecutionContext.FireInstanceId"/><seealso cref="M:Quartz.IScheduler.Interrupt(Quartz.JobKey)"/><param nane="fireInstanceId">the unique identifier of the job instance to  be interrupted (see <see cref="P:Quartz.IJobExecutionContext.FireInstanceId"/></param><param name="fireInstanceId"/>
        /// <returns>
        /// true if the identified job instance was found and interrupted.
        /// </returns>
        public bool Interrupt(string fireInstanceId)
        {
            return _scheduler.Interrupt(fireInstanceId);
        }

        /// <summary>
        /// Determine whether a <see cref="T:Quartz.IJob"/> with the given identifier already 
        ///             exists within the scheduler.
        /// </summary>
        /// <param name="jobKey">the identifier to check for</param>
        /// <returns>
        /// true if a Job exists with the given identifier
        /// </returns>
        public bool CheckExists(JobKey jobKey)
        {
            return _scheduler.CheckExists(jobKey);
        }

        /// <summary>
        /// Determine whether a <see cref="T:Quartz.ITrigger"/> with the given identifier already 
        ///             exists within the scheduler.
        /// </summary>
        /// <param name="triggerKey">the identifier to check for</param>
        /// <returns>
        /// true if a Trigger exists with the given identifier
        /// </returns>
        public bool CheckExists(TriggerKey triggerKey)
        {
            return _scheduler.CheckExists(triggerKey);
        }

        /// <summary>
        /// Clears (deletes!) all scheduling data - all <see cref="T:Quartz.IJob"/>s, <see cref="T:Quartz.ITrigger"/>s
        ///             <see cref="T:Quartz.ICalendar"/>s.
        /// </summary>
        public void Clear()
        {
            _scheduler.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Stop();
        }

        public void ScheduleJobs(IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> triggersAndJobs, bool replace)
        {
           _scheduler.ScheduleJobs(triggersAndJobs, replace);
        }
    }
}