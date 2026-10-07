using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace memoriajatek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> size = ["2 x 2", "4 x 4", "6 x 6"];
        private List<string> type = ["number", "emoji", "text"];
        private List<string> emojis = ["😀", "😂", "😍", "😎", "🤔", "😴", "😡", "🥳", "😇", "🤩", "🥰", "😘", "😜", "🤪", "🤑", "🤗", "🙄", "😏", "😢", "😭", "😱", "😤", "🤬", "🤯", "😳", "🥺", "😶", "😐", "😑", "🙃", "😅", "😂"];
        private List<string> data = [];
        private List<Button> pressed = [];
        private int clicked, tries, found, selected;

        public MainWindow()
        {
            InitializeComponent();
            LBox_size.ItemsSource = size;
            LBox_type.ItemsSource = type;
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (LBox_size.SelectedItem == null || LBox_type.SelectedItem == null) return;
            selected = (int)char.GetNumericValue(LBox_size.SelectedItem.ToString()![0]);
            Grid_Game.Children.Clear();
            Grid_Game.RowDefinitions.Clear(); // clear grid
            Grid_Game.ColumnDefinitions.Clear();

            switch (LBox_type.SelectedItem.ToString())
            {
                case "number": // numbers
                {
                    for (int i = 0; i < (selected * selected) / 2; i++)
                    {
                        data.Add(i.ToString());
                        data.Add(i.ToString());
                    }

                    break;
                }
                case "emoji": // emojis
                {
                    for (int i = 0; i < (selected * selected) / 2; i++)
                    {
                        data.Add(emojis[i]);
                        data.Add(emojis[i]);
                    }

                    break;
                }
            }
            data = [.. data.Shuffle()]; // randomize list
            Make_Grid(selected);
        }

        private void Make_Grid(int maxSize)
        {
            for (int i = 0; i < maxSize; i++)
            {
                Grid_Game.RowDefinitions.Add(new RowDefinition());
                Grid_Game.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int index = 0;
            for (int i = 0; i < maxSize; i++) // rows
            {
                for (int j = 0; j < maxSize; j++) // columns
                {
                    Button btn = new Button // create buttons
                    {
                        Content = "?",
                        FontSize = 40,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(3),
                        DataContext = data[index++],
                        Background = Brushes.Azure,
                        Foreground = Brushes.BlueViolet
                    };

                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);

                    btn.Click += Button_Click;
                    Grid_Game.Children.Add(btn);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (pressed.Contains(button) || pressed.Count >= 2) return;
            pressed.Add(button);
            button.Content = button.DataContext;
            clicked++;

            if (clicked < 2) return;
            tries++;
            Lbl_score.Content = tries;

            clicked = 0;
            await Task.Delay(500);
            Check();
        }

        private void Check() // checks if the correct button was pressed
        {
            if (pressed[0].DataContext == pressed[1].DataContext) // sets color to green & deactivates button
            {
                foreach (var btn in pressed)
                {
                    btn.Click -= Button_Click;
                    btn.Foreground = Brushes.Green;
                }
                found++;
                if (found >= (selected * selected) / 2) // game over
                {
                    Grid_Game.Children.Clear();
                }
            }
            else // sets button back to original state
            {
                foreach (var btn in pressed)
                {
                    btn.Content = "?";
                }
            }
            pressed.Clear();
        }
    }
}