using System;
using Castle.Facilities.QuartzIntegration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace SampleApp {
    internal class Program {
        private static void Main(string[] args) {
            var container = new WindsorContainer(new XmlInterpreter());
            container.AddFacility("quartznet", new QuartzFacility());

            Console.WriteLine("Started");
        }
    }
}