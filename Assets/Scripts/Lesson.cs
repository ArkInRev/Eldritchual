using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lesson", menuName ="Lesson")]
public class Lesson : ScriptableObject {

    public string lessonTitle;
    [TextArea(3,15)]
    public string lessonText;

    public Sprite lessonImage;



}
