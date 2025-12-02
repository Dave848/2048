using _2048.Models.DTO;
using _2048.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);

            if (DataContext is GameViewModel vm)
            {
                vm.AchievementComplete += ShowAchievementPopup;
            }
        }

        private async void ShowAchievementPopup(object? sender, AchievementDTO achievement)
        {
            var popup = new ContentControl
            {
                Content = achievement,
                ContentTemplate = (DataTemplate)FindResource("AchievementPopupTemplate")
            };

            AchievementPopupPanel.Children.Insert(0, popup);

            popup.Loaded += (_, _) =>
            {
                Storyboard show = (Storyboard)FindResource("ShowAchievementAnimation");
                show.Begin(popup);
            };

            await Task.Delay(2400);

            Storyboard hide = (Storyboard)FindResource("HideAchievementAnimation");
            hide.Completed += (s, e) => AchievementPopupPanel.Children.Remove(popup);
            hide.Begin(popup);
        }
    }
}
