using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loading : MonoBehaviour
{
    private float time;
    [SerializeField]
    private Image fillImage;

    //start is called before the first frame update
    void Start()
    {
        time = 0f;
        Invoke("LoadGame", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 5)
        {
            time += Time.deltaTime;
            if(fillImage != null)
            {
                fillImage.fillAmount = time / 5;
            }
        }
    }    
     public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
    
}
