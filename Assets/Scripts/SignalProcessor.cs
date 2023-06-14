using System.Collections.Generic;
using UnityEngine;

public class SignalProcessor
{
    public enum Peak {
        None,
        Minimum,
        Maximum
    }

    private bool _autoRange;
    private float _lowerLimit;
    private float _upperLimit;
    
    private int _bufferSize;
    private Queue<float> _buffer;

    private float _lastValue = 0;
    private float _lastDiff = 0;

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
                _lowerLimit--;

            while(value > _upperLimit)
                _upperLimit++;
        }

        // Add value to queue
        _buffer.Enqueue(value);
        if (_buffer.Count > _bufferSize)
            _buffer.Dequeue();
    }

    public float GetSmoothed()
    {
        float sum = 0;
        foreach (var value in _buffer)
            sum += value;

        return sum / _buffer.Count;
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

    // Method overload for int values
    public float GetNormalized(int value)
    {
        return GetNormalized((float)value);
    }

    // Method overload for float values
    public float GetNormalized(float value)
    {
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

    public Peak DetectPeak()
    {
        var value = GetSmoothed();
        var diff = GetSmoothed() - _lastValue;

        Peak result;

        // Detect peaks
        if (diff > 0 && _lastDiff < 0)
            // Local minimum
            result = Peak.Minimum;
        else if (diff < 0 && _lastDiff > 0)
            // Local maximum
            result = Peak.Maximum;
        else 
            // No Peak
            result = Peak.None;

        // Apply new values
        _lastValue = value;
        _lastDiff = diff;

        return result;
    }

    public (float, float) GetLimits()
    {
        return (_lowerLimit, _upperLimit);
    }
}
