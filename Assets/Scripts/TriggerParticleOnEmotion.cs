using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoodMe;

public class TriggerMultipleParticlesOnEmotion : MonoBehaviour
{
    [System.Serializable]

    public class EmotionParticleEffect
    {
        public GetEmotionValue.EmotionEnum emotion;  // The emotion that triggers the particle system
        public ParticleSystem particleSystem;        // The particle system to trigger
        public float emotionThreshold = 0.7f;        // The threshold for this emotion to trigger the effect
    }

    // A list of particle effects, each tied to a specific emotion and threshold
    public List<EmotionParticleEffect> emotionParticleEffects;
    private bool particlesOn = false;

    // Update is called once per frame
    void Update()
    {
        if (particlesOn)
        {
            // Loop through each emotion-particle pair
            foreach (EmotionParticleEffect effect in emotionParticleEffects)
            {
                // Get the current value for the emotion
                float currentEmotionValue = GetEmotionValueFromManager(effect.emotion);

                // Trigger the particle system if the emotion exceeds the threshold
                if (currentEmotionValue >= effect.emotionThreshold)
                {
                    if (!effect.particleSystem.isPlaying)
                    {
                        effect.particleSystem.Play();
                    }
                }
                else
                {
                    if (effect.particleSystem.isPlaying)
                    {
                        effect.particleSystem.Stop();
                    }
                }
            }
        }
        else
        {
            return;
        }
    }

    public void SetParticlesToTrue()
    {
        particlesOn = true;
    }
    public void SetParticlesToFalse()
    {
        particlesOn = false;
    }

    // Helper method to get the emotion value from the EmotionsManager
    private float GetEmotionValueFromManager(GetEmotionValue.EmotionEnum emotion)
    {
        switch (emotion)
        {
            case GetEmotionValue.EmotionEnum.Angry:
                return EmotionsManager.Emotions.angry;
            case GetEmotionValue.EmotionEnum.Disgust:
                return EmotionsManager.Emotions.disgust;
            case GetEmotionValue.EmotionEnum.Happy:
                return EmotionsManager.Emotions.happy;
            case GetEmotionValue.EmotionEnum.Neutral:
                return EmotionsManager.Emotions.neutral;
            case GetEmotionValue.EmotionEnum.Sad:
                return EmotionsManager.Emotions.sad;
            case GetEmotionValue.EmotionEnum.Scared:
                return EmotionsManager.Emotions.scared;
            case GetEmotionValue.EmotionEnum.Surprised:
                return EmotionsManager.Emotions.surprised;
            default:
                return 0f;
        }
    }
}