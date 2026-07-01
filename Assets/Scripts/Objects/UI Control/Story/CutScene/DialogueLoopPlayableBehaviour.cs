using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueLoopPlayableBehaviour : PlayableBehaviour
{
    public CutscenePauseMethod pauseMethod = CutscenePauseMethod.Pause;
    bool firstFrame = true;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (firstFrame)
        {
            Debug.Log("ProcessFrame called. Pause Method: " + pauseMethod);

            switch (pauseMethod)
            {
                case CutscenePauseMethod.Loop:
                    var start = 0f;
                    var end = (float)playable.GetDuration();
                    Debug.Log($"Looping from start: {start} to end: {end}");

                    if (Cutscene.ActiveCutscene != null)
                    {
                        Cutscene.ActiveCutscene.Loop(start, end, withOffset: false);
                        Debug.Log("Cutscene loop activated.");
                    }
                    else
                    {
                        Debug.LogWarning("Cutscene.ActiveCutscene is null.");
                    }
                    break;

                case CutscenePauseMethod.Pause:
                    if (Cutscene.ActiveCutscene != null)
                    {
                        Cutscene.ActiveCutscene.Pause();
                        Debug.Log("Cutscene paused.");
                    }
                    else
                    {
                        Debug.LogWarning("Cutscene.ActiveCutscene is null.");
                    }
                    break;

                default:
                    Debug.LogWarning("Unknown pause method.");
                    break;
            }

            firstFrame = false;
        }
    }
}
