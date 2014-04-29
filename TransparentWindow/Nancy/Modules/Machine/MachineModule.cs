using System;
using System.Diagnostics;
using Nancy;

namespace TransparentWindow.Nancy.Modules.Machine
{
    public class MachineModule
        :NancyModule
    {
        public MachineModule()
            :base("machine")
        {
            Get["/performancecounters/{category_name}/{counter_name}"] = GetPerformanceCounter;
            Get["/name"] = _ => new { name = Environment.MachineName };
            Get["/uptime"] = _ =>
            {
                var tspan = TimeSpan.FromMilliseconds(Environment.TickCount);
                return new { seconds = tspan.TotalSeconds };
            };
        }

        private dynamic GetPerformanceCounter(dynamic request)
        {
            //Using this is generally a bad idea, creating performance counters seems very heavyweight. Need to find a way to cache them.

            using (var c = new PerformanceCounter((string) request.category_name, (string) request.counter_name))
            {
                return new
                {
                    Value = c.NextValue()
                };
            }
        }
    }
}
