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
using System.Windows.Media.Animation;
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
        private string capData = "M 0 0 A 10,5 0 0 0 20,0 A 10,10 0 0 0 0,0";
        private string pinData = "M 0 0 L 0 12 A 3,1 0 0 0 6,12 L 6 0 A 3,1 0 0 0 0 0";
        private Brush[] capBrushes = {Brushes.Transparent, Brushes.DarkGoldenrod, Brushes.Green, Brushes.Lime, Brushes.Blue,
                                       Brushes.DeepSkyBlue, Brushes.BlueViolet, Brushes.Red, Brushes.Yellow};
        private bool coverUp;
        private int showPins;
        private const int capDistance = 30;
        private const int capAndCapPlaceDiference = 3;
        private const int distanceFromBottomEdge = 50;
        private const int distanceCapsFromLeftEdge = 30;
        private const int distancePinsFromLeftEdge = 190;
        private const int distanceHiddenCapsFromTopEdge = 60;
        private const int distanceColorCapsFromRightEdge = 25;
        private const int distanceColorCapsFromBottomEdge = 75;
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
                CreateNewPinCover(j);
                for (int i = 0; i < GameBoardState.fieldsCount; i++)
                {
                    double caps3DNarrowing = 2 - i * 0.5;
                    double pins3DNarrowing = 15 - j * 0.3;
                    CreateCapPlaces(i, j, caps3DNarrowing);
                    CreateTransparentCaps(i, j, caps3DNarrowing);
                    CreatePinPlaces(i,j, pins3DNarrowing);
                    CreateTransparentPins(i,j, pins3DNarrowing);
                }
            }
            for (int color = 1; color < capBrushes.Count(); color++)
            {
                CreateColorCaps(color);
            }
            CoverUp = false;
            ShowPins = -1;
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
                double caps3DNarrowing = 2 - i * 0.5;
                Canvas.SetLeft(hiddenCap, distanceCapsFromLeftEdge - capAndCapPlaceDiference + 13 * caps3DNarrowing + i * capDistance);
                Canvas.SetTop(hiddenCap, distanceHiddenCapsFromTopEdge);
            }
        }
        private void CreateCapPlaces(int i, int j, double caps3DNarrowing)
        {
            Ellipse capPlace = new Ellipse()
            {
                Width = 12,
                Height = 7,
                Fill = Brushes.Black
            };
            board.Children.Add(capPlace);
            Canvas.SetLeft(capPlace, capDistance +j*caps3DNarrowing + i * capDistance);
            Canvas.SetBottom(capPlace, distanceFromBottomEdge + j * capDistance);
        }
        private void CreateTransparentCaps(int i, int j, double cap3DNarrowing)
        {
            Path transparentCap = new Path();
            transparentCap.Tag = i.ToString() + "," + j.ToString();
            transparentCap.Data = Geometry.Parse(capData);
            transparentCap.Fill = Brushes.Transparent;
            transparentCap.MouseLeftButtonDown += PlacedCap_MouseLeftButtonDown;
            board.Children.Add(transparentCap);
            Canvas.SetLeft(transparentCap, capDistance - capAndCapPlaceDiference + j * cap3DNarrowing + i * capDistance);
            Canvas.SetBottom(transparentCap, distanceFromBottomEdge - capAndCapPlaceDiference + j * capDistance);
            caps[i, j] = transparentCap;
        }
        private void CreatePinPlaces(int i, int j, double pins3DNarrowing)
        {
            Ellipse resultPlace = new Ellipse()
            {
                Width = 6,
                Height = 3,
                Fill = Brushes.Black
            };
            board.Children.Add(resultPlace);
            Canvas.SetLeft(resultPlace, distancePinsFromLeftEdge + i * pins3DNarrowing);
            Canvas.SetBottom(resultPlace, distanceFromBottomEdge + j * capDistance);
        }
        private void CreateTransparentPins(int i, int j, double pins3DNarrowing)
        {
            Path transparentPin = new Path();
            transparentPin.Data = Geometry.Parse(pinData);
            transparentPin.Fill = Brushes.Transparent;
            board.Children.Add(transparentPin);
            Canvas.SetLeft(transparentPin, distancePinsFromLeftEdge + i * pins3DNarrowing);
            Canvas.SetBottom(transparentPin, distanceFromBottomEdge + j * capDistance);
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
            double colorCaps3DNarrowing = 2 * color;
            Canvas.SetRight(colorCap, distanceColorCapsFromRightEdge + colorCaps3DNarrowing);
            Canvas.SetBottom(colorCap, distanceColorCapsFromBottomEdge + capDistance * color);
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
                ShowPins = gbState.actualRow;
                if (evalPins.BlackPins == GameBoardState.fieldsCount)
                {
                    EndGame("Vyhrál jsi!");
                }
                else if (gbState.actualRow == GameBoardState.rowsCount - 1)
                {
                    EndGame("Prohrál jsi...");
                }
                else
                {
                    gbState.MoveToNextRow();
                }
            }           
        }
        private void EndGame(string message)
        {
            CoverUp = true;
            MessageBox.Show(message);
            CoverUp = false;
            ResetBoard();
        }
        private void CreateNewPinCover(int row)
        {
            var move = new DoubleAnimation() {From = distanceFromBottomEdge + 3 + capDistance * row, 
                To = distanceFromBottomEdge + 15 + capDistance * row, Duration = new Duration(TimeSpan.FromSeconds(1)) };
            Storyboard.SetTargetProperty(move, new PropertyPath(Canvas.BottomProperty));
            var storyboard = new Storyboard();
            storyboard.Children.Add(move);
            var action = new BeginStoryboard() { Storyboard = storyboard };
            var trigger = new DataTrigger() { Binding = new Binding("ShowPins"), Value = row};
            trigger.EnterActions.Add(action);
            Style style = new Style() { TargetType = typeof(Rectangle)};
            style.Triggers.Add(trigger);
            Rectangle newPinCover = new Rectangle() { Fill = Brushes.LightGray, Width = 80, Height = 12 };
            newPinCover.Style = style;
            board.Children.Add(newPinCover);
            Canvas.SetLeft(newPinCover, distancePinsFromLeftEdge - 10);
            Canvas.SetBottom(newPinCover, distanceFromBottomEdge + 3 + capDistance * row);
            Canvas.SetZIndex(newPinCover, 1);
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
