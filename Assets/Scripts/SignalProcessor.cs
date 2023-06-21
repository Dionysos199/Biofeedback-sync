using System.Collections.Generic;
using System.Linq;
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

        // Update or reset range boundaries
        var smoothedValue = _buffer.Average();
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
        // This method estimates the phase shift between two
        // waveform input signals by projecting the readings
        //  to shifted cosine waves



    //     // DetectPeak();
    //     // return  _lastMax - _lastMin;
    // }

    public float GetNormalized()
    {
        var value = _buffer.Average();

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
            // Should only happen before first auto-range reset
            Debug.LogWarning("Sensor reading out of range.");
            return 0;
        }
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
        // Reset range boundaries
        _lowerLimit = _upperLimit = value;

        // Reset buffer if using reading-based min/max to prevent out-of-bound readings:
        // _buffer.Clear();

        // Reset waveform processing
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
