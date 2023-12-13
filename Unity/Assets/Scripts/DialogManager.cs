using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using TMPro;//TextMeshPro
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{//Displays a Selection of Strings on a Corresponding canvas.
    [Header("Text object to display text")]
    [SerializeField] private TextMeshProUGUI dialogText;// Textfield where the text should appear

    [Header("Content")]
    [SerializeField] private string[] dialogSentences;// The short Phrases of Text, which should be shown, one after onother. [keep in mind, that the text fits on the Textfield]
    [SerializeField] private AudioClip[] audioClips;// The short Phrases of Text, which should be shown, one after onother. [keep in mind, that the text fits on the Textfield]


    [Header("Gameobject to trigger the next Dialuge optional")]
    [SerializeField] private GameObject continueButton;// Gameobject which should trigger the next Phase shown.

    [Header("Animation Controllers")]
    [SerializeField] private Animator bubbleAnimator;

    [SerializeField] private UnityEvent onActionFinished;

    [SerializeField]
    [Tooltip("Activate gameobject on Action Finished.")]
    private GameObject[] activateOnActionFinished;

    public enum ContinueType { ContinueAutomatically, ButtonPress, ButtonPressOnLast, Both, }
    [SerializeField] private ContinueType contiueTrigger = ContinueType.ButtonPress;

    public TypingSpeedFactory setTypingSpeedBy;//Needs to be public

    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("Delay Speech Bubble start.")]
    private float startDelay = 0f;

    [Range(0f, 10f)]
    [Tooltip("Delay text start after speech bubble appearance.")]
    public float textAnimationDelay = 0.5f;

    private int index = 0;
    private TypingSpeedStyle typingSpeed = null;
    public void Start()
    {// Wrapper for StartDialoge
        typingSpeed = setTypingSpeedBy.CreateAbility();

        StartCoroutine(StartDialog());
    }

    public void OnValidate()
    {
        if (setTypingSpeedBy.type == TypingSpeedFactory.TypingSpeedOptions.AudioLength)
        {
            if (dialogSentences.Length > audioClips.Length)
            {
                throw new MissingReferenceException("Cannot use Audio length typing style without using one audio clip per sentence.");
            }
        }
        
    }

    private IEnumerator StartDialog()
    {// Opens the Dialog Box and Shows the first Dialoge

        yield return new WaitForSeconds(startDelay);

        switch (contiueTrigger)
        {
            case ContinueType.Both:
                continueButton.SetActive(true);
                break;
            default:
                continueButton.SetActive(false);
                break;
        }
        bubbleAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(textAnimationDelay);// wait for animation to finish
        StartCoroutine(TypeDialog());
    }
    private IEnumerator TypeDialog()
    {
        int charactersVisible = 0;

        AudioClip currentAudioClip = null;
        if (audioClips != null && audioClips.Length > index)
        {
            currentAudioClip = audioClips[index];
        }
        
        float period = typingSpeed.getTimeBetweenCharacters(dialogSentences[index].Length, currentAudioClip);
        int textLength = dialogSentences[index].ToCharArray().Length;
        dialogText.maxVisibleCharacters = charactersVisible;
        dialogText.text = dialogSentences[index];
        PlayNextAudioClip();

        while (charactersVisible < textLength)
        {
            dialogText.maxVisibleCharacters = ++charactersVisible;
            yield return new WaitForSeconds(period);
        }

        yield return new WaitForSeconds(typingSpeed.timeRemaining());

        switch (contiueTrigger)
        {
            case ContinueType.ContinueAutomatically:
            case ContinueType.Both:
                TriggerContinueDialog();
                break;
            case ContinueType.ButtonPressOnLast:
                if (index < dialogSentences.Length - 1)
                {
                    TriggerContinueDialog();
                }
                else
                {
                    continueButton.SetActive(true);
                }
                break;
            case ContinueType.ButtonPress:
                continueButton.SetActive(true);
                break;
            default:
                Debug.Log("DefaultCaseHapped");
                break;

        }
    }
    private IEnumerator ContinueDialog()
    {
        // Closes the current Dialog and opens the next one if the end isn't reached yet
        if (index < dialogSentences.Length - 1)
        {
            if (contiueTrigger != ContinueType.Both) continueButton.SetActive(false);
            bubbleAnimator.SetTrigger("Close");
            yield return new WaitForSeconds(textAnimationDelay);
            dialogText.text = "";
            bubbleAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(textAnimationDelay);
            index++;
            dialogText.text = string.Empty;
            StartCoroutine(TypeDialog());
        }
        else
        {
            dialogText.text = string.Empty;
            bubbleAnimator.SetTrigger("Close");
            StartCoroutine(ActionFinished());
        }
    }
    public void TriggerContinueDialog()
    {
        //Wrapper for ContinueDialog()
        StartCoroutine(ContinueDialog());
    }
    public IEnumerator ActionFinished()
    {
        yield return new WaitForSeconds(.5f);
        onActionFinished.Invoke();

        foreach (GameObject gameObject in activateOnActionFinished)
        {
            gameObject.SetActive(true);
        }
    }

    private void PlayNextAudioClip()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        if (index < audioClips.Length)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClips[index]);
        }
    }

}
[System.Serializable]
public class TypingSpeedFactory
{
    // factory to allow different styles of input Speed.
    // have a look at https://www.linkedin.com/pulse/unity-hack-dynamic-property-field-inspector-zhen-stephen-gou
    public enum TypingSpeedOptions
    {
        TypingSpeed,
        AudioLength,
        SetTime,
    }
    public TypingSpeedOptions type = TypingSpeedOptions.TypingSpeed;

    //make sure the variable names here match the type, needed for reflection.
    public TypingSpeed TypingSpeed = new TypingSpeed();
    public AudioLength AudioLength = new AudioLength();
    public SetTime SetTime = new SetTime();

    public TypingSpeedStyle CreateAbility()
    {
        return GetAbilityFromType(type);
    }

    public System.Type GetClassType(TypingSpeedOptions typingSpeedOptions)
    {
        return GetAbilityFromType(typingSpeedOptions).GetType();
    }

    private TypingSpeedStyle GetAbilityFromType(TypingSpeedOptions type1)
    {
        switch (type1)
        {
            case TypingSpeedOptions.TypingSpeed:
                return TypingSpeed;
            case TypingSpeedOptions.AudioLength:
                return AudioLength;
            case TypingSpeedOptions.SetTime:
                return SetTime;
            default:
                return TypingSpeed;
        }
    }
}
[System.Serializable]
public abstract class TypingSpeedStyle
{
    [SerializeField] protected float timeWaitAfterFullTextDisplayed = 0.0f;
    public abstract float getTimeBetweenCharacters(int messageLength, AudioClip currentAudioClip = null);
    public virtual float timeRemaining()
    {
        return timeWaitAfterFullTextDisplayed;
    }
}
[System.Serializable]
public class TypingSpeed : TypingSpeedStyle
{
    [SerializeField] private float TimeBetweenCharacters = 0.05f;

    public override float getTimeBetweenCharacters(int messageLength, AudioClip currentAudioClip = null)
    {
        return TimeBetweenCharacters;
    }
}
[System.Serializable]
public class AudioLength : TypingSpeedStyle
{
    [SerializeField] private float timeToFinishBeforeVideoEnd = .5f;//in s
    public override float getTimeBetweenCharacters(int messageLength, AudioClip currentAudioClip = null)
    {
        if (currentAudioClip == null)
        {
            throw new System.NullReferenceException("Cannot use Audio length typing style without using audio.");
        }
        float finishtime = currentAudioClip.length - timeToFinishBeforeVideoEnd;
        if (finishtime < 0) finishtime = 0;
        return finishtime / (float)messageLength;
    }
    public override float timeRemaining()
    {
        return timeToFinishBeforeVideoEnd + timeWaitAfterFullTextDisplayed;
    }
}
[System.Serializable]
public class SetTime : TypingSpeedStyle
{
    [SerializeField] private float totalTime = 15.0f;

    public override float getTimeBetweenCharacters(int messageLength, AudioClip currentAudioClip = null)
    {
        return totalTime / (float)messageLength;
    }
}
