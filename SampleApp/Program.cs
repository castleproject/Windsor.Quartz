using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.Commons;

namespace SampleApp {
	internal class Program {
		private static void Main(string[] args) {
			IoC.Initialize(new WindsorContainer(new XmlInterpreter()));
			Console.WriteLine("Started");
		}
	}
}