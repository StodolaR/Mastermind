using System;
using System.Collections.Generic;
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
    public partial class GameBoard : UserControl
    {
        GameBoardState gbState;
        private bool freePlaces;
        private Path[,] caps;
        private string capData = "M 0 0 A 11,5 0 0 0 22,0 A 11,11 0 0 0 0,0";
        private Brush[] capBrushes = {Brushes.Transparent, Brushes.Red, Brushes.Green, Brushes.Orange, Brushes.Purple, Brushes.Brown, 
                                        Brushes.Yellow, Brushes.White, Brushes.Blue};
        public GameBoard()
        {
            InitializeComponent();
            gbState = new GameBoardState();
            freePlaces = true;
            caps = new Path[GameBoardState.fieldsCount,GameBoardState.rowsCount];
            for (int j = 0; j < GameBoardState.rowsCount; j++)
            {
                for (int i = 0; i < GameBoardState.fieldsCount; i++)
                {
                    CreateCapPlaces(i, j);
                    CreateTransparentCaps(i, j);
                }
            }
            for (int color = 1;color < capBrushes.Count();color++)
            {
                CreateColorCaps(color);
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
            Ellipse resultPlace = new Ellipse()
            {
                Width = 5,
                Height = 3,
                Fill = Brushes.Black
            };
            board.Children.Add(resultPlace);
            Canvas.SetLeft(resultPlace, 200 + i * 15);
            Canvas.SetBottom(resultPlace, 30 + j * 30);
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
        private void CreateColorCaps(int color)
        {
            Path colorCap = new Path();
            colorCap.Tag = color.ToString();
            colorCap.Data = Geometry.Parse(capData);
            colorCap.Fill = capBrushes[color];
            colorCap.MouseLeftButtonDown += ColorCap_MouseLeftButtonDown;
            board.Children.Add(colorCap);
            Canvas.SetRight(colorCap, 45);
            Canvas.SetBottom(colorCap, 100 + 30 * color);
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
            freePlaces = gbState.MoveActualField(color);
            if (!freePlaces)
            {
                MessageBox.Show("Prohrál jsi...");
                for (int j = 0; j < GameBoardState.rowsCount; j++)
                {
                    for (int i = 0; i < GameBoardState.fieldsCount; i++)
                    {
                        caps[i,j].Fill = Brushes.Transparent;
                    }
                }
            }
        }
    }
}
