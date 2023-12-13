using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Status
{
    public string DeviceName { get; set; }
    public string DeviceModel { get; set; }
    public string DeviceType { get; set; }
    public float BatteryLevel { get; set; }
    public string BatteryStatus { get; set; }
    public string CurrentScene { get; set; }
    public float RunTime { get; set; }
    public bool IsMuted { get; set; }
    public float fps { get; set; }
}
