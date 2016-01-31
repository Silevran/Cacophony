using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour 
{
    [SerializeField]
    private GameObject player1 = null;
    [SerializeField]
    private GameObject player2 = null;
    [SerializeField]
    private GameObject winner = null;
    [SerializeField]
    private Text winnerText = null;

    [SerializeField]
    private SelectPlayers _selectPlayer = null;

    bool _gameOver = false;

    float timeWait = 2f;

    void OnTriggerEnter2D(Collider2D collide)
    {
        if(collide.gameObject.name == "LeftCollider")
        {
            player1.SetActive(false);
            player2.SetActive(false);
            winner.SetActive(true);
            winnerText.text = "Player 2 Wins";

            _gameOver = true;
        }
        else if (collide.gameObject.name == "RightCollider")
        {
            player1.SetActive(false);
            player2.SetActive(false);
            winner.SetActive(true);
            winnerText.text = "Player 1 Wins";

            _gameOver = true;
        }
    }

    void Update()
    {
        if(_gameOver)
        {
            timeWait -= Time.deltaTime;
            if (timeWait <= 0f)
            {
                if (Input.GetButtonDown("Player1Start") || Input.GetButtonDown("Player2Start"))
                {
                    player1.SetActive(true);
                    player2.SetActive(true);
                    winner.SetActive(false);

                    _selectPlayer.NewGame();
                }
            }
        }
    }
}
