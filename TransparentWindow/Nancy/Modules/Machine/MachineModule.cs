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
            Get["/username"] = _ => new { username = Environment.UserName };
            Get["/uptime"] = _ =>
            {
                var tspan = TimeSpan.FromMilliseconds(Environment.TickCount);
                return new { seconds = tspan.TotalSeconds };
            };
        }

        private static dynamic GetPerformanceCounter(dynamic request)
        {
            //Using this is generally a bad idea, creating performance counters seems very heavyweight. Need to find a way to cache them.
            //perhaps instead offer websockets to get performance data. That way when all connections to a given socket are closed you can dispose the associated performance counter

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
