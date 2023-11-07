using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    [System.Serializable]
    public class DialogueAnswer
    {
        public string npcText;
        public bool forceQuit;
        public string questId;
    }

[System.Serializable]
    public class DialogueQuery
    {
        public string playerText;
        public DialogueAnswer answer;
        public bool isAsked;
        public bool isAlwaysAsked;
    }

    [System.Serializable]
    public class Dialogue 
    {
        public string welcomeText;
        public DialogueQuery[] queries;
    }
}
