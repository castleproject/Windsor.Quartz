using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace SampleApp {
    internal class Program {
        private static void Main(string[] args) {
            var container = new WindsorContainer(new XmlInterpreter());

            Console.WriteLine("Started");

            var task = Task.Run(() => Thread.Sleep(1000 * 1000));
            task.Wait();
        }
    }
}