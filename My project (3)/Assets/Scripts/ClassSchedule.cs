using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Days {
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}

public class ClassSchedule
{
    private List<Days> days;
    private string startTime;
    private string endTime;
    
    public ClassSchedule(List<Days> days, string startTime, string endTime) {
        this.days = days;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    // Getters
    public List<Days> GetDays() {
        return days;
    }

    public string GetStartTime() {
        return startTime;
    }

    public string GetEndTime() {
        return endTime;
    }

    // Setters
    public void AddDay(Days day) {
        days.Add(day);
    }

    public void SetStartTime(string startTime){
        this.startTime = startTime;
    }

    public void SetEndTime(string endTime) {
        this.endTime = endTime;
    }
}
