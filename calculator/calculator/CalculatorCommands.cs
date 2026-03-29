using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace lab2_wpf
{
    public partial class MainWindow : Window
    {
        Stack<CalculatorCommand> undoStack = new Stack<CalculatorCommand>();
        Stack<CalculatorCommand> redoStack = new Stack<CalculatorCommand>();

        public bool IsNewEntry { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Execute(CalculatorCommand command)
        {
            command.Execute();
            if (command.HasStateChanged())
            {
                undoStack.Push(command);
                redoStack.Clear();
            }
        }

       
    }
}