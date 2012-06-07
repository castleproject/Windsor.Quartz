using System;
using Quartz;

namespace SampleApp {
	public class SampleJob : IJob {
		public void Execute(IJobExecutionContext context) {
			Console.WriteLine("Hello world!");
		}
	}
}