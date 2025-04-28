using System.Collections.Generic;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Services
{
    public class RoundRobinSchedulingPolicy : ISchedulingPolicy
    {
        private readonly int _quantum;
        private int _lastIndex = -1;

        public RoundRobinSchedulingPolicy(int quantum)
        {
            _quantum = quantum;
        }

        public Process SelectNextProcess(List<Process> readyQueue, int currentTime)
        {
            if (readyQueue.Count == 0)
                return null;

            _lastIndex = (_lastIndex + 1) % readyQueue.Count;
            return readyQueue[_lastIndex];
        }
    }
}
