using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TutorialPoints : MonoBehaviour
{
    [Header("Talking Points")]
    [SerializeField] private Text talkingPoint1;
    [SerializeField] private Text talkingPoint2;
    [SerializeField] private Text talkingPoint3;
    [SerializeField] private Text talkingPoint4;
    
    [Header("Stats Text")]
    [SerializeField] private Text goldText;
    [SerializeField] private Text jibbitText;
    [SerializeField] private Text healthText;
    
    [Header("Instructions")]
    [SerializeField] private string instruction1;
    [SerializeField] private string instruction2;
    [SerializeField] private string instruction3;
    [SerializeField] private string instruction4;
    
    [Header("Stats Instructions")]
    [SerializeField] private string gold;
    [SerializeField] private string jibbit;
    [SerializeField] private string health;

    private bool starting = true;
    private void Start()
    {
        StartCoroutine(StartingSequence());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this != null)
        {
            if (other.CompareTag("Player"))
            {
                if (!starting)
                {
                    talkingPoint1.text = instruction1 + "";
                    talkingPoint2.text = instruction2 + "";
                    talkingPoint3.text = instruction3 + "";
                    talkingPoint4.text = instruction4 + "";

                    jibbitText.text = jibbit + "";
                    goldText.text = gold + "";
                    healthText.text = health + "";
                }
                
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (this != null)
        {
            if (other.CompareTag("Player"))
            {
                talkingPoint1.text =  "";
                talkingPoint2.text =  "";
                talkingPoint3.text =  "";
                talkingPoint4.text =  "";

                jibbitText.text =  "";
                goldText.text =  "";
                healthText.text =  "";
            }
        }
        
    }

    private  IEnumerator StartingSequence()
    {
        talkingPoint1.text = "Welcome to Croc-Stars!";
        yield return new WaitForSeconds(1.5f);
        talkingPoint1.text = "Lets Get Started";
        yield return new WaitForSeconds(1.5f);
        talkingPoint1.text = "Use 'A' and 'D' to Move Left and Right";
        talkingPoint2.text = "'SpaceBar' to Jump";
        starting = false;
    }
}
