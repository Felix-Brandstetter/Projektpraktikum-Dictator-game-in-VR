using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    [Tooltip("On first contact ignoring start delay.")]
    public UnityEvent OnLookStart;

    [Tooltip("After start delay.")]
    public UnityEvent OnInteractionStart;

    [Tooltip("If look is aborted during start delay.")]
    public UnityEvent OnLookAbort;

    [Tooltip("When object is left after valid interaction.")]
    public UnityEvent OnInteractionEnd;

    [SerializeField]
    private bool animateCursor = true;

    [Range(0, 10)]
    public float StartDelay = 0;

    protected InputHandlerState state = InputHandlerState.Inactive;
    protected int counter = 0;

    protected void FixedUpdate()
    {
        switch (state)
        {
            case InputHandlerState.Inactive:
                break;
            case InputHandlerState.Waiting:
                state = InputHandlerState.CheckWaiting;
                break;
            case InputHandlerState.CheckWaiting:
                state = InputHandlerState.Inactive;
                OnLookAbort.Invoke();
                TriggerCursor("Abort");
                counter = 0;
                break;
            case InputHandlerState.Active:
                state = InputHandlerState.CheckActivity;
                break;
            case InputHandlerState.CheckActivity:
                state = InputHandlerState.Inactive;
                counter = 0;
                OnInteractionEnd.Invoke();
                break;
        }
    }

    public void Trigger()
    {
        if (this.enabled)
        {
            counter++;

            if (state == InputHandlerState.Inactive)
            {
                OnLookStart.Invoke();
                TriggerCursor("Load");
            }

            switch (state)
            {
                case InputHandlerState.Inactive:
                case InputHandlerState.CheckWaiting:
                case InputHandlerState.Waiting:
                    if (Time.fixedDeltaTime * counter > StartDelay)
                    {
                        state = InputHandlerState.Active;
                        StartCoroutine(OnInteractionStartCoroutine());
                        TriggerCursor("Click");
                    }
                    else
                    {
                        state = InputHandlerState.Waiting;
                    }
                    break;
                case InputHandlerState.Active:
                    break;
                case InputHandlerState.CheckActivity:
                    state = InputHandlerState.Active;
                    break;
            }
        }

    }

    protected IEnumerator OnInteractionStartCoroutine()
    {
        // Wait until Click-Animation ends
        yield return new WaitForSeconds(.4f);
        
        OnInteractionStart.Invoke();
    }

    protected enum InputHandlerState
    {
        Inactive = 0,
        Waiting = 1,
        CheckWaiting = 2,
        Active = 3,
        CheckActivity = 4,
    }

    protected void TriggerCursor(string trigger)
    {
        Animator cursorAnimator = Camera.main.GetComponent<Animator>();

        if (animateCursor && cursorAnimator != null)
        {
            ResetCursorTrigger(cursorAnimator);
            cursorAnimator.SetTrigger(trigger);
        }
    }

    public void ResetCursorTrigger(Animator cursorAnimator)
    {
        cursorAnimator.ResetTrigger("Load"); // Avoid double loading when quickly hovering over multiple targets
        cursorAnimator.ResetTrigger("Abort");

    }
}
