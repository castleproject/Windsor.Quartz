using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace SampleApp {
	internal class Program {
		private static void Main(string[] args) {
			new WindsorContainer(new XmlInterpreter());
			Console.WriteLine("Started");
		}
	}
}