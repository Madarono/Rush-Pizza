using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make X pizzas - Easy
// Make X money - Easy

// Don't lose more than X money - Easy
// Make X tip - Medium

// Don't make X customers leave - Easy
// Don't lower than X patience - Medium
// Make X customers happy - Medium

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}


[CreateAssetMenu(fileName = "Challenge", menuName = "Custom/Challenges")]
public class Challenge : ScriptableObject
{
    public Difficulty difficulty;
    public string englishVisual;
    public string deutschVisual;
    public int valueNeeded;
    public float reward;
    public int indexOfValue; //Take the index of DailyChallenge.cs stored values.
    
    [Header("Special Condition")]
    public bool lessThan; //Like in refunds, condiiton is met when it is less than
}