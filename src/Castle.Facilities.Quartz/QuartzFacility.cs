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
        private JobListener[] _jobListeners;
        private IDictionary<string, string> _properties;
        private IDictionary<string, object> _schedulerContext;
        private ISchedulerListener[] _schedulerListeners;
        private TriggerListener[] _triggerListeners;

        protected override void Init()
        {
            AddComponent<FileScanJob>();
            AddComponent<IJobFactory, JobFactory>();
            AddComponent<IReleasingJobListener, ReleasingJobListener>();
            AddComponent<IScheduler, Scheduler>((k, p) => p["props"] = _properties ?? new Dictionary<string, string>());

            // Configure Schedule Context
            var scheduler = Kernel.Resolve<IScheduler>();

            if (_schedulerContext != null)
                foreach (var scheduleContextEntry in _schedulerContext)
                    scheduler.Context.Add(scheduleContextEntry.Key, scheduleContextEntry.Value);

            // Configure global trigger listeners
            if (_triggerListeners != null)
                foreach (var triggerListener in _triggerListeners)
                    scheduler.ListenerManager.AddTriggerListener(triggerListener.Listener, triggerListener.Matchers);

            // Configure global job listeners
            if (_jobListeners != null)
                foreach (var jobListener in _jobListeners)
                    scheduler.ListenerManager.AddJobListener(jobListener.Listener, jobListener.Matchers);

            // Configure scheduler listeners
            if (_schedulerListeners != null)
                foreach (var scheduleListener in _schedulerListeners)
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

        public QuartzFacility SetJobListeners(params JobListener[] jobListeners)
        {
            _jobListeners = jobListeners;
            return this;
        }

        public QuartzFacility SetTriggerListeners(params TriggerListener[] triggerListeners)
        {
            _triggerListeners = triggerListeners;
            return this;
        }

        public QuartzFacility SetSchedulerListeners(params ISchedulerListener[] schedulerListeners)
        {
            _schedulerListeners = schedulerListeners;
            return this;
        }

        public QuartzFacility SetProperties(IDictionary<string, string> properties)
        {
            _properties = properties;
            return this;
        }

        public QuartzFacility SetSchedulerContext(IDictionary<string, object> schedulerContext)
        {
            _schedulerContext = schedulerContext;
            return this;
        }

        #endregion Configure facility
    }
}