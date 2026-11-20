using UnityEngine;

// Helper de una linea para los efectos: si hay AudioSource lo usa, si no crea uno temporal
// en la posicion que se le pase. Asi un minijuego solo necesita el AudioClip.
public static class Sfx
{
    public static void Play(AudioClip clip, AudioSource source, Vector3 position, float volume = 1f)
    {
        if (clip == null)
            return;

        if (source != null)
            source.PlayOneShot(clip, volume);
        else
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
