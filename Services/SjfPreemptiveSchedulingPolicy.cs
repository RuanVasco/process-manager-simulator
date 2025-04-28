using System.Collections.Generic;
using System.Linq;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Services
{
    public class SjfPreemptiveSchedulingPolicy : ISchedulingPolicy
    {
        public Process SelectNextProcess(List<Process> readyQueue, int currentTime)
        {
            return readyQueue.OrderBy(p => p.RemainingTime).FirstOrDefault();
        }
    }
}
