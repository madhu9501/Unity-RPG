using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public enum QuestStatus{
        ACTIVE,
        COMPLETED,
        FAILED
    }

    [System.Serializable]
    public class AcceptedQuests : Quests {
        public QuestStatus questStatus;
        public AcceptedQuests(Quests quest){
            uid = quest.uid;
            questName = quest.questName;
            questDescription = quest.questDescription;
            questGiver = quest.questGiver;
            type = quest.type;
            experience = quest.experience;
            gold = quest.gold;
            targets = quest.targets;
            targetCount = quest.targetCount;
            exploreLoc = quest.exploreLoc;
            talkTo = quest.talkTo;
            questStatus = QuestStatus.ACTIVE; 

        }

    }
    public class QuestLog : MonoBehaviour
    {
        public List<AcceptedQuests> acceptedQuests = new List<AcceptedQuests>();

        public void AddQuest(Quests quest){
            acceptedQuests.Add(new AcceptedQuests(quest));
        }

    }
    
}

