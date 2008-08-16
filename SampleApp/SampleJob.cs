using System;
using Quartz;

namespace SampleApp {
	public class SampleJob : IJob {
		public void Execute(JobExecutionContext context) {
			Console.WriteLine("Hello world!");
		}
	}
}