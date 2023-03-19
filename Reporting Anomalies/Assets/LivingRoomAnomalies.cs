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

   public AudioSource Music;

   public ReportingAnomalies Mod;

   public GameObject Intruder;
   public AudioClip TankTheme;

   public GameObject[] ExtraObjects;
   int ExtraObj;

   public GameObject[] ObjectDisappearance;
   int DisObj;

   public GameObject[] LightAnomaly; //NA
   public GameObject Door;

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
   }

   public void ChooseAnomaly () {
      Mod.ActiveAnomalies++;
      int RandomAnomaly = Rnd.Range(0, 9);
      do {
         RandomAnomaly = Rnd.Range(0, 9);
      } while (ActiveAnomalies[RandomAnomaly] || RandomAnomaly == 3 || (Mod.BrokenCam != -1 && RandomAnomaly == 5));

      string[] ATypes = { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence"};

      Mod.LogAnomalies(ATypes[RandomAnomaly], "Living Room");

      ActiveAnomalies[RandomAnomaly] = true;

      switch (RandomAnomaly) {
         case 0:
            IntruderInit();
            break;
         case 1:
            ExtraInit();
            Mod.LogAnomalies(new string[] { "Rug", "Chair", "Tea set", "Wine bottle" }[ExtraObj]);
            break;
         case 2:
            DisappearInit();
            Mod.LogAnomalies(new string[] { "Hood", "Mug that you probably didn't see to begin with", "Blue pillow", "Coffee table that you probably didn't see to begin with", "Dining table", "Chair", "Ottoman" }[DisObj]);
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
            Mod.LogAnomalies(new string[] { "Right couch", "Clock hands", "Outlets that you probably didn't see to begin with" }[MovedObject]);
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

   public void IntruderInit () {
      Intruder.SetActive(true);
      Music.clip = TankTheme;
      IntruderCor = StartCoroutine(TankSong());
      Music.Play();
   }

   public void FixIntruder () {
      Intruder.SetActive(false);
      Music.Stop();
      StopCoroutine(IntruderCor);
   }

   IEnumerator TankSong () {
      //Intruder.SetActive(true);
      while (true) {
         if (Mod.CameraPos != 2) {
            Music.volume = 0;
         }
         else {
            Music.clip = TankTheme;
            Music.volume = .66f;
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
      Debug.Log("A LIGHT ANOMALY HAS OCCURED IN LIVING ROOM. AUTOMATICALLY SOLVING.");
      Mod.Solve();
   }

   public void FixLight () {
      Debug.Log("A LIGHT ANOMALY HAS BEEN FIXED IN LIVING ROOM SOMEHOW. AUTOMATICALLY SOLVING.");
      Mod.Solve();
   }

   #endregion

   #region Door Opening

   public void DoorInit () {
      //Debug.Log(Door);
      StartCoroutine(MoveDoor());
   }

   public void FixDoor () {
      //Debug.Log(Door);
      StartCoroutine(ResetDoor());
   }

   IEnumerator MoveDoor () {
      //Debug.Log("Pemus");
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y > -87f) {
         Door.transform.localEulerAngles = new Vector3(270, 0, Mathf.Lerp(0, -87f, elapsed / duration));
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   IEnumerator ResetDoor () {
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y < 0) {
         Door.transform.localEulerAngles = new Vector3(270, 0, Mathf.Lerp(-87f, 0, elapsed / duration));
         yield return null;
         elapsed += Time.deltaTime;
      }
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
      PosterAnomaly[0].SetActive(true);
      PosterAnomaly[TypeOfPainting].SetActive(false);
      TypeOfPainting = -1;
   }

   #endregion

   #region Object Movement

   public void MoveInit () {//-2.8

      MovedObject = Rnd.Range(0, 3);
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
   }

   #endregion
}
