using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsController : MonoBehaviour {

    public Sprite emptyHeartSprite;
    public Sprite fullHeartSprite;
    private int maxHealth;
    private List<Image> heartImages;
    private float heartSize = 70;

    public void SetupHearts(int maxHealth) {
        heartImages = new List<Image>();
        for (int i = 0; i < maxHealth; i++) {
            GameObject childGameObject = new GameObject("Heart");
            childGameObject.transform.parent = gameObject.transform;
            Image heartImage = childGameObject.AddComponent<Image>();
            RectTransform rectTransform = childGameObject.GetComponent<RectTransform>();
            RectTransform parentRectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetPositionAndRotation(new Vector3(i * heartSize * 0.8f + heartSize / 2, parentRectTransform.position.y, parentRectTransform.position.z), Quaternion.identity);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, heartSize);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heartSize);
            heartImage.sprite = fullHeartSprite;
            heartImages.Add(heartImage);
        }
    }

    public void SetNumberOfHearts(int health, int maxHealth) {
        for (int i = 0; i < maxHealth; i++) {
            if (i >= health) {
                heartImages[i].sprite = emptyHeartSprite;
            } else {
                heartImages[i].sprite = fullHeartSprite;
            }
        }
    }
}
