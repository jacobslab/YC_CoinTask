using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoPlay : MonoBehaviour
{

    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
    private VideoPlayer movie;
    //	MovieTexture movie;
    AudioSource movieAudio;

    public CanvasGroup group;

    void Awake()
    {
        group.alpha = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        movie = GetComponent<VideoPlayer>();
        movie.enabled = false;
        movieAudio = GetComponent<AudioSource>();
        movieAudio.enabled = false;
        //		RawImage rim = GetComponent<RawImage>();
        //		if(rim != null){
        //			if(rim.texture != null){
        //				movie = (MovieTexture)rim.mainTexture;
        //			}
        //		}
        //		movieAudio = GetComponent<AudioSource> ();
    }

    bool isMoviePaused = false;
    void Update()
    {
        		if (movie != null) {
        			if (movie.isPlaying) {
        				if (Input.GetAxis (Config_CoinTask.ActionButtonName) > 0.2f) { //skip movie!
        					Debug.Log("skip movie");
        					Stop ();
        				}
        				if (TrialController.isPaused) {
        					Pause ();
        				}
        			}
        			if (!TrialController.isPaused) {
        				if (isMoviePaused) {
        					UnPause ();
        				}
       			}
        		} 
        else {
        Debug.Log("No movie attached! Can't update.");
       }
    }

    bool shouldPlay = false;
    public IEnumerator Play()
    {
        if (movie.clip != null)
        {
            float clipLength = (float)movie.clip.length;
            Debug.Log("playing instruction video of length " + clipLength.ToString());
            yield return StartCoroutine(AskIfShouldPlay());

            if (shouldPlay)
            {
                Debug.Log("playing now");
                movie.enabled = true;
                movieAudio.enabled = true;
                group.alpha = 1.0f;

                //				movie.Stop ();
                //				movieAudio.Play ();
                //				movie.Play ();
                float timer = 0f;
                bool buttonPressed = false;
                while (timer < clipLength && !buttonPressed)
                {
                  //  Debug.Log("timer is:" + timer.ToString());
                    if (!TrialController.isPaused)
                    {
                        timer += Time.deltaTime;
                        movie.playbackSpeed = 1f;
                       // Debug.Log("playback speed set to one");
                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            buttonPressed = true;
                        }
                    }
                    else
                    {
                        Debug.Log("playback speed set to zero");
                        movie.playbackSpeed = 0f;
                    }
                    yield return 0;
                }
                //				movie.Stop ();
                movie.enabled = false;
                group.alpha = 0.0f;
            }
            yield return 0;
        }
        else
        {
            Debug.Log("No movie attached! Can't play.");
        }
    }

    IEnumerator AskIfShouldPlay()
    {
        exp.currInstructions.SetInstructionsColorful();
        exp.currInstructions.DisplayText("Play instruction video? (y/n)");
        Debug.Log("show instructions");
        bool isValidInput = false;
        while (!isValidInput)
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                isValidInput = true;
                shouldPlay = true;
            }
            else if (Input.GetKeyUp(KeyCode.N))
            {
                isValidInput = true;
                shouldPlay = false;
            }
            yield return 0;
        }

        exp.currInstructions.SetInstructionsBlank();
        exp.currInstructions.SetInstructionsTransparentOverlay();
    }

    void Pause()
    {
        if (movie != null)
        {
            movie.Pause();
            movieAudio.Pause();
            isMoviePaused = true;
        }
        else
        {
            Debug.Log("No movie attached! Can't pause.");
        }
    }

    void UnPause()
    {
        if (movie != null)
        {
            movie.Play();
            movieAudio.UnPause();
            isMoviePaused = false;
        }
        else
        {
            Debug.Log("No movie attached! Can't unpause.");
        }
    }

    void Stop()
    {
        if (movie != null)
        {
            isMoviePaused = false;
            movie.Stop();
        }
        else
        {
            Debug.Log("No movie attached! Can't stop.");
        }

        if(movieAudio!=null)
        {
            movieAudio.Stop();
            movieAudio.enabled = false;
        }
    }

}