using System;
using System.Diagnostics;
using System.Linq;
using Nancy;

namespace TransparentWindow.Nancy.Modules.Machine.Processes
{
    public class ProcessesModule
        :NancyModule
    {
        public ProcessesModule()
            :base("machine/processes")
        {
            Get["/"] = GetProcessList;
            Get["/{nameorid}"] = GetProcess;
            Post["/"] = CreateProcess;
        }

        public dynamic SerializeProcess(Process p)
        {
            if (p.HasExited)
            {
                return new
                {
                    p.Id,
                    p.HasExited
                };
            }
            else
            {
                return new
                {
                    p.Id,
                    p.ProcessName,
                    p.WorkingSet64,
                    p.VirtualMemorySize64,
                    p.Responding,
                    p.HasExited
                };
            }
        }

        private dynamic GetProcessList(dynamic o)
        {
            return Process.GetProcesses().Select(a => SerializeProcess(a)).ToArray();
        }

        private object GetProcess(dynamic o)
        {
            var nid = (string)o.nameorid;

            int id;
            if (int.TryParse(nid, out id))
            {
                try
                {
                    var p = Process.GetProcessById(id);
                    return SerializeProcess(p);
                }
                catch (ArgumentException)
                {
                    //No process with this ID, try using it as a name instead
                }
            }

            var ps = Process.GetProcessesByName(nid);
            if (ps.Length != 0)
                return ps.Select(SerializeProcess).ToArray();

            return HttpStatusCode.NotFound;
        }

        private object CreateProcess(object arg)
        {
            var name = Request.Query["name"];
            var args = Request.Query["args"];

            var p = Process.Start(name, args);
            return SerializeProcess(p);
        }
    }
}
