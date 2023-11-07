using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public class PlayerStats : MonoBehaviour, IMessageReciver
    {
        public int maxLevel;
        public int currentLvl;
        public int currentExp; 
        public int[] avilableLvl;
        public int expToNextLvl{ get {return avilableLvl[currentLvl]- currentExp; }} 



        private void Awake(){
            avilableLvl = new int[maxLevel];
            ComputeLevles(maxLevel);
        }

        private void ComputeLevles( int levelCount){
            for(int i =0; i < maxLevel; i++){

                int level = i+1;
                float levelPower = Mathf.Pow(level,2);
                var expToLvl = Convert.ToInt32(levelPower * levelCount);

                avilableLvl[i] = expToLvl;

            }
        }

        public void OnMessageRecive(MessageType type, object sender, object msg)
        {
            if(type == MessageType.DEAD)
            {
                GainExp((sender as Damageable).experience);
            }
        }

        public void GainExp(int expGained)
        {
            if(expGained > expToNextLvl){
                var remExp = expGained - expToNextLvl;
                currentExp =0;
                currentLvl ++;
                GainExp(remExp);
            }else if(expGained == expToNextLvl){
                currentExp =0;
                currentLvl ++;
            }else{
                currentExp += expGained;
            }

        }


    }
}

