using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HPBar : MonoBehaviour 
{
    private char[] intToCharType = { 'A', 'B', 'X', 'Y' }; 

    [SerializeField]
    private KeyboardControl p1 = null;
    [SerializeField]
    private KeyboardControl p2 = null;

    [SerializeField]
    private RectTransform centerPosObject = null;
    private float _damageToDo = 0f;

    private float _originalPosition = 0f;

    [SerializeField]
    private List<GameObject> _p1DamageObject = new List<GameObject>();
    private Dictionary<char, int> _p1DamageWaiting = new Dictionary<char, int>();
    public List<bool> _p1Attacking = new List<bool>();

    [SerializeField]
    private List<GameObject> _p2DamageObject = new List<GameObject>();
    private Dictionary<char, int> _p2DamageWaiting = new Dictionary<char, int>();
    public List<bool> _p2Attacking = new List<bool>();

    int CharToType(char index)
    {
        switch (index)
        {
            case 'A':
                return 0;
            case 'B':
                return 1;
            case 'X':
                return 2;
            case 'Y':
                return 3;
            default:
                return -1;
        }
    }

    void Start()
    {
        _p1DamageWaiting.Add('A', 0);
        _p1DamageWaiting.Add('B', 0);
        _p1DamageWaiting.Add('X', 0);
        _p1DamageWaiting.Add('Y', 0);

        _p2DamageWaiting.Add('A', 0);
        _p2DamageWaiting.Add('B', 0);
        _p2DamageWaiting.Add('X', 0);
        _p2DamageWaiting.Add('Y', 0);

        for(int i = 0; i < _p1DamageWaiting.Count; i++)
        {
            _p1Attacking.Add(false);
            _p2Attacking.Add(false);
        }

        _originalPosition = centerPosObject.localPosition.x;
    }

    public void PrepareDamage(int player, int damage, char index)
    {
        int convIndex = CharToType(index);

        if(player == 1)
        {
            _p1DamageWaiting[index] = damage;
            if (damage > 0)
            {
                _p1DamageObject[convIndex].SetActive(true);
                RectTransform trans = (RectTransform)_p1DamageObject[convIndex].transform;
                trans.sizeDelta = new Vector2(2 + _p1DamageWaiting[index], trans.rect.height);
            }
            else
            {
                _p1DamageObject[convIndex].SetActive(false);
            }
        }
        else if (player == 2)
        {
            _p2DamageWaiting[index] = damage;
            if (damage > 0)
            {
                _p2DamageObject[convIndex].SetActive(true);
                RectTransform trans = (RectTransform)_p2DamageObject[convIndex].transform;
                trans.sizeDelta = new Vector2(2 + _p2DamageWaiting[index], trans.rect.height);
            }
            else
            {
                _p2DamageObject[convIndex].SetActive(false);
            }
        }
        UpdateRegions();
    }

    public void RemoveDamage(int player, char type)
    {
        int index = CharToType(type);

        if(player == 1)
        {
            _p1DamageObject[index].SetActive(false);
        }
        else if (player == 2)
        {
            _p2DamageObject[index].SetActive(false);
        }
    }

    private void UpdateRegions()
    {
        float xPos = centerPosObject.localPosition.x;
        float width = centerPosObject.rect.width;

        for(int i = _p1DamageObject.Count-1; i >= 0; i--)
        {
            if (!_p1Attacking[i])
            {
                if (_p1DamageObject[i].activeSelf)
                {
                    RectTransform trans = (RectTransform)_p1DamageObject[i].transform;
                    Vector3 pos = trans.localPosition;
                    pos.x = xPos - trans.rect.width / 2;
                    trans.localPosition = pos;

                    xPos -= trans.rect.width;
                }
            }
        }

        xPos = centerPosObject.localPosition.x;

        for (int i = _p2DamageObject.Count - 1; i >= 0; i--)
        {
            if (!_p2Attacking[i])
            {
                if (_p2DamageObject[i].activeSelf)
                {
                    RectTransform trans = (RectTransform)_p2DamageObject[i].transform;
                    Vector3 pos = trans.localPosition;
                    pos.x = xPos + trans.rect.width / 2 + width;
                    trans.localPosition = pos;

                    xPos += trans.rect.width;
                }
            }
        }
    }

    public void Attack(int player, char type)
    {
        int index = CharToType(type);

        if (player == 1)
        {
            _p1Attacking[index] = true;
        }
        else if(player == 2)
        {
            _p2Attacking[index] = true;
        }
    }

    void Update()
    {
        if(_damageToDo != 0)
        {
            Vector3 pos = centerPosObject.localPosition;

            float newPos = Mathf.Lerp(pos.x, pos.x + _damageToDo, Time.deltaTime);
            _damageToDo -= (newPos - pos.x);

            pos.x = newPos;
            centerPosObject.localPosition = pos;

            if(Mathf.Abs(_damageToDo) < 0.1f)
            {
                _damageToDo = 0f;
            }

            UpdateRegions();
        }

        for (int i = 0; i < _p1Attacking.Count; i++)
        {
            if(_p1Attacking[i])
            {
                _p1DamageObject[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(2000f, 0f));
            }
        }

        for (int i = 0; i < _p2Attacking.Count; i++)
        {
            if (_p2Attacking[i])
            {
                _p2DamageObject[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(-2000f, 0f));
            }
        }
    }

    public void AttackDone(int player, int type)
    {
        if(player == 1)
        {
            _p1DamageObject[type].SetActive(false);
            p1.AttackDone(intToCharType[type]);

            _damageToDo += _p1DamageWaiting[intToCharType[type]];
            _p1DamageWaiting[intToCharType[type]] = 0;
            _p1Attacking[type] = false;
        }
        else if(player == 2)
        {
            _p2DamageObject[type].SetActive(false);
            p2.AttackDone(intToCharType[type]);

            _damageToDo -= _p2DamageWaiting[intToCharType[type]];
            _p2DamageWaiting[intToCharType[type]] = 0;
            _p2Attacking[type] = false;
        }
    }

    public void NewGame()
    {
        Vector3 pos = centerPosObject.localPosition;
        pos.x = _originalPosition;
        centerPosObject.localPosition = pos;

        foreach(char c in _p1DamageWaiting.Keys)
        {
            _p1DamageWaiting[c] = 0;
            _p2DamageWaiting[c] = 0;
        }

        for(int i = 0; i < _p1Attacking.Count; i++)
        {
            _p1Attacking[i] = false;
            _p2Attacking[i] = false;
            _p1DamageObject[i].SetActive(false);
            _p2DamageObject[i].SetActive(false);

            _p1DamageObject[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            _p2DamageObject[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
