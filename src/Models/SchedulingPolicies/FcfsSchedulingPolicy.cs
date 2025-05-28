using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class FcfsSchedulingPolicy : ISchedulingAlgorithm {
	public string Name => "FCFS";

	public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
		// retorna o processo com menor ordem de chegada

		return readyQueue
			.Where(p => p.ArrivalTime <= currentTime)
			.OrderBy(p => p.ArrivalTime)
			.FirstOrDefault();
	}
}