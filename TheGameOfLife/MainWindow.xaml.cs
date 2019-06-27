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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TheGameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[,] grid;
        int screenWidth;
        int gridWidth;
        int cellWidth;
        long frames;
        WriteableBitmap canvas;


        public MainWindow()
        {
            InitializeComponent();
            gridWidth = 100;
            InitializeGrid();
        }

        void InitializeGrid()
        {
            var rnd = new Random();
            grid = new byte[gridWidth, gridWidth];
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    grid[i, j] = (byte)rnd.Next(2); // 0 or 1
                }
            }
        }

        private void Screen_Loaded(object sender, RoutedEventArgs e)
        {
            screenWidth = Convert.ToInt32(screen.Width);
            cellWidth = screenWidth / gridWidth;
            canvas = BitmapFactory.New(screenWidth, screenWidth);
            screen.Source = canvas;
            canvas.FillRectangle(0, 0, screenWidth, screenWidth, Colors.Black);
            CompositionTarget.Rendering += UpdateScreen;
        }

        void UpdateScreen(object sender, EventArgs e)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if (grid[i, j] == 1)
                    {
                        canvas.FillRectangle(i * cellWidth, j * cellWidth, i * cellWidth + cellWidth, j * cellWidth + cellWidth, Colors.White);
                        canvas.DrawRectangle(i * cellWidth, j * cellWidth, i * cellWidth + cellWidth, j * cellWidth + cellWidth, Colors.Black);
                    }
                    else
                    {
                        canvas.FillRectangle(i * cellWidth, j * cellWidth, i * cellWidth + cellWidth, j * cellWidth + cellWidth, Colors.Black);
                    }
                }
            }
            Monitoring();
            ComputeNextGeneration();
        }
        void ComputeNextGeneration()
        {
            byte[,] nextGrid = new byte[gridWidth, gridWidth];
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    byte state = grid[i, j];
                    // Count neighbors
                    int neighbors = 0;
                    for (int deltaI = -1; deltaI <= 1; deltaI++)
                    {
                        for (int deltaJ = -1; deltaJ <= 1; deltaJ++)
                        {
                            neighbors += grid[(i + deltaI + gridWidth) % gridWidth,
                                              (j + deltaJ + gridWidth) % gridWidth];
                        }
                    }
                    neighbors -= state; // the current cell can't be the neighbor of itself

                    // Computing the cell in nextGrid
                    if (state == 0 && neighbors == 3)
                    {
                        nextGrid[i, j] = 1;
                    }
                    else if (state == 1 && neighbors < 2 || neighbors > 3)
                    {
                        nextGrid[i, j] = 0;
                    }
                    else
                    {
                        nextGrid[i, j] = state;
                    }
                }
            }
            grid = nextGrid;
        }

        void Monitoring()
        {
            frames++;
            int liveCells = 0;
            int deadCells;
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    liveCells += grid[i, j];
                }
            }
            deadCells = grid.Length - liveCells;
            framesLabel.Content = $"Frames: {frames}";
            liveCellsLabel.Content = $"Live cells: {liveCells}";
            deadCellsLabel.Content = $"Dead cells: {deadCells}";
        }
    }
}
