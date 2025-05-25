using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    // Remove or comment out the old constant
    // private const int NumElements = 300;

    private int NumElements => NumberOfBars;
    // Optionally, update BarWidth to be calculated dynamically if needed
    // private const int BarWidth = 2;
    private int BarWidth => (int)Math.Max(1, BubbleSortCanvas.ActualWidth / (double)NumElements);

    // Replace the problematic field initializations with lazy initialization in the constructor or a method.
    private int[] dataBubble;
    private int[] dataSelection;
    private int[] dataInsertion;
    private int[] dataQuick;
    private int[] dataRadix;
    private int[] dataMerge;
    private int[] dataCounting;

    private UIElement[] barsBubble;
    private UIElement[] barsSelection;
    private UIElement[] barsInsertion;
    private UIElement[] barsQuick;
    private UIElement[] barsRadix;
    private UIElement[] barsMerge;
    private UIElement[] barsCounting;

    private Random rnd = new Random();
    private Color color1;
    private Color color2;

    private int _delay = 10; // Default delay in milliseconds

    private CancellationTokenSource? _cts;
    public int Delay
    {
        get => _delay;
        set
        {
            if (_delay != value)
            {
                _delay = value;
                // Optionally notify property changed if using INotifyPropertyChanged
            }
        }
    }

    private int _numberOfBars = 200; // Default number of bars is now 200

    public int NumberOfBars
    {
        get => _numberOfBars;
        set
        {
            if (_numberOfBars != value)
            {
                _numberOfBars = value;

                ResetButton.IsEnabled = true;
                StartSortsButton.IsEnabled = true;
                // Re-initialize data and bars arrays with new size
                InitArrays();
                // Regenerate data and redraw bars
                GenerateData();
                DrawBarsOnAllCanvaces();
                // Optionally notify property changed if using INotifyPropertyChanged
            }
        }
    }

    private void DelayTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "0";
            }
        }
    }

    private int MinHeights = 100;
    private int MaxHeights = 200;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        InitArrays();

    }

    private /* async */void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ResetData();
    }

    private void InitArrays()
    {
        // Initialize arrays here, where NumElements is accessible
        dataBubble = new int[NumElements];
        dataSelection = new int[NumElements];
        dataInsertion = new int[NumElements];
        dataQuick = new int[NumElements];
        dataRadix = new int[NumElements];
        dataMerge = new int[NumElements];
        dataCounting = new int[NumElements];

        barsBubble = new UIElement[NumElements];
        barsSelection = new UIElement[NumElements];
        barsInsertion = new UIElement[NumElements];
        barsQuick = new UIElement[NumElements];
        barsRadix = new UIElement[NumElements];
        barsMerge = new UIElement[NumElements];
        barsCounting = new UIElement[NumElements];
    }

    private void GenerateData()
    {
        int[] original = new int[NumElements];
        for (int i = 0; i < NumElements; i++)
        {
            original[i] = rnd.Next(MinHeights, MaxHeights);
        }

        dataBubble = (int[])original.Clone();
        dataSelection = (int[])original.Clone();
        dataInsertion = (int[])original.Clone();
        dataQuick = (int[])original.Clone();
        dataRadix = (int[])original.Clone();
        dataMerge = (int[])original.Clone();
        dataCounting = (int[])original.Clone();

        color1 = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
        color2 = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));

        BubbleSortCanvas.Height = MaxHeights;
        SelectionSortCanvas.Height = MaxHeights;
        InsertionSortCanvas.Height = MaxHeights;
        QuickSortCanvas.Height = MaxHeights;
        RadixSortCanvas.Height = MaxHeights;
        MergeSortCanvas.Height = MaxHeights;
        CountingSortCanvas.Height = MaxHeights;
    }

    private void DrawBars(Canvas canvas, int[] data, ref UIElement[] bars)
    {
        canvas.Children.Clear();
        double barWidth = canvas.ActualWidth / NumElements;
        bars = new UIElement[NumElements];

        for (int i = 0; i < NumElements; i++)
        {
            var grid = new Grid();

            Rectangle rect = new Rectangle
            {
                Width = BarWidth,
                Height = data[i],
                Fill = GetColorFromHeight(data[i]),
                RadiusX = 2,
                RadiusY = 2,
                StrokeThickness = 0
            };
            Canvas.SetLeft(rect, 0);
            Canvas.SetBottom(rect, 0);
            grid.Children.Add(rect);

            Canvas.SetLeft(grid, i * barWidth);
            Canvas.SetBottom(grid, 0);
            canvas.Children.Add(grid);

            bars[i] = grid;
        }
    }

    private Brush GetColorFromHeight(int height)
    {
        double dif = (double)MaxHeights - (double)MinHeights;
        double t = (height - MinHeights) / dif; // since height is between MinHeights and MaxHeights

        byte r = (byte)(color1.R + (color2.R - color1.R) * t);
        byte g = (byte)(color1.G + (color2.G - color1.G) * t);
        byte b = (byte)(color1.B + (color2.B - color1.B) * t);

        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }


    private void SwapBars(int i, int j)
    {
        // Swap rectangles in array
        (barsBubble[i], barsBubble[j]) = (barsBubble[j], barsBubble[i]);

        double barWidth = BubbleSortCanvas.ActualWidth / NumElements;

        // Update positions
        Canvas.SetLeft(barsBubble[i], i * barWidth);
        Canvas.SetLeft(barsBubble[j], j * barWidth);
    }

    private async Task RunSorts(CancellationToken token)
    {
        var bubbleTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.BubbleSort(data, bars, canvas, Delay, NumElements, token), dataBubble, barsBubble, BubbleSortCanvas, BubbleSortTimeText, "Bubble Sort", token);
        var selectionTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.SelectionSort(data, bars, canvas, Delay, NumElements, token), dataSelection, barsSelection, SelectionSortCanvas, SelectionSortTimeText, "Selection Sort", token);
        var insertionTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.InsertionSort(data, bars, canvas, Delay, NumElements, token), dataInsertion, barsInsertion, InsertionSortCanvas, InsertionSortTimeText, "Insertion Sort", token);
        var quickTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.QuickSort(data, bars, canvas, Delay, NumElements, token), dataQuick, barsQuick, QuickSortCanvas, QuickSortTimeText, "Quick Sort", token);
        var radixTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.RadixSort(data, bars, canvas, Delay, NumElements, token), dataRadix, barsRadix, RadixSortCanvas, RadixSortTimeText, "Radix Sort", token);
        var mergeTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.MergeSort(data, bars, canvas, Delay, NumElements, token), dataMerge, barsMerge, MergeSortCanvas, MergeSortTimeText, "Merge Sort", token);
        var countingTask = RunSortWithTiming((data, bars, canvas, token) => SortAlgorithms.CountingSort(data, bars, canvas, Delay, NumElements, token), dataCounting, barsCounting, CountingSortCanvas, CountingSortTimeText, "Counting Sort", token);

        await Task.WhenAll(bubbleTask, selectionTask, insertionTask, quickTask, radixTask, mergeTask, countingTask);
    }

    private async Task RunSortWithTiming(
        Func<int[], UIElement[], Canvas, CancellationToken, Task> sortFunc,
        int[] data, UIElement[] bars, Canvas canvas, TextBlock timeText, string name,
        CancellationToken token)
    {
        timeText.Text = $"{name}";

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await sortFunc(data, bars, canvas, token);
        sw.Stop();
        var elapsed = sw.Elapsed;
        timeText.Text = $"{name} Time: {elapsed.TotalMilliseconds:F3} ms ({elapsed.TotalMicroseconds} μs)";

        var microseconds = sw.ElapsedTicks * 1_000_000.0 / System.Diagnostics.Stopwatch.Frequency;
        timeText.Text = $"{name} Time: {sw.ElapsedMilliseconds} ms ({microseconds:F0} μs)";
    }

    private void ResetTexts()
    {
        BubbleSortTimeText.Text = "Bubble Sort";
        SelectionSortTimeText.Text = "Selection Sort";
        InsertionSortTimeText.Text = "Insertion Sort";
        QuickSortTimeText.Text = "Quick Sort";
        RadixSortTimeText.Text = "Radix Sort";
        MergeSortTimeText.Text = "Merge Sort";
        CountingSortTimeText.Text = "Counting Sort";
    }
    private void DrawBarsOnAllCanvaces()
    {
        DrawBars(BubbleSortCanvas, dataBubble, ref barsBubble);
        DrawBars(SelectionSortCanvas, dataSelection, ref barsSelection);
        DrawBars(InsertionSortCanvas, dataInsertion, ref barsInsertion);
        DrawBars(QuickSortCanvas, dataQuick, ref barsQuick);
        DrawBars(RadixSortCanvas, dataRadix, ref barsRadix);
        DrawBars(MergeSortCanvas, dataMerge, ref barsMerge);
        DrawBars(CountingSortCanvas, dataCounting, ref barsCounting);
    }



    private void ResetData()
    {
        ResetButton.IsEnabled = false;

        ResetTexts();
        GenerateData();
        DrawBarsOnAllCanvaces();

        ResetButton.IsEnabled = true;
        StartSortsButton.IsEnabled = true;
    }

    //BUTTON CLICK HANDLERS

    private async void StartSortsButton_Click(object sender, RoutedEventArgs e)
    {
        StartSortsButton.IsEnabled = false;
        _cts = new CancellationTokenSource();
        try
        {
            await RunSorts(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Optionally update UI to show cancellation
        }
        finally
        {
            StartSortsButton.IsEnabled = true;
            _cts = null;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        // Cancel any running sorts
        _cts?.Cancel();
        ResetData();
    }
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ResetTexts();
        // Cancel any running sorts
        _cts?.Cancel();
    }
}