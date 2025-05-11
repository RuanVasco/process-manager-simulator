using System;
using Avalonia.Controls;
using Avalonia.Input;
using ProcessManagerSimulator.Models;

namespace ProcessManagerSimulator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NumeroDeProcessosTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (NumberInputMask(e)) {
            e.Handled = true; 
        }
    }

    private void NumeroDeProcessosTextBox_TextInput(object sender, TextInputEventArgs e)
    {
        if (!int.TryParse(e.Text, out _))
        {
            e.Handled = true;
        }
    }

    private void QuantumTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (NumberInputMask(e)) {
            e.Handled = true; 
        }
    }

    private void QuantumTextBox_TextInput(object sender, TextInputEventArgs e) {
        if (!int.TryParse(e.Text, out _)) {
            e.Handled = true;
        }
    }

    private void CriarButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        if (int.TryParse(NumeroDeProcessosTextBox.Text, out int numeroDeProcessos))
        {
            ResulTextBox.Text = $"Criar {numeroDeProcessos} processos.";

            for (int i = 0; i < numeroDeProcessos; i++)
            {
                var processo = new Process(arrivalTime: 0, executionTime: 5);
                ResulTextBox.Text += $"Processo criado: PID {processo.Pid}";
            }
        }
        else
        {
            Console.WriteLine("Número de processos inválido!");
        }

        if (int.TryParse(QuantumTextBox.Text, out int quantum))
        {
            Console.WriteLine($"Quantum definido para {quantum} unidades de tempo.");
        }
        else
        {
            Console.WriteLine("Quantum inválido!");
        }
    }

    private static Boolean NumberInputMask(KeyEventArgs e) {
        if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&   
            !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) && 
            e.Key != Key.Back &&
            e.Key != Key.Delete &&
            e.Key != Key.Tab &&
            e.Key != Key.Left &&
            e.Key != Key.Right)
        {
            return true;
        }

        return false;
    }
}
