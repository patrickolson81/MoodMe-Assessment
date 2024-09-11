using System;
using System.Collections.Generic;

[Serializable]
public class MoodData
{
    public string day;
    public float sadValue;
    public float surprisedValue;
    public float neutralValue;

    public MoodData(string day, float sadValue, float surprisedValue, float neutralValue)
    {
        this.day = day;
        this.sadValue = sadValue;
        this.surprisedValue = surprisedValue;
        this.neutralValue = neutralValue;
    }
}

[Serializable]
public class MoodDataContainer
{
    public List<MoodData> moodDataList = new List<MoodData>();
}