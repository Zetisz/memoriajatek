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

namespace memoriajatek;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<string> size = ["2 x 2", "4 x 4", "6 x 6"];
    private List<string> type = ["number", "emoji", "color"];
    private List<string> emojis = ["😀", "😂", "😍", "😎", "🤔", "😴", "😡", "🥳", "😇", "🤩", "🥰", "😘", "😜", "🤪", "🤑", "🤗", "🙄", "😏", "😢", "😭", "😱", "😤", "🤬", "🤯", "😳", "🥺", "😶", "😐", "😑", "🙃", "😅", "😂"];
    private List<SolidColorBrush> colors = [Brushes.DarkSeaGreen, Brushes.IndianRed, Brushes.BurlyWood, Brushes.CornflowerBlue, Brushes.MediumPurple, Brushes.Orange, Brushes.PaleVioletRed, Brushes.DarkCyan, Brushes.Gold, Brushes.SlateBlue, Brushes.OliveDrab, Brushes.Tomato, Brushes.SteelBlue, Brushes.MediumAquamarine, Brushes.Plum, Brushes.Peru, Brushes.Teal, Brushes.SandyBrown];
    private List<string> data = [];
    private List<SolidColorBrush> buttonColors = [];
    private List<Button> pressed = [];
    private int clicked, tries, found, selected;
    private int leastTries = 1000;
    private bool color;

    public MainWindow()
    {
        InitializeComponent();
        LBoxSize.ItemsSource = size;
        LBoxType.ItemsSource = type;
    }

    private void Btn_Start_Click(object sender, RoutedEventArgs e)
    {
        if (LBoxSize.SelectedItem == null || LBoxType.SelectedItem == null) return;
        selected = (int)char.GetNumericValue(LBoxSize.SelectedItem.ToString()![0]); // selected grid size
        GridGame.Children.Clear();
        GridGame.RowDefinitions.Clear(); // clear grid
        GridGame.ColumnDefinitions.Clear();
        data.Clear();
        buttonColors.Clear();
        tries = 0;
        found = 0;
        color = false;

        switch (LBoxType.SelectedItem.ToString()) // selected mode
        {
            case "number":
            {
                for (int i = 0; i < (selected * selected) / 2; i++)
                {
                    data.Add(i.ToString());
                    data.Add(i.ToString());
                }

                break;
            }
            case "emoji":
            {
                for (int i = 0; i < (selected * selected) / 2; i++)
                {
                    data.Add(emojis[i]);
                    data.Add(emojis[i]);
                }

                break;
            }
            case "color":
            {
                color = true;
                for (int i = 0; i < (selected * selected) / 2; i++)
                {
                    buttonColors.Add(colors[i]);
                    buttonColors.Add(colors[i]);
                }

                buttonColors = [.. buttonColors.Shuffle()]; // randomize colors
                
                break;
            }
        }
        if (!color) data = [.. data.Shuffle()]; // randomize list
        Make_Grid(selected);
    }

    private void Make_Grid(int maxSize)
    {
        for (int i = 0; i < maxSize; i++)
        {
            GridGame.RowDefinitions.Add(new RowDefinition());
            GridGame.ColumnDefinitions.Add(new ColumnDefinition());
        }
        int index = 0;
        for (int i = 0; i < maxSize; i++) // rows
        {
            for (int j = 0; j < maxSize; j++) // columns
            {
                Button btn = new Button(); // create buttons
                if (color)
                {
                    btn.Background = Brushes.Azure;
                    btn.DataContext = buttonColors[index++];
                }
                else
                {
                    btn.Content = "?";
                    btn.FontSize = 60;
                    btn.DataContext = data[index++];
                    btn.FontWeight = FontWeights.Bold;
                    btn.Background = Brushes.Azure;
                    btn.Foreground = Brushes.DarkSlateBlue;
                }
                
                btn.Margin = new Thickness(3);
                Grid.SetRow(btn, i);
                Grid.SetColumn(btn, j);

                btn.Click += Button_Click;
                GridGame.Children.Add(btn);
            }
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        if (pressed.Contains(button) || pressed.Count >= 2) return;
        pressed.Add(button);
        if (color)
        {
            button.Background = (SolidColorBrush)button.DataContext;
            await Task.Delay(300);
        }
        else
        {
            button.Content = button.DataContext;
        }
        clicked++;

        if (clicked < 2) return;
        tries++;
        LblScore.Content = tries;

        clicked = 0;
        await Task.Delay(500); // delay before checking
        Check();
    }

    private void Check() // checks if the correct button was pressed
    {
        if (pressed[0].DataContext == pressed[1].DataContext)
        {
            foreach (var btn in pressed)
            {
                btn.Click -= Button_Click;
                if (!color) btn.Foreground = Brushes.DarkSeaGreen;
            }
            found++;
            if (found >= (selected * selected) / 2) // game over
            {
                GridGame.Children.Clear();
                if (tries < leastTries)
                {
                    leastTries = tries;
                    LblLeastTries.Content = "Least tries: " + leastTries;
                }
            }
        }
        else // sets buttons back to original state
        {
            foreach (var btn in pressed)
            {
                if (color)
                {
                    btn.Background =  Brushes.Azure;
                }
                else
                {
                    btn.Content = "?";
                }
            }
        }
        pressed.Clear();
    }
}