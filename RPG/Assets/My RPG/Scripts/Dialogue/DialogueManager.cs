using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRPG
{
    public class DialogueManager : MonoBehaviour
    {

        public GameObject DialogueUI;
        public Text headerText;
        private Dialogue dialogue;
        public bool hasActiveDialogue{ get{ return dialogue != null; }}
        private PlayerInput player;
        private QuestGiver npc;
        public float DistToTgt{ get { return Vector3.Distance(player.transform.position, npc.transform.position); }}
        public GameObject dialogueOptionList;
        public Button dialogueOptionPrefab;
        private float dialogueDist = 2f;
        public Text answerText;
        private float optionTopPosition;
        public float distBtwOptions = 60f;
        private float timerToShowOptions;
        private float timeToShowoptions = 2f;
        private bool forceQuit;

        void Awake(){
            DialogueUI.SetActive(false);
        }

        void Start(){
            player = PlayerInput.Instance;
        }
        void Update()
        {

            if(!hasActiveDialogue && player != null && player.OptionClickTgt != null ){
                if(player.OptionClickTgt.CompareTag("QuestGiver")){
                    npc = player.OptionClickTgt.GetComponent<QuestGiver>();
                    
                    if(DistToTgt <= dialogueDist){
                        StartDialogue();
                    }
                }
            }

            if (hasActiveDialogue && DistToTgt > dialogueDist +1f ){
                StopDialogue();
            }

            if(timerToShowOptions > 0){
                timerToShowOptions += Time.deltaTime;
                if(timerToShowOptions >= timeToShowoptions){
                    if(forceQuit){
                        StopDialogue();
                    }else{
                        DisplayDialogueOption();
                    }
                    timerToShowOptions = 0;                    
                }
            }
        }

        private void StartDialogue(){
            dialogue = npc.dialogue;
            DialogueUI.SetActive(true);
            headerText.text = npc.name;
            ClearDialogueOptions();
            DisplayAnsTxt(dialogue.welcomeText);
            TriggerDialogueOption();
        }

        void DisplayAnsTxt(string txt){
            answerText.gameObject.SetActive(true);
            answerText.text = txt;
        }

        void DisplayDialogueOption(){
            HideAnsTxt();
            CreateDialogueMenue();
        }

        void TriggerDialogueOption(){
            timerToShowOptions = 0.001f;
        }

        void HideAnsTxt(){
            answerText.gameObject.SetActive(false);
        }

        private void CreateDialogueMenue(){
            optionTopPosition =0f;
            DialogueQuery[] queries = Array.FindAll(dialogue.queries, query => !query.isAsked);
            
            foreach(var query in queries){
                optionTopPosition += distBtwOptions;
                Button dialogueOption = CreateDialogueOption(query.playerText);
                RegisterOptionClickHandler(dialogueOption, query);                
            }
        }

        Button CreateDialogueOption(string optionText){
            var buttonInstance = Instantiate(dialogueOptionPrefab, dialogueOptionList.transform);
            buttonInstance.GetComponentInChildren<Text>().text = optionText;
            RectTransform rt = buttonInstance.GetComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, optionTopPosition, rt.rect.height);
            return buttonInstance;
        }


        void RegisterOptionClickHandler(Button dialogueOption, DialogueQuery query){

            //Add component
            EventTrigger trigger = dialogueOption.gameObject.AddComponent<EventTrigger>();
            // initialize new event trigger
            EventTrigger.Entry pointerDown = new EventTrigger.Entry{
                eventID =EventTriggerType.PointerDown
            };
            // or initialize and specify the type of trigger seperately
            // EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            // pointerDown.eventID =EventTriggerType.PointerDown;

            pointerDown.callback.AddListener((e) => {
                if(!String.IsNullOrEmpty(query.answer.questId)){
                    player.GetComponent<QuestLog>().AddQuest(npc.quest);
                }
                if(query.answer.forceQuit){
                    forceQuit =true;
                }

                if(!query.isAlwaysAsked){
                    query.isAsked = true;
                }
                
                ClearDialogueOptions();
                DisplayAnsTxt(query.answer.npcText);
                TriggerDialogueOption();                
            });


            // Create/declare the trigger 
            trigger.triggers.Add(pointerDown);

        }

        private void ClearDialogueOptions(){
            foreach (Transform dialougeOption in dialogueOptionList.transform){
                Destroy(dialougeOption.gameObject);
            }
        }

        private void StopDialogue(){
            npc = null;
            dialogue = null;
            forceQuit = false;
            timerToShowOptions = 0f;
            DialogueUI.SetActive(false);
        }
    }
    
}

