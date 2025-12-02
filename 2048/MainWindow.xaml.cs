using _2048.Stores;
using _2048.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseMainWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult confirmExit = MessageBox.Show(
                "Do you want to close application?",
                "Exit Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirmExit == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}