using System;
using UnityEngine;

namespace MyRPG
{
    public enum QuestType{
        HUNT,
        GATHER,
        TALK,
        EXPLORE
    }

    [System.Serializable]
    public class Quests
    {
        public string uid;
        public string questName;
        public string questDescription;

        public string questGiver;
        public QuestType type;

        public int experience;
        public int gold;

        public string[] targets;
        public int targetCount;

        public Vector3 exploreLoc;
        public string talkTo;


    }
    
}

