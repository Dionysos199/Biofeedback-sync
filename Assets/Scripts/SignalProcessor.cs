using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalProcessor
{
    // Auto-ranging
    private bool _resetAutoRange = false;
    private float _lowerLimit;
    private float _upperLimit;

    // Signal inversion
    private bool _invertReadings;

    // Buffer
    private int _bufferSize;
    private Queue<float> _buffer;

    // Peak detection and waveform processing
    private float _lastValue;
    private float _lastDiff;
    private float _lastMin;
    private float _lastMax;

    public SignalProcessor(int bufferSize, bool invertReadings = false)
    {
        // Configure buffer
        _bufferSize = bufferSize;
        _buffer = new Queue<float>();

        // Configure signal inversion
        _invertReadings = invertReadings;

        // Configure auto-ranging
        ResetAutoRange(0);
    }

    public void RequestAutoRangeReset()
    {
        // Set reset flag
        _resetAutoRange = true;
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
        var smoothedValue = GetSmoothed();
        if (_resetAutoRange)
        {
            ResetAutoRange(smoothedValue);
        }
        else
        {
            if (smoothedValue < _lowerLimit)
                _lowerLimit = smoothedValue;
            else if (smoothedValue > _upperLimit)
                _upperLimit = smoothedValue;
        }
    }

    public float GetNormalized()
    {
        var value = GetSmoothed();

        // Prevent division errors after range reset
        if (_lowerLimit == _upperLimit)
            return 0;

        if (_lowerLimit <= value && value <= _upperLimit)
        {
            float normalizedValue = (value - _lowerLimit) / (_upperLimit - _lowerLimit);
            if (_invertReadings)
                return 1 - normalizedValue;
            else
                return normalizedValue;
        }
        else
        {
            // Should only happen when auto-ranging is turned off/before first reset
            Debug.LogError("Sensor reading out of range.");
            return 0;
        }
    }

    public float GetAmplitude()
    {
        DetectPeak();
        return  _lastMax - _lastMin;
    }

    private float GetSmoothed()
    {
        float sum = 0;
        foreach (var value in _buffer)
            sum += value;

        return sum / _buffer.Count;
    }

    private void DetectPeak()
    {
        var value = GetNormalized();
        var diff = value - _lastValue;

        if (diff == 0)
        {
            // Ignore plateaus
            _lastValue = value;
        }
        else
        {
            // Detect peaks
            if (diff > 0 && _lastDiff < 0)
            {
                // Local minimum
                _lastMin = value;
                Debug.Log("Local Minimum: diff: " + diff + ", lastDiff: " + _lastDiff);
            }
            else if (diff < 0 && _lastDiff > 0)
            {
                // Local maximum
                _lastMax = value;
                Debug.Log("Local Maximum: diff: " + diff + ", lastDiff: " + _lastDiff);
            }

            _lastValue = value;
            _lastDiff = diff;
        }
    }

    private void ResetAutoRange(float value)
    {
        // Use soft buffer reset for moving average-based min/max:
        _lowerLimit = _upperLimit = value;

        // Use hard buffer reset for reading-based min/max:
        // while (_buffer.Count > 0)
        //     _buffer.Dequeue();
        // _lowerLimit = _upperLimit = _buffer.LastOrDefault();

        // Reset peak detection and waveform processing, too.
        // These functions deal only with normalized values!
        _lastValue = 0;
        _lastDiff = 0;
        _lastMin = 0;
        _lastMax = 0;

        // Unset reset flag
        _resetAutoRange = false;
    }
}
