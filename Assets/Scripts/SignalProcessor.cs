using System.Collections.Generic;
using System.Linq;
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
        ResetAutoRange();
    }

    public void ResetAutoRange()
    {
        // Use soft buffer reset for moving average-based min/max:
        _lowerLimit = _upperLimit = GetSmoothed();

        // Use hard buffer reset for reading-based min/max:
        // while (_buffer.Count > 1)
        //     _buffer.Dequeue();
        // _lowerLimit = _upperLimit = _buffer.LastOrDefault();
    }

    // Method overload for int values
    public void AddValue(int value)
    {
        AddValue((float)value);
    }

    public void AddValue(float value)
    {
        // Add value to queue
        _buffer.Enqueue(value);
        if (_buffer.Count > _bufferSize)
            _buffer.Dequeue();

        // Update range automatically
        if(_autoRange)
        {
            var smoothedValue = GetSmoothed();

            if (smoothedValue < _lowerLimit)
                _lowerLimit = smoothedValue;
            else if (smoothedValue > _upperLimit)
                _upperLimit = smoothedValue;
        }  
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

        // Prevent division errors after range reset
        if (_lowerLimit == _upperLimit)
            return 0;

        if (_lowerLimit <= value && value <= _upperLimit)
            return (value - _lowerLimit) / (_upperLimit - _lowerLimit);
        else
            // Should only happen when auto-ranging is turned off
            Debug.LogError("Sensor reading out of range.");
        return 0;
    }

    public float Invert(float value)
    {
        if (0 <= value && value <= 1)
            return 1 - value;
        else
            Debug.Log("Only normalized values can be inverted.");
        return 0;
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
