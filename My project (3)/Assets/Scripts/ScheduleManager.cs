using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScheduleManager
{
    private List<ClassSchedule> schedules = new List<ClassSchedule>();
    private string filePath = Application.persistentDataPath + "/schedule.json";

    public ScheduleManager()
    {
        filePath = Application.persistentDataPath + "/schedule.json";
        Debug.Log($"File path: {filePath}");
        LoadSchedule();
    }

    // Add a new schedule entry
    public void AddSchedule(List<Days> days, string startTime, string endTime)
    {
        ClassSchedule newSchedule = new ClassSchedule(days, startTime, endTime);
        schedules.Add(newSchedule);
        Debug.Log($"{startTime} - {endTime}");
        SaveSchedule();
        Debug.Log("Added new schedule!");
    }

    // Save schedules to JSON
    private void SaveSchedule()
    {
        List<SerializableSchedule> serializableSchedules = new List<SerializableSchedule>();
        foreach (var schedule in schedules)
        {
            serializableSchedules.Add(new SerializableSchedule(schedule));
        }

        string jsonString = JsonUtility.ToJson(new ScheduleWrapper { schedules = serializableSchedules }, true);
        Debug.Log("GJ");
        File.WriteAllText(filePath, jsonString);
        DisplaySchedules();
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
        } else {
            // File doesn't exist, create it with an empty schedule structure
            ScheduleWrapper emptyScheduleWrapper = new ScheduleWrapper { schedules = new List<SerializableSchedule>() };
            string emptyJson = JsonUtility.ToJson(emptyScheduleWrapper, true);
            File.WriteAllText(filePath, emptyJson);

            Debug.Log("File not found, creating new schedule file!");
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
