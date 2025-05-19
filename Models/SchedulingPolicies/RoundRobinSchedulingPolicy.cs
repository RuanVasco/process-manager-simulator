using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class RoundRobinSchedulingPolicy : ISchedulingAlgorithm {
    
    public string Name => "Round Robin";
    
    public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
        if (readyQueue.Count == 0) {
            throw new InvalidOperationException("A fila de prontos está vazia. Não há processos para escalonar.");
        }
            
        return readyQueue.OrderBy(p => p.ArrivalTime).FirstOrDefault();
    }
}