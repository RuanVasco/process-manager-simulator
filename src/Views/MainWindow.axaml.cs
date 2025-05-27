using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ProcessManagerSimulator.Models;
using ProcessManagerSimulator.Models.SchedulingPolicies;

namespace ProcessManagerSimulator.Views;

public partial class MainWindow : Window {
	private readonly List<ISchedulingAlgorithm> algorithms;
	public ObservableCollection<Process> Processes { get; } = new();


	public MainWindow() {
		InitializeComponent();
		DataContext = this;

		algorithms = new List<ISchedulingAlgorithm> {
			new RoundRobinSchedulingPolicy(),
			new FcfsSchedulingPolicy(),
			new SjfSchedulingPolicy(),
			new SjfPreemptiveSchedulingPolicy()
		};

		AlgorithmComboBox.ItemsSource = algorithms;
	}


	private void IntTextBox_KeyDown(object sender, KeyEventArgs e) {
		if (NumberInputMaskForInt(e)) e.Handled = true;
	}

	private void IntTextBox_TextInput(object sender, TextInputEventArgs e) {
		if (!int.TryParse(e.Text, out _)) e.Handled = true;
	}

	private void FloatTextBox_KeyDown(object sender, KeyEventArgs e) {
		e.Handled = NumberInputMaskForFloat((TextBox)sender, e);
	}

	private void FloatTextBox_TextInput(object sender, TextInputEventArgs e) {
		var textBox = (TextBox)sender;
		var ch      = e.Text;

		if (char.IsDigit(ch, 0))
			return;

		if ((ch == "." || ch == ",") && !textBox.Text.Contains('.') && !textBox.Text.Contains(','))
			return;

		e.Handled = true;
	}

	private void AddProcessButton_Click(object? sender, RoutedEventArgs e) {
		if (!int.TryParse(ArrivalTextBox.Text, out var arrival) || arrival < 0 ||
		    !int.TryParse(ExecTextBox.Text, out var exec) || exec <= 0) {
			ResulTextBox.Text = "Chegada ou execução inválidos.\n";
			return;
		}
		
		Process process = new Process(arrival, exec);
		Processes.Add(process);
		ArrivalTextBox.Text = ExecTextBox.Text = "";
		ArrivalTextBox.Focus();
	}

	private void RemoveProcessButton_Click(object? sender, RoutedEventArgs e) {
		if (sender is Button { DataContext: Process p })
			Processes.Remove(p);
	}

	private void SimulateButton_Click(object sender, RoutedEventArgs e) {
		ResulTextBox.Text = "";

		if (!float.TryParse(TtcTextBox.Text, out var ttc) || ttc <= 0) {
			ResulTextBox.Text = "TTC inválido!\n";
			return;
		}

		if (!int.TryParse(QuantumTextBox.Text, out var quantum) || quantum <= 0) quantum = 1;

		var selectedAlgorithm = AlgorithmComboBox.SelectedItem as ISchedulingAlgorithm;
		if (selectedAlgorithm == null) {
			ResulTextBox.Text = "Nenhum algoritmo de escalonamento selecionado.\n";
			return;
		}

		// executa a simulação passando as opções selecionadas como argumento
		SimulateExecution(selectedAlgorithm, quantum, ttc);
	}

	private void SimulateExecution(
		ISchedulingAlgorithm alg,
		int quantum,
		float ttc
	) {
		var procs = Processes;
		// inicia o tempo como 0
		var time = 0;

		// boolean pra evitar o tempo de contexto na primeira execução
		var firstDispatch = true;

		// pid - tempo da primeira execução (para calcular o tempo de espera) 
		var firstExecution = new Dictionary<int, int>();

		var log = new StringBuilder()
			.AppendLine($"Algoritmo: {alg.Name}")
			.AppendLine($"Quantum: {quantum}")
			.AppendLine($"TTC: {ttc}\n");

		// cria uma lista encadeada dos processos para melhorar a busca linear
		var ready = new LinkedList<Process>(procs);
		
		// contador para o log
		var counter = 0;

		while (ready.Any(p => p.RemainingTime > 0)) {
			// guardar o tempo de inicio da execução
			var startTime = time; 
			
			// seleciona o próximo processo, passando para a política selecionada a lista com os processos restantes
			var next = alg.SelectNextProcess(
				ready.Where(p => p.RemainingTime > 0).ToList(),
				time
			);

			// cpu ociosa, não chegou nenhum processo ainda.
			// pula para o próximo processo disponível com menor tempo de chegada
			// não conta o ttc
			if (next == null) {
				time = ready.Where(p => p.RemainingTime > 0)
					.Min(p => p.ArrivalTime);
				firstDispatch = true;
				continue;
			}

			// adiciona o ttc ao tempo de execução
			if (!firstDispatch)
				time += (int)ttc;
			firstDispatch = false;

			var isRR = alg is RoundRobinSchedulingPolicy;

			// define o tempo de execução do processo, se for roundrobin o tempo máximo é o quantum e o minímo é o tempo de execução restante do processo
			var slice = isRR
				? Math.Min(quantum, next.RemainingTime)
				: next.RemainingTime;

			// se for a primeira execução, adiciona ao array o pid e o tempo atual
			if (!firstExecution.ContainsKey(next.Pid))
				firstExecution[next.Pid] = time;

			// subtrai do tempo restante do processo o tempo de execução
			next.RemainingTime -= slice;

			// se o tempo restante zerar, então remover da lista encadeada
			if (next.RemainingTime <= 0) ready.Remove(next);

			// soma ao tempo da cpu o tempo de execuçaõ
			time += slice;

			// se for roundrobin e ainda restar tempo de execução, adicionar ao fim da lista encadeada
			if (isRR && next.RemainingTime > 0) {
				ready.Remove(next);
				ready.AddLast(next);
			}

			log.AppendLine(
				$"{counter} - t={startTime} -> PID {next.Pid} por {slice} unidades de tempo");

			counter++;
		}

		log.AppendLine($"\nFim em t={time}");

		// se for fcfs ou sjf (não preemptivo) calcular o tempo médio de espera
		if (alg.Name == "FCFS" || alg.Name == "SJF") {
			double totalWaitTime = 0;
			foreach (var p in procs) {
				// tempo de espera do processo é o tempo da cpu na primeira execução - o tempo de chegada do processo
				var wait = firstExecution[p.Pid] - p.ArrivalTime;

				// soma ao tempo total
				totalWaitTime += wait;
			}

			// calcula a média de tempo de espera (total / número de processos)
			var averageWaitTime = totalWaitTime / procs.Count;

			log.AppendLine($"Tempo médio de espera: {averageWaitTime:F2} unidades de tempo");
		}

		ResulTextBox.Text = log.ToString();
	}

	private static bool NumberInputMaskForInt(KeyEventArgs e) {
		if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&
		    !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&
		    e.Key != Key.Back &&
		    e.Key != Key.Delete &&
		    e.Key != Key.Tab &&
		    e.Key != Key.Left &&
		    e.Key != Key.Right)
			return true;

		return false;
	}

	private static bool NumberInputMaskForFloat(TextBox textBox, KeyEventArgs e) {
		if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
		    e.Key == Key.Left || e.Key == Key.Right)
			return false;

		if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
		    (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
			return false;

		if (e.Key == Key.OemComma || e.Key == Key.OemPeriod || e.Key == Key.Decimal) {
			var text = textBox.Text ?? "";
			if (text.Contains(",") || text.Contains("."))
				return true;
			return false;
		}

		return true;
	}
}