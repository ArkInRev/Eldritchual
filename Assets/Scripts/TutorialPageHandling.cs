using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.

public class TutorialPageHandling : MonoBehaviour {

    //public TMP_Text lessonTitle;
    public TMP_Text lessonTitle;
    public TMP_Text lessonText;
    public Image lessonImage; 

    public Lesson[] lessons;

    public Button nextLessonButton;
    public Button prevLessonButton;

    private int currentLesson;

	// Use this for initialization
	void Start () {
        currentLesson = 0;
        lessonTitle.text = lessons[currentLesson].lessonTitle;
        lessonText.text = lessons[currentLesson].lessonText;
        lessonImage.sprite = lessons[currentLesson].lessonImage;
	}
	
    public void nextLesson()
    {
        currentLesson++;
        isLastLesson();
        isFirstLesson();
        lessonTitle.text = lessons[currentLesson].lessonTitle;
        lessonText.text = lessons[currentLesson].lessonText;
        lessonImage.sprite = lessons[currentLesson].lessonImage;
    }

    public void prevLesson()
    {
        currentLesson--;
        isFirstLesson();
        isLastLesson();
        lessonTitle.text = lessons[currentLesson].lessonTitle;
        lessonText.text = lessons[currentLesson].lessonText;
        lessonImage.sprite = lessons[currentLesson].lessonImage;
    }

    public bool isFirstLesson()
    {

        if (currentLesson == 0)
        {
            //disable previous lesson button
            prevLessonButton.interactable = false;
            return true;
        } else {
            //enable previous lesson button
            prevLessonButton.interactable = true;
            return false;
        }
        
    }

    public bool isLastLesson()
    {
        if (currentLesson == (lessons.Length-1))
        {

            //disable NextLesson Button
            nextLessonButton.interactable = false;
            return true;
        }
        else
        {
            //enable next lesson button
            nextLessonButton.interactable = true;
            return false;
        }
    }




}
