using UnityEngine;

public class MusicControl : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string music = "event:/Music/Survival_Shooter";

    private FMOD.Studio.EventInstance musicEv;

    void Start()
    {
     
        musicEv = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEv.start();
    }

    //Player is less than 50% health
    public void IsUnderHalfHealthMusic()
    {
        musicEv.setParameterByName("IsUnderHalfHealth", 1f);
    }
    //PlayerHealth <=0 AKA: GAME OVER
    public void IsDeadMusic()
    {
        musicEv.setParameterByName("IsDead", 1f);
    }

    public void StopMusic()
    {
        if (musicEv.isValid())
        {
            musicEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEv.release();
        }
    }
}