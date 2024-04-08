using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioRecorder : MonoBehaviour
{
    public AudioClip recordedClip;
    private AudioSource audioSource;
    private Button recordButton;
    private TMP_Text recordButtonText;
    private SavWav savWav;
    private HTTPRequests httpRequests;
    string device;
    float startTime;
    int count = 0;

    void Start()
    {
        // Check if microphone is available
        if (Microphone.devices.Length > 0)
        {
            device = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }

        recordButton = GameObject.Find("RecordButton").GetComponent<Button>();
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        recordButtonText = GameObject.Find("RecordDisplay").GetComponent<TMP_Text>();
        savWav = GetComponent<SavWav>();
        httpRequests = GetComponent<HTTPRequests>();
    }

    public void OnRecordButtonClicked()
    {
        if (Microphone.IsRecording(device))
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        // Start recording
        startTime = Time.time;
        // MAXIMUM RECORDING TIME: 10 seconds !!!
        recordedClip = Microphone.Start(device, true, 10, 16000); // lengthSec, frequency
        recordButtonText.text = "Recording...";
    }

    private void StopRecording()
    {
        // Stop recording
        Microphone.End(device);
        recordButtonText.text = "Recorded";

        // Calculate the duration of the recorded clip
        float duration = Time.time - startTime;

        // Trim the recorded clip to the actual duration
        int lengthSamples = (int)(duration * recordedClip.frequency);
        AudioClip trimmedClip = AudioClip.Create("RecordedClip", lengthSamples, recordedClip.channels, recordedClip.frequency, false);
        float[] data = new float[lengthSamples];
        recordedClip.GetData(data, 0);
        trimmedClip.SetData(data, 0);

        recordedClip = trimmedClip;

        // wait for 2 seconds
        Invoke("SaveClip", 2);
    }

    private void SaveClip()
    {
        // Save audio clip
        recordButtonText.text = "Save File";
        string recordingFileName = "recorded_"+PlayerPrefs.GetString("Username")+"_"+PlayerPrefs.GetString("LevelFilename")+"_"+count;
        string path = Application.persistentDataPath + "/" + recordingFileName + ".wav"; // C:/Users/(user)/AppData/LocalLow/DefaultCompany/CSPuzzle
        // string path = "./Assets/LevelsJSON/audio/" + recordingFileName + ".wav";
        SavWav.Save(path, recordedClip);
        Debug.Log("Saved audio clip to: " + path);
        
        count++;

        Invoke("PlayClip", 0.2f); // not from path, but from the recordedClip
        
        // wait for the clip to be saved
        SendHttpClip(path);
    }

    private void SendHttpClip(string path)
    {
        // Send audio clip to the server
        recordButtonText.text = "Sending...";
        // wait until file at path exists
        while (!System.IO.File.Exists(path))
        {
            Debug.Log("Waiting for file to exist: " + path);
        }
        StartCoroutine(httpRequests.SendAudioClipRequest(path));
        Debug.Log("Sent audio clip to the server: " + path);
    }

    private void PlayClip() 
    {
        // Play audio clip
        recordButtonText.text = "Playing...";
        audioSource.clip = recordedClip;
        Debug.Log("Playing audio clip: "+ recordedClip.length + " seconds");
        audioSource.Play();

        Invoke("ReturnToInitialState", recordedClip.length); // wait for the clip to finish playing
    }

    private void ReturnToInitialState()
    {
        recordButtonText.text = "Talk with\n Nono AI";
    }
}
