using System;
using MilliGolf.Rando.Manager;

namespace MilliGolf.Rando.Settings
{
    public class GolfRandoSettings
    {
        public bool Enabled = false;
        public bool CourseAccess = false;
        public bool CourseCompletion = false;
        public MaxTier GlobalGoals = MaxTier.None;
        public bool EnableGoalPreviews = true;
        public bool CourseTransitions = false;
    }

    public enum MaxTier
    {
        None = 0,
        Attuned = 1,
        Ascended = 2,
        Radiant = 3,
        Master = 4,
        Grandmaster = 5
    }

    public class RandoSaveState
    {
        public CourseList courseAccess = new();
        public CourseList courseCompletion = new();
        public int globalGoals = 0;
    }

    public class CourseList
    {
        public bool Town { get; set; }
        public bool Crossroads_07 { get; set; }
        public bool RestingGrounds_05 { get; set; }
        public bool Hive_03 { get; set; }
        public bool Fungus1_31 { get; set; }
        public bool Fungus3_02 { get; set; }
        public bool Deepnest_East_11 { get; set; }
        public bool Waterways_02 { get; set; }
        public bool Cliffs_01 { get; set; }
        public bool Abyss_06_Core { get; set; }
        public bool Fungus2_12 { get; set; }
        public bool Ruins1_30 { get; set; }
        public bool Abyss_04 { get; set; }
        public bool Fungus3_04 { get; set; }
        public bool Ruins1_03 { get; set; }
        public bool Deepnest_35 { get; set; }
        public bool Mines_23 { get; set; }
        public bool White_Palace_19 { get; set; }

        public T GetVariable<T>(string propertyName) {
            var property = typeof(CourseList).GetProperty(propertyName) ?? throw new ArgumentException($"Course '{propertyName}' not found.");
            return (T)property.GetValue(this);
        }

        public void SetVariable<T>(string propertyName, T value) {
            var property = typeof(CourseList).GetProperty(propertyName) ?? throw new ArgumentException($"Course '{propertyName}' not found.");
            property.SetValue(this, value);
        }

        public bool AnyTrue()
        {            
            foreach ((string name, string scene) in GolfManager.courseList)
            {
                if (GetVariable<bool>(scene))
                    return true;
            }
            return false;
        }

        public bool AnyFalse()
        {            
            foreach ((string name, string scene) in GolfManager.courseList)
            {
                if (!GetVariable<bool>(scene))
                    return true;
            }
            return false;
        }
    }
}