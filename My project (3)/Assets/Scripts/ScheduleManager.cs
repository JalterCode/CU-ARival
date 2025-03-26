using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ScheduleManager : MonoBehaviour
{
    void Start() {
        LoadSchedule();   
    }
    
    public List<ClassSchedule> schedules = new List<ClassSchedule>();
    private string filePath = "/schedule.json";
    
    public void AddSchedule(List<Days> days, string startTime, string endTime) {

        ClassSchedule newSchedule = new ClassSchedule(days, startTime, endTime);
        schedules.Add(newSchedule);
        SaveSchedule(newSchedule);
    }

    public void SaveSchedule(ClassSchedule _data) {
        List<SerializableSchedule> serializableSchedules = new List<SerializableSchedule>();
        foreach (var schedule in schedules)
        {
            serializableSchedules.Add(new SerializableSchedule(schedule));
        }

        string json = JsonUtility.ToJson(new ScheduleWrapper { schedules = serializableSchedules }, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Schedule saved!");
    }


    // Load schedules from JSON
    private void LoadSchedule()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ScheduleWrapper loadedData = JsonUtility.FromJson<ScheduleWrapper>(json);

            if (loadedData != null && loadedData.schedules != null)
            {
                schedules.Clear();
                foreach (var loadedSchedule in loadedData.schedules)
                {
                    schedules.Add(loadedSchedule.ToClassSchedule());
                }

                Debug.Log("Schedule loaded!");
            }
        }
    }

    // Display saved schedules
    public void DisplaySchedules()
    {
        foreach (var schedule in schedules)
        {
            string daysString = string.Join(", ", schedule.GetDays());
            Debug.Log($"Class on: {daysString}, from {schedule.GetStartTime()} to {schedule.GetEndTime()}");
        }
    }

    // Helper classes for JSON serialization
    [System.Serializable]
    private class SerializableSchedule
    {
        public int[] days;
        public string startTime;
        public string endTime;

        public SerializableSchedule(ClassSchedule schedule)
        {
            days = schedule.GetDays().ConvertAll(day => (int)day).ToArray();
            startTime = schedule.GetStartTime();
            endTime = schedule.GetEndTime();
        }

        public ClassSchedule ToClassSchedule()
        {
            List<Days> dayList = new List<Days>();
            foreach (int day in days)
            {
                dayList.Add((Days)day);
            }
            return new ClassSchedule(dayList, startTime, endTime);
        }
    }

    [System.Serializable]
    private class ScheduleWrapper
    {
        public List<SerializableSchedule> schedules;
    }

}

