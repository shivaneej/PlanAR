using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Mode : MonoBehaviour
{
    public ModeState state;

    [HideInspector]
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void OnModeEnabled()
    {
        canvasGroup.interactable = true;
        canvasGroup.blockRaycasts = true;
        canvasGroup.alpha = 1;
    }
    
    public virtual void OnModeDisabled()
    {
        canvasGroup.interactable = false;
        canvasGroup.blockRaycasts = false;
        canvasGroup.alpha = 0;   
    }
}

public enum ModeState
{
    PlaneMode,
    EditMode,
    None
}
