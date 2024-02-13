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

   public bool DEBUGMODEACTIVE = true;

   public static string[] ignoredModules = null;

   public GameObject NotMod;

   public GameObject[] Rooms;

   public Camera[] CamsToRender;

   public AudioSource WarningSoundSystem;

   public GameObject LoadingScreen; //Used so that the mod doesn't show it moving cams

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
   string[] RoomNames = { "Bedroom", "Library", "Living Room" };

   float[][] ToppinsHeight = {
      new float[] { 2.301f, 1.860f, 1.419f, 0.979f, 0.537f }, //Mushroom
      new float[] { 2.287f, 1.849f, 1.407f, 0.965f, 0.521f }, //Cheese
      new float[] { 2.279f, 1.839f, 1.398f, 0.955f, 0.515f }, //Tomato
      new float[] { 2.297f, 1.856f, 1.413f, 0.969f, 0.531f }, //Sausage
      new float[] { 2.296f, 1.852f, 1.414f, 0.972f, 0.533f } // :)
   };
   int[] RandHeight = { 0, 1, 2, 3, 4 };
   public GameObject[] Toppins;

   public BedroomAnomalies Bedr;
   public LibraryAnomalies Libr;
   public LivingRoomAnomalies Livi;
   public int BrokenCam = -1;
   static int StaticCam = -1;

   int[] RoomChoosingOrder = { 0, 1, 2};

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

   string[] AltIntroText = {
      "Warning! Warning!",
      "Differences are being spotted in the Unity House!",
      ">m<",
      "Find all of them and we can ice cream after school!!!!!!!"
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
   string[] AnomalyTypesStr = { "intruder", "extra object", "object disappearance", "light anomaly", "door opening", "camera malfunction", "object movement", "painting anomaly", "abyss presence" };
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

   static int RACount;
   static int[] ViewingRooms = new int[3];

   bool NoModsLeft;

   void Awake () {
      ModuleId = ModuleIdCounter++;

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


      LeftButton.OnInteract += delegate () { Audio.PlaySoundAtTransform("Tick", LeftButton.transform); LeftPress(); return false; };
      RightButton.OnInteract += delegate () { Audio.PlaySoundAtTransform("Tick", RightButton.transform); RightPress(); return false; };

      ReportButton.OnInteract += delegate () { OpenMenu(); return false; };
      BackButton.OnInteract += delegate () { CloseMenu(); return false; };
      SendButton.OnInteract += delegate () { FileReport(); return false; };

      WarningSystem.SetActive(false);

      RACount++;

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
                "Übermodule",
                "Ultimate Custom Night",
                "The Very Annoying Button",
                "Whiteout"
            });
      }
   }

   void Start () {
      WaitForModCount = true;
      ViewingRooms[0]++;
      LoadingScreen.SetActive(false);
      if (!CanModuleOperate) {
         return;
      }

      NotMod.transform.localScale = new Vector3((float) ((1 / NotMod.transform.lossyScale.x) - .2), (float) ((1 / NotMod.transform.lossyScale.y) - .2), (float) ((1 / NotMod.transform.lossyScale.z) - .2));
      if (Rnd.Range(0, 5) != 0) { //80% chance to randomize Toppins Positioning
         RandHeight.Shuffle();
         for (int i = 0; i < 5; i++) {
            Toppins[i].transform.localPosition = new Vector3(Toppins[i].transform.localPosition.x, ToppinsHeight[i][RandHeight[i]], Toppins[i].transform.localPosition.z);
         }
      }

      AnomalyRNG = Rnd.Range(30, 41);
      Debug.LogFormat("[Reporting Anomalies #{0}] Anomalies have a {1}% chance of appearing.", ModuleId, AnomalyRNG);
      Menu.SetActive(false);
      DeloadRooms();
      if (RACount > 1) {
         StartCoroutine(FixMaterialForMultipleRAs());
      }
      StartCoroutine(StartAnim());
      StartCoroutine(Test());
   }

   IEnumerator Test () { //If I want to test an anomaly/anything for a bug
      yield return new WaitForSeconds(5f);
      //Libr.IntruderInit();
      Bedr.IntruderInit();
      yield return new WaitForSeconds(10f);
      Bedr.FixIntruder();
   }

   IEnumerator FixMaterialForMultipleRAs () {
      LoadingScreen.SetActive(true);
      for (int i = 0; i < 3; i++) {
         RightPress();
         NowViewingText.text = "";
         yield return new WaitForSeconds(.4f);
      }
      LoadingScreen.SetActive(false);
      RightPress();
      LeftPress();
   }

   #region Reset Things

   void OnDestroy () {
      FirstRAPresent = false;
      ModuleSolvedStatic = false;
      ViewingRooms = new int[3];
      StaticCam = -1;
      Cursor.visible = true;
      RACount = 0;
   }

   void Reset () {
      AnomalyType = -1;
      RoomLocation = -1;
   }

   #endregion

   #region Buttons

   void LeftPress () {
      ViewingRooms[CameraPos]--;
      CameraPos--;
      CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      ViewingRooms[CameraPos]++;
      if (BrokenCam == CameraPos) {
         CameraPos--;
         CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      }
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = RoomNames[CameraPos];
      DeloadRooms();
      //RenderCameraMaterials();
   }

   void RightPress () {

      ViewingRooms[CameraPos]--;
      CameraPos++;
      CameraPos %= CameraMats.Length;
      ViewingRooms[CameraPos]++;
      if (BrokenCam == CameraPos) {
         CameraPos++;
         CameraPos %= CameraMats.Length;
      }
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = RoomNames[CameraPos];
      DeloadRooms();
      //RenderCameraMaterials();
   }

   public void RenderCameraMaterials () {
      for (int i = 0; i < 3; i++) {
         if (CamsToRender[i].gameObject.activeSelf) {
            CamsToRender[i].Render();
         }
      }
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


   void DeloadRooms () {
      if (!CanModuleOperate) {
         return;
      }
      /*Debug.Log("----------");
      Debug.Log(ViewingRooms[0]);
      Debug.Log(ViewingRooms[1]);
      Debug.Log(ViewingRooms[2]);*/

      for (int i = 0; i < Rooms.Length; i++) {
         if (ViewingRooms[i] == 0) {
            Rooms[i].SetActive(false);
         }
         else {
            Rooms[i].SetActive(true);
         }
      }
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

   #endregion

   #region Logging

   public void LogAnomalies (string AType, string RType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Creating a(n) {1} Anomaly in {2} at solve #{3}.", ModuleId, AType, RType, SolveCount);
   }

   public void LogAnomalies (string SubType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Subtype: {1}.", ModuleId, SubType);
   }

   public void LogFixes (string AType, string RType) {
      Debug.LogFormat("[Reporting Anomalies #{0}] Fixed a(n) {1} Anomaly in {2}.", ModuleId, AType, RType);
   }

   #endregion

   #region Animations

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
               if (AnomalyType == 5) {
                  BrokenCam = -1;
                  StaticCam = -1;
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
               if (AnomalyType == 5) {
                  BrokenCam = -1;
                  StaticCam = -1;
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
               if (AnomalyType == 5) {
                  BrokenCam = -1;
                  StaticCam = -1;
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
      Reset();
   }

   IEnumerator FixingScreen () {
      PleaseStandBy.SetActive(true);
      Cursor.visible = false;
      Audio.PlaySoundAtTransform("Fixing", transform);
      yield return new WaitForSeconds(1.368f);
      for (int i = 0; i < CamsToRender.Length; i++) {
         if (CamsToRender[i].gameObject.activeSelf) {
            CamsToRender[i].Render();
         }
      }
      Cursor.visible = true;
      Filing = false;
      PleaseStandBy.SetActive(false);
      RenderCameraMaterials();
   }

   IEnumerator WrongAnomaly () {
      //WrongCanvas.SetActive(true);
      WrongText.text = WrongText1 + AnomalyTypesStr[AnomalyType] + WrongText2 + RoomNames[RoomLocation] + ".";
      WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, 0);
      var duration = .25f;
      while (WrongText.color.a < 1.0f) {
         //Debug.Log(WrongText.color.a);
         WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, WrongText.color.a + (Time.deltaTime / duration));
         yield return null;
      }
      yield return new WaitForSeconds(5f);
      while (WrongText.color.a > 0) {
         WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, WrongText.color.a - (Time.deltaTime / duration));
         yield return null;
         //Debug.Log(WrongText.color.a);
      }
      WrongText.color = new Color(WrongText.color.r, WrongText.color.g, WrongText.color.b, 0);
      Filing = false;
      Reset();
   }

   IEnumerator StartAnim () {
      PlayingIntro = true;
      yield return new WaitForSeconds(3f);
      Flash = StartCoroutine(FlipWarningState());
      WarningSystem.SetActive(true);
      WarningSoundSystem.Play();
      yield return new WaitForSeconds(2f);
      if (Rnd.Range(0, 20) == 0) {
         for (int j = 0; j < 4; j++) {
            for (int i = 0; i < AltIntroText[j].Length; i++) {
               WarningT.text += AltIntroText[j][i].ToString();
               yield return new WaitForSeconds(.08f);
            }
            yield return new WaitForSeconds(1f);
            WarningT.text = "";
         }
      }
      else {
         for (int j = 0; j < 4; j++) {
            for (int i = 0; i < IntroText[j].Length; i++) {
               WarningT.text += IntroText[j][i].ToString();
               yield return new WaitForSeconds(.08f);
            }
            yield return new WaitForSeconds(1f);
            WarningT.text = "";
         }
      }

      WarningSystem.SetActive(false);
      WarningSoundSystem.Stop();
      PlayingIntro = false;
      StopCoroutine(Flash);
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

   #endregion

   void AnomalyInit () {

      RoomChoosingOrder = RoomChoosingOrder.Shuffle();
      bool MadeAnomaly = false;

      for (int i = 0; i < 3; i++) {
         switch (RoomChoosingOrder[i]) {
            case 0:
               if (!Bedr.DoesItSoftlock()) {
                  Bedr.ChooseAnomaly();
                  MadeAnomaly = true;
               }
               break;
            case 1:
               if (!Libr.DoesItSoftlock()) {
                  Libr.ChooseAnomaly();
                  MadeAnomaly = true;
               }
               break;
            case 2:
               if (!Livi.DoesItSoftlock()) {
                  Livi.ChooseAnomaly();
                  MadeAnomaly = true;
               }
               break;
         }
         if (MadeAnomaly) {
            break;
         }
      }

      StaticCam = BrokenCam;
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
      BrokenCam = StaticCam;
      if (CameraPos == BrokenCam) {
         RightButton.OnInteract();
      }

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

      ModCount = Bomb.GetSolvableModuleNames().Count(x => !ignoredModules.Contains(x));

      //Debug.Log(ModCount);

      if (SolveCount != Bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x))) {
         while (SolveCount != Bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x))) { //In case multiple modules solve simultaneously
            SolveCount++;
            if (ActiveAnomalies >= 4) {
               Strike();
            }
         }
         if ((Rnd.Range(0, 99) <= AnomalyRNG && !(SolveCount >= ModCount)) || DEBUGMODEACTIVE) {
            AnomalyInit();
         }
      }
      if (ActiveAnomalies == 3 && !FirstTimeThreeAnomalies) {
         if (!PlayingIntro) {
            StartCoroutine(Warning());
         }
         FirstTimeThreeAnomalies = true;
      }
      if (SolveCount >= ModCount && !NoModsLeft) {
         NoModsLeft = true;
         AnomalyInit(); //Guarantees one spawned anomaly at the end
         Debug.LogFormat("[Reporting Anomalies #{0}] All other non-ignored solvables have been solved. Production of anomalies has been shut down. There {1} {2} remaining active {3}.", ModuleId, ActiveAnomalies == 1 ? "is" : "are", ActiveAnomalies, ActiveAnomalies == 1 ? "anomaly" : "anomalies");
      }
      if (SolveCount >= ModCount && ActiveAnomalies == 0) {
         //Debug.Log(SolveCount);
         //Debug.Log(ModCount);
         Solve();
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} left/right to switch between cameras. Use !{0} report X in Y to report that specific anomaly in that specific room. Use !{0} anomalies to output the anomaly names to chat. Note that underscores must be used in place of spaces for anomaly and room names when reporting.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      string[] RoomTypes = RoomNames.ToArray();
      string[] ReportTypes = AnomalyTypesStr.ToArray();
      for (int i = 0; i < ReportTypes.Length; i++) {
         ReportTypes[i] = ReportTypes[i].ToUpper().Replace(" ", "_");
         if (i < 3) {
            RoomTypes[i] = RoomTypes[i].ToUpper().Replace(" ", "_");
         }
      }
      string[] Parameters = Command.Trim().ToUpper().Split(' ');
      if (Parameters.Length == 4 && Parameters[0] == "REPORT" && ReportTypes.Contains(Parameters[1]) && Parameters[2] == "IN" && RoomTypes.Contains(Parameters[3])) {
         if (!CanModuleOperate) {
            yield return "sendtochaterror The report feature on this module has shat itself!";
            yield break;
         }
         ReportButton.OnInteract();
         yield return new WaitForSeconds(.1f);
         RoomSelectionButtons[Array.IndexOf(RoomTypes, Parameters[3])].OnInteract();
         yield return new WaitForSeconds(.1f);
         AnomalousButtons[Array.IndexOf(ReportTypes, Parameters[1])].OnInteract();
         yield return new WaitForSeconds(.1f);
         SendButton.OnInteract();
      }
      else if (Parameters.Length == 1 && Parameters[0] == "LEFT") {
         LeftButton.OnInteract();
      }
      else if (Parameters.Length == 1 && Parameters[0] == "RIGHT") {
         RightButton.OnInteract();
      }
      else if (Parameters.Length == 1 && Parameters[0] == "ANOMALIES") {
         yield return "sendtochat Anomalies: " + AnomalyTypesStr.Join(", ");
      }
      else {
         yield return "sendtochaterror I don't understand!";
      }
   }

   void TwitchHandleForcedSolve () {
      Solve();
   }
}
