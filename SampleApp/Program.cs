using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using QuartzNetIntegration;

namespace SampleApp {
    internal class Program {
        private static void Main(string[] args) {
            var container = new WindsorContainer(new XmlInterpreter());
            container.AddFacility("quartznet", new QuartzNetFacility());

            Console.WriteLine("Started");
        }
    }
}