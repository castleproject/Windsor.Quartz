using Quartz;
using Quartz.Spi;
using Rhino.Commons;

namespace QuartzNetIntegration {
	public class WindsorJobFactory : IJobFactory {
		public IJob NewJob(TriggerFiredBundle bundle) {
			var job = IoC.Container.Resolve(bundle.JobDetail.JobType);
			return job as IJob;
		}
	}
}