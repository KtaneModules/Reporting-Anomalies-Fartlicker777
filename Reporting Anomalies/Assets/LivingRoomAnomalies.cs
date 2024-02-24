using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class LivingRoomAnomalies : MonoBehaviour {

   /*
   * Todo:
   * ~~Intruder~~
   * ~~Extra~~
   * ~~Remove~~
   * ~~Light~~
   * ~~Door~~
   * ~~Camera~~
   * ~~Move~~
   * ~~Painting~~
   * ~~Abyss~~
   */

   public AnalogGlitch GlitchEffect;

   public AudioSource[] Music;

   public ReportingAnomalies Mod;

   public GameObject[] Intruder;
   int IntruderSubType;

   public GameObject[] ExtraObjects;
   int ExtraObj;

   public GameObject[] ObjectDisappearance;
   int DisObj;

   public GameObject[] LightAnomaly; //NA

   public GameObject Door;
   public GameObject OvenDoor;

   //public Camera CamGirls;

   public GameObject[] ObjectMovement;
   public GameObject FakeClockHand;
   int MovedObject;
   //public GameObject[] FakeMovements;
   //public Vector3[] StartingMovementVals = new Vector3[3];

   public GameObject[] PosterAnomaly;
   int TypeOfPainting;

   public GameObject Abyss;
   //0.1095824, 0.002272631, 0.1095824
   //23.41538, 0.002272631, 23.41538

   Coroutine IntruderCor;
   Coroutine MusicFazCor;
   IEnumerator AbyssCor;

   public bool[] ActiveAnomalies = new bool[9];

   public void CheckFix () {
      Mod.ActiveAnomalies--;
      ActiveAnomalies[Mod.AnomalyType] = false;
      switch (Mod.AnomalyType) {
         case 0:
            FixIntruder();
            break;
         case 1:
            FixExtra();
            break;
         case 2:
            FixDisappear();
            break;
         case 3:
            FixLight();
            break;
         case 4:
            FixDoor();
            break;
         case 5:
            FixCamera();
            break;
         case 6:
            FixMove();
            break;
         case 7:
            FixPainting();
            break;
         case 8:
            FixAbyss();
            break;
      }
      string[] ATypes = { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence" };
      Mod.LogFixes(ATypes[Mod.AnomalyType], "living room");
      Mod.RenderCameraMaterials();
   }

   public bool DoesItSoftlock () {

      for (int i = 0; i < 9; i++) { //Goes through the anomalies in the order { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence" };
         if (i == 3) {
            continue;
         }
         else if (i == 5 && Mod.BrokenCam != -1) { //Checks if there is a broken camera anywhere
            continue;
         }
         else if (ActiveAnomalies[i]) { //Checks if that anomaly is active
            continue;
         }
         else { //Returns false because anomaly[i] is inactive
            return false;
         }
      }

      return true; //Only possible when it continues 9 times in a row.
   }

   public void ChooseAnomaly () {
      Mod.ActiveAnomalies++;
      int RandomAnomaly = Rnd.Range(0, 9);
      do {
         RandomAnomaly = Rnd.Range(0, 9);
      } while (ActiveAnomalies[RandomAnomaly] || (Mod.BrokenCam != -1 && RandomAnomaly == 5));

      string[] ATypes = { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence"};

      Mod.LogAnomalies(ATypes[RandomAnomaly], "Living Room");

      ActiveAnomalies[RandomAnomaly] = true;

      switch (RandomAnomaly) {
         case 0:
            IntruderInit();
            Mod.LogAnomalies(new string[] { "Wow Deaf, you are so cool for animating the Left 4 Dead 2 Tank!", "Sayori :(" }[IntruderSubType]);
            break;
         case 1:
            ExtraInit();
            Mod.LogAnomalies(new string[] { "Rug", "Chair", "Part of tea set", "Wine bottle on cabinet", "Wine bottle in shelf", "Ladle in sink", "Plate on counter" }[ExtraObj]);
            break;
         case 2:
            DisappearInit();
            Mod.LogAnomalies(new string[] { "Hood", "Mug that you probably didn't see to begin with", "Blue pillow", "Coffee table that you probably didn't see to begin with", "Dining table", "Chair", "Ottoman", "Fasteners", "Butter knife" }[DisObj]);
            break;
         case 3:
            LightInit();
            break;
         case 4:
            DoorInit();
            break;
         case 5:
            CameraInit();
            break;
         case 6:
            MoveInit();
            Mod.LogAnomalies(new string[] { "Right couch", "Clock hands", "Outlets that you probably didn't see to begin with", "Pot", "Silverware" }[MovedObject]);
            break;
         case 7:
            PaintingInit();
            break;
         case 8:
            AbyssInit();
            break;
      }
   }

   #region Intruder

   public void IntruderInit () { //REMEMBER TO MAKE THIS 0 TO LENGTH UPON FULL RELEASE
      IntruderSubType = Rnd.Range(0, Intruder.Length);
      Intruder[IntruderSubType].SetActive(true);
      switch (IntruderSubType) {
         case 0:
            IntruderCor = StartCoroutine(TankSong());
            Music[0].Play();
            break;
         case 1:
            IntruderCor = StartCoroutine(SayonaraSong());
            Music[1].Play();
            //CamGirls.
            GlitchEffect.scanLineJitter = 0.154f;
            GlitchEffect.verticalJump = 0.059f;
            GlitchEffect.colorDrift = 0.13f;
            break;
         default:
            break;
      }
   }

   public void FixIntruder () {
      Intruder[IntruderSubType].SetActive(false);
      for (int i = 0; i < Music.Length; i++) {
         Music[i].Stop();
      }
      GlitchEffect.scanLineJitter = 0;
      GlitchEffect.verticalJump = 0;
      GlitchEffect.colorDrift = 0;
      StopCoroutine(IntruderCor);
   }

   IEnumerator TankSong () {
      //Intruder.SetActive(true);
      while (true) {
         if (Mod.CameraPos != 2) {
            Music[0].volume = 0;
         }
         else {
            Music[0].volume = .66f;
         }
         yield return null;
      }
   }

   IEnumerator SayonaraSong () {
      //Intruder.SetActive(true);
      while (true) {
         if (Mod.CameraPos != 2) {
            Music[1].volume = 0;
         }
         else {
            Music[1].volume = 1f;
         }
         yield return null;
      }
   }

   #endregion

   #region Extra Object

   public void ExtraInit () {
      ExtraObj = Rnd.Range(0, ExtraObjects.Length);
      ExtraObjects[ExtraObj].SetActive(true);
   }

   public void FixExtra () {
      ExtraObjects[ExtraObj].SetActive(false);
      ExtraObj = -1;
   }

   #endregion

   #region Disappearing object

   public void DisappearInit () {
      DisObj = Rnd.Range(0, ObjectDisappearance.Length);
      ObjectDisappearance[DisObj].SetActive(false);
   }

   public void FixDisappear () {
      ObjectDisappearance[DisObj].SetActive(true);
      DisObj = -1;
   }

   #endregion

   #region Light Anomaly

   public void LightInit () {
      //Debug.Log("A LIGHT ANOMALY HAS OCCURED IN LIVING ROOM. AUTOMATICALLY SOLVING.");
      //Mod.Solve();
      for (int i = 0; i < LightAnomaly.Length; i++) {
         LightAnomaly[i].SetActive(false);
      }
   }

   public void FixLight () {
      //Debug.Log("A LIGHT ANOMALY HAS BEEN FIXED IN LIVING ROOM SOMEHOW. AUTOMATICALLY SOLVING.");
      //Mod.Solve();
      for (int i = 0; i < LightAnomaly.Length; i++) {
         LightAnomaly[i].SetActive(true);
      }
   }

   #endregion

   #region Door Opening

   public void DoorInit () {
      //Debug.Log(Door);
      if (Rnd.Range(0, 2) == 0) {
         StartCoroutine(MoveDoor());
      }
      else {
         StartCoroutine(MoveOvenDoor());
      }
   }

   public void FixDoor () {
      //Debug.Log(Door);
      StartCoroutine(ResetDoor());
      StartCoroutine(ResetOvenDoor());
   }

   IEnumerator MoveOvenDoor () {
      var duration = .1f;
      var elapsed = 0f;
      while (OvenDoor.transform.localEulerAngles.x < 30f) {
         //Debug.Log(Door.transform.localEulerAngles.y);
         OvenDoor.transform.localEulerAngles = new Vector3(Mathf.Lerp(0, 30f, elapsed / duration), 0, 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
      OvenDoor.transform.localEulerAngles = new Vector3(30f, 0, 0);
   }

   IEnumerator MoveDoor () {
      //Debug.Log("Pemus");
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y < 270f || Door.transform.localEulerAngles.y > 276f) {
         //Debug.Log(Door.transform.localEulerAngles.y);
         Door.transform.localEulerAngles = new Vector3(270, 0, Mathf.Lerp(0, -87f, elapsed / duration));
         yield return null;
         elapsed += Time.deltaTime;
      }
      Door.transform.localEulerAngles = new Vector3(270, 0, -87f);
   }

   IEnumerator ResetDoor () {
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y > 3 || Door.transform.localEulerAngles.y < -3) {
         Door.transform.localEulerAngles = new Vector3(270, 0, Mathf.Lerp(-87f, 0, elapsed / duration));
         yield return null;
         elapsed += Time.deltaTime;
      }
      Door.transform.localEulerAngles = new Vector3(270, 0, 0);
   }

   IEnumerator ResetOvenDoor () {
      var duration = .1f;
      var elapsed = 0f;
      while (OvenDoor.transform.localEulerAngles.x > 0) {
         //Debug.Log(Door.transform.localEulerAngles.y);
         OvenDoor.transform.localEulerAngles = new Vector3(Mathf.Lerp(30, 0, elapsed / duration), 0, 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
      OvenDoor.transform.localEulerAngles = new Vector3(0, 0, 0);
   }

   #endregion

   #region Painting

   public void PaintingInit () {
      //PosterAnomaly[0].SetActive(false);
      TypeOfPainting = Rnd.Range(0, PosterAnomaly.Length);
      //TypeOfPainting = 0;
      //Debug.Log(TypeOfPainting);
      PosterAnomaly[0].SetActive(false);
      PosterAnomaly[TypeOfPainting].SetActive(true);
   }

   public void FixPainting () {
      PosterAnomaly[TypeOfPainting].SetActive(false);
      PosterAnomaly[0].SetActive(true);
      TypeOfPainting = -1;
   }

   #endregion

   #region Object Movement

   public void MoveInit () {//-2.8

      MovedObject = Rnd.Range(0, ObjectMovement.Length);
      //MovedObject = 2;

      StartCoroutine(ShowMove());
      //ObjectMovement[MovedObject].SetActive(false);
      //FakeMovements[MovedObject].SetActive(true);
   }

   public IEnumerator ShowMove () {
      var duration = .25f;
      var elapsed = 0f;
      switch (MovedObject) {
         case 0:
            while (ObjectMovement[0].transform.localPosition.x > -2.8f) {
               ObjectMovement[0].transform.localPosition = new Vector3(Mathf.Lerp(-2.532239f, -2.8f, elapsed / duration), 0, -0.26194f);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 1: //Special case since bullshit rotation. Tinkercad lookin object.
            /*while (elapsed < duration) {
               ObjectMovement[1].transform.rotation = new Vector3(7.2052f, 1.405f, Mathf.Lerp(-5.2043f, -4.5f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }*/
            ObjectMovement[1].SetActive(false);
            FakeClockHand.SetActive(true);
            break;
         case 2:
            while (ObjectMovement[2].transform.localPosition.z > -.4f) {
               ObjectMovement[2].transform.localPosition = new Vector3(0, 0, Mathf.Lerp(0, -.4f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 3:
            while (elapsed < duration) {
               ObjectMovement[3].transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 90f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 4:
            while (elapsed < duration) {
               ObjectMovement[4].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 180f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         default:
            yield return null;
            break;
      }
   }

   public IEnumerator FixMoveAnim () {
      var duration = .25f;
      var elapsed = 0f;
      switch (MovedObject) {
         case 0:

            while (ObjectMovement[0].transform.localPosition.x < -2.532239f) {
               ObjectMovement[0].transform.localPosition = new Vector3(Mathf.Lerp(-2.8f, -2.532239f, elapsed / duration), 0, -0.26194f);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;
         case 1:
            ObjectMovement[1].SetActive(true);
            FakeClockHand.SetActive(false);
            break;
         case 2:
            while (ObjectMovement[2].transform.localPosition.z < 0) {
               ObjectMovement[2].transform.localPosition = new Vector3(0, 0, Mathf.Lerp(-.4f, 0, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 3:
            while (elapsed < duration) {
               ObjectMovement[3].transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(90, 0f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 4:
            while (elapsed < duration) {
               ObjectMovement[4].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(180f, 0, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         default:

            yield return null;
            break;
      }
   }

   public void FixMove () {
      StartCoroutine(FixMoveAnim());
      MovedObject = -1;
   }

   #endregion

   #region Camera Malfunction

   public void CameraInit () {
      Mod.BrokenCam = 2;
      if (Mod.CameraPos == 2) {
         Mod.RightButton.OnInteract();
      }
   }

   public void FixCamera () {
      Mod.BrokenCam = -1;
   }

   #endregion

   #region Abyss

   public void AbyssInit () {
      Abyss.SetActive(true);
      //AbyssCor = StartCoroutine(AbyssGrow(Abyss.transform.localScale));
      AbyssCor = AbyssGrow(Abyss.transform.localScale); //I have never had this issue happen wtf? It's only in this room too.
      StartCoroutine(AbyssCor);
   }

   public void FixAbyss () {
      if (AbyssCor != null) {
         StopCoroutine(AbyssCor);
      }
      //Debug.Log(AbyssCor == null);
      //AbyssCor = StartCoroutine(AbyssShrink(Abyss.transform.localScale));
      AbyssCor = AbyssShrink(Abyss.transform.localScale);
      StartCoroutine(AbyssCor);
   }

   public IEnumerator AbyssGrow (Vector3 From) {
      //Debug.Log(From);
      var duration = 20f;
      var elapsed = 0f;
      Vector3 To = new Vector3(23.41538f, 0.002272631f, 23.41538f);
      while (elapsed < duration) {
         Abyss.transform.localScale = Vector3.Lerp(From, To, elapsed / duration);
         //Debug.Log(Abyss.transform.localScale);
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   public IEnumerator AbyssShrink (Vector3 From) {
      Debug.Log(From);
      var duration = .1f;
      var elapsed = 0f;
      Vector3 To = new Vector3(0.10958f, 0.002272631f, 0.10958f);
      while (elapsed < duration) {
         Abyss.transform.localScale = Vector3.Lerp(From, To, elapsed / duration);
         //Debug.Log(Abyss.transform.localScale);
         yield return null;
         elapsed += Time.deltaTime;
      }
      Abyss.SetActive(false);
      Abyss.transform.localScale = new Vector3(0.10958f, 0.002272631f, 0.10958f);
   }

   #endregion
}
