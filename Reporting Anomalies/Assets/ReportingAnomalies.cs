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

   public bool DEBUGMODEACTIVE = false;

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

   public GameObject[] Books;

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
   static int StaticCam = -1; //I genuinely cannot tell you what the point of this was. Probably has to do with multiple RAs

   int[] RoomChoosingOrder = { 0, 1, 2 };

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

   string[] AltEmergencyText = {
      "This is a message for Grace Vanderwhaal:",
      "Emergency rotating_light fuckass bob will NOT pay for my dinner.",
      "Find the differecbnes now!",
      " ...Or face the rath of the king of Texas.",
      "We are currently on lockdown."
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
   public bool CanModuleOperate = true;

   int AnomalyRNG;

   int AnomalyRNGLowerBound = 30;
   int AnomalyRNGUpperBound = 41;
   bool ModifiedAnomalyRNG = false;

   //Coroutine PSBScreen;
   Coroutine Flash;

   static int ModuleIdCounter = 1;
   public int ModuleId;
   int SolveCount;
   private bool ModuleSolved;
   private static bool ModuleSolvedStatic;

   static int RACount;
   static bool[] ViewingRooms = new bool[] { false, false, false};

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

      string missionDesc = KTMissionGetter.Mission.Description;
      if (missionDesc != null) {
         Regex regex = new Regex(@"(\[)(Reporting Anomalies)(\s)(\d{1,3})(%)?(,)(\s)*(\d{1,3})(%)?(\])"); // Valid string would look something like "[Reporting Anomalies] 10%, 93%"
         var match = regex.Match(missionDesc);
         if (match.Success) {
            string matchstr = match.ToString().Replace(" ",""); //Removes spaces from the valid set to make indexing easier
            Debug.Log(matchstr);

            int length = matchstr.IndexOf('%');

            int LowerBound = int.Parse(matchstr.Substring(19, length - 19)); //19 when the first number would appear in "[REPORTINGANOMALIES10%,93%]";
            Debug.Log(LowerBound);
            //                                                                                                           012345678901234567890123456
            length = matchstr.LastIndexOf('%');
            Debug.Log(matchstr.IndexOf(',') + 1);
            Debug.Log(length);
            Debug.Log(matchstr.Substring(matchstr.IndexOf(',') + 1, length - (matchstr.IndexOf('%') + 2)));
            int UpperBound = int.Parse(matchstr.Substring(matchstr.IndexOf(',') + 1, length - matchstr.IndexOf('%') + 2)); //Has to start from first % since it can be a 1-3 digit number.
            Debug.Log(UpperBound);

            if (LowerBound > UpperBound) { //Makes sure lower and upper bounds are truly lower and upper
               int temp = LowerBound;
               LowerBound = UpperBound;
               UpperBound = temp;
            }
            if (UpperBound > 100) { //Maxes at 100 just so it looks nicer in log
               UpperBound = 100;
            }
            if (LowerBound == UpperBound) { //Prevents the Rng.Range(x, x) bug that happened in Challenge and Contact that one time
               UpperBound++;
            }
            ModifiedAnomalyRNG = true;
         }
      }

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
      ViewingRooms[0] = true;
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

      for (int i = 0; i < Books.Length; i++) {
         Books[i].transform.localEulerAngles = new Vector3(0, Rnd.Range(0, 360f));
      }

      AnomalyRNG = Rnd.Range(AnomalyRNGLowerBound, AnomalyRNGUpperBound);
      if (Application.isEditor) {
         AnomalyRNG = 100;
      }
      if (ModifiedAnomalyRNG) {
         Debug.LogFormat("[Reporting Anomalies #{0}] The mission you are playing has altered the RNG of anomalies appearing. The range of the frequency of anomalies is from {1}% to {2}%", ModuleId, AnomalyRNGLowerBound, AnomalyRNGUpperBound);
      }
      
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

      yield return new WaitForSeconds(8f);
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
      ViewingRooms = new bool[3];
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
      if (!CanModuleOperate) {
         return;
      }
      ViewingRooms[CameraPos] = false;
      CameraPos--;
      CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      if (BrokenCam == CameraPos) {
         CameraPos--;
         CameraPos = CameraPos < 0 ? CameraPos + CameraMats.Length : CameraPos;
      }
      ViewingRooms[CameraPos] = true;
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = RoomNames[CameraPos];
      DeloadRooms();
      //RenderCameraMaterials();
   }

   void RightPress () {
      if (!CanModuleOperate) {
         return;
      }
      ViewingRooms[CameraPos] = false;
      CameraPos++;
      CameraPos %= CameraMats.Length;
      if (BrokenCam == CameraPos) {
         CameraPos++;
         CameraPos %= CameraMats.Length;
      }
      ViewingRooms[CameraPos] = true;
      Screen.GetComponent<MeshRenderer>().material = CameraMats[CameraPos];
      NowViewingText.text = RoomNames[CameraPos];
      DeloadRooms();
      //RenderCameraMaterials();
   }

   public void RenderCameraMaterials () { //Makes sure that Unity isn't dumb and manually renders the camera for any anomalies. Essentially nullified with multiple RAs.
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


   void DeloadRooms () { //Prevents a LOT of lag by not loading rooms unless they are looked at.
      if (!CanModuleOperate) {
         return;
      }
      /*Debug.Log("----------");
      Debug.Log(ViewingRooms[0]);
      Debug.Log(ViewingRooms[1]);
      Debug.Log(ViewingRooms[2]);*/

      for (int i = 0; i < Rooms.Length; i++) {
         if (!ViewingRooms[i]) {
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
      if (Rnd.Range(0, 20) == 0) { //Fish
         for (int j = 0; j < 5; j++) {
            for (int i = 0; i < AltEmergencyText[j].Length; i++) {
               WarningT.text += AltEmergencyText[j][i].ToString();
               yield return new WaitForSeconds(.08f);
            }
            yield return new WaitForSeconds(1f);
            WarningT.text = "";
         }
      }
      else { //Default emergency text
         for (int j = 0; j < 3; j++) {
            for (int i = 0; i < EmergencyText[j].Length; i++) {
               WarningT.text += EmergencyText[j][i].ToString();
               yield return new WaitForSeconds(.08f);
            }
            yield return new WaitForSeconds(1f);
            WarningT.text = "";
         }
      }
      
      StopCoroutine(Flash);
      WarningSystem.SetActive(false);
      WarningSoundSystem.Stop();
   }

   #endregion

   void AnomalyInit () {
      /*
       * Goes through each room and sees if it can make an anomaly.
       * This basically prevents dumbfucks from creating an infinite
       * loop that would happen if it tries to make an anomaly in a
       * room that physically can't make any more anomalies.
      */
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
         if (i == 2 && !MadeAnomaly) {
            Debug.LogFormat("[Reporting Anomalies #{0}] The ~~town~~ house is too ~~evil~~ anomalous to ~~find anyone good~~ create any more anomalies!", ModuleId); //le funny town of salem reference
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
      if (CanModuleOperate) {
         DeloadRooms();
         RenderCameraMaterials();
      }
      else {
         LoadingScreen.SetActive(true);
      }

      if (CameraPos == BrokenCam) { //If
         RightButton.OnInteract();
      }

      if (ModuleSolved || !WaitForModCount) { //Makes sure that all mods are actually counted
         return;
      }

      if (ModuleSolvedStatic) { //Solves all Reporting Anomalies simultaneously when the real one solves
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
      if (ActiveAnomalies == 3 && !FirstTimeThreeAnomalies) { //If there are three anomalies active, it plays a one time warning
         if (!PlayingIntro) {
            StartCoroutine(Warning());
         }
         FirstTimeThreeAnomalies = true;
      }
      if (SolveCount >= ModCount && !NoModsLeft) {
         NoModsLeft = true;
         AnomalyInit(); //Guarantees one spawned anomaly at the end
         Debug.LogFormat("[Reporting Anomalies #{0}] All other non-ignored solvables have been solved. Production of anomalies has been shut down. There {1} {2} remaining active {3}.", ModuleId, ActiveAnomalies == 1 ? "is" : "are", ActiveAnomalies, ActiveAnomalies == 1 ? "anomaly" : "anomalies");
         Debug.LogFormat("[Reporting Anomalies #{0}] They are:", ModuleId);
         for (int i = 0; i < 9; i++) {
            if (Bedr.ActiveAnomalies[i]) {
               Debug.LogFormat("[Reporting Anomalies #{0}] A {1} anomaly in the {2}.", ModuleId, AnomalyTypesStr[i], Rnd.Range(0, 100) == 99 ? "breeding room" : "bedroom");
            }
            if (Libr.ActiveAnomalies[i]) {
               Debug.LogFormat("[Reporting Anomalies #{0}] A {1} anomaly in the {2}.", ModuleId, AnomalyTypesStr[i], Rnd.Range(0, 100) == 99 ? "liberal room" : "library");
            }
            if (Livi.ActiveAnomalies[i]) {
               Debug.LogFormat("[Reporting Anomalies #{0}] A {1} anomaly in the {2}.", ModuleId, AnomalyTypesStr[i], Rnd.Range(0, 100) == 99 ? "SCP-002" : "living room");
            }
         }
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
