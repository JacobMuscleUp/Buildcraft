using System.Collections.Generic;
using UnityEngine;

public class VirtualAxisManager
{
    static VirtualAxisManager ins;

    static List<string> axisNames;
    static Dictionary<string, bool> axisDownMap = new Dictionary<string, bool>();
    static Dictionary<string, bool> axisUpMap = new Dictionary<string, bool>();
    static Dictionary<string, float> axisDownTimeMap = new Dictionary<string, float>();
    static Dictionary<string, float> axisUpTimeMap = new Dictionary<string, float>();

    public static bool GetAxis(string _axisName)
    {
        Initialize();
        if (Input.GetAxis(_axisName) != 0)
            return true;
        return false;
    }

    public static bool GetAxisDown(string _axisName)
    {
        Initialize();
        if (Input.GetAxis(_axisName) != 0) {
            if (axisDownMap.ContainsKey(_axisName)) {
                if (!axisDownMap[_axisName] || axisDownTimeMap[_axisName] == Time.time) {
                    axisDownMap[_axisName] = true;
                    axisDownTimeMap[_axisName] = Time.time;
                    return true;
                }
            }
        }
        else
            axisDownMap[_axisName] = false;
        return false;
    }

    public static bool GetAxisUp(string _axisName)
    {
        Initialize();
        if (!GetAxis(_axisName)) {
            if (axisUpMap[_axisName] || axisUpTimeMap[_axisName] == Time.time) {
                axisUpMap[_axisName] = false;
                axisUpTimeMap[_axisName] = Time.time;
                return true;
            }
            return false;
        }
        if (!axisUpMap[_axisName])
            axisUpMap[_axisName] = true;
        return false;
    }

    static void Initialize()
    {
        if (ins == null) {
            ins = new VirtualAxisManager();

            axisNames = new List<string>() { "Fire1", "Fire2", "Fire3", "Mouse ScrollWheel" };
            foreach (var axisName in axisNames) {
                axisDownMap[axisName] = false;
                axisUpMap[axisName] = false;
                axisDownTimeMap[axisName] = 0;
                axisUpTimeMap[axisName] = 0;
            }
        }
    }
}
