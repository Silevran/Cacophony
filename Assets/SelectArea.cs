using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectArea : MonoBehaviour 
{
    [SerializeField]
    private Image _mainColour = null;
    [SerializeField]
    private Image _borderImage = null;

    private Color _baseColour;

    public char buttonType = '0';
    public bool isActive = false;
    public bool wasHit = false;
    private int interObjects = 0;

    void Start()
    {
        _baseColour = _mainColour.color;
    }

    //Set the area to the specified button colour
    public void SetColour(Color c)
    {
        _mainColour.color = c;
    }

    public void Reset(bool all)
    {
        if (all)
        {
            isActive = false;
            _mainColour.color = _baseColour;
            buttonType = '0';
        }
        
        interObjects = 0;
        _borderImage.enabled = false;

        wasHit = false;
    }

    public void ActivateArea(bool active, char type, Color newColour)
    {
        //Clearing area
        if (!active)
        {
            isActive = false;
            _mainColour.color = _baseColour;
            buttonType = '0';
            interObjects = 0;
            _borderImage.enabled = false;
        }

        //different button was hit first
        if(isActive && active)
        {
            return;
        }
        else if(active && buttonType == '0')
        {
            isActive = true;
            buttonType = type;
            _mainColour.color = newColour;
            wasHit = true;
            _borderImage.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        if(isActive)
        {
            return;
        }

        SpawnedObject so = collide.gameObject.GetComponent<SpawnedObject>();
        if(so != null)
        {
            if (so.CanActivate())
            {
                interObjects++;
                _borderImage.enabled = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collide)
    {
        if(isActive)
        {
            return;
        }

        SpawnedObject so = collide.gameObject.GetComponent<SpawnedObject>();
        if (so != null)
        {
            interObjects--;
            if (interObjects == 0)
            {
                _borderImage.enabled = false;
            }
        }
    }

    public void NewGame()
    {
        buttonType = '0';
        isActive = false;
        wasHit = false;
        interObjects = 0;
    }
}
