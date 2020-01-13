    using System;

public class Program
{
    public static void Main()
    {
        var arrayOutOfOrder = new int[] { 19,2,43,15,1,5,23,4,9 };
        QuickSort(arrayOutOfOrder, 0, 8);
        for(var i = 0; i <= 8; i++)
        {
            Console.WriteLine(arrayOutOfOrder[i]);
        }
    }

    public static void QuickSort(int[] array, int low, int high)
    {
        if(low < high)
        {
            var partitionIndex = partition(array, low, high);
            QuickSort(array, low, partitionIndex - 1);
            QuickSort(array, partitionIndex + 1, high); 
        }
    }

    private static int partition(int[] array, int low, int high)
    {
        var pivot = array[high];
        
        var i = (low - 1);
        for(var j = low; j <= high - 1; j++)
        {
            if(array[j] < pivot)
            {
                i++;
                swapValues(array, i, j);
            }
        }
        swapValues(array, i + 1, high);
        return (i + 1);
    }

    private static void swapValues(int[] array, int leftIndex, int rightIndex)
    {
        var temp = array[leftIndex];
        array[leftIndex] = array[rightIndex];
        array[rightIndex] = temp;
    }
}