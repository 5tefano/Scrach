using System;

public class Program
{
    public static void Main()
    {
        var array = new int[] { 38, 27, 43, 3, 9, 82, 10 };
        printArray(array);  
        MergeSort(array, 0, array.Length - 1);
        printArray(array);
    }

    public static void MergeSort(int[] array, int leftIndex, int rightIndex)
    {
        if (leftIndex < rightIndex)       
        {
            var midPoint = (leftIndex+rightIndex)/2;
            MergeSort(array, leftIndex, midPoint);
            MergeSort(array, midPoint+1, rightIndex);            
            Merge(array, leftIndex, midPoint, rightIndex);
        }
    }

    public static void Merge(int[] array, int leftIndex, int midPoint, int rightIndex)
    {
        var leftArraySize = midPoint-leftIndex + 1;
        var rightArraySize = rightIndex - midPoint;
        var leftArray = new int[leftArraySize];
        var rightArray = new int[rightArraySize];
        int i, j, k;
        
        for(i = 0; i < leftArray.Length; i++)
        {
            leftArray[i] = array[leftIndex + i];
        }

        for (i = 0; i < rightArraySize; i++)
        {
            rightArray[i] = array[midPoint + 1 + i];
        }       

        i = 0;
        j = 0;
        k = leftIndex;
        
        while(i < leftArraySize && j < rightArraySize)
        {            
            if(leftArray[i] < rightArray[j])
            {
                array[k] = leftArray[i];
                i++;
            }
            else
            {
                array[k] = rightArray[j];
                j++;
            }
            k++;
        }

        while (i < leftArraySize)
        {
            array[k] = leftArray[i];
            i++;
            k++;
        }

        while(j < rightArraySize)
        {
            array[k] = rightArray[j];
            j++;
            k++;
        }
    }

    public static void printArray(int[] array)
    {
        Console.Write("{ ");
        for(var i = 0; i < array.Length; i++)
        {
            Console.Write(array[i]);
            if(i < array.Length - 1)
            {
                Console.Write(", ");
            }
        }
        Console.Write(" }\n");
    }
}