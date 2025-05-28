using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class SjfSchedulingPolicy : ISchedulingAlgorithm {
	public string Name => "SJF";

	public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
		if (readyQueue.Count == 0) return null;

		// retorna o processo com menor tempo de execução (total)
		return readyQueue
			.Where(p => p.ArrivalTime <= currentTime)
			.OrderBy(p => p.ExecutionTime)
			.FirstOrDefault();
	}
}