using System.Collections.Generic;
using System;
using System.Linq;

public class Smoother
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
        float smoothedValue = buffer.Average();
        return smoothedValue;
    }
}


