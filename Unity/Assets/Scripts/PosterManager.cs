using UnityEngine;

public class PosterManager : MonoBehaviour
{
    // TODO: Fix error: Error executing result (An invalid seek position was passed to this function. ) UnityEngine.StackTraceUtility:ExtractStackTrace()
    // TODO: Sync audio with scale/position automation including start/stop
    // TODO: Animate lookAt (-> rotation)
    // TODO: Revert changes caused by lookAt

    // Config variables
    [SerializeField]
    private GameObject poster;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    [Tooltip("Set scaling factor.")]
    private float scaleBy;

    [SerializeField]
    [Tooltip("Set adjustment of the local position of this GameObject relative to the parent when scaling.")]
    private Vector3 movBy;

    [SerializeField]
    [Tooltip("Set animation duration in seconds")]
    private float animationDuration;

    // Other variables
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector3 targetScale;
    private Vector3 targetPosition;

    private Animation anim;
    private AnimationClip clip;
    // private Animator animator;

    private AudioSource audioSource;
    private int initialAudioTimeSamples;

    private bool lookedAt = false;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = poster.transform.localScale;
        initialPosition = poster.transform.localPosition;

        InitAnimation();

        // animator = poster.GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        initialAudioTimeSamples = audioSource.clip.samples;
    }

    // Update is called once per frame
    void Update()
    {
        if (lookedAt)
        {
            poster.transform.LookAt(player.transform);
            poster.transform.Rotate(90, 0, 0);
        }
    }

    public void ZoomIn()
    {
        lookedAt = true;

        ManipulateAnimationForward(poster.transform);
        anim.AddClip(clip, clip.name);
        anim.wrapMode = WrapMode.ClampForever;
        anim.Play(clip.name);

        // Vector3 newScale = initialScale * scaleBy;
        // poster.transform.localScale = newScale;
        // animator.SetTrigger("ZoomIn");

        audioSource.timeSamples = initialAudioTimeSamples;
        audioSource.pitch = 1;
        audioSource.Play(0);
    }

    public void ZoomOut()
    {
        lookedAt = false;

        ManipulateAnimationBackward(poster.transform);
        // Reverse Animation Play
        // anim[clip.name].speed = -1;
        // anim[clip.name].time = anim[clip.name].length;
        // anim.Play(clip.name);
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);

        // poster.transform.localScale = initialScale;
        // animator.SetTrigger("ZoomOut");

        audioSource.timeSamples = audioSource.clip.samples - 1;
        audioSource.pitch = -1;
        audioSource.Play();
    }

    public void InitAnimation()
    {
        anim = GetComponent<Animation>();

        clip = new AnimationClip
        {
            legacy = true
        };

        // Scale animation
        targetScale = initialScale * scaleBy;
        clip.SetCurve("", typeof(Transform), "localScale.x", AnimationCurve.EaseInOut(0, initialScale.x, animationDuration, initialScale.x * scaleBy));
        clip.SetCurve("", typeof(Transform), "localScale.y", AnimationCurve.EaseInOut(0, initialScale.y, animationDuration, initialScale.y * scaleBy));
        clip.SetCurve("", typeof(Transform), "localScale.z", AnimationCurve.EaseInOut(0, initialScale.z, animationDuration, initialScale.z * scaleBy));
 
        // Position Animation
        targetPosition = initialPosition + movBy;
        clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.EaseInOut(0, initialPosition.x, animationDuration, targetPosition.x));
        clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.EaseInOut(0, initialPosition.y, animationDuration, targetPosition.y));
        clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.EaseInOut(0, initialPosition.z, animationDuration, targetPosition.z));
    }

    public void ManipulateAnimationForward(Transform currTransform)
    {
        // Calculate the remaining animation time based on the scaling progress
        Vector3 currScale = currTransform.localScale;
        float remainingProgress;
        float remainingDuration;

        if (targetScale.x - currScale.x != 0)
        {
            remainingProgress = Mathf.Abs(targetScale.x - currScale.x) / Mathf.Abs(targetScale.x - initialScale.x);
        } else if (targetScale.y - currScale.y != 0) {
            remainingProgress = Mathf.Abs(targetScale.y - currScale.y) / Mathf.Abs(targetScale.y - initialScale.y);
        } else if (targetScale.z - currScale.z != 0)
        {
            remainingProgress = Mathf.Abs(targetScale.z - currScale.z) / Mathf.Abs(targetScale.z - initialScale.z);
        } else
        {
            remainingProgress = 0f;
        }

        remainingDuration = animationDuration * remainingProgress;

        // Manipulate scale animation
        clip.SetCurve("", typeof(Transform), "localScale.x", AnimationCurve.EaseInOut(0, currScale.x, remainingDuration, targetScale.x));
        clip.SetCurve("", typeof(Transform), "localScale.y", AnimationCurve.EaseInOut(0, currScale.y, remainingDuration, targetScale.y));
        clip.SetCurve("", typeof(Transform), "localScale.z", AnimationCurve.EaseInOut(0, currScale.z, remainingDuration, targetScale.z));

        // Manipulate position animation
        Vector3 curPosition = currTransform.position;
        clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.EaseInOut(0, curPosition.x, remainingDuration, targetPosition.x));
        clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.EaseInOut(0, curPosition.y, remainingDuration, targetPosition.y));
        clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.EaseInOut(0, curPosition.z, remainingDuration, targetPosition.z));
    }

    public void ManipulateAnimationBackward(Transform currTransform)
    {
        // Calculate the remaining animation time based on the scaling regress
        Vector3 currScale = currTransform.localScale;
        float remainingRegress;
        float remainingDuration;


        if (initialScale.x - currScale.x != 0)
        {
            remainingRegress = Mathf.Abs(initialScale.x - currScale.x) / Mathf.Abs(initialScale.x - targetScale.x);
        }
        else if (initialScale.y - currScale.y != 0)
        {
            remainingRegress = Mathf.Abs(initialScale.y - currScale.y) / Mathf.Abs(initialScale.y - targetScale.y);
        }
        else if (initialScale.z - currScale.z != 0)
        {
            remainingRegress = Mathf.Abs(initialScale.z - currScale.z) / Mathf.Abs(initialScale.z - targetScale.z);
        }
        else
        {
            remainingRegress = 0f;
        }

        remainingDuration = animationDuration * remainingRegress;

        // Manipulate scale animation
        clip.SetCurve("", typeof(Transform), "localScale.x", AnimationCurve.EaseInOut(0, currScale.x, remainingDuration, initialScale.x));
        clip.SetCurve("", typeof(Transform), "localScale.y", AnimationCurve.EaseInOut(0, currScale.y, remainingDuration, initialScale.y));
        clip.SetCurve("", typeof(Transform), "localScale.z", AnimationCurve.EaseInOut(0, currScale.z, remainingDuration, initialScale.z));

        // Manipulate position animation
        Vector3 curPosition = currTransform.position;
        clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.EaseInOut(0, curPosition.x, remainingDuration, initialPosition.x));
        clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.EaseInOut(0, curPosition.y, remainingDuration, initialPosition.y));
        clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.EaseInOut(0, curPosition.z, remainingDuration, initialPosition.z));
    }
}
