using System;
using System.Collections.Generic;
using System.Linq;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Services
{
    public class FcfsSchedulingPolicy : ISchedulingPolicy
    {
        public Process SelectNextProcess(List<Process> readyQueue, int currentTime)
        {
            if (readyQueue.Count == 0)
                {
                    throw new InvalidOperationException("A fila de prontos está vazia. Não há processos para escalonar.");
                }
            
            return readyQueue.OrderBy(p => p.ArrivalTime).FirstOrDefault();
        }
       
    }
}
