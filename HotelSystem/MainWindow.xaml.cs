using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.Windows.Controls.Primitives;


namespace HotelSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RoomType[] roomTypes = new RoomType[]
        {
            new RoomType("одноместный", 3000, 12),
            new RoomType("двухместный", 4000, 8),
            new RoomType("полулюкс", 6000, 5),
            new RoomType("люкс", 7000, 3)
        };

        private Model model = new Model();

        private BackgroundWorker backgroundWorker;

        private Popup occupancyPercentagePopup;
        private TextBlock popupText;

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker = (BackgroundWorker)this.FindResource("BackgroundWorker");

            GridRoomTypes.ItemsSource = roomTypes;

            PeriodTextBox.DataContext = model;
            Statistics.DataContext = model;

            StackPanel popupStackPanel = new StackPanel();
            occupancyPercentagePopup = new Popup()
            {
                StaysOpen = false,
                PlacementTarget = this,
                Placement = PlacementMode.Center,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Child = new Border()
                {
                    Background = Brushes.WhiteSmoke,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Margin = new Thickness(0,0,4,4),
                    Effect = new DropShadowEffect() { Opacity = 0.5 },
                    Padding = new Thickness(20),
                    Child = popupStackPanel
                }
            };

            TextBlock tb = new TextBlock()
            {
                Text = "Процент загруженности номеров",
                FontSize = 14,
                TextDecorations = TextDecorations.Underline
            };
            popupStackPanel.Children.Add(tb);

            popupText = new TextBlock() { FontSize = 14 };
            popupStackPanel.Children.Add(popupText);
        }

        private bool IsValid(DependencyObject obj)
        {
            if (Validation.GetHasError(obj))
            {
                return false;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (!IsValid(child))
                {
                    return false;
                }
            }

            return true;
        }

        private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (GridRoomTypes != null && PeriodTextBox != null)
            {
                e.CanExecute = !backgroundWorker.IsBusy &&
                    !Validation.GetHasError(PeriodTextBox) && IsValid(GridRoomTypes);
            }
        }

        private void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PeriodTextBox.IsReadOnly = true;
            CostColumn.IsReadOnly = true;
            NumberColumn.IsReadOnly = true;

            StopButton.IsEnabled = true;

            DayTextBlock.Text = "1";

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            model.Start(roomTypes, backgroundWorker);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string occupancy = "";
            for (int i = 0; i < model.OccupancyPercentage.Length; i++)
            {
                occupancy += "\n" + roomTypes[i].Name + ":  " +
                    Math.Ceiling(model.OccupancyPercentage[i]) + "%";
            }
            popupText.Text = occupancy;

            occupancyPercentagePopup.IsOpen = true;

            PeriodTextBox.IsReadOnly = false;
            CostColumn.IsReadOnly = false;
            NumberColumn.IsReadOnly = false;

            StopButton.IsEnabled = false;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DayTextBlock.Text = e.ProgressPercentage.ToString();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
    }
}
