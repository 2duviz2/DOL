namespace DOL.Classes;

using UnityEngine;

// God bless Bryan :3
/// <summary> Structure for handling all the fancy linear interpolation(lerp) stuff for any Vector3's. </summary>
public struct Vector3Lerp
{
    /// <summary> The 2 values to interpolate between. </summary>
    public Vector3 PreviousValue, TargetValue;

    /// <summary> How long we should take to interpolate between the values. </summary>
    public float Duration;

    /// <summary> Start time, the time when we got the newest target value. 
    /// <para> This is used to calculate how long we've been interpolating for so we can calculate "t" in Grab().
    /// </para></summary>
    public float StartTime;

    /// <summary> Grabs the currently interpolated value between PreviousValue and TargetValue. </summary>
    public Vector3 Grab() =>
        Vector3.Lerp(PreviousValue, TargetValue, (Time.realtimeSinceStartup - StartTime) / Duration);

    /// <summary> Sets a new target value, shifting previous target and restarting interpolation timer. </summary>
    public void Set(Vector3 value)
    {
        PreviousValue = StartTime == 0f ? value : Grab(); // if start time is zero that means that this is the first value and we have no previous data so just go to the newest value
        TargetValue = value;

        // packets are only sent at 20hz(every 0.05 seconds) so the duration cannot be lower than that
        Duration = Mathf.Max(Time.realtimeSinceStartup - StartTime, Client.interval);
        StartTime = Time.realtimeSinceStartup;
    }
}