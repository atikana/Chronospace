using UnityEngine;
using UnityEngine.UI;

public class TutorialTextControl : MonoBehaviour
{
    public Transform playerTransform;

    private GameObject text;
    public Image dashArrow;
    public Image timeWarpArrow;
    private bool animatingDashArrow = false;
    private float dashArrowAlpha = 0;
    private bool dashArrowFadeIn = true;
    private bool animatingTimeWarpArrow = false;
    private float timeWarpArrowAlpha = 0;
    private bool timeWarpArrowFadeIn = true;
    private float animationScale = 2f;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (playerTransform.position.z > -420 && playerTransform.position.z < -392)
        {
            ShowText(0);
        }
        else if (playerTransform.position.z > -387 && playerTransform.position.z < -352)
        {
            ShowText(1);
        }
        else if (playerTransform.position.z > -347 && playerTransform.position.z < -295)
        {
            ShowText(2);
        }
        else if (playerTransform.position.z > -291 && playerTransform.position.z < -242)
        {
            ShowText(3);
        }
        else if (playerTransform.position.z > -88 && playerTransform.position.z < -41)
        {
            ShowText(4);
            animatingDashArrow = false;
        }
        else if (playerTransform.position.z > 53 && playerTransform.position.z < 92)
        {
            ShowText(5);
            dashArrow.gameObject.SetActive(true);
            animatingDashArrow = true;
        }
        else if (playerTransform.position.z > 139 && playerTransform.position.z < 188)
        {
            ShowText(6);
            animatingDashArrow = false;
            animatingTimeWarpArrow = false;
        }
        else if (playerTransform.position.z > 301 && playerTransform.position.z < 336)
        {
            ShowText(7);
            timeWarpArrow.gameObject.SetActive(true);
            animatingTimeWarpArrow = true;
        }
        else if (playerTransform.position.z > 371 && playerTransform.position.z < 409)
        {
            ShowText(8);
            animatingTimeWarpArrow = false;
        }
        else if (playerTransform.position.z > 512 && playerTransform.position.z < 621)
        {
            ShowText(9);
        }

        AnimateArrows();
    }

    private void AnimateArrows()
    {
        // Animate the dash arrow.
        if (animatingDashArrow || dashArrowAlpha > 0)
        {
            if (dashArrowFadeIn)
            {
                dashArrowAlpha += animationScale * Time.fixedDeltaTime;
            }
            else
            {
                dashArrowAlpha -= animationScale * Time.fixedDeltaTime;
            }

            dashArrowAlpha = Mathf.Clamp(dashArrowAlpha, 0, 1);

            // Switch directions.
            if (dashArrowAlpha == 0 || dashArrowAlpha == 1)
            {
                dashArrowFadeIn = !dashArrowFadeIn;
            }
        }
        else if (dashArrowAlpha == 0)
        {
            dashArrow.gameObject.SetActive(false);
        }

        if (dashArrow.gameObject.activeInHierarchy)
        {
            dashArrow.color = new Color(dashArrow.color.r, dashArrow.color.g, dashArrow.color.b, dashArrowAlpha);
        }

        // Animate the time warp arrow.
        if (animatingTimeWarpArrow || timeWarpArrowAlpha > 0)
        {
            if (timeWarpArrowFadeIn)
            {
                timeWarpArrowAlpha += animationScale * Time.fixedDeltaTime;
            }
            else
            {
                timeWarpArrowAlpha -= animationScale * Time.fixedDeltaTime;
            }

            timeWarpArrowAlpha = Mathf.Clamp(timeWarpArrowAlpha, 0, 1);

            // Switch directions.
            if (timeWarpArrowAlpha == 0 || timeWarpArrowAlpha == 1)
            {
                timeWarpArrowFadeIn = !timeWarpArrowFadeIn;
            }
        }
        else if (timeWarpArrowAlpha == 0)
        {
            timeWarpArrow.gameObject.SetActive(false);
        }

        if (timeWarpArrow.gameObject.activeInHierarchy)
        {
            timeWarpArrow.color = new Color(timeWarpArrow.color.r, timeWarpArrow.color.g, timeWarpArrow.color.b, timeWarpArrowAlpha);
        }
    }

    private void ShowText(int index)
    {
        if (text != null && text.activeInHierarchy)
        {
            text.SetActive(false);
        }
        text = transform.GetChild(index).gameObject;
        text.SetActive(true);
    }
}



