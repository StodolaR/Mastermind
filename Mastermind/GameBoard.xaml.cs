using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mastermind
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl, INotifyPropertyChanged
    {
        GameBoardState gbState;
        private Path[,] caps;
        private Path[,] pins;
        private string capData = "M 0 0 A 11,5 0 0 0 22,0 A 11,11 0 0 0 0,0";
        private string pinData = "M 0 0 L 0 12 A 3,1 0 0 0 6,12 L 6 0 A 3,1 0 0 0 0 0";
        private Brush[] capBrushes = {Brushes.Transparent, Brushes.Red, Brushes.Green, Brushes.Orange, Brushes.Purple, Brushes.Brown, 
                                        Brushes.Yellow, Brushes.White, Brushes.Blue};
        private bool coverUp;
        private int showPins;

        public bool CoverUp
        {
            get => coverUp;
            set
            {
                coverUp = value;
                OnPropertyChanged(nameof(CoverUp));
            }
        }
        public int ShowPins 
        {
            get => showPins;
            set 
            {
                showPins = value;
                OnPropertyChanged(nameof(ShowPins));
            } 
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public GameBoard()
        {
            InitializeComponent();
            gbState = new GameBoardState();
            gbState.FillSecretFields(capBrushes.Count());
            CreateHiddenCaps();
            caps = new Path[GameBoardState.fieldsCount,GameBoardState.rowsCount];
            pins = new Path[GameBoardState.fieldsCount,GameBoardState.rowsCount];            
            for (int j = 0; j < GameBoardState.rowsCount; j++)
            {
                for (int i = 0; i < GameBoardState.fieldsCount; i++)
                {
                    CreateCapPlaces(i, j);
                    CreateTransparentCaps(i, j);
                    CreatePinPlaces(i,j);
                    CreateTransparentPins(i,j);
                }
            }
            for (int color = 1; color < capBrushes.Count(); color++)
            {
                CreateColorCaps(color);
            }
            CoverUp = false;
            ShowPins = 0;
            DataContext = this;
        }
        private void CreateHiddenCaps()
        {
            for (int i = 0; i  < GameBoardState.fieldsCount; i++)
            {
                Path hiddenCap = new Path();
                hiddenCap.Data = Geometry.Parse(capData);
                hiddenCap.Fill = capBrushes[gbState.secretFields[i]];
                board.Children.Add(hiddenCap);
                Canvas.SetLeft(hiddenCap, 27 + i * 30);
                Canvas.SetTop(hiddenCap, 60);
            }
        }
        private void CreateCapPlaces(int i, int j)
        {
            Ellipse capPlace = new Ellipse()
            {
                Width = 15,
                Height = 7,
                Fill = Brushes.Black
            };
            board.Children.Add(capPlace);
            Canvas.SetLeft(capPlace, 30 + i * 30);
            Canvas.SetBottom(capPlace, 30 + j * 30);
        }
        private void CreateTransparentCaps(int i, int j)
        {
            Path transparentCap = new Path();
            transparentCap.Tag = i.ToString() + "," + j.ToString();
            transparentCap.Data = Geometry.Parse(capData);
            transparentCap.Fill = Brushes.Transparent;
            transparentCap.MouseLeftButtonDown += PlacedCap_MouseLeftButtonDown;
            board.Children.Add(transparentCap);
            Canvas.SetLeft(transparentCap, 27 + i * 30);
            Canvas.SetBottom(transparentCap, 27 + j * 30);
            caps[i, j] = transparentCap;
        }
        private void CreatePinPlaces(int i, int j)
        {
            Ellipse resultPlace = new Ellipse()
            {
                Width = 6,
                Height = 3,
                Fill = Brushes.Black
            };
            board.Children.Add(resultPlace);
            Canvas.SetLeft(resultPlace, 200 + i * 15);
            Canvas.SetBottom(resultPlace, 30 + j * 30);
        }
        private void CreateTransparentPins(int i, int j)
        {
            Path transparentPin = new Path();
            transparentPin.Data = Geometry.Parse(pinData);
            transparentPin.Fill = Brushes.Transparent;
            board.Children.Add(transparentPin);
            Canvas.SetLeft(transparentPin, 200 + i * 15);
            Canvas.SetBottom(transparentPin, 30 + j * 30);
            pins[i, j] = transparentPin;
        }
        private void CreateColorCaps(int color)
        {
            Path colorCap = new Path();
            colorCap.Tag = color.ToString();
            colorCap.Data = Geometry.Parse(capData);
            colorCap.Fill = capBrushes[color];
            colorCap.MouseLeftButtonDown += ColorCap_MouseLeftButtonDown;
            board.Children.Add(colorCap);
            Canvas.SetRight(colorCap, 30);
            Canvas.SetBottom(colorCap, 50 + 30 * color);
        }
        private void PlacedCap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Path removedCap = (Path)sender;
            string[] coordinates = ((string)removedCap.Tag).Split(",");
            int rowField = Convert.ToInt32(coordinates[0]);
            int row = Convert.ToInt32(coordinates[1]);
            if (row == gbState.actualRow)
            {
                removedCap.Fill = Brushes.Transparent;
                gbState.ReturnActualField(rowField);
            }
        }
        private void ColorCap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Path actualCap = caps[gbState.actualField, gbState.actualRow];
            actualCap.Fill = ((Path)sender).Fill;
            int color = Convert.ToInt32(((Path)sender).Tag); 
            bool canMoveToNext = gbState.AddColorAndMoveToNext(color);
            if(!canMoveToNext)
            {
                Pins evalPins = gbState.EvaluateRow();
                for (int i = 0; i < evalPins.BlackPins; i++)
                {
                    pins[i, gbState.actualRow].Fill = Brushes.Black;
                }
                for(int i = evalPins.BlackPins; i < evalPins.BlackPins + evalPins.WhitePins;i++)
                {
                    pins[i, gbState.actualRow].Fill = Brushes.White;
                }
                ShowPins = gbState.actualRow + 1;
                if (gbState.actualRow == 0)
                {
                    CreateNewShowPin();
                }
                if (evalPins.BlackPins == GameBoardState.fieldsCount)
                {
                    CoverUp = true;
                    MessageBox.Show("Vyhrál jsi!");
                    CoverUp = false;
                    ResetBoard();
                }
                else if (gbState.actualRow == GameBoardState.rowsCount - 1)
                {
                    CoverUp = true;
                    MessageBox.Show("Prohrál jsi...");
                    CoverUp = false;
                    ResetBoard();
                }
                else
                {
                    gbState.MoveToNextRow();
                }
            }           
        }
        private void CreateNewShowPin()
        {
            Rectangle newShowPin = new Rectangle()
            {
                Fill = Brushes.Blue,
                Width = 85,
                Height = 12,
            };
            newShowPin.Style = (Style)FindResource("pinEjection");
            board.Children.Add(newShowPin);
            Canvas.SetLeft(newShowPin, 190);
            Canvas.SetBottom(newShowPin, 33);
            Canvas.SetZIndex(newShowPin, 1);
        }
        private void ResetBoard()
        {
            for (int j = 0; j < GameBoardState.rowsCount; j++)
            {
                for (int i = 0; i < GameBoardState.fieldsCount; i++)
                {
                    caps[i, j].Fill = Brushes.Transparent;
                    pins[i,j].Fill = Brushes.Transparent;
                }
            }
            gbState.ResetState();
            gbState.FillSecretFields(capBrushes.Count());
            CreateHiddenCaps();
        }
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
