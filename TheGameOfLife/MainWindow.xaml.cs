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
        bool[,] grid;
        int screenWidth;
        int gridWidth;
        int cellWidth;
        WriteableBitmap writableBmp;


        public MainWindow()
        {
            gridWidth = 10;
            InitializeGrid();
            InitializeComponent();
        }

        void InitializeGrid()
        {
            var rnd = new Random();
            grid = new bool[gridWidth, gridWidth];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    grid[x, j] = rnd.Next(0, 2) == 1;
                }
            }
        }

        // !!! Нужно вот это дописать. Это основная логика программы P.S и она почему-то не работает
        void ComputeNextGeneration()
        {
            bool[,] nextGrid = new bool[gridWidth, gridWidth];
            int cellNeighbors = 0;

            for (int x = 0; x < gridWidth; x++)
            {
                if (x == 0 || x == gridWidth - 1) continue;
                for (int y = 0; y < gridWidth; y++)
                {
                    if (y == 0 || y == gridWidth - 1) continue;
                    // count neighbors
                    for (int deltaX = -1; deltaX <= 1; deltaX++)
                    {
                        for (int deltaY = -1; deltaY <= 1; deltaY++)
                        {
                            cellNeighbors += grid[x + deltaX, y + deltaY] ? 1 : 0;
                        }
                    }
                    cellNeighbors--;

                    // A dead cell with exactly three live neighbours becomes a live cell, as if by reproduction
                    nextGrid[x, y] = !grid[x, y] && cellNeighbors == 3;
                    // A live cell with fewer than two live or with more then tree live neighbors dies, as if by underpopulation or by overpopulation respectively
                    nextGrid[x, y] = grid[x, y] && (cellNeighbors < 2 || cellNeighbors > 3);
                }
            }
            //for (int x = 0; x < gridWidth; x++)
            //{
            //    for (int j = 0; j < gridWidth; j++)
            //    {
            //        grid[x, j] = nextGrid[x, j];
            //    }
            //}
            grid = nextGrid;
        }

        // Prepare WritableBitmap
        private void Screen_Loaded(object sender, RoutedEventArgs e)
        {
            screenWidth = (int)screen.Width;
            cellWidth = screenWidth / gridWidth;
            writableBmp = BitmapFactory.New(screenWidth, screenWidth);
            screen.Source = writableBmp;
            writableBmp.FillRectangle(0, 0, screenWidth, screenWidth, Colors.White);
            CompositionTarget.Rendering += CompositionTarget_Rendering;

        }

        // Screen update rendering
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridWidth; y++)
                {
                    if (grid[x, y])
                    {
                        writableBmp.FillRectangle(x * cellWidth, y * cellWidth, x * cellWidth + cellWidth, y * cellWidth + cellWidth, Colors.Black);
                    }
                    else
                    {
                        writableBmp.FillRectangle(x * cellWidth, y * cellWidth, x * cellWidth + cellWidth, y * cellWidth + cellWidth, Colors.White);
                    }
                }
            }
            ComputeNextGeneration();
        }
    }
}
