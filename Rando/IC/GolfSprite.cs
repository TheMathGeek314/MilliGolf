using ItemChanger;
using ItemChanger.Internal;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MilliGolf.Rando.IC
{
    [Serializable]
    public class GolfSprite : ISprite
    {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(GolfSprite).Assembly, "MilliGolf.Images.");
        public string Key { get; set; }
        public GolfSprite(string key)
        {
            if (!string.IsNullOrEmpty(key))
                Key = key;
        }
        [JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(Key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}