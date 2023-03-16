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

   public static string[] ignoredModules = null;

   public GameObject NotMod;

   public AudioSource WarningSoundSystem;

   public KMSelectable LeftButton;
   public KMSelectable ReportButton;
   public KMSelectable RightButton;

   public GameObject WarningSystem;
   public Text WarningT;

   public GameObject PleaseStandBy;
   public GameObject FlashingWarning;

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
   public BedroomAnomalies Bedr;
   public LibraryAnomalies Libr;
   public LivingRoomAnomalies Livi;
   public int BrokenCam = -1;

   public int RoomLocation = -1;
   public int AnomalyType = -1;

   public int ActiveAnomalies;

   bool FirstTimeThreeAnomalies;
   bool PlayingIntro;

   string[] IntroText = {
      "ATTENTION EMPLOYEE",
      "ANOMALIES HAVE BEEN PREVIOUSLY SIGHTED IN THE MONITORED HOUSE",
      "PAY EXTREMELY CLOSE ATTENTION TO THE SURVEILLANCE CAMERA FOOTAGE",
      "FILE AN ANOMALY REPORT ASAP WHEN YOU NOTICE SOMETHING HAS CHANGED"
   };

   string[] EmergencyText = {
      "THIS IS AN EMERGENCY WARNING!",
      "WE ARE RECEIVING REPORTS OF MULTIPLE ACTIVE ANOMALIES IN YOUR AREA",
      "PLEASE LOCATE THE ANOMALIES AND SEND REPORTS ASAP"
   };

   bool Filing;

   //Boss mod shit
   int ModCount = 8008135;
   bool WaitForModCount;

   string WrongText1 = "No anomaly of type ";
   string[] AnomalyTypesStr = { "intruder", "extra object", "object disappearance", "light anomaly", "door opening", "camera malfunction", "object movement", "painting anomaly", "abyss presence"};
   string WrongText2 = " found in ";
   //string[] LocationNamesStr = { "bedroom", "dungeon", "fur den"};

   public int CameraPos;

   public static bool FirstRAPresent;
   bool CanModuleOperate = true;

   int AnomalyRNG;

   //Coroutine PSBScreen;
   Coroutine Flash;

   static int ModuleIdCounter = 1;
   public int ModuleId;
   int SolveCount;
   private bool ModuleSolved;
   private static bool ModuleSolvedStatic;

   void Awake () {
      ModuleId = ModuleIdCounter++;

      GetComponent<KMBombInfo>().OnBombExploded += Exploded;

      if (FirstRAPresent) {
         Destroy(NotMod);//Prevents a lot of lag if I have only one.
         CanModuleOperate = false;
         Menu.SetActive(false);
         ReportButton.gameObject.SetActive(false);
      }
      FirstRAPresent = true;

      WarningSystem.SetActive(false);

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

      WarningSystem.SetActive(false);

      if (ignoredModules == null) {
         ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Reporting Anomalies", new string[] {
                "14",
                "42",
                "501",
                "A>N<D",
                "Bamboozling Time Keeper",
                "Black Arrows",
                "Brainf---",
                "The Board Walk",
                "Busy Beaver",
                "Don't Touch Anything",
                "Floor Lights",
                "Forget Any Color",
                "Forget Enigma",
                "Forget Ligma",
                "Forget Everything",
                "Forget Infinity",
                "Forget It Not",
                "Forget Maze Not",
                "Forget Me Later",
                "Forget Me Not",
                "Forget Perspective",
                "Forget The Colors",
                "Forget Them All",
                "Forget This",
                "Forget Us Not",
                "Iconic",
                "Keypad Directionality",
                "Kugelblitz",
                "Multitask",
                "OmegaDestroyer",
                "OmegaForest",
                "Organization",
                "Password Destroyer",
                "Purgatory",
                "Reporting Anomalies",
                "RPS Judging",
                "Security Council",
                "Shoddy Chess",
                "Simon Forgets",
                "Simon's Stages",
                "Souvenir",
                "Speech Jammer",
                "Tallordered Keys",
                "The Time Keeper",
                "Timing is Everything",
                "The Troll",
                "Turn The Key",
                "The Twin",
                "‹bermodule",
                "Ultimate Custom Night",
                "The Very Annoying Button",
                "Whiteout"
            });
         }
      }

   void Exploded () {
      FirstRAPresent = false;
      ModuleSolved = false;
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
      WaitForModCount = true;
      if (!CanModuleOperate) {
         return;
      }
      NotMod.transform.localScale = new Vector3((float) ((1 / NotMod.transform.lossyScale.x) - .2), (float) ((1 / NotMod.transform.lossyScale.y) - .2), (float) ((1 / NotMod.transform.lossyScale.z) - .2));
      AnomalyRNG = Rnd.Range(25, 33);
      Debug.LogFormat("[Reporting Anomalies #{0}] Anomalies have a {1}% chance of appearing.", ModuleId, AnomalyRNG);
      Menu.SetActive(false);
      StartCoroutine(StartAnim());
      //StartCoroutine(Test());
   }

   IEnumerator Test () {
      yield return new WaitForSeconds(2f);
      Bedr.IntruderInit();
   }

   public void LogAnomalies (string AType, string RType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Creating a(n) {1} Anomaly in {2} at solve #{3}.", ModuleId, AType, RType, SolveCount);
   }

   public void LogAnomalies (string SubType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Subtype: {1}.", ModuleId, SubType);
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
      //Debug.Log(Bedr.ActiveAnomalies[AnomalyType]);
      
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
            if (Bedr.ActiveAnomalies[AnomalyType]) {
               StartCoroutine(FixingScreen());
               while (PleaseStandBy.activeSelf) {
                  yield return null;
               }
               Bedr.CheckFix();
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
         case 2:
            if (Livi.ActiveAnomalies[AnomalyType]) {
               StartCoroutine(FixingScreen());
               while (PleaseStandBy.activeSelf) {
                  yield return null;
               }
               Livi.CheckFix();
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
      Filing = false;
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
      PlayingIntro = true;
      yield return new WaitForSeconds(3f);
      Flash = StartCoroutine(FlipWarningState());
      WarningSystem.SetActive(true);
      WarningSoundSystem.Play();
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
      WarningSoundSystem.Stop();
      PlayingIntro = false;
      StopCoroutine(Flash);
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

   void AnomalyInit () {
      switch (Rnd.Range(0, 3)) {
         case 0:
            Bedr.ChooseAnomaly();
            break;
         case 1:
            Libr.ChooseAnomaly();
            break;
         case 2:
            Livi.ChooseAnomaly();
            break;
      }
   }

   IEnumerator FlipWarningState () {
      while (true) {
         yield return new WaitForSeconds(1.5f);
         FlashingWarning.SetActive(!FlashingWarning.activeSelf);
      }
   }

   IEnumerator Warning () {
      WarningSystem.SetActive(true);
      Flash = StartCoroutine(FlipWarningState());
      WarningSoundSystem.Play();
      yield return new WaitForSeconds(2f);
      for (int j = 0; j < 3; j++) {
         for (int i = 0; i < EmergencyText[j].Length; i++) {
            WarningT.text += EmergencyText[j][i].ToString();
            yield return new WaitForSeconds(.08f);
         }
         yield return new WaitForSeconds(1f);
         WarningT.text = "";
      }
      StopCoroutine(Flash);
      WarningSystem.SetActive(false);
      WarningSoundSystem.Stop();
   }

   public void Solve () {
      GetComponent<KMBombModule>().HandlePass();
      ModuleSolved = true;
      ModuleSolvedStatic = true;
   }

   public void Strike () {
      GetComponent<KMBombModule>().HandleStrike();
   }

   void Update () {
      if (ModuleSolved || !WaitForModCount) {
         return;
      }

      if (ModuleSolvedStatic) {
         Solve();
      }

      if (!CanModuleOperate) { //Separate to prevent solve method from happening like a billion times;
         return;
      }

      int Ignored = 0;
      for (int i = 0; i < Bomb.GetSolvableModuleNames().Count(); i++) {
         if (ignoredModules.Contains(Bomb.GetSolvableModuleNames()[i])) {
            Ignored++;
         }
      }

      ModCount = Bomb.GetSolvableModuleNames().Count() - Ignored;

      //Debug.Log(ModCount);

      if (SolveCount != Bomb.GetSolvedModuleNames().Count()) {
         while (SolveCount != Bomb.GetSolvedModuleNames().Count()) { //In case multiple modules solve simultaneously
            SolveCount++;
            if (ActiveAnomalies >= 4) {
               Strike();
            }
         }
         if (Rnd.Range(0, 99) <= AnomalyRNG) {
            AnomalyInit();
         }
      }
      if (ActiveAnomalies == 3 && !FirstTimeThreeAnomalies) {
         if (!PlayingIntro) {
            StartCoroutine(Warning());
         }
         FirstTimeThreeAnomalies = true;
      }
      if (SolveCount >= ModCount && ActiveAnomalies == 0) {
         //Debug.Log(SolveCount);
         //Debug.Log(ModCount);
         Solve();
      }
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
