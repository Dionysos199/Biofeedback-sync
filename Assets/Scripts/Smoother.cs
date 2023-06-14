using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoother : MonoBehaviour
{
    private int bufferSize;
    private Queue<float> buffer;

    public Smoother(int bufferSize)
    {
        this.bufferSize = bufferSize;
        buffer = new Queue<float>(bufferSize);
    }

    public float SmoothValue(float rawValue)
    {
        buffer.Enqueue(rawValue);

        if (buffer.Count > bufferSize)
        {
            buffer.Dequeue();
        }

        float sum = 0;
        foreach (var value in buffer)
        {
            sum += value;
        }

        float smoothedValue = sum / buffer.Count;
        return smoothedValue;
    }
}
