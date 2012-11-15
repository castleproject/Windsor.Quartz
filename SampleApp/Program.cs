using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace SampleApp {
    internal class Program {
        private static void Main(string[] args) {
            var container = new WindsorContainer(new XmlInterpreter());

            Console.WriteLine("Started");
        }
    }
}