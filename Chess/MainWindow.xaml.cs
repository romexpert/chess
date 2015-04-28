using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
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
using Chess.Properties;
using Common;
using Brush = System.Drawing.Brush;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const byte GridSize = 8;

        private Chessman _draggedFeagure = null;
        private Border _draggedFeagureParent = null;
        private bool _gameOver = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                for (var x = 0; x < GridSize; x++)
                {
                    for (var y = 0; y < GridSize; y++)
                    {
                        var rectangle = new Rectangle();

                        rectangle.Fill = x%2 == 0 && y%2 == 0 || x%2 == 1 && y%2 == 1
                            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 217, 181))
                            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(181, 136, 99));

                        Grid.SetRow(rectangle, y);
                        Grid.SetColumn(rectangle, x);

                        ChessGridBackground.Children.Add(rectangle);
                    }
                }

                for (var x = 0; x < GridSize; x++)
                {
                    for (var y = 0; y < GridSize; y++)
                    {
                        var border = new Border();

                        //border.Background = x % 2 == 0 && y % 2 == 0 || x % 2 == 1 && y % 2 == 1 ? Brushes.White : Brushes.Aquamarine;
                        border.Background = Brushes.Transparent;

                        border.AllowDrop = true;
                        border.DragEnter += RectangleOnDragEnter;
                        border.DragLeave += RectangleODragLeave;

                        border.Drop += RectangleOnDrop;

                        Grid.SetRow(border, y);
                        Grid.SetColumn(border, x);

                        var feagure = DefaultChessmen.Items.FirstOrDefault(_ => _.HasPosition(x, y));

                        if (feagure != null)
                        {
                            feagure.MouseLeftButtonDown += image_MouseLeftButtonDown;
                            border.Child = feagure;
                        }

                        ChessGrid.Children.Add(border);
                    }
                }

                Trace.TraceInformation("Start game");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }
        
        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_gameOver)
                    return;

                var chessman = (Chessman) sender;
                if (!chessman.IsTurn)
                    return;

                var data = new DataObject();
                data.SetData("data", 13);
                //data.SetData("image", (Image)sender);

                _draggedFeagure = chessman;
                _draggedFeagureParent = (Border) _draggedFeagure.Parent;
                _draggedFeagureParent.Child = null;

                Panel.SetZIndex(_draggedFeagure, 2);
                MainCanvas.Children.Add(_draggedFeagure);
                _draggedFeagureParent.Background = Brushes.SlateGray;
                _draggedFeagureParent.Opacity = 0.3;

                var result = DragDrop.DoDragDrop((Image) sender, data, DragDropEffects.Move);

                if (result == DragDropEffects.None)
                {
                    HandleFailedDrop();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }
        
        
        private void RectangleOnDragEnter(object sender, DragEventArgs e)
        {
            try
            {
                var border = ((Border) sender);

                var point = new System.Drawing.Point(Grid.GetColumn(border), Grid.GetRow(border));

                border.BorderBrush = _draggedFeagure.CanGo(point) ? Brushes.LawnGreen : Brushes.Red;
                border.BorderThickness = new Thickness(3);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }

        void RectangleODragLeave(object sender, DragEventArgs e)
        {
            try
            {
                ((Border)sender).BorderBrush = Brushes.Transparent;
                ((Border)sender).BorderThickness = new Thickness(0);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }

        private void MainCanvas_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                var position = e.GetPosition(ChessGrid);

                Canvas.SetLeft(_draggedFeagure, position.X - 40);
                Canvas.SetTop(_draggedFeagure, position.Y - 40);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }



        void RectangleOnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var border = ((Border) sender);
                border.BorderBrush = Brushes.Transparent;
                border.BorderThickness = new Thickness(0);

                var point = new System.Drawing.Point(Grid.GetColumn(border), Grid.GetRow(border));

                if (!_draggedFeagure.CanGo(point, true))
                {
                    HandleFailedDrop();

                    return;
                }

                MainCanvas.Children.Remove(_draggedFeagure);

                _draggedFeagureParent.Background = Brushes.Transparent;
                _draggedFeagureParent.Opacity = 1;

                try
                {
                    _draggedFeagure.Go(point);
                }
                catch (RequireExchangeException)
                {
                    var chooseFeagureDialog = new ChooseFeagure();
                    chooseFeagureDialog.Owner = this;

                    chooseFeagureDialog.ShowDialog();

                    _draggedFeagure.Mutate(chooseFeagureDialog.SelectedFeagure);
                }
                catch (FatalityException ex)
                {
                    MessageBox.Show(String.Format("{0} win.", ex.WinColor));

                    Trace.TraceInformation("{0} win. Game over.", ex.WinColor);
                    _gameOver = true;
                }

                border.Child = _draggedFeagure;

                _draggedFeagure = null;

                Game.Current.ChangeTurn();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                _gameOver = true;
            }
        }

        void HandleFailedDrop()
        {
            MainCanvas.Children.Remove(_draggedFeagure);
            _draggedFeagureParent.Background = Brushes.Transparent;
            _draggedFeagureParent.Opacity = 1;
            _draggedFeagureParent.Child = _draggedFeagure;
            _draggedFeagureParent = null;
            _draggedFeagure = null;
        }
    }
}