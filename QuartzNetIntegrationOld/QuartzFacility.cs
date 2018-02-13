using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Quartz;
using Quartz.Job;
using Quartz.Spi;

namespace Castle.Facilities.QuartzIntegration {
    /// <summary>
    /// Castle facility for Quartz.NET
    /// </summary>
    /// <seealso cref="Castle.MicroKernel.Facilities.AbstractFacility" />
    public class QuartzFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ConfigurationStore.AddComponentConfiguration(typeof(QuartzNetScheduler).AssemblyQualifiedName, BuildConfig(FacilityConfig));
            AddComponent<FileScanJob>();
            AddComponent<IJobScheduler, QuartzNetSimpleScheduler>();
            AddComponent<IJobFactory, WindsorJobFactory>();
            AddComponent<IScheduler, QuartzNetScheduler>();
        }

        internal IConfiguration BuildConfig(IConfiguration config)
        {
            if (config == null)
                throw new FacilityException("Please define the configuration for Quartz.Net facility");

            var quartzNet = config.Children["quartz"];
            if (quartzNet == null)
                throw new FacilityException("Please define the Quartz.Net properties");

            var componentConfig = new MutableConfiguration(typeof(QuartzNetScheduler).AssemblyQualifiedName);
            var parameters = componentConfig.CreateChild("parameters");
            BuildProps(quartzNet, parameters.CreateChild("props"));

            var globalJobListeners = config.Children["globalJobListeners"];
            if (globalJobListeners != null)
                BuildServiceArray<IJobListener>(globalJobListeners, parameters.CreateChild("SetGlobalJobListeners"));

            var globalTriggerListeners = config.Children["globalTriggerListeners"];
            if (globalTriggerListeners != null)
                BuildServiceArray<ITriggerListener>(globalTriggerListeners, parameters.CreateChild("SetGlobalTriggerListeners"));

            var jobListeners = config.Children["jobListeners"];
            if (jobListeners != null)
                BuildServiceDictionary<string, IJobListener[]>(jobListeners, parameters.CreateChild("jobListeners"));

            var triggerListeners = config.Children["triggerListeners"];
            if (triggerListeners != null)
                BuildServiceDictionary<string, ITriggerListener[]>(triggerListeners, parameters.CreateChild("triggerListeners"));

            var schedulerListeners = config.Children["schedulerListeners"];
            if (schedulerListeners != null)
                BuildServiceArray<ISchedulerListener>(schedulerListeners, parameters.CreateChild("SetSchedulerListeners"));

            return componentConfig;
        }

        internal void BuildServiceDictionary<TKey, TValue>(IConfiguration config, MutableConfiguration parameters)
        {
            var dict = parameters.CreateChild("dictionary");
            dict.Attribute("keyType", typeof(TKey).AssemblyQualifiedName);
            dict.Attribute("valueType", typeof(TValue).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children)
            {
                var job = dict.CreateChild("entry")
                    .Attribute("key", c.Attributes["name"]);
                BuildServiceArray<TValue>(c, job);
            }
        }

        internal void BuildServiceList<T>(IConfiguration config, MutableConfiguration parameters)
        {
            var array = parameters.CreateChild("list");
            array.Attribute("type", typeof(T).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children)
            {
                array.CreateChild("item", c.Value);
            }
        }

        internal void BuildServiceArray<T>(IConfiguration config, MutableConfiguration parameters)
        {
            var array = parameters.CreateChild("array");
            array.Attribute("type", typeof(T).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children)
            {
                array.CreateChild("item", c.Value);
            }
        }

        internal void BuildProps(IConfiguration config, MutableConfiguration props)
        {
            var dict = props.CreateChild("dictionary");
            foreach (IConfiguration c in config.Children)
            {
                dict.CreateChild("item", c.Value)
                    .Attribute("key", c.Attributes["key"]);
            }
        }

        internal string AddComponent<T>()
        {
            string key = typeof(T).AssemblyQualifiedName;
            Kernel.Register(Component.For(typeof(T)).Named(key));
            return key;
        }

        internal string AddComponent<I, T>() where T : I
        {
            string key = typeof(T).AssemblyQualifiedName;
            Kernel.Register(Component.For(typeof(I)).ImplementedBy(typeof(T)).Named(key));
            return key;
        }
    }
}