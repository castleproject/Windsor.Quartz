using System;
using System.Collections.Generic;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Quartz;
using Quartz.Job;
using Quartz.Spi;

namespace QuartzNetIntegration {
    public class QuartzNetFacility : AbstractFacility {
        protected override void Init() {
            Kernel.ConfigurationStore.AddComponentConfiguration(typeof (QuartzNetScheduler).AssemblyQualifiedName, BuildConfig(FacilityConfig));
            AddComponent<IScheduler, QuartzNetScheduler>();
            AddComponent<IJobScheduler, QuartzNetSimpleScheduler>();
            AddComponent<IJobFactory, WindsorJobFactory>();
            AddComponent<FileScanJob>();
        }

        internal IConfiguration BuildConfig(IConfiguration config) {
            if (config == null)
                throw new FacilityException("Please define the configuration for Quartz.Net facility");
            var quartzNet = config.Children["quartzNet"];
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

            return componentConfig;
        }

        internal void BuildServiceDictionary<K, V>(IConfiguration config, MutableConfiguration parameters) {
            var dict = parameters.CreateChild("dictionary");
            dict.Attribute("keyType", typeof (K).AssemblyQualifiedName);
            dict.Attribute("valueType", typeof(V).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children) {
                var job = dict.CreateChild("entry")
                    .Attribute("key", c.Attributes["name"]);
                BuildServiceArray<V>(c, job);
            }
        }

        internal void BuildServiceList<T>(IConfiguration config, MutableConfiguration parameters) {
            var array = parameters.CreateChild("list");
            array.Attribute("type", typeof (T).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children) {
                array.CreateChild("item", c.Value);
            }            
        }

        internal void BuildServiceArray<T>(IConfiguration config, MutableConfiguration parameters) {
            var array = parameters.CreateChild("array");
            array.Attribute("type", typeof(T).AssemblyQualifiedName);
            foreach (IConfiguration c in config.Children) {
                array.CreateChild("item", c.Value);
            }
        }

        internal void BuildProps(IConfiguration config, MutableConfiguration props) {
            var dict = props.CreateChild("dictionary");
            foreach (IConfiguration c in config.Children) {
                dict.CreateChild("item", c.Value)
                    .Attribute("key", c.Attributes["key"]);
            }
        }

        internal string AddComponent<T>() {
            string key = typeof (T).AssemblyQualifiedName;
            Kernel.AddComponent(key, typeof (T));
            return key;
        }

        internal string AddComponent<I, T>() where T : I {
            string key = typeof (T).AssemblyQualifiedName;
            Kernel.AddComponent(key, typeof (I), typeof (T));
            return key;
        }
    }
}