using System.Collections.Generic;
using ItemChanger;
using MilliGolf.Rando.IC;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace MilliGolf.Rando.Manager 
{
    internal static class ItemHandler
    {   
        internal static void Hook()
        {
            DefineObjects();
            RequestBuilder.OnUpdate.Subscribe(0f, AddObjects);
        }

        public static void DefineObjects()
        {
            float f = -2.55f;
            foreach ((string name, string scene) in GolfManager.courseList)
            {
                Finder.DefineCustomItem(new GolfItem(name, scene, "Course_Access"));
                Finder.DefineCustomItem(new GolfItem(name, scene, "Course_Completion"));
                Finder.DefineCustomLocation(new CourseCompletionLocation(name, scene, f, 0.6f));
                f += 0.3f;
            }
            Finder.DefineCustomItem(new GolfItem("Goal_Tier", "_", "Milligolf"));
            Finder.DefineCustomLocation(new GlobalGoalLocation("Attuned", 0, -0.45f, 0.9f));
            Finder.DefineCustomLocation(new GlobalGoalLocation("Ascended", golfMilestones.Ascended, -0.15f, 0.9f));
            Finder.DefineCustomLocation(new GlobalGoalLocation("Radiant", golfMilestones.Radiant, 0.15f, 0.9f));
            Finder.DefineCustomLocation(new GlobalGoalLocation("Master", golfMilestones.Master, 0.45f, 0.9f));
            Finder.DefineCustomLocation(new GlobalGoalLocation("Grandmaster", golfMilestones.Grandmaster, 0.75f, 0.9f));
        }

        public static void AddObjects(RequestBuilder rb)
        {
            if (!GolfManager.GlobalSettings.Enabled)
                return;
            
            if (GolfManager.GlobalSettings.CourseAccess)
            {
                foreach((string name, string scene) in GolfManager.courseList)
                {
                    rb.AddItemByName($"Course_Access-{name}");
                    rb.EditItemRequest($"Course_Access-{name}", info => 
                    {
                        info.getItemDef = () => new()
                        {
                            MajorItem = false,
                            Name = $"Course_Access-{name}",
                            Pool = "Golf",
                            PriceCap = 500
                        };
                    });
                }
            }
            else
            {
                foreach((string name, string scene) in GolfManager.courseList)
                {
                    rb.AddToVanilla(new VanillaDef($"Course_Access-{name}", "Start"));
                }
            }

            if (GolfManager.GlobalSettings.CourseCompletion)
            {
                foreach((string name, string scene) in GolfManager.courseList)
                {
                    rb.AddItemByName($"Course_Completion-{name}");
                    rb.EditItemRequest($"Course_Completion-{name}", info => 
                    {
                        info.getItemDef = () => new()
                        {
                            MajorItem = false,
                            Name = $"Course_Completion-{name}",
                            Pool = "Golf",
                            PriceCap = 500
                        };
                    });

                    rb.AddLocationByName($"Milligolf_Course-{name}");
                    rb.EditLocationRequest($"Milligolf_Course-{name}", info =>
                    {
                        info.getLocationDef = () => new()
                        {
                            Name = $"Milligolf_Course-{name}",
                            SceneName = SceneNames.Town,
                            FlexibleCount = false,
                            AdditionalProgressionPenalty = false
                        };
                    });
                }
            }
            else
            {
                foreach((string name, string scene) in GolfManager.courseList)
                {
                    rb.AddToVanilla(new VanillaDef($"Course_Completion-{name}", $"Milligolf_Course-{name}"));
                }
            }

            if (GolfManager.GlobalSettings.GlobalGoals > Settings.MaxTier.None)
            {
                rb.AddItemByName("Milligolf-Goal_Tier", (int)GolfManager.GlobalSettings.GlobalGoals);
                rb.EditItemRequest("Milligolf-Goal_Tier", info => 
                {
                    info.getItemDef = () => new()
                    {
                        MajorItem = false,
                        Name = "Milligolf-Goal_Tier",
                        Pool = "Golf",
                        PriceCap = 500
                    };
                });

                List<(string, int)> ranks = new() {("Attuned", 1), ("Ascended", 2), ("Radiant", 3), ("Master", 4), ("Grandmaster", 5)};
                foreach ((string rank, int tier) in ranks)
                {
                    if ((int)GolfManager.GlobalSettings.GlobalGoals >= tier)
                    {
                        rb.AddLocationByName($"Milligolf_{rank}");
                        rb.EditLocationRequest($"Milligolf_{rank}", info =>
                        {
                            info.getLocationDef = () => new()
                            {
                                Name = $"Milligolf_{rank}",
                                SceneName = SceneNames.Town,
                                FlexibleCount = false,
                                AdditionalProgressionPenalty = false
                            };
                        });
                    }
                }
            }
        }
    }
}