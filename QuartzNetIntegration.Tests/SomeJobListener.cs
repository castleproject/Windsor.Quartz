using System;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
    public class SomeJobListener : IJobListener {
        public void JobToBeExecuted(JobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobExecutionVetoed(JobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return GetType().AssemblyQualifiedName; }
        }
    }
}