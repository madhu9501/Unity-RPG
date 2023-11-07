using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyRPG;
using System.IO;
using System;

namespace MyRPG
{

    public class JsonHelper{
        public class Wrapper<T>{
            public T[] array;
        }
        public static T[] GetJsonArray<T>(string json){
            string newJson = "{ \"array\": "+ json + " }";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }          

        // public class Wrapper<Quests>{
        //     public Quests[] array;
        // }
        // public static Quests[] GetJsonArray(string json){
        //     string newJson = "{ \"array\": "+ json + " }";
        //     Wrapper<Quests> wrapper = JsonUtility.FromJson<Wrapper<Quest>>(newJson);
        //     return wrapper.array;
        // }
    }

    public class QuestManager : MonoBehaviour, IMessageReciver
    {
        public Quests[] quests;
        private PlayerStats playerStats;

        void Awake(){
            playerStats = FindObjectOfType<PlayerStats>();
            LoadFromQuest();
            AssignQuests();
            
        }

        void LoadFromQuest(){

            // using(StreamReader reader = new StreamReader("Assets/My RPG/Scripts/DB/Quests.json")){
            //     string json = reader.ReadToEnd();
            //     quests = JsonUtility.FromJson<Quests[]>(json);   => will fail
            // }  

            // So use
            // using (StreamReader reader = new StreamReader("Assets/My RPG/Scripts/DB/Quests.json")){
                // string json = reader.ReadToEnd();
                // var loadedQuests = JsonHelper.GetJsonArray<Quests>(json);
                // quests = new Quests[loadedQuests.Length];
                // quests = loadedQuests;
            // }            or use old way -> try catch finally bocks insted of "using"
            // StreamReader reader = new StreamReader("Assets/My RPG/Scripts/DB/Quests.json");
            // try{
            //     string json = reader.ReadToEnd();
            //     var loadedQuests = JsonHelper.GetJsonArray<Quests>(json);
            //     quests = new Quests[loadedQuests.Length];
            //     quests = loadedQuests;
            // }
            // finally{
            //     reader.Dispose();
            // }

            using StreamReader reader = new StreamReader("Assets/My RPG/Scripts/Quests/DB/Quests.json");
            string json = reader.ReadToEnd();
            var loadedQuests = JsonHelper.GetJsonArray<Quests>(json);
            // Quests[] loadedQuests = JsonHelper.GetJsonArray(json);
            quests = new Quests[loadedQuests.Length];
            quests = loadedQuests;
        }

        private void AssignQuests(){
            QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();
            if(questGivers != null && questGivers.Length > 0){                
                foreach(QuestGiver questGiver in questGivers){
                    AssignQuestTo(questGiver);
                }
            }
        }

        private void AssignQuestTo(QuestGiver questGiver)
        {
            foreach(Quests quest in quests ){
                if(quest.questGiver == questGiver.GetComponent<UniqueId>().Uid){
                    questGiver.quest = quest;
                }
            }
        }

        // public void OnMessageRecive(MessageType type, Damageable sender, Damageable.DamageMessage msg)
        // {
        //     if(type == MessageType.DEAD){
        //         CheckQuestWhenEnemyDead(sender, msg);
        //     }
        // }

        public void OnMessageRecive(MessageType type, object sender, object msg)
        {
            if(type == MessageType.DEAD){
                CheckQuestWhenEnemyDead((Damageable)sender, (Damageable.DamageMessage)msg);
            }
        }

        // private void CheckQuestWhenEnemyDead(Damageable sender, Damageable.DamageMessage msg){
        private void CheckQuestWhenEnemyDead(Damageable sender, Damageable.DamageMessage msg){
            QuestLog questLog = msg.damageSource.GetComponent<QuestLog>();

            foreach( var acceptedQuest in questLog.acceptedQuests ){
                if(acceptedQuest.questStatus == QuestStatus.ACTIVE){
                    if(acceptedQuest.type == QuestType.HUNT && Array.Exists(acceptedQuest.targets, (tragetsUid) => sender.GetComponent<UniqueId>().Uid == tragetsUid )){
                        acceptedQuest.targetCount -=1;
                        if(acceptedQuest.targetCount == 0){
                            acceptedQuest.questStatus = QuestStatus.COMPLETED;
                            playerStats.GainExp(acceptedQuest.experience);
                        }
                    }
                }
            }
        }

    }
}
