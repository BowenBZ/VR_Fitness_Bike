using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    Renderer lpSign;
    Renderer hdSign;
    Renderer lpspSign;
    Renderer lpCount;
    Renderer lppsCount;
    Renderer hdCount;
    Renderer logo;

    Rotate rotate;


    Renderer[] HDSignsPlain;
    Renderer[] HDSignsDecay;
    Renderer[] HDSignsWithScrews;
    Renderer[] SpeedPostLowPlain;
    Renderer[] SpeedPostLowDecay;
    Renderer[] SpeedPostLowWithScrews;
    Renderer[] PostNSpeedPlain;
    Renderer[] PostNSpeedDecay;
    Renderer[] PostNSpeedWithScrews;

    void Awake() {
        lpSign = GameObject.Find("LPLabel").GetComponent<Renderer>();
        lpspSign = GameObject.Find("LPSpeedNSignLabel").GetComponent<Renderer>();
        hdSign = GameObject.Find("HDLabel").GetComponent<Renderer>();
        lpCount = GameObject.Find("LPCount").GetComponent<Renderer>();
        lppsCount = GameObject.Find("LPPSCount").GetComponent<Renderer>();
        hdCount = GameObject.Find("HDCount").GetComponent<Renderer>();
        logo = GameObject.Find("LargeLogo").GetComponent<Renderer>();
        rotate = GameObject.Find("SUN").GetComponent <Rotate> ();

        lpSign.enabled = false;
        lpspSign.enabled = false;

        lpCount.enabled = false;
        lppsCount.enabled = false;

        logo.enabled = false;


        HDSignsPlain = GameObject.Find("HDSigns-Plain").GetComponentsInChildren<Renderer>();
        HDSignsDecay = GameObject.Find("HDSigns-Decay").GetComponentsInChildren<Renderer>();
        HDSignsWithScrews = GameObject.Find("HDSigns-WithScrews").GetComponentsInChildren<Renderer>();

        SpeedPostLowPlain = GameObject.Find("Speed-Post-Low - Plain").GetComponentsInChildren<Renderer>();
        SpeedPostLowDecay = GameObject.Find("Speed-Post-Low - Decay").GetComponentsInChildren<Renderer>();
        SpeedPostLowWithScrews = GameObject.Find("Speed-Post-Low - WithScrews").GetComponentsInChildren<Renderer>();

        PostNSpeedPlain = GameObject.Find("PostNSpeed - Plain").GetComponentsInChildren<Renderer>();
        PostNSpeedDecay = GameObject.Find("PostNSpeed - Decay").GetComponentsInChildren<Renderer>();
        PostNSpeedWithScrews = GameObject.Find("PostNSpeed - With Screws").GetComponentsInChildren<Renderer>();


        foreach (Renderer r in HDSignsDecay) { r.enabled = false; }
        foreach (Renderer r in HDSignsWithScrews) { r.enabled = false; }

        foreach (Renderer r in SpeedPostLowPlain) { r.enabled = false; }
        foreach (Renderer r in SpeedPostLowDecay) { r.enabled = false; }
        foreach (Renderer r in SpeedPostLowWithScrews) { r.enabled = false; }

        foreach (Renderer r in PostNSpeedPlain) { r.enabled = false; }
        foreach (Renderer r in PostNSpeedDecay) { r.enabled = false; }
        foreach (Renderer r in PostNSpeedWithScrews) { r.enabled = false; }

    }

    IEnumerator Start () {

        // Render HD First
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in HDSignsPlain) { r.enabled = false; }
        foreach (Renderer r in HDSignsDecay) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in HDSignsDecay) { r.enabled = false; }
        foreach(Renderer r in HDSignsWithScrews) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in HDSignsWithScrews) { r.enabled = false; }
        hdCount.enabled = false;
        hdSign.enabled = false;
        // End of HD

        // LP
        lpSign.enabled = true;
        lpCount.enabled = true;
        foreach (Renderer r in SpeedPostLowPlain) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in SpeedPostLowPlain) { r.enabled = false; }
        foreach (Renderer r in SpeedPostLowDecay) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in SpeedPostLowDecay) { r.enabled = false; }
        foreach (Renderer r in SpeedPostLowWithScrews) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in SpeedPostLowWithScrews) { r.enabled = false; }
        lpSign.enabled = false;
        lpCount.enabled = false;
        // End of LP

        // Post & Sign Start
        lpspSign.enabled = true;
        lppsCount.enabled = true;
        foreach (Renderer r in PostNSpeedPlain) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in PostNSpeedPlain) { r.enabled = false; }
        foreach (Renderer r in PostNSpeedDecay) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in PostNSpeedDecay) { r.enabled = false; }
        foreach (Renderer r in PostNSpeedWithScrews) { r.enabled = true; }
        yield return StartCoroutine(Wait15Secs());
        foreach (Renderer r in PostNSpeedWithScrews) { r.enabled = false; }
        lpspSign.enabled = false;
        lppsCount.enabled = false;
        // End of Post & Sign

        yield return StartCoroutine(Wait15Secs());
        rotate.rot = false;
        logo.enabled = true;


    }


    IEnumerator Wait15Secs() { yield return new WaitForSecondsRealtime(17); }

}
