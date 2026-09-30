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
        private List<int> nums = [];
        private List<Button> pressed = [];
        private int clicked = 0;

        public MainWindow()
        {
            InitializeComponent();
            LBox_size.ItemsSource = size;
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (LBox_size.SelectedItem == null) return;
            int selected = (int)char.GetNumericValue(LBox_size.SelectedItem.ToString()![0]);
            Grid_Main.Children.Clear();


            for (int i = 0; i < (selected * selected) / 2; i++)
            {
                nums.Add(i);
                nums.Add(i);
            }
            nums = [.. nums.Shuffle()];

            Make_Grid(selected);
        }

        private void Make_Grid(int selected)
        {
            for (int i = 0; i < selected; i++)
            {
                Grid_Main.RowDefinitions.Add(new RowDefinition());
                Grid_Main.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int index = 0;
            for (int i = 0; i < selected; i++) // sorok
            {
                for (int j = 0; j < selected; j++) // oszlopok
                {
                    Button btn = new Button
                    {
                        Content = "?",
                        FontSize = 40,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(3),
                        DataContext = nums[index++]
                    };

                    btn.Background = Brushes.Azure;
                    btn.Foreground = Brushes.BlueViolet;
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);

                    btn.Click += Button_Click;
                    Grid_Main.Children.Add(btn);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (pressed.Contains(button)) return;
            clicked++;

            if (clicked >= 3) { // should be 2 if fixed
                clicked = 0;
                Clear_Buttons();
            }
            else
            {
                pressed.Add(button);
                button.Content = button.DataContext;
            }

        }

        private void Clear_Buttons()
        {
            if ((int)pressed[0].DataContext != (int)pressed[1].DataContext)
            {
                foreach (var btn in pressed)
                {
                    btn.Content = "?";
                }
            }
            else 
            { 
                foreach(var btn in pressed)
                {
                    btn.Click -= Button_Click;
                    btn.Foreground = Brushes.Green;
                }
            }
            pressed.Clear();
        }
    }
}