using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsController : MonoBehaviour {

    public Sprite emptyHeartSprite;
    public Sprite fullHeartSprite;
    private List<SpriteRenderer> spriteRenderers;
    [HideInInspector]
    public int numberOfHearts;
    private PlayerController player;

    private void Awake()
    {
        spriteRenderers = new List<SpriteRenderer>();
    }

    public void SetupHearts(PlayerController player)
    {
        this.player = player;
        this.numberOfHearts = player.maxHealth;
        for (int i = 0; i < player.maxHealth; i++)
        {
            GameObject childGameObject = new GameObject("Heart");
            childGameObject.transform.parent = gameObject.transform;
            childGameObject.transform.position = new Vector3(-3,9,0) + new Vector3(i*1.5f, 0,0);
            SpriteRenderer spriteRenderer = childGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "UI";
            spriteRenderer.sprite = fullHeartSprite;
            spriteRenderers.Add(spriteRenderer);
        }
    }

    public void AddHeart() {
        numberOfHearts++;
        spriteRenderers[numberOfHearts - 1].sprite = fullHeartSprite;
    }

    public void SubtractHeart()
    {
        spriteRenderers[numberOfHearts - 1].sprite = emptyHeartSprite;
        numberOfHearts--;
        if (numberOfHearts == 0)
        {
            player.die();
        }
    }
}
