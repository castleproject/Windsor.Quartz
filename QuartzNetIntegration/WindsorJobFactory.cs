using Castle.MicroKernel;
using Quartz;
using Quartz.Spi;

namespace Castle.Facilities.QuartzIntegration {
	public class WindsorJobFactory : IJobFactory {
		private readonly IKernel kernel;

		public bool ResolveByJobName { get; set; }

		public WindsorJobFactory(IKernel kernel) {
			this.kernel = kernel;
		}

		public IJob NewJob(TriggerFiredBundle bundle) {
			return (IJob) (ResolveByJobName ? kernel.Resolve(bundle.JobDetail.Name, typeof(IJob)) : kernel.Resolve(bundle.JobDetail.JobType));
		}
	}
}