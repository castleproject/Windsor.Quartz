using System;
using Quartz;
using Castle.MicroKernel;

namespace Castle.Facilities.QuartzIntegration {
    public class ReleasingJobListener: IJobListener {
        private readonly IKernel kernel;

        public ReleasingJobListener(IKernel kernel) {
            this.kernel = kernel;
        }

        public void JobExecutionVetoed(JobExecutionContext context) {
            kernel.ReleaseComponent(context.JobInstance);
        }

        public void JobToBeExecuted(JobExecutionContext context) {
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            kernel.ReleaseComponent(context.JobInstance);
        }

        public string Name {
            get { return GetType().FullName; }
        }
    }
}
