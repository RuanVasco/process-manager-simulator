using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessManagerSimulator.Models.SchedulingPolicies;

public class RoundRobinSchedulingPolicy : ISchedulingAlgorithm {
	public string Name => "Round Robin";

	public Process? SelectNextProcess(List<Process> readyQueue, int currentTime) {
		if (readyQueue.Count == 0)
			throw new InvalidOperationException("A fila de prontos está vazia. Não há processos para escalonar.");

		// retorna o processo com menor tempo de chegada (a lógica do algoritmo fica no código de execução)
		return readyQueue
			.FirstOrDefault(p => p.ArrivalTime <= currentTime);
	}
}