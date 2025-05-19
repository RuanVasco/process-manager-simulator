using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ProcessManagerSimulator.Models;
using ProcessManagerSimulator.Models.SchedulingPolicies;

namespace ProcessManagerSimulator.Views;

public partial class MainWindow : Window {
	private static readonly Random _rng = new();
	private readonly List<ISchedulingAlgorithm> algorithms;

	public MainWindow() {
		InitializeComponent();

		algorithms = new List<ISchedulingAlgorithm> {
			new RoundRobinSchedulingPolicy(),
			new FcfsSchedulingPolicy(),
			new SjfSchedulingPolicy(),
			new SjfPreemptiveSchedulingPolicy()
		};

		AlgorithmComboBox.ItemsSource = algorithms;
	}

	private void ProcessNumberTextBox_KeyDown(object sender, KeyEventArgs e) {
		if (NumberInputMaskForInt(e)) e.Handled = true;
	}

	private void ProcessNumberTextBox_TextInput(object sender, TextInputEventArgs e) {
		if (!int.TryParse(e.Text, out _)) e.Handled = true;
	}

	private void QuantumTextBox_KeyDown(object sender, KeyEventArgs e) {
		if (NumberInputMaskForInt(e)) e.Handled = true;
	}

	private void QuantumTextBox_TextInput(object sender, TextInputEventArgs e) {
		if (!int.TryParse(e.Text, out _)) e.Handled = true;
	}

	private void TtcTextBox_KeyDown(object sender, KeyEventArgs e) {
		if (NumberInputMaskForFloat(TtcTextBox, e)) e.Handled = true;
	}

	private void TtcTextBox_TextInput(object sender, TextInputEventArgs e) {
		if (!int.TryParse(e.Text, out _)) e.Handled = true;
	}

	private void CreateButton_Click(object sender, RoutedEventArgs e) {
		ResulTextBox.Text = "";

		if (!int.TryParse(ProcessNumberTextBox.Text, out var processNumber) || processNumber <= 0) {
			ResulTextBox.Text = "Número de processos inválido!\n";
			return;
		}

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

		var processes = GenerateProcesses(processNumber);
		SimulateExecution(processes, selectedAlgorithm, quantum, ttc);
	}

	private void SimulateExecution(
		List<Process> procs,
		ISchedulingAlgorithm alg,
		int quantum,
		float ttc
	) {
		var time = 0;
		var firstDispatch = true;

		var log = new StringBuilder()
			.AppendLine($"Algoritmo: {alg.Name}")
			.AppendLine($"Quantum: {quantum}")
			.AppendLine($"TTC: {ttc}\n");

		var ready = new LinkedList<Process>(procs);

		while (ready.Any(p => p.RemainingTime > 0)) {
			var next = alg.SelectNextProcess(
				ready.Where(p => p.RemainingTime > 0).ToList(),
				time
			);

			if (next == null) {
				time = ready.Where(p => p.RemainingTime > 0)
					.Min(p => p.ArrivalTime);
				firstDispatch = true;
				continue;
			}

			if (!firstDispatch)
				time += (int)ttc;
			firstDispatch = false;

			var isRR = alg is RoundRobinSchedulingPolicy;
			var slice = isRR
				? Math.Min(quantum, next.RemainingTime)
				: next.RemainingTime;

			log.AppendLine($"t={time} → PID {next.Pid} por {slice}");

			next.RemainingTime -= slice;

			if (next.RemainingTime <= 0) ready.Remove(next);

			time += slice;

			if (isRR && next.RemainingTime > 0) {
				ready.Remove(next);
				ready.AddLast(next);
			}
		}

		log.AppendLine($"\nFim em t={time}");
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

	private List<Process> GenerateProcesses(int count) {
		var list = new List<Process>(count);
		for (var i = 0; i < count; i++) {
			var arrival = _rng.Next(0, count / 2);
			var exec = _rng.Next(1, 11);
			list.Add(new Process(arrival, exec));
		}

		return list;
	}
}