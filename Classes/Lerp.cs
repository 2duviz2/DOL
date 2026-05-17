namespace DOL.Classes;

using UnityEngine;

/// <summary> Structure for handling all the fancy linear interpolation(lerp) stuff for any Vector3's. </summary>
public struct Vector3Lerp
{
    /// <summary> The 2 values to interpolate between. </summary>
    public Vector3 PreviousValue, TargetValue;

    /// <summary> How long we should take to interpolate between the values. </summary>
    public float Duration;

    /// <summary> Start time, the time when we got the newest target value. </summary>
    /// <remarks> This is used to calculate how long we've been interpolating for so we can calculate "t" in Grab(). </remarks>
    public float StartTime;

    /// <summary> Grabs the currently interpolated value between PreviousValue and TargetValue. </summary>
    public Vector3 Grab() =>
        Vector3.LerpUnclamped(PreviousValue, TargetValue, (Time.realtimeSinceStartup - StartTime) / Duration);

    /// <summary> Sets a new target value, shifting previous target and restarting interpolation timer. </summary>
    public void Set(Vector3 value)
    {
        PreviousValue = StartTime == 0f ? value : Grab();
        TargetValue = value;
        
        Duration = Mathf.Max(Time.realtimeSinceStartup - StartTime, Player.interval);
        StartTime = Time.realtimeSinceStartup;
    }
}

/// <summary> Structure for handling all the fancy linear interpolation(lerp) stuff for any Quaternion's. </summary>
public struct QuaternionLerp
{
    /// <summary> The 2 values to interpolate between. </summary>
    public Quaternion PreviousValue, TargetValue;

    /// <summary> How long we should take to interpolate between the values. </summary>
    public float Duration;

    /// <summary> Start time, the time when we got the newest target value. </summary>
    /// <remarks> This is used to calculate how long we've been interpolating for so we can calculate "t" in Grab(). </remarks>
    public float StartTime;

    /// <summary> Grabs the currently interpolated value between PreviousValue and TargetValue. </summary>
    public Quaternion Grab() =>
        Quaternion.LerpUnclamped(PreviousValue, TargetValue, (Time.realtimeSinceStartup - StartTime) / Duration);

    /// <summary> Sets a new target value, shifting previous target and restarting interpolation timer. </summary>
    public void Set(Quaternion value)
    {
        PreviousValue = StartTime == 0f ? value : Grab();
        TargetValue = value;

        Duration = Mathf.Max(Time.realtimeSinceStartup - StartTime, Player.interval);
        StartTime = Time.realtimeSinceStartup;
    }
}