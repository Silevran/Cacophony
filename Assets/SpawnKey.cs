using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpawnKey : MonoBehaviour 
{
    [SerializeField]
    private GameObject _self = null;
    [SerializeField]
    private GameObject _spawn = null;
    [SerializeField]
    private Transform _spawnParent = null;
    [SerializeField]
    KeyboardControl _control = null;
    [SerializeField]
    private char _index = '0';
    [SerializeField]
    private Image _button = null;
    [SerializeField]
    private Text _activationCount = null;
    [SerializeField]
    private RectTransform _activationTimer = null;
    float _baseHeight = 0f;

    private bool _spawned = false;

    private float _resetTime = 5f;
    private float _timer = 0f;

    private bool _disabled = false;

    public int soundIndex = 0;

    private Color _activeColour   = new Color(1f, 1f, 1f, 1f);
    private Color _inactiveColour = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color _disabledColour = new Color(0.1f, 0.1f, 0.1f, 1f);

    void Start()
    {
        _control.RegisterKey(this, _index);

        _baseHeight = _activationTimer.rect.height;
        _activationTimer.gameObject.SetActive(false);
    }

    void Update()
    {
        if(_disabled)
        {
            _timer += Time.deltaTime;
            if(_timer > _resetTime)
            {
                _disabled = false;
                _timer = 0f;
                _button.color = _inactiveColour;
            }
        }
    }

    public void UpdateActivationInfo(float percentage, int amount)
    {
        _activationCount.text = amount.ToString();
        _activationTimer.sizeDelta = new Vector2(_activationTimer.rect.width, _baseHeight * percentage);
    }

    public void Disable()
    {
        _disabled = true;
        _button.color = _disabledColour;
    }

    public void SetEnabled(bool enabled)
    {
        if(_disabled)
        {
            return;
        }

        if(enabled && !_spawned)
        {
            _button.color = _activeColour;
        }
        else
        {
            _button.color = _inactiveColour;
        }
    }

    public void SpawnKeyObject()
    {
        if (!_spawned)
        {
            GameObject obj = GameObject.Instantiate(_spawn);
            obj.transform.SetParent(_spawnParent);

            obj.transform.position = _self.transform.position;
            obj.transform.localScale = Vector3.one;

            SpawnedObject so = obj.GetComponent<SpawnedObject>();
            so.SetStartPoint(obj.transform.position);
            so.spawner = this;
            so.SetController(_control);
            so.soundIndex = soundIndex;

            _spawned = true;

            _activationTimer.gameObject.SetActive(true);
        }
    }

    public void Respawn()
    {
        _spawned = false;
        _activationTimer.gameObject.SetActive(false);
    }

    public bool HasSpawned()
    {
        return _spawned;
    }

    public void NewGame()
    {
        _spawned = false;

        _resetTime = 5f;
        _timer = 0f;

        _disabled = false;

        soundIndex = 0;
    }
}
