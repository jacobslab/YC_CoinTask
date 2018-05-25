//-----------------------------------------------------------------------
// Copyright © 2018 Tobii AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Tobii.Research.Unity
{
    public class Calibration : MonoBehaviour
    {
        /// <summary>
        /// Instance of <see cref="Calibration"/> for easy access.
        /// Assigned in Awake() so use earliest in Start().
        /// </summary>
        public static Calibration Instance { get; private set; }

        /// <summary>
        /// Flag indicating if the latest calibration was successful
        /// or not, true/false.
        /// </summary>
        public bool LatestCalibrationSuccessful { get; private set; }

        /// <summary>
        /// Is calibration in progress?
        /// </summary>
        public bool CalibrationInProgress { get { return _calibrationInProgress; } }

        [SerializeField]
        [Tooltip("This key will start a calibration.")]
        private KeyCode _startKey = KeyCode.None;

        /// <summary>
        /// Calibration points.
        /// Example:
        /// (0.2f, 0.2f)
        /// (0.8f, 0.2f)
        /// (0.2f, 0.8f)
        /// (0.8f, 0.8f)
        /// (0.5f, 0.5f)
        /// </summary>
        [SerializeField]
        [Tooltip("Calibration points.")]
        private Vector2[] _points;

        [SerializeField]
        private Image _calibrationPoint;

		public Text varianceText;

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private Image _panel;

        private CalibrationPoint _pointScript;

		public CalibrationPointCollection calibPointCollection;

		public GameObject validationPointGroup;
		public CanvasGroup calibrationFailedPanel;

		public CanvasGroup validationInstructionPanel;
		public CanvasGroup calibResultPanel;

        // Handle blocking calls to calibration in a separate thread.
        private CalibrationThread _calibrationThread;
        private bool _calibrationInProgress;

		public int maxSamples = 30;



        private bool ShowCalibrationPanel
        {
            get
            {
                return _showCalibrationPanel;
            }

            set
            {
                _showCalibrationPanel = value;
                _pointScript.gameObject.SetActive(_showCalibrationPanel);
                _canvas.gameObject.SetActive(_showCalibrationPanel);
                _panel.color = _showCalibrationPanel ? Color.black : new Color(0, 0, 0, 0);
            }
        }

        private bool _showCalibrationPanel;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _pointScript = _calibrationPoint.GetComponent<CalibrationPoint>();
			calibResultPanel.alpha = 0f;
			calibrationFailedPanel.alpha = 0f;
			validationInstructionPanel.alpha = 0f;
            ShowCalibrationPanel = false;
        }

        /// <summary>
        /// Start a calibration. Either provide a set of calibration points,
        /// or null for default. The result callback provides a true or false
        /// answer to the success of the calibration.
        /// </summary>
        /// <param name="points">An array of calibration points, or null for default.</param>
        /// <param name="resultCallback">A result callback or null for none.</param>
        /// <returns>True if calibration was not already started, false otherwise.</returns>
        public bool StartCalibration(Vector2[] points = null, System.Action<bool> resultCallback = null)
        {
            if (_calibrationInProgress)
            {
                Debug.Log("Already performing calibration");
                return false;
            }

            _calibrationInProgress = true;
            StartCoroutine(PerformCalibration(points, resultCallback));
            return true;
        }

        /// <summary>
        /// Wait for the <see cref="CalibrationThread.MethodResult"/> to be ready.
        /// </summary>
        /// <param name="result">The method result</param>
        /// <returns>An enumerator</returns>
        private IEnumerator WaitForResult(CalibrationThread.MethodResult result)
        {
            // Wait for the thread to finish the blocking call.
            while (!result.Ready)
            {
                yield return new WaitForSeconds(0.02f);
            }

            Debug.Log(result);
        }

        /// <summary>
        /// Calibration coroutine. Drives the calibration thread states.
        /// </summary>
        /// <param name="points">Optional point list. Null means default set.</param>
        /// <param name="resultCallback">A result callback or null for none.</param>
        /// <returns>An enumerator</returns>
        private IEnumerator PerformCalibration(Vector2[] points, System.Action<bool> resultCallback)
        {
			calibrationFailedPanel.alpha = 0f;
            if (points != null)
            {
                _points = points;
            }

            if (_calibrationThread != null)
            {
                _calibrationThread.StopThread();
                _calibrationThread = null;
            }

            // Create and start the calibration thread.
            Debug.Log("about to start the calib thread");
            _calibrationThread = new CalibrationThread(EyeTracker.Instance.EyeTrackerInterface, screenBased: true);

            // Only continue if the calibration thread is running.

            for (int i = 0; i < 10; i++)
            {

                Debug.Log("calib thread running");
                if (_calibrationThread.Running)
                {
                    break;
                }

                yield return new WaitForSeconds(0.1f);
            }

            if (!_calibrationThread.Running)
            {
                Debug.LogError("Failed to start calibration thread");
                _calibrationThread.StopThread();
                _calibrationThread = null;
                _calibrationInProgress = false;
                yield break;
            }

            ShowCalibrationPanel = true;

            Debug.Log("entering calib mode");
            var enterResult = _calibrationThread.EnterCalibrationMode();

            // Wait for the call to finish
            yield return StartCoroutine(WaitForResult(enterResult));

            // Iterate through the calibration points.
            foreach (var pointPosition in _points)
            {

                Debug.Log("iterating through calib points");
                // Set the local position and start the point animation
                _calibrationPoint.rectTransform.anchoredPosition = new Vector2(Screen.width * pointPosition.x, Screen.height * (1 - pointPosition.y));
                _pointScript.StartAnim();

                // Wait for animation.
                yield return new WaitForSeconds(1f);

                // As of this writing, adding a point takes about 175 ms. A failing add can take up to 3000 ms.
                var collectResult = _calibrationThread.CollectData(new CalibrationThread.Point(pointPosition));
                
                // Wait for the call to finish
                yield return StartCoroutine(WaitForResult(collectResult));

                // React to the result of adding a point.
                if (collectResult.Status == CalibrationStatus.Failure)
                {
                    Debug.Log("There was an error gathering data for this calibration point: " + pointPosition);
                }

            }

            // Compute and apply the result of the calibration. A succesful compute currently takes about 300 ms. A failure may bail out in a few ms.
            var computeResult = _calibrationThread.ComputeAndApply();

            // Wait for the call to finish
            yield return StartCoroutine(WaitForResult(computeResult));

			//doing after compute and apply
			var calibResult = _calibrationThread.GetResult ();

			yield return StartCoroutine (WaitForResult (calibResult));

			calibPointCollection = calibResult.Result.CalibrationPoints;

            // Leave calibration mode.
            var leaveResult = _calibrationThread.LeaveCalibrationMode();

            // Wait for the call to finish
            yield return StartCoroutine(WaitForResult(leaveResult));


            // Stop the thread.
            _calibrationThread.StopThread();
            _calibrationThread = null;

            // Finish up or restart if failure.
            LatestCalibrationSuccessful = computeResult.Status == CalibrationStatus.Success;

//            ShowCalibrationPanel = false;

            if (resultCallback != null)
            {
				Debug.Log ("calibration success? " + LatestCalibrationSuccessful.ToString ());
                resultCallback(LatestCalibrationSuccessful);
            }

            _calibrationInProgress = false;
			if (LatestCalibrationSuccessful) {
				StartCoroutine (PerformValidation ());
			} else {
				calibrationFailedPanel.alpha = 1f;
				bool waitForRestart = true;
				while (waitForRestart) {
					if (Input.GetKeyDown (KeyCode.C)) {
						InitiateCalibration ();
						waitForRestart = false;
						calibrationFailedPanel.alpha = 0f;
					}
					yield return 0;
				}
			}
        }

		IEnumerator PerformValidation()
		{
			calibResultPanel.alpha = 1f;
			validationInstructionPanel.alpha = 1f;

			float totalVarianceLeftX = 0f;
			float totalVarianceLeftY = 0f;
			float totalVarianceRightX = 0f;
			float totalVarianceRightY = 0f;

			float totalMeanLeftX = 0f;
			float totalMeanLeftY = 0f;
			float totalMeanRightX = 0f;
			float totalMeanRightY = 0f;

			Debug.Log ("total calib points : " + calibPointCollection.Count.ToString ());
			for (int i = 0; i < calibPointCollection.Count; i++) {
				List<float> leftSamplesX = new List<float> ();
				List<float> leftSamplesY = new List<float> ();
				List<float> rightSamplesX = new List<float> ();
				List<float> rightSamplesY = new List<float> ();
				Debug.Log ("calib samples in point " + i.ToString() + " is: " + calibPointCollection [i].CalibrationSamples.Count.ToString ());
				for (int j = 0; j < calibPointCollection [i].CalibrationSamples.Count; j++) {
					Debug.Log ("calib point " + i.ToString() + " sample: " + j.ToString());
					CalibrationSample calibSample = calibPointCollection [i].CalibrationSamples [j];
					//left
					//							calibResultPanel.transform.GetChild (5).gameObject.GetComponent<CanvasGroup>().alpha=1f;
					int start = (i * 5) + j;
					int end = (i * 5) + j + 1;
					Debug.Log("child count is: " +validationPointGroup.transform.childCount.ToString());
					if (end < 28) {

						validationPointGroup.transform.GetChild (start).gameObject.SetActive (true);

						//left
						Debug.Log ("calibsample left: " + calibSample.LeftEye.PositionOnDisplayArea.X.ToString() + " , " + calibSample.LeftEye.PositionOnDisplayArea.Y.ToString ());
						validationPointGroup.transform.GetChild (start).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (Screen.width * calibSample.LeftEye.PositionOnDisplayArea.X, Screen.height * (1f - calibSample.LeftEye.PositionOnDisplayArea.Y));
						validationPointGroup.transform.GetChild (start).GetComponent<Image> ().color = (calibSample.LeftEye.Validity == CalibrationEyeValidity.ValidAndUsed) ? Color.green : Color.white;
						leftSamplesX.Add (Mathf.Abs(calibSample.LeftEye.PositionOnDisplayArea.X-calibPointCollection[i].PositionOnDisplayArea.X));
						leftSamplesY.Add (Mathf.Abs(calibSample.LeftEye.PositionOnDisplayArea.Y-calibPointCollection[i].PositionOnDisplayArea.Y));

						//right
						validationPointGroup.transform.GetChild (end).gameObject.SetActive (true);
						Debug.Log ("calibsample right: " + calibSample.RightEye.PositionOnDisplayArea.X.ToString() + " , " + calibSample.RightEye.PositionOnDisplayArea.Y.ToString ());
						validationPointGroup.transform.GetChild (end).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (Screen.width * calibSample.RightEye.PositionOnDisplayArea.X, Screen.height * (1f - calibSample.RightEye.PositionOnDisplayArea.Y));
						validationPointGroup.transform.GetChild (end).GetComponent<Image> ().color = (calibSample.RightEye.Validity == CalibrationEyeValidity.ValidAndUsed) ? Color.green : Color.white;
						rightSamplesX.Add (Mathf.Abs(calibSample.RightEye.PositionOnDisplayArea.X-calibPointCollection[i].PositionOnDisplayArea.X));
						rightSamplesY.Add (Mathf.Abs(calibSample.RightEye.PositionOnDisplayArea.Y-calibPointCollection[i].PositionOnDisplayArea.Y));

						Debug.Log ("updated points from " + start.ToString() + " to " + end.ToString());

					}
					//						Debug.Log ("calib sample LEFT EYE: " + calibPointCollection [i].CalibrationSamples [j].LeftEye.PositionOnDisplayArea.ToString () + " with validity " + calibPointCollection [i].CalibrationSamples [j].LeftEye.Validity.ToString ());
					//						Debug.Log ("calib sample RIGHT EYE: " + calibPointCollection [i].CalibrationSamples [j].RightEye.PositionOnDisplayArea.ToString () + " with validity " + calibPointCollection [i].CalibrationSamples [j].RightEye.Validity.ToString ());
				}
				Debug.Log ("about to calculate mean and variance");
				totalMeanLeftX += Experiment_CoinTask.Instance.mathOperations.Mean (leftSamplesX,0,leftSamplesX.Count);
//				totalVarianceLeftX += Experiment_CoinTask.Instance.mathOperations.Variance (leftSamplesX, meanLeftX, 0, leftSamplesX.Count);

				totalMeanLeftY += Experiment_CoinTask.Instance.mathOperations.Mean (leftSamplesY,0,leftSamplesY.Count);
//				totalVarianceLeftY += Experiment_CoinTask.Instance.mathOperations.Variance (leftSamplesY, meanLeftY, 0, leftSamplesY.Count);

				totalMeanRightX += Experiment_CoinTask.Instance.mathOperations.Mean (rightSamplesX,0,rightSamplesX.Count);
//				totalVarianceRightX += Experiment_CoinTask.Instance.mathOperations.Variance (rightSamplesX, meanRightX, 0, rightSamplesX.Count);

				totalMeanRightY += Experiment_CoinTask.Instance.mathOperations.Mean (rightSamplesY,0,rightSamplesY.Count);
//				totalVarianceRightY += Experiment_CoinTask.Instance.mathOperations.Variance (rightSamplesY, meanRightY, 0, rightSamplesY.Count);

			}

			float avgVarLeftX = totalMeanLeftX / calibPointCollection.Count;
			float avgVarLeftY = totalMeanLeftY / calibPointCollection.Count;
			float avgVarRightX = totalMeanRightX / calibPointCollection.Count;
			float avgVarRightY = totalMeanRightY / calibPointCollection.Count;

			varianceText.text = "Avg Mean Left: (" + avgVarLeftX.ToString ("F2") + ", " + avgVarLeftY.ToString ("F2") + ") \n Avg Mean Right: (" + avgVarRightX.ToString ("F2") + " , " + avgVarRightY.ToString ("F2") + ")";

			bool waitForResponse = true;
			int response = -1;
			while (waitForResponse) {
				if(Input.GetKeyDown(KeyCode.C))
				{
					response = 0; //redo calibration
					waitForResponse=false;
				}
				if (Input.GetKeyDown (KeyCode.A)) {
					response = 1; //accept validated points and move on
					waitForResponse = false;
				}
				yield return 0;
			}

			validationInstructionPanel.alpha = 0f;
			calibResultPanel.alpha = 0f;

			if (response == 0) {
				InitiateCalibration ();
			} else {
				EyetrackerManager.isCalibrating = false;
				ShowCalibrationPanel = false;
			}
			yield return null;
		}

        /// <summary>
        /// This function is called when the behaviour becomes disabled() or inactive.
        /// </summary>
        private void OnDisable()
        {
            // Stop the calibration thread if it is not null.
            if (_calibrationThread != null)
            {
                var result = _calibrationThread.StopThread();
                _calibrationThread = null;
                Debug.Log("Calibration thread stopped: " + (result ? "YES" : "NO"));
            }
        }

		void InitiateCalibration()
		{
			var calibrationStartResult = StartCalibration(
				resultCallback: (calibrationResult) =>
				Debug.Log("Calibration was " + (calibrationResult ? "successful" : "unsuccessful"))
			);
			Debug.Log("Calibration " + (calibrationStartResult ? "" : "not ") + "started");
		}

        private void Update()
        {
            if (Input.GetKeyDown(_startKey))
            {
				EyetrackerManager.isCalibrating = true;

                EyetrackerManager.waitForCalibration = false;
				InitiateCalibration ();
            }

        }
    }
}
