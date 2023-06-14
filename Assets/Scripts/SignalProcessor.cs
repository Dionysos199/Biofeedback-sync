using System.Collections.Generic;
using System;

public class SignalProcessor
{
    private bool _autoRange;
    private float _lowerLimit;
    private float _upperLimit;
    private int _bufferSize;
    private Queue<float> _buffer;

    public SignalProcessor(int bufferSize, bool autoRange = true)
    {
        // Configure buffer
       _bufferSize = bufferSize;
        _buffer = new Queue<float>();

        // Configure auto-ranging
        _autoRange = autoRange;
        ResetAutoRange(0);
    }

    // Method overload for int values
    public void ResetAutoRange(int value)
    {
        ResetAutoRange((float)value);
    }

    public void ResetAutoRange(float value)
    {
        _lowerLimit = _upperLimit = value;
    }

    // Method overload for int values
    public void AddValue(int value)
    {
        AddValue((float)value);
    }

    public void AddValue(float value)
    {
        // Adjust range automatically
        if(_autoRange)
        {
            while(value < _lowerLimit)
                lowerLimit--;

            while(value > _upperLimit)
                upperLimit++;
        }

        // Add value to queue
        _buffer.Enqueue(value);
        if (_buffer.Count > _bufferSize)
            _buffer.Dequeue();
    }

    public float GetNormalized()
    {
        var value = GetSmoothed();
        
        if (_lowerLimit <= value && value <= _upperLimit)
        {
            return (value - _lowerLimit) / (_upperLimit - _lowerLimit);
        }
        else
        {
            Debug.LogError("Sensor reading out of range");
            return 0;
        }
    }

    private float GetSmoothed()
    {
        float sum = 0;
        foreach (var value in buffer)
            sum += value;

        return sum / buffer.Count;
    }
}
