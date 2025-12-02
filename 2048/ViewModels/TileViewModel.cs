using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _2048.ViewModels
{
    public class TileViewModel : ViewModelBase
    {
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private bool _justSpawned;
        public bool JustSpawned
        {
            get => _justSpawned;
            set
            {
                _justSpawned = value;
                OnPropertyChanged();
            }
        }

        private bool _justMerged;
        public bool JustMerged
        {
            get => _justMerged;
            set
            {
                _justMerged = value;
                OnPropertyChanged();
            }
        }

        public Brush BackgroundColor => Value switch
        {
            0 => Brushes.Gray,
            2 => Brushes.Beige,
            4 => Brushes.Bisque,
            8 => Brushes.Orange,
            16 => Brushes.DarkOrange,
            32 => Brushes.Coral,
            64 => Brushes.Red,
            128 => Brushes.Yellow,
            256 => Brushes.Gold,
            512 => Brushes.YellowGreen,
            1024 => Brushes.Green,
            2048 => Brushes.DarkGreen,
            _ => Brushes.Black,
        };

        public void ResetAnimationFlags()
        {
            JustSpawned = false;
            JustMerged = false;
        } 
    }
}