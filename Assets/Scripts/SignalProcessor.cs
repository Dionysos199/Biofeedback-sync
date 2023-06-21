using System.Collections.Generic;
using UnityEngine;

public class SignalProcessor
{
    // Auto-ranging
    private bool _resetAutoRange = false;
    private float _lowerLimit;
    private float _upperLimit;

    // Basic signal processing
    private int _bufferSize;
    private Queue<float> _buffer;
    private bool _invertReadings;

    // Waveform processing
    private float _lastValue;
    private float _lastDiff;
    private float _lastMin;
    private float _lastMax;
    private int _frequencyCount;
    private int _maxCount;
    private int _lastMaxCount;

    public SignalProcessor(int bufferSize, bool invertReadings = false)
    {
        // Configuration
        _bufferSize = bufferSize;
        _buffer = new Queue<float>();
        _invertReadings = invertReadings;

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

        // Remove surplus item(s) from buffer
        while (_buffer.Count > _bufferSize)
            _buffer.Dequeue();

        // Update range boundaries
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

    public float GetAmplitude()
    {
        DetectPeak();
        return  _lastMax - _lastMin;
    }

    public float GetFrequency()
    {
        DetectPeak();
        return _lastMaxCount / _maxCount;
    }

    // public float GetPhaseShift()
    // {
    //     // DetectPeak();
    //     // return  _lastMax - _lastMin;
    // }

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
                Debug.Log("Local Minimum");

                UpdateFrequency();
            }
            else if (diff < 0 && _lastDiff > 0)
            {
                // Local maximum
                _lastMax = value;
                Debug.Log("Local Maximum");

                UpdateFrequency();
            }

            _lastValue = value;
            _lastDiff = diff;
        }
        _frequencyCount++;
    }

    private void UpdateFrequency()
    {
        // Save current count
        _lastMaxCount = _frequencyCount;

        // Update absolute maximum if necessary
        if (_lastMaxCount > _maxCount)
            _maxCount = _lastMaxCount;

        // Reset frequency counter
        _frequencyCount = 0;
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
        _frequencyCount = 0;
        _maxCount = 0;
        _lastMaxCount = 0;

        // Unset reset flag
        _resetAutoRange = false;
    }
}
