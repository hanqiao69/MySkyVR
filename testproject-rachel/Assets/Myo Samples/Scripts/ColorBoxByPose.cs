using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

// Change the material when certain poses are made with the Myo armband.
// Vibrate the Myo armband when a fist pose is made.
public class ColorBoxByPose : MonoBehaviour
{
    // Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public GameObject myo = null;

    // Materials to change to when poses are made.
//    public Material waveInMaterial;
//    public Material waveOutMaterial;
//    public Material doubleTapMaterial;
	private Light light1;
//	private Light light2;
	private Light[] lights;
	private Component cons1;
	private Component cons2;

    // The pose from the last update. This is used to determine if the pose has changed
    // so that actions are only performed upon making them rather than every frame during
    // which they are active.
    private Pose _lastPose = Pose.Unknown;

    // Update is called once per frame.
    void Update ()
    {
        // Access the ThalmicMyo component attached to the Myo game object.
        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();

//		lights = GetComponentsInChildren<Light>();
//		light1 = lights[0];
//		light2 = lights[1];

		Component[] comps = GetComponents<Component> ();
		cons1 = comps [0];
		cons2 = comps [1];
		lights = cons1.GetComponentsInChildren<Light>();
		int length = lights.Length;

        // Check if the pose has changed since last update.
        // The ThalmicMyo component of a Myo game object has a pose property that is set to the
        // currently detected pose (e.g. Pose.Fist for the user making a fist). If no pose is currently
        // detected, pose will be set to Pose.Rest. If pose detection is unavailable, e.g. because Myo
        // is not on a user's arm, pose will be set to Pose.Unknown.
        if (thalmicMyo.pose != _lastPose) {
            _lastPose = thalmicMyo.pose;

            // Vibrate the Myo armband when a fist is made.
            if (thalmicMyo.pose == Pose.Fist) {
                thalmicMyo.Vibrate (VibrationType.Medium);
				for (int i = 0; i < length; i++) {
					lights[i].color = Color.white;
				}

                ExtendUnlockAndNotifyUserAction (thalmicMyo);

            // Change material when wave in, wave out or double tap poses are made.
            } else if (thalmicMyo.pose == Pose.WaveIn) {
//				light1.color = Color.black;
//				light2.color = Color.white;
//				light1 = lights[0];
//				light1.color = Color.black;
				for (int i = 0; i < 12; i++) {
					lights[i].color = Color.black;
				}
				for (int i = 13; i < length; i++) {
					lights[i].color = Color.black;
				}
                ExtendUnlockAndNotifyUserAction (thalmicMyo);
             }  else if (thalmicMyo.pose == Pose.WaveOut) {
				for (int i = 0; i < length; i++) {
					lights[i].color = Color.blue;
				}

                ExtendUnlockAndNotifyUserAction (thalmicMyo);
            } else if (thalmicMyo.pose == Pose.DoubleTap) {
				for (int i = 0; i < length; i++) {
					lights[i].color = Color.green;
				}

                ExtendUnlockAndNotifyUserAction (thalmicMyo);
            }
        }
    }

    // Extend the unlock if ThalmcHub's locking policy is standard, and notifies the given myo that a user action was
    // recognized.
    void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard) {
            myo.Unlock (UnlockType.Timed);
        }

        myo.NotifyUserAction ();
    }
}
