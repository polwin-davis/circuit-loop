using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        ScaleToScreen();
    }

    // Scales the background sprite to fill the camera view
    void ScaleToScreen()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Camera cam = Camera.main;

        if (sr == null || cam == null)
            return;

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteWidth = sr.sprite.bounds.size.x;

        transform.localScale = new Vector3(
            screenWidth / spriteWidth,
            screenHeight / spriteHeight,
            1f
        );
    }
}
