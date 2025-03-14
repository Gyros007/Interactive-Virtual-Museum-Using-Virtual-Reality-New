﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VIDE_Data;
using UnityEngine.UI;

public class QuestChartDemo : MonoBehaviour
{
    public static VIDE_Assign assigned;

    public GameObject questChartContainer;
    public GameObject ovGameObject;
    public GameObject peGameObject;

    //Tasks
    static int totalInteractions = 13;
    static int cylinderGuyTotal = 9;
    static List<string> interactedWith = new List<string>();
    static List<int> cylinderGuyInteractions = new List<int>();

    public Variables var;


    void Awake()
    {
        //Will load saved progress in PlayerPrefs
        LoadProgress();
    }

    void OnEnable()
    {
        assigned = GetComponent<VIDE_Assign>();
    }

    void Update()
    {

        if (interactedWith.Count == totalInteractions)
        {
            var.metEveryone = true;
        }

        //This is checking if the QuestChart UI Manager is 
        if (gameObject != null)
            if (questChartContainer.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    PlayerPrefs.DeleteAll();
                    if (System.IO.Directory.Exists(Application.dataPath + "/VIDE/saves"))
                    {
                        System.IO.Directory.Delete(Application.dataPath + "/VIDE/saves", true);
                        #if UNITY_EDITOR
                        UnityEditor.AssetDatabase.Refresh();
                        #endif
                    }

                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                }
            }
    }

    // Will Use the SetVisible method to switch the visibility of a comment
    // When a comment is not visible, its content will not be included in the nodeData arrays
    // The method will also add info to an ExtraVariables key to mark the completion of a quest
    public static void SetQuest(int quest, bool visible)
    {
        VD.SetVisible(assigned.assignedDialogue, 0, quest, visible);
        Dictionary<string, object> newEV = VD.GetExtraVariables(assigned.assignedDialogue, 1);
        newEV["complete"] += "[" + quest.ToString() + "]";
        VD.SetExtraVariables(assigned.assignedDialogue, 1, newEV);
    }

    //Will start and end the assigned dialogue
    public void Interact()
    {
        if (!questChartContainer.activeSelf)
        {
            questChartContainer.SetActive(true);
            VD.NodeData nd = VD.BeginDialogue(assigned);
            LoadChart(nd);
        }
        else
        {
            for (int i = 0; i < peGameObject.transform.parent.childCount; i++)
                if (i != 0) Destroy(peGameObject.transform.parent.GetChild(i).gameObject);

            for (int i = 0; i < ovGameObject.transform.parent.childCount; i++)
                if (i != 0) Destroy(ovGameObject.transform.parent.GetChild(i).gameObject);

            questChartContainer.SetActive(false);
            VD.EndDialogue();
        }
    }

    // Uses both NodeData and local variables to populate the Quest UI
    public void LoadChart(VD.NodeData data)
    {

        if (var.metZeus && !var.metZeusOnce)
        {
            data.comments[0] = "Talk to all the Gods";
            VD.SetComment(VD.assigned.assignedDialogue, 0, 0, "Talk to all the Gods");
            VD.SetComment(VD.assigned.assignedDialogue, 1, 0, "Talk to all the Gods");
            var.metZeusOnce = true;
        }

        if (var.metArtemis && !var.metArtemisOnce)
        {
            data.comments[1] = "Find 3 arrows for Artemis";
            VD.SetComment(VD.assigned.assignedDialogue, 0, 1, "Find 3 arrows for Artemis");
            VD.SetComment(VD.assigned.assignedDialogue, 1, 1, "Find 3 arrows for Artemis");
            var.metArtemisOnce = true;
        }
        if (var.numArrows == 3 && var.returnedToArtemis)
        {
            //data.comments[0] = "";
            //VD.SetComment(VD.assigned.assignedDialogue, 0, 0, "");
            SetQuest(1, true);
        }

        if (interactedWith.Count >= totalInteractions && var.returnedToZeus && !var.metEveryoneOnce)
        {
            //data.comments[0] = "";
            //VD.SetComment(VD.assigned.assignedDialogue, 0, 0, "");
            SetQuest(0, true);
            var.metEveryone = true;
            var.metEveryoneOnce = true;
        }
        ////Pending quests
        //for (int i = 0; i < data.comments.Length; i++)
        //{
        //    GameObject pe = (GameObject)Instantiate(peGameObject);
        //    pe.transform.SetParent(peGameObject.transform.parent, true);
        //    pe.GetComponent<RectTransform>().sizeDelta = new Vector2(367, 40);
        //    pe.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -15 - (15 * i));
        //    pe.GetComponent<Text>().text = data.comments[i];
        //    if (data.comments[i].Contains("Talk to all Gods")) pe.GetComponent<Text>().text += " (" + interactedWith.Count.ToString() + "/" + totalInteractions.ToString() + ")";
        //    pe.SetActive(true);
        //}

        //Overview quests
        VD.NodeData overviewData = VD.GetNodeData(assigned.assignedDialogue, 1, true);
        for (int i = 0; i < data.comments.Length; i++)
        {
            string completeKey = (string)overviewData.extraVars["complete"];

            GameObject ov = (GameObject)Instantiate(ovGameObject);
            if (completeKey.Contains("[" + i.ToString() + "]"))
            {
                ov.GetComponent<Text>().text = overviewData.comments[i] + " [✓]";
                ov.GetComponent<Text>().color = new Color32(191, 192, 177, 255);
            }
            else
            {
                ov.GetComponent<Text>().text = overviewData.comments[i];
            }
            ov.transform.SetParent(ovGameObject.transform.parent, true);
            ov.GetComponent<RectTransform>().sizeDelta = new Vector2(367, 60);
            ov.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -15 - (50 * i));
            if (data.comments[i].Contains("Talk to all the Gods"))
            {
                if (!var.metEveryone)
                    ov.GetComponent<Text>().text += " (" + interactedWith.Count.ToString() + "/" + totalInteractions.ToString() + ")";
                else if (!var.returnedToZeus)
                    ov.GetComponent<Text>().text += " (Return to Zeus)";
            }
            else if (data.comments[i].Contains("Find 3 arrows for Artemis"))
                if (var.numArrows < 3)
                    ov.GetComponent<Text>().text += " (" + var.numArrows.ToString() + "/3)";
                else if (var.numArrows == 3 && !var.returnedToArtemis)
                    ov.GetComponent<Text>().text += "\n(Return to Artemis)";
            ov.SetActive(true);
        }
    }

    //Set CylinderGuy quest
    public static void CylinderGuyAddInteraction(int index)
    {
        if (!cylinderGuyInteractions.Contains(index))
            cylinderGuyInteractions.Add(index);
    }


    //Check some of the Quests completion
    public static void CheckTaskCompletion(VD.NodeData data)
    {
        if (VD.assigned == null) return;

        if (!interactedWith.Contains(VD.assigned.gameObject.name))
            interactedWith.Add(VD.assigned.gameObject.name);

        //Check
        // 0 Talk to Everyone
        // 1 Listen to CylinderGuy
        // 2 Get all items from Crazy Cap
        // 3 Threaten Charlie

        if (cylinderGuyInteractions.Count == cylinderGuyTotal) SetQuest(1, false);
    }

    //Set Charlie quest
    public void SetCharlieQuestComplete()
    {
        SetQuest(3, false);
    }

    public static void SaveProgress()
    {
        //var player = GameObject.Find("Player").GetComponent<VIDEDemoPlayer>();

        //List<string> items = player.demo_ItemInventory;
        //PlayerPrefs.SetInt("interactedWith", interactedWith.Count);
        //PlayerPrefs.SetInt("cylinderGuyInteractions", cylinderGuyInteractions.Count);
        //PlayerPrefs.SetInt("example_ItemInventory", items.Count);

        //for (int i = 0; i < interactedWith.Count; i++)
        //{
        //    PlayerPrefs.SetString("interWith" + i.ToString(), interactedWith[i]);
        //}

        //for (int i = 0; i < cylinderGuyInteractions.Count; i++)
        //{
        //    PlayerPrefs.SetInt("cylGuyInt" + i.ToString(), cylinderGuyInteractions[i]);
        //}

        //for (int i = 0; i < items.Count; i++)
        //{
        //    PlayerPrefs.SetString("item" + i.ToString(), items[i]);
        //}
    }

    public static void LoadProgress()
    {
        //var player = GameObject.Find("Player").GetComponent<VIDEDemoPlayer>();

        //if (!PlayerPrefs.HasKey("interactedWith")) return;

        //List<string> items = new List<string>(); ;

        //interactedWith = new List<string>();
        //cylinderGuyInteractions = new List<int>();

        //for (int i = 0; i < PlayerPrefs.GetInt("interactedWith"); i++)
        //{
        //    interactedWith.Add(PlayerPrefs.GetString("interWith" + i.ToString()));
        //}

        //for (int i = 0; i < PlayerPrefs.GetInt("cylinderGuyInteractions"); i++)
        //{
        //    cylinderGuyInteractions.Add(PlayerPrefs.GetInt("cylGuyInt" + i.ToString()));
        //}

        //for (int i = 0; i < PlayerPrefs.GetInt("example_ItemInventory"); i++)
        //{
        //    items.Add(PlayerPrefs.GetString("item" + i.ToString()));
        //}

        //player.demo_ItemInventory = items;
    }
}