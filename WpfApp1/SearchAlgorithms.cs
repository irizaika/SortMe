namespace WpfApp1;

public class SearchAlgorithms
{

    // Linear Search (returns index or -1)
    private int LinearSearch(int[] data, int value)
    {
        for (int i = 0; i < data.Length; i++)
            if (data[i] == value)
                return i;
        return -1;
    }

    // Binary Search (returns index or -1, assumes sorted)
    private int BinarySearch(int[] data, int value)
    {
        int left = 0, right = data.Length - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (data[mid] == value)
                return mid;
            else if (data[mid] < value)
                left = mid + 1;
            else
                right = mid - 1;
        }
        return -1;
    }
}