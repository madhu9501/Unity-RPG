using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public enum MessageType{
        DAMAGED,
        DEAD
    }
    public interface IMessageReciver
    {
        // void OnMessageRecive(MessageType type, Damageable sender, Damageable.DamageMessage msg);
        //or make it more generic as below and use casting when calling this method as in QuestGiver
        void OnMessageRecive(MessageType type, object sender, object msg);
    }
}

