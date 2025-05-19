using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class SjfPreemptiveSchedulingPolicy : ISchedulingAlgorithm {
	
	public string Name => "SJF (Preemptivo)";
	
	public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
		return readyQueue
			.Where(p => p.ArrivalTime <= currentTime && p.RemainingTime > 0)
			.OrderBy(p => p.RemainingTime)
			.ThenBy(p => p.ArrivalTime) // Desempate por chegada mais antiga
			.FirstOrDefault();
	}
}