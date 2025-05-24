using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class SjfPreemptiveSchedulingPolicy : ISchedulingAlgorithm {
	public string Name => "SJF (Preemptivo)";

	public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
		// retorna o processo ordenado com menor tempo de execução restante 
		return readyQueue
			.Where(p => p.ArrivalTime <= currentTime && p.RemainingTime > 0)
			.OrderBy(p => p.RemainingTime)
			.ThenBy(p => p.ArrivalTime)
			.FirstOrDefault();
	}
}