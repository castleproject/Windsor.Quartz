using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Quartz;
using Quartz.Job;
using Quartz.Spi;

namespace Castle.Facilities.Quartz
{
    /// <summary>
    ///     Castle facility for Quartz.NET
    /// </summary>
    /// <seealso cref="Castle.MicroKernel.Facilities.AbstractFacility" />
    public class QuartzFacility : AbstractFacility
    {
        protected override void Init()
        {
            AddComponent<FileScanJob>();
            AddComponent<IJobFactory, JobFactory>();
            AddComponent<IReleasingJobListener, ReleasingJobListener>();
            AddComponent<IScheduler, Scheduler>((k, p) => p["props"] = Properties ?? new Dictionary<string, string>());

            // Configure Schedule Context
            var scheduler = Kernel.Resolve<IScheduler>();

            if (SchedulerContext != null)
                foreach (var scheduleContextEntry in SchedulerContext)
                    scheduler.Context.Add(scheduleContextEntry.Key, scheduleContextEntry.Value);

            // Configure global trigger listeners
            if (TriggerListeners != null)
                foreach (var triggerListener in TriggerListeners)
                    scheduler.ListenerManager.AddTriggerListener(triggerListener.Listener, triggerListener.Matchers);

            // Configure global job listeners
            if (JobListeners != null)
                foreach (var jobListener in JobListeners)
                    scheduler.ListenerManager.AddJobListener(jobListener.Listener, jobListener.Matchers);

            // Configure scheduler listeners
            if (SchedulerListeners != null)
                foreach (var scheduleListener in SchedulerListeners)
                    scheduler.ListenerManager.AddSchedulerListener(scheduleListener);
        }

        protected string AddComponent<T>()
        {
            var key = typeof(T).AssemblyQualifiedName;
            Kernel.Register(Component.For(typeof(T)).Named(key));
            return key;
        }

        /// <summary>
        ///     Adds the component.
        /// </summary>
        /// <param name="dynamicParameters">Optional lambda describing dependencies</param>
        /// <typeparam name="TService">Type of component's interface</typeparam>
        /// <typeparam name="TComponent">Type of component</typeparam>
        /// <returns>
        ///     key of added component
        /// </returns>
        protected string AddComponent<TService, TComponent>(DynamicParametersDelegate dynamicParameters = null)
            where TService : class
            where TComponent : class, TService
        {
            var key = typeof(TComponent).AssemblyQualifiedName;

            var component = Component
                                .For<TService>()
                                .ImplementedBy<TComponent>()
                                .Named(key);

            if (dynamicParameters != null)
            {
                component.DynamicParameters(dynamicParameters);
            }

            Kernel.Register(component);

            return key;
        }

        #region Configure facility

        public JobListener[] JobListeners { get; set; }
        public TriggerListener[] TriggerListeners { get; set; }
        public ISchedulerListener[] SchedulerListeners { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public IDictionary<string, object> SchedulerContext { get; set; }

        #endregion Configure facility
    }
}