// TiltControl.cs
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;



public class TiltControl : MonoBehaviour
{
    public GameObject maze;     // Maze component
    public GameObject cam;   // main camera
    public TextMeshProUGUI scoreText;
    public GameObject winTextObject;  // text displayed when a player wins
    public GameObject againButton;        // button to play again


    public float sensitivity = 9.8F;

    private Rigidbody playerRb; // Player rigid body
    private float movementX;
    private float movementY;
    private int score;
    private int high_score;

    private GameObject[] collectibles;


    private Vector3 rotation;         // current Euler angle of the maze

    // Start is called before the first frame update
    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        score = 0;
        high_score = 0;

        winTextObject.SetActive(false);
        againButton.SetActive(false);

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            cam.transform.SetPositionAndRotation(
                new Vector3(0, 7, 0), Quaternion.Euler(90, 0, 0));
            playerRb.useGravity = true;
        }
        else
        {
            cam.transform.SetPositionAndRotation(
                new Vector3(0, 65, 0), Quaternion.Euler(90, 0, 0));
        }

        SetScoreText();
        Reset();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Destination Wall")
        {
            if(score > high_score)
            {
                high_score = score;
                SetScoreText();
            }
            winTextObject.SetActive(true);
            againButton.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            other.gameObject.SetActive(false);
            score += 100;
            SetScoreText();
        }
    }

    // FixedUpdate is called at a fixed interval. This is useful for physics
    // simulation and also for the Rigidbody update.
    private void FixedUpdate()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            // For mobile devices, we add the force to the player based on
            // the acceleration from the accelerometer
            playerRb.AddForce(
                new Vector3(Input.acceleration.x, 0, Input.acceleration.y)
                    * sensitivity);
        }
        else
        {
            Vector3 movement = new Vector3(
                Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
            rotation += movement;
            maze.transform.rotation = Quaternion.Euler(rotation);

            //playerRb.AddForce(movement * speed);
        }

        if (playerRb.position.y <= -30)
            Reset();
    }

    void SetScoreText()
    {
        scoreText.text = "Current Score: " + score.ToString() + "\n" + "High Score: " + high_score.ToString();
    }

    // Resets the state. This is called manually.
    public void Reset()
    {
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(true);
        }
        score = 0;
        SetScoreText();

        rotation = Vector3.zero;
        playerRb.transform.position = new Vector3(25f, 3f, 25f);
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        winTextObject.SetActive(false);
        againButton.SetActive(false);
    }
}