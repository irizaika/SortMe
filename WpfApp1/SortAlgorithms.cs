using System.Windows.Controls;
using System.Windows;

namespace WpfApp1;

public static class SortAlgorithms
{
    public static async Task BubbleSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        int n = data.Length;
        double barWidth = canvas.ActualWidth / numElements;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                token.ThrowIfCancellationRequested();
                if (data[j] > data[j + 1])
                {
                    (data[j], data[j + 1]) = (data[j + 1], data[j]);
                    (bars[j], bars[j + 1]) = (bars[j + 1], bars[j]);
                    Canvas.SetLeft(bars[j], j * barWidth);
                    Canvas.SetLeft(bars[j + 1], (j + 1) * barWidth);
                    await Task.Delay(delay, token);
                }
            }
        }
    }

    public static async Task SelectionSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        int n = data.Length;
        double barWidth = canvas.ActualWidth / numElements;
        for (int i = 0; i < n - 1; i++)
        {
            int minIdx = i;
            for (int j = i + 1; j < n; j++)
            {
                token.ThrowIfCancellationRequested();
                if (data[j] < data[minIdx])
                    minIdx = j;
            }
            if (minIdx != i)
            {
                (data[i], data[minIdx]) = (data[minIdx], data[i]);
                (bars[i], bars[minIdx]) = (bars[minIdx], bars[i]);
                Canvas.SetLeft(bars[i], i * barWidth);
                Canvas.SetLeft(bars[minIdx], minIdx * barWidth);
                await Task.Delay(delay, token);
            }
        }
    }

    public static async Task InsertionSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        int n = data.Length;
        double barWidth = canvas.ActualWidth / numElements;
        for (int i = 1; i < n; i++)
        {
            int key = data[i];
            UIElement keyBar = bars[i];
            int j = i - 1;
            while (j >= 0 && data[j] > key)
            {
                token.ThrowIfCancellationRequested();
                data[j + 1] = data[j];
                bars[j + 1] = bars[j];
                Canvas.SetLeft(bars[j + 1], (j + 1) * barWidth);
                j--;
                await Task.Delay(delay, token);
            }
            token.ThrowIfCancellationRequested();
            data[j + 1] = key;
            bars[j + 1] = keyBar;
            Canvas.SetLeft(bars[j + 1], (j + 1) * barWidth);
            await Task.Delay(delay, token);
        }
    }

    public static async Task QuickSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        double barWidth = canvas.ActualWidth / numElements;
        async Task Sort(int left, int right)
        {
            if (left < right)
            {
                int pivotIdx = await Partition(left, right);
                await Sort(left, pivotIdx - 1);
                await Sort(pivotIdx + 1, right);
            }
        }
        async Task<int> Partition(int left, int right)
        {
            int pivot = data[right];
            UIElement pivotBar = bars[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                token.ThrowIfCancellationRequested();
                if (data[j] < pivot)
                {
                    i++;
                    (data[i], data[j]) = (data[j], data[i]);
                    (bars[i], bars[j]) = (bars[j], bars[i]);
                    Canvas.SetLeft(bars[i], i * barWidth);
                    Canvas.SetLeft(bars[j], j * barWidth);
                    await Task.Delay(delay, token);
                }
            }
            token.ThrowIfCancellationRequested();
            (data[i + 1], data[right]) = (data[right], data[i + 1]);
            (bars[i + 1], bars[right]) = (bars[right], bars[i + 1]);
            Canvas.SetLeft(bars[i + 1], (i + 1) * barWidth);
            Canvas.SetLeft(bars[right], right * barWidth);
            await Task.Delay(delay, token);
            return i + 1;
        }
        await Sort(0, data.Length - 1);
    }

    public static async Task CountingSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        int n = data.Length;
        int min = data.Min();
        int max = data.Max();
        int range = max - min + 1;
        int[] count = new int[range];
        int[] output = new int[n];
        UIElement[] outputBars = new UIElement[n];
        double barWidth = canvas.ActualWidth / numElements;

        for (int i = 0; i < n; i++)
            count[data[i] - min]++;

        for (int i = 1; i < range; i++)
            count[i] += count[i - 1];

        for (int i = n - 1; i >= 0; i--)
        {
            int idx = data[i] - min;
            output[count[idx] - 1] = data[i];
            outputBars[count[idx] - 1] = bars[i];
            count[idx]--;
        }

        for (int i = 0; i < n; i++)
        {
            token.ThrowIfCancellationRequested();
            data[i] = output[i];
            bars[i] = outputBars[i];
            Canvas.SetLeft(bars[i], i * barWidth);
            await Task.Delay(delay, token);
        }
    }

    public static async Task RadixSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        int n = data.Length;
        double barWidth = canvas.ActualWidth / numElements;
        int max = data.Max();

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            int[] output = new int[n];
            UIElement[] outputBars = new UIElement[n];
            int[] count = new int[10];

            for (int i = 0; i < n; i++)
                count[(data[i] / exp) % 10]++;

            for (int i = 1; i < 10; i++)
                count[i] += count[i - 1];

            for (int i = n - 1; i >= 0; i--)
            {
                int idx = (data[i] / exp) % 10;
                output[count[idx] - 1] = data[i];
                outputBars[count[idx] - 1] = bars[i];
                count[idx]--;
            }

            for (int i = 0; i < n; i++)
            {
                token.ThrowIfCancellationRequested();
                data[i] = output[i];
                bars[i] = outputBars[i];
                Canvas.SetLeft(bars[i], i * barWidth);
                await Task.Delay(delay, token);
            }
        }
    }

    public static async Task MergeSort(int[] data, UIElement[] bars, Canvas canvas, int delay, int numElements, CancellationToken token)
    {
        double barWidth = canvas.ActualWidth / numElements;
        async Task Sort(int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                await Sort(left, mid);
                await Sort(mid + 1, right);
                await Merge(left, mid, right);
            }
        }
        async Task Merge(int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;
            int[] L = new int[n1];
            int[] R = new int[n2];
            UIElement[] Lbars = new UIElement[n1];
            UIElement[] Rbars = new UIElement[n2];

            for (int i = 0; i < n1; i++)
            {
                L[i] = data[left + i];
                Lbars[i] = bars[left + i];
            }
            for (int j = 0; j < n2; j++)
            {
                R[j] = data[mid + 1 + j];
                Rbars[j] = bars[mid + 1 + j];
            }

            int i1 = 0, j1 = 0, k = left;
            while (i1 < n1 && j1 < n2)
            {
                token.ThrowIfCancellationRequested();
                if (L[i1] <= R[j1])
                {
                    data[k] = L[i1];
                    bars[k] = Lbars[i1];
                    i1++;
                }
                else
                {
                    data[k] = R[j1];
                    bars[k] = Rbars[j1];
                    j1++;
                }
                Canvas.SetLeft(bars[k], k * barWidth);
                await Task.Delay(delay, token);
                k++;
            }
            while (i1 < n1)
            {
                token.ThrowIfCancellationRequested();
                data[k] = L[i1];
                bars[k] = Lbars[i1];
                Canvas.SetLeft(bars[k], k * barWidth);
                await Task.Delay(delay, token);
                i1++; k++;
            }
            while (j1 < n2)
            {
                token.ThrowIfCancellationRequested();
                data[k] = R[j1];
                bars[k] = Rbars[j1];
                Canvas.SetLeft(bars[k], k * barWidth);
                await Task.Delay(delay, token);
                j1++; k++;
            }
        }
        await Sort(0, data.Length - 1);
    }
}