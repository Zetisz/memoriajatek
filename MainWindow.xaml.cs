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
    private readonly string[] size = ["2 x 2", "4 x 4", "6 x 6"];
    private readonly string[] type = ["number", "emoji", "color"];
    private readonly string[] emojis = ["😀", "😂", "😍", "😎", "🤔", "😴", "😡", "🥳", "😇", "🤩", "🥰", "😘", "😜", "🤪", "🤑", "🤗", "🙄", "😏", "😢", "😭", "😱", "😤", "🤬", "🤯", "😳", "🥺", "😶", "😐", "😑", "🙃", "😅"];
    private readonly SolidColorBrush[] colors = [Brushes.DarkSeaGreen, Brushes.IndianRed, Brushes.BurlyWood, Brushes.CornflowerBlue, Brushes.MediumPurple, Brushes.Orange, Brushes.PaleVioletRed, Brushes.DarkCyan, Brushes.Gold, Brushes.SlateBlue, Brushes.OliveDrab, Brushes.Tomato, Brushes.SteelBlue, Brushes.MediumAquamarine, Brushes.Plum, Brushes.Peru, Brushes.Teal, Brushes.SandyBrown];
    private List<Button> pressed = [];
    private int tries, found, selected;
    private int leastTries = int.MaxValue;
    private bool color, isChecking;

    public MainWindow()
    {
        InitializeComponent();
        LBoxSize.ItemsSource = size;
        LBoxType.ItemsSource = type;
    }

    private void Btn_Start_Click(object sender, RoutedEventArgs e)
    {
        if (LBoxSize.SelectedItem == null || LBoxType.SelectedItem == null) return;
        var data = new List<string>();
        var buttonColors = new List<SolidColorBrush>();
        selected = int.Parse(LBoxSize.SelectedItem.ToString()!.Split(' ')[0]); // selected grid size
        GridGame.Children.Clear();
        GridGame.RowDefinitions.Clear(); // clear grid
        GridGame.ColumnDefinitions.Clear();
        tries = 0;
        found = 0;
        color = false;
        
        var pairCount = selected * selected / 2;

        switch (LBoxType.SelectedItem.ToString()) // selected mode
        {
            case "number":
            {
                for (int i = 0; i < pairCount; i++)
                {
                    data.Add(i.ToString());
                    data.Add(i.ToString());
                }

                break;
            }
            case "emoji":
            {
                for (int i = 0; i < pairCount; i++)
                {
                    data.Add(emojis[i]);
                    data.Add(emojis[i]);
                }

                break;
            }
            case "color":
            {
                color = true;
                for (int i = 0; i < pairCount; i++)
                {
                    buttonColors.Add(colors[i]);
                    buttonColors.Add(colors[i]);
                }

                buttonColors = [.. buttonColors.Shuffle()]; // randomize colors
                
                break;
            }
        }
        if (!color) data = [.. data.Shuffle()]; // randomize list
        Make_Grid(selected, data, buttonColors);
    }

    private void Make_Grid(int maxSize, List<string> data , List<SolidColorBrush> buttonColors)
    {
        for (int i = 0; i < maxSize; i++)
        {
            GridGame.RowDefinitions.Add(new RowDefinition());
            GridGame.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int i = 0; i < maxSize; i++) // rows
        {
            for (int j = 0; j < maxSize; j++) // columns
            {
                var index = i * maxSize + j;
                var btn = new Button(); // create buttons
                if (color)
                {
                    btn.Tag = buttonColors[index];
                }
                else
                {
                    btn.Content = "?";
                    btn.FontSize = 60;
                    btn.Tag = data[index];
                    btn.FontWeight = FontWeights.Bold;
                    
                    btn.Foreground = Brushes.DarkSlateBlue;
                }
                
                btn.Background = Brushes.Azure;
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
        var button = (Button)sender;
        if (pressed.Contains(button) || pressed.Count >= 2 || isChecking) return;
        pressed.Add(button);
        
        if (color)
            button.Background = (SolidColorBrush)button.Tag;
        else
            button.Content = button.Tag;

        if (pressed.Count < 2) return;

        isChecking = true;
        tries++;
        LblScore.Content = tries;
        
        await Task.Delay(500); // delay before checking
        Check();
        isChecking = false;
    }

    private void Check() // checks if the correct button was pressed
    {
        if (pressed[0].Tag == pressed[1].Tag)
        {
            foreach (var btn in pressed)
            {
                if (color)
                {
                    btn.Click -= Button_Click;
                }
                else
                {
                    btn.IsEnabled = false;
                    btn.Foreground = Brushes.DarkSeaGreen;
                }
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