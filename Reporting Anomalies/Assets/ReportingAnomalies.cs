using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using UnityEngine.UI;


public class ReportingAnomalies : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable LeftButton;
   public KMSelectable ReportButton;
   public KMSelectable RightButton;

   public GameObject WarningSystem;
   public Text WarningT;

   public GameObject PleaseStandBy;

   public KMSelectable[] AnomalousButtons;
   public KMSelectable[] RoomSelectionButtons;
   public KMSelectable BackButton;
   public KMSelectable SendButton;
   public GameObject Menu;
   public Material UnselectedButton;
   public Material SelectedButton;
   //public GameObject WrongCanvas;
   public Text WrongText;

   public Text ReportPending;

   public Material[] CameraMats;
   public GameObject Screen;
   public TextMesh NowViewingText;
   string[] RoomNames = { "bedroom", "dungeon", "living room"};
   public BedroomAnomalies Yeah;
   public LibraryAnomalies Libr;
   public int BrokenCam = -1;

   public int RoomLocation = -1;
   public int AnomalyType = -1;

   public int ActiveAnomalies;

   string[] IntroText = {
      "ATTENTION EMPLOYEE",
      "ANOMALIES HAVE BEEN PREVIOUSLY SIGHTED IN THE MONITORED HOUSE",
      "PAY EXTREMELY CLOSE ATTENTION TO THE SURVEILLANCE CAMERA FOOTAGE",
      "FILE AN ANOMALY REPORT ASAP WHEN YOU NOTICE SOMETHING HAS CHANGED"
   };

   bool Filing;

   string WrongText1 = "No anomaly of type ";
   string[] AnomalyTypesStr = { "intruder", "extra object", "object disappearance", "light anomaly", "door opening", "camera malfunction", "object movement", "painting anomaly", "abyss presence"};
   string WrongText2 = " found in ";
   //string[] LocationNamesStr = { "bedroom", "dungeon", "fur den"};

   public int CameraPos;

   //Coroutine PSBScreen;

   static int ModuleIdCounter = 1;
   public int ModuleId;
   int SolveCount;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      
      foreach (KMSelectable B in AnomalousButtons) {
          B.OnInteract += delegate () { AnomButtPress(B); return false; };
      }

      foreach (KMSelectable B in RoomSelectionButtons) {
         B.OnInteract += delegate () { RoomButtPress(B); return false; };
      }


      LeftButton.OnInteract += delegate () { LeftPress(); return false; };
      RightButton.OnInteract += delegate () { RightPress(); return false; };

      ReportButton.OnInteract += delegate () { OpenMenu(); return false; };
      BackButton.OnInteract += delegate () { CloseMenu(); return false; };
      SendButton.OnInteract += delegate () { FileReport(); return false; };

   }

   void AnomButtPress (KMSelectable B) {
      for (int i = 0; i < AnomalousButtons.Length; i++) {
         if (B == AnomalousButtons[i]) {
            for (int j = 0; j < 9; j++) {
               AnomalousButtons[j].GetComponent<MeshRenderer>().material = UnselectedButton;
            }
            AnomalousButtons[i].GetComponent<MeshRenderer>().material = SelectedButton;
            AnomalyType = i;
         }
      }
   }

   void RoomButtPress (KMSelectable B) {
      for (int i = 0; i < RoomSelectionButtons.Length; i++) {
         if (B == RoomSelectionButtons[i]) {
            for (int j = 0; j < 3; j++) {
               RoomSelectionButtons[j].GetComponent<MeshRenderer>().material = UnselectedButton;
            }
            RoomSelectionButtons[i].GetComponent<MeshRenderer>().material = SelectedButton;
            RoomLocation = i;
         }
      }
   }

   void Start () {

      Menu.SetActive(false);
      StartCoroutine(StartAnim());
      StartCoroutine(Test());
   }

   IEnumerator Test () {
      yield return new WaitForSeconds(2f);
      //Yeah.CameraInit();
      //Yeah.MoveInit();
   }

   public void LogAnomalies (string AType, string RType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Creating a(n) {1} Anomaly in {2} at solve #{3}.", ModuleId, AType, RType, SolveCount);
   }

   void OpenMenu () {
      Audio.PlaySoundAtTransform("Tick", ReportButton.transform);
      if (Filing) {
         return;
      }
      Menu.SetActive(true);
   }

   void CloseMenu () {
      Audio.PlaySoundAtTransform("Tick", BackButton.transform);
      Menu.SetActive(false);
   }

   void FileReport () {
      //Audio.PlaySoundAtTransform("Tick", SendButton.transform);
      if (RoomLocation == -1 || AnomalyType == -1) {
         return;
      }
      Filing = true;
      for (int i = 0; i < 9; i++) {
         AnomalousButtons[i].GetComponent<MeshRenderer>().material = UnselectedButton;
         RoomSelectionButtons[i / 3].GetComponent<MeshRenderer>().material = UnselectedButton;
      }
      CloseMenu();
      StartCoroutine(WaitForAnomalousReport());
      //Debug.Log(Yeah.ActiveAnomalies[AnomalyType]);
      
   }

   IEnumerator WaitForAnomalousReport () {
      for (int j = 0; j < 2; j++) {
         ReportPending.text = "Report pending";
         for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(1f);
            ReportPending.text += ".";
         }
         yield return new WaitForSeconds(1f);
      }
      ReportPending.text = "";
      switch (RoomLocation) {
         case 0:
            if (Yeah.ActiveAnomalies[AnomalyType]) {
               StartCoroutine(FixingScreen());
               while (PleaseStandBy.activeSelf) {
                  yield return null;
               }
               Yeah.CheckFix();
            }
            else {
               StartCoroutine(WrongAnomaly());
            }
            break;
         case 1:
            if (Libr.ActiveAnomalies[AnomalyType]) {
               StartCoroutine(FixingScreen());
               while (PleaseStandBy.activeSelf) {
                  yield return null;
               }
               Libr.CheckFix();
            }
            else {
               StartCoroutine(WrongAnomaly());
            }
            break;
         default:
            break;
      }
   }

   IEnumerator FixingScreen () {
      PleaseStandBy.SetActive(true);
      Audio.PlaySoundAtTransform("Fixing", transform);
      yield return new WaitForSeconds(1.368f);
      PleaseStandBy.SetActive(false);
   }

   IEnumerator WrongAnomaly () {
      Filing = false;
      //WrongCanvas.SetActive(true);
      WrongText.text = WrongText1 + AnomalyTypesStr[AnomalyType] + WrongText2 + RoomNames[RoomLocation] + ".";
      WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, 0);
      var duration = .25f;
      while (WrongText.color.a < 1.0f) {
         WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, WrongText.color.a + (Time.deltaTime / duration));
         yield return null;
      }
      yield return new WaitForSeconds(5f);
      while (WrongText.color.a != 0) {
         WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, WrongText.color.a - (Time.deltaTime / duration));
         yield return null;
      }
      Reset();
   }

   void Reset () {
      AnomalyType = -1;
      RoomLocation = -1;
   }

   IEnumerator StartAnim () {
      yield return new WaitForSeconds(3f);
      WarningSystem.SetActive(true);
      Audio.PlaySoundAtTransform("Warning Noise", transform);
      yield return new WaitForSeconds(2f);
      for (int j = 0; j < 4; j++) {
         for (int i = 0; i < IntroText[j].Length; i++) {
            WarningT.text += IntroText[j][i].ToString();
            yield return new WaitForSeconds(.08f);
         }
         yield return new WaitForSeconds(1f);
         WarningT.text = "";
      }
      WarningSystem.SetActive(false);
   }

   void LeftPress () {
      Audio.PlaySoundAtTransform("Tick", LeftButton.transform);
      CameraPos--;
      CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      if (BrokenCam == CameraPos) {
         CameraPos--;
         CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      }
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = "Now viewing: " + RoomNames[CameraPos];
   }

   void RightPress () {
      Audio.PlaySoundAtTransform("Tick", RightButton.transform);
      CameraPos++;
      CameraPos %= CameraMats.Length;
      if (BrokenCam == CameraPos) {
         CameraPos++;
         CameraPos %= CameraMats.Length;
      }
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = "Now viewing: " + RoomNames[CameraPos];
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
