using System.Collections.Generic;
using System.Linq;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Services
{
    public class SjfNonPreemptiveSchedulingPolicy : ISchedulingPolicy
    {
        public Process SelectNextProcess(List<Process> readyQueue, int currentTime)
        {
            return readyQueue.OrderBy(p => p.ExecutionTime).FirstOrDefault();
        }
    }
}
