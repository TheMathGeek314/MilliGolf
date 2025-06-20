using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace MilliGolf {
    public class GolfScene {
        public static List<string> courseList = new();
        public static Dictionary<string, GolfScene> courseDict = new();
        public static List<string> customCourseList = new();
        public static Dictionary<string, GolfScene> customCourseDict = new();

        public string name, scene, startTransition, holeTarget;
        public Vector3 millibelleSpawn;
        public Vector3 knightSpawn; //there's probably a better way to do this but eh
        public Color doorColor, secondaryDoorColor;
        public bool customHoleObject, hasQuirrel;
        public (Vector3, Vector3) customHolePosition;
        public (float, float, bool, string) quirrelData;
        public (string, float, float) flagData;
        public List<string> objectsToDisable = new();
        public Dictionary<string, List<string>> childrenToDisable = new();

        public void create(bool custom) {
            if(custom) {
                customCourseList.Add(scene);
                customCourseDict.Add(scene, this);
            }
            else {
                courseList.Add(scene);
                courseDict.Add(scene, this);
            }
        }
    }

    public class GolfJsonScene {
        public string name, scene, startTransition, holeTarget;
        public JsonVec2 millibelleSpawn, knightSpawn;
        public JsonColor doorColor, secondaryDoorColor;
        public JsonPosBounds customHolePosition;
        public JsonQuirrel quirrelData;
        public JsonFlag flagData;
        public List<string> objectsToDisable;
        public Dictionary<string, List<string>> childrenToDisable;

        public GolfScene translate() {
            GolfScene gScene = new() {
                name = name,
                scene = scene,
                startTransition = startTransition,
                holeTarget = holeTarget,
                millibelleSpawn = new Vector3(millibelleSpawn.x, millibelleSpawn.y, 0.006f),
                knightSpawn = new Vector3(knightSpawn.x, knightSpawn.y, 0.004f),
                doorColor = new Color(doorColor.r, doorColor.g, doorColor.b, 0.466f)
            };
            if(secondaryDoorColor != null) {
                gScene.secondaryDoorColor = new Color(secondaryDoorColor.r, secondaryDoorColor.g, secondaryDoorColor.b, 0.466f);
            }
            if(customHolePosition == null) {
                gScene.customHoleObject = false;
            }
            else {
                gScene.customHoleObject = true;
                gScene.customHolePosition = (new Vector3(customHolePosition.x, customHolePosition.y), new Vector3(customHolePosition.sx, customHolePosition.sy));
                gScene.holeTarget = "Custom Hole";
            }
            if(quirrelData == null) {
                gScene.hasQuirrel = false;
            }
            else {
                gScene.hasQuirrel = true;
                gScene.quirrelData = (quirrelData.x, quirrelData.y, quirrelData.faceRight, quirrelData.dialogueKey);
            }
            gScene.flagData = (flagData.filename, flagData.x, flagData.y);
            if(objectsToDisable != null)
                gScene.objectsToDisable = objectsToDisable;
            if(childrenToDisable != null)
                gScene.childrenToDisable = childrenToDisable;
            return gScene;
        }
    }

    public class ParseJson {
        private readonly string _jsonFilePath;
        private readonly Stream _jsonStream;
        private bool isPath;
        
        public ParseJson(string jsonFilePath) {
            _jsonFilePath = jsonFilePath;
            isPath = true;
        }

        public ParseJson(Stream jsonStream) {
            _jsonStream = jsonStream;
            isPath = false;
        }

        public List<GolfJsonScene> parseCourses() {
            if(isPath) {
                using StreamReader reader = new(_jsonFilePath);
                var json = reader.ReadToEnd();
                List<GolfJsonScene> gScenes = JsonConvert.DeserializeObject<List<GolfJsonScene>>(json);

                return gScenes;
            }
            else {
                using StreamReader reader = new(_jsonStream);
                var json = reader.ReadToEnd();
                List<GolfJsonScene> gScenes = JsonConvert.DeserializeObject<List<GolfJsonScene>>(json);

                return gScenes;
            }
        }
    }

    public class JsonColor {
        public float r, g, b;
    }

    public class JsonVec2 {
        public float x, y;
    }

    public class JsonPosBounds {
        public float x, y, sx, sy;
    }

    public class JsonQuirrel {
        public float x, y;
        public bool faceRight;
        public string dialogueKey;
    }

    public class JsonFlag {
        public string filename;
        public float x, y;
    }
}
