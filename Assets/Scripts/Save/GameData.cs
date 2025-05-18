using System;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class GameData
    {
        public Vector3 position;
        public float hp;
        public float stamina;
        public List<ItemInventoryData> items;
        public Dictionary<int, List<ItemInventoryData>> chests;
        
        public SerializableColor skinColor;
        public SerializableColor hatColor;
        public SerializableColor pantsColor;
        public SerializableColor shirtColor;

        public int hatAnimationIndex;
        public int pantsAnimationIndex;
        public int shirtAnimationIndex;

        public GameData()
        {
            position = Vector3.zero;
            hp = 300f;
            stamina = 1440f;
            items = new List<ItemInventoryData>();
            chests = new Dictionary<int, List<ItemInventoryData>>();
            
            skinColor = new SerializableColor(Color.white);
            hatColor = new SerializableColor(Color.white);
            pantsColor = new SerializableColor(Color.white);
            shirtColor = new SerializableColor(Color.white);

            hatAnimationIndex = 0;
            pantsAnimationIndex = 0;
            shirtAnimationIndex = 0;
        }
    }
    
    [Serializable]
    public class ItemInventoryData 
    {
        public int id;
        public bool istool;
        public int count;

        public ItemInventoryData(ItemInventory item)
        {
            id = item.id;
            istool = item.istool;
            count = item.count;
        }
        public ItemInventoryData() { }
    }
    
    [Serializable]
    public struct SerializableColor
    {
        public float r, g, b, a;

        public SerializableColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }

}