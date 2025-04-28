using System.Collections.Generic;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Services
{
    public interface ISchedulingPolicy
    {
        Process SelectNextProcess(List<Process> readyQueue, int currentTime);
    }
}
