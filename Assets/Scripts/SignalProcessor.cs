using System;
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
    private float _lastCount;

    bool Max;

    public SignalProcessor(int bufferSize, bool invertReadings = false)
    {
        // Configuration
        _bufferSize = bufferSize;
        _buffer = new Queue<float>();
        _invertReadings = invertReadings;
        _resetAutoRange = true;
        // Sets the initial values of all waveform processing variables, otherwise they might be undefined
        // ResetAutoRange(0);
    }

    public void RequestAutoRangeReset()
    {
        // Set reset flag
        _resetAutoRange = true;
    }

    // Method overload for int values
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
        _maxCount = 1;
        _lastCount = 0;


        // Unset reset flag
        _resetAutoRange = false;

        Debug.Log("you bastard" + _resetAutoRange);
    }

    public void AddValue(float value)
    {
        if (_resetAutoRange)
        {
            ResetAutoRange(value);
        }
        // Add value to queue
        _buffer.Enqueue(value);

        // Remove surplus item(s) from buffer
        while (_buffer.Count > _bufferSize)
            _buffer.Dequeue();


        // Update or reset range boundaries
        var smoothedValue = _buffer.Average();

        UpdateLimits(smoothedValue);


        // Run peak detection
        // DetectPeak();
    }
    private Queue<float> _LimitBuffer = new Queue<float>(50);
    int _LimitBufferSize = 20;
    void BufferLowerLimit(float value) { 
    // Add value to queue
    _LimitBuffer.Enqueue(value);

        // Remove surplus item(s) from buffer
        while (_LimitBuffer.Count > _LimitBufferSize)
            _LimitBuffer.Dequeue();

        _lowerLimit = _LimitBuffer.Min();

        Debug.Log(_lowerLimit + "  lower Limit");
        }
    void BufferUpperLimit(float value)
    {
        // Add value to queue
        _LimitBuffer.Enqueue(value);

        // Remove surplus item(s) from buffer
        while (_LimitBuffer.Count > _LimitBufferSize)
            _LimitBuffer.Dequeue();

        _upperLimit = _LimitBuffer.Max();

        Debug.Log(_upperLimit + "  upper Limit");
    }
    void UpdateLimits(float value)
    {

        //Debug.Log("lower limit" + _lowerLimit);
        //if (value < _lowerLimit)
        //{

        //    _lowerLimit = value;
        ////    Debug.Log("value"+value+"lower limit" + _lowerLimit);
        //}

        BufferLowerLimit(value);
        Debug.Log("_lowerLimit Limit  " + _lowerLimit + "  upper Limit  " + _upperLimit);
        if (value > _upperLimit)
        {
            _upperLimit = value;
        //    Debug.Log("upper limit" + _upperLimit);
        }
    }
    public float GetAmplitude()
    {
        return  _lastMax - _lastMin;
    }

    public float GetFrequency()
    {
        return 1 - ((float)_lastCount / (float)_maxCount);
    }

    public float GetPhaseShift(float secondCoeff)
    {
        // Estimate the phase shift between two waveforms by projecting
        // the normalized readings onto shifted cosine waves of type
        // f(x) = 0.5 * cos(x - a) + 0.5.
        // The result is returned as a normalized value between 0 and 1.

        var coeff = Acos();
        float phaseShift = (secondCoeff - coeff) / (4 * Mathf.PI) + 0.5f;

        if (0 <= phaseShift && phaseShift <= 1)
            return phaseShift;
        else
            Debug.LogError("Phase shift out of range [0; 1]");
        return 0;
    }

    public float Acos()
    {
        // Calculate coefficient a from projected cosine wave of
        // type f(x) = 0.5 * cos(x - a) + 0.5 based on normalized
        // reading as x

        var value = GetNormalized();
        var diff = value - _lastValue;
        _lastValue = value;

        var coeff = Mathf.Acos(2 * value - 1);
        //return coeff;
        if (diff >= 0)
            return coeff;
        else
            return Mathf.PI-coeff;
    }

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
    bool UpOrDown()
    {

        var value = GetNormalized();
        var diff = value - _lastValue;
        if (diff > 0)
        {

            _lastValue = value;
            return true;
        }
        else 
        {

            _lastValue = value;
            return false;
        }

    }
    public bool MaxReached()
    {
        if (Max) return true;
        else return false;
    }
    public void extremum()
    {
        var value = GetNormalized();
        var diff = value - _lastValue;
        Max = false;
        if (diff == 0)
        {
            // Ignore plateaus
            _lastValue = value;
        }
        else
        {
            // Detect peaks
            if (diff > 0 )
            {

                if (_lastDiff < 0) {
                    // Local minimum
                    _lastMin = value;
                    Debug.Log("Local Minimum");
                    UpdateFrequency();
                }
            }
            else if (diff < 0 && _lastDiff > 0)
            {
                // Local maximum
                _lastMax = value;

                if (_lastMax-_lastMin > .2)
                {
                    Max = true;

                    Debug.Log("miao " + _lastMax);
                }

                UpdateFrequency();
            }

            _lastValue = value;
            _lastDiff = diff;
        }
        _frequencyCount++;
        Debug.Log(_frequencyCount);
    }

    private void UpdateFrequency()
    {
        // Save current count
        float dt = Time.time-_lastCount;

        Debug.Log("Local Maximum  " + dt);
        _lastCount = Time.time;
        // Update absolute maximum if necessary
        //  if (_lastCount > _maxCount)
        // _maxCount = _lastCount;

        // Reset frequency counter
        _frequencyCount = 0;
    }
}
