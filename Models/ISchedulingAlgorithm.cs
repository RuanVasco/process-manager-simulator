using System.Collections.Generic;

namespace ProcessManagerSimulator.Models;

public interface ISchedulingAlgorithm {
    string Name { get; }
    Process? SelectNextProcess(List<Process> readyQueue, int currentTime);
}