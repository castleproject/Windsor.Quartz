using System;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
    public class SomeJobListener : IJobListener {
        public void JobToBeExecuted(IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobExecutionVetoed(IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return GetType().AssemblyQualifiedName; }
        }
    }
}