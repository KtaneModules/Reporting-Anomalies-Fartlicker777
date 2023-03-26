using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class LibraryAnomalies : MonoBehaviour {

   public AudioSource Music;

   public ReportingAnomalies Mod;

   public SpriteRenderer Intruder;

   public Sprite[] SpaceBoyfriend;

   public GameObject[] ExtraObjects;
   int ExtraObj;

   public GameObject[] ObjectDisappearance;
   int DisObj;
   //bool PCDis;
   //bool BedDis;

   public GameObject[] LightAnomaly;
   public GameObject[] FiresForCandles;
   int LightIndex;

   public GameObject Door;

   public GameObject[] ObjectMovement;
   //bool PCMove;
   int MovedObject;
   //bool BedMove;
   //public GameObject[] FakeMovements;
   //public Vector3[] StartingMovementVals = new Vector3[3];

   //public GameObject[] PosterAnomaly;
   //int TypeOfPainting;

   public GameObject Abyss;
   //0.1095824, 0.002272631, 0.1095824
   //23.41538, 0.002272631, 23.41538

   Coroutine IntruderCor;
   //Coroutine MusicFazCor;
   Coroutine AbyssCor;

   float From;
   float To;

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
      } while (ActiveAnomalies[RandomAnomaly] || RandomAnomaly == 7 || (Mod.BrokenCam != -1 && RandomAnomaly == 5));

      ActiveAnomalies[RandomAnomaly] = true;

      string[] ATypes = { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence" };

      Mod.LogAnomalies(ATypes[RandomAnomaly], "Library");

      switch (RandomAnomaly) {
         case 0:
            IntruderInit();
            break;
         case 1:
            ExtraInit();
            Mod.LogAnomalies(new string[] { "candle", "book", "chandelier", "sword" }[ExtraObj]);
            break;
         case 2:
            DisappearInit();
            Mod.LogAnomalies(new string[] { "close bookshelf", "far bookshelf" }[DisObj]);
            break;
         case 3:
            LightInit();
            Mod.LogAnomalies(new string[] { "close chandelier", "far chandelier", "table candles" }[LightIndex]);
            break;
         case 4:
            DoorInit();
            break;
         case 5:
            CameraInit();
            break;
         case 6:
            MoveInit();
            Mod.LogAnomalies(new string[] { "book", "far candle", "fallen close chandelier", "fallen far chandelier", "rotated chandeliers"}[MovedObject]);
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
      //Intruder.SetActive(true);
      IntruderCor = StartCoroutine(SpaceBoyfriendAnim());
      //Music.clip = SBTheme;
      //Music.volume = .25f;
      Music.Play();
   }

   public void FixIntruder () {
      Intruder.sprite = null;
      //Music.volume = 1f;
      Music.Stop();
      StopCoroutine(IntruderCor);
   }

   IEnumerator SpaceBoyfriendAnim () {
      int counter = 0;
      while (true) {
         if (Mod.CameraPos != 1) {
            Music.volume = 0;
         }
         else {
            //Music.clip = SBTheme;
            Music.volume = .25f;
         }
         counter++;
         counter %= 4;
         Intruder.sprite = SpaceBoyfriend[counter];
         yield return new WaitForSeconds(.1f);
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
      LightIndex = Rnd.Range(0, 3);
      LightIndex = 2;
      for (int i = 0; i < 4; i++) {
         LightAnomaly[i + 4 * LightIndex].SetActive(false);
         FiresForCandles[i + 4 * LightIndex].SetActive(false);
      }
   }

   public void FixLight () {
      for (int i = 0; i < 4; i++) {
         LightAnomaly[i + 4 * LightIndex].SetActive(true);
         FiresForCandles[i + 4 * LightIndex].SetActive(true);
      }
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
      while (Door.transform.localEulerAngles.y < 180f) {
         Door.transform.localEulerAngles = new Vector3(270, Mathf.Lerp(90f, 180f, elapsed / duration), 0);
         //Door.transform.localPosition = new Vector3(-0.01723932f, -0.03f, Mathf.Lerp(-33.824f, -33.824f, elapsed / duration)); //-0.01723932, -0.02999997, -33.824
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   IEnumerator ResetDoor () {
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y > 90f) {
         Door.transform.localEulerAngles = new Vector3(270, Mathf.Lerp(180f, 90f, elapsed / duration), 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   #endregion

   #region Painting

   public void PaintingInit () {
      Debug.Log("A PAINTING ANOMALY HAS OCCURED IN LIBRARY. AUTOMATICALLY SOLVING.");
      Mod.Solve();
   }

   public void FixPainting () {
      Debug.Log("A PAINTING ANOMALY HAS BEEN FIXED IN LIBRARY SOMEHOW. AUTOMATICALLY SOLVING.");
      Mod.Solve();
   }

   #endregion

   #region Object Movement

   public void MoveInit () {
      MovedObject = Rnd.Range(0, ObjectMovement.Length + 1);
      MovedObject = 2;
      /*do {
         MovedObject = Rnd.Range(0, 3);
      } while ((PCDis && MovedObject == 1) || (PCMove && MovedObject == 2));
      
      if (MovedObject == 1) {
         PCMove = true;
      }
      else if (MovedObject == 2) {
         BedMove = true;
      }
      */
      StartCoroutine(ShowMove());
      //ObjectMovement[MovedObject].SetActive(false);
      //FakeMovements[MovedObject].SetActive(true);
   }

   public IEnumerator ShowMove () {
      Debug.Log(ObjectMovement[2].transform.localPosition.y);
      var duration = .25f;
      var elapsed = 0f;
      switch (MovedObject) {
         case 0:
            From = ObjectMovement[0].transform.localPosition.z;
            To = -28.61f;
            while (elapsed < duration) {
               ObjectMovement[0].transform.localPosition = new Vector3(2.925f, 1.362f, Mathf.Lerp(From, To, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            
            break;
         case 1:

            From = ObjectMovement[0].transform.localPosition.z;
            To = -30.46f;

            while (elapsed < duration) {
               ObjectMovement[1].transform.localPosition = new Vector3(-3.5f, 1.289f, Mathf.Lerp(-27.775f, -30.46f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 2:

            

            duration = .15f;
            From = 5.019f;
            To = 0.24f;

            while (ObjectMovement[2].transform.localPosition.y > 0.24f) { //This is fucking stupid
               ObjectMovement[2].transform.localPosition = new Vector3(-0.1682601f, Mathf.Lerp(5.019f, 0.24f, elapsed / duration), -29.279f);
               //ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -21.163f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            //Debug.Log(ObjectMovement[2].transform.localPosition.y);
            break;
         case 3:

            duration = .15f;
            From = 5.019f;
            To = 0.24f;

            while (ObjectMovement[3].transform.localPosition.y > 0.24f) {
               ObjectMovement[3].transform.localPosition = new Vector3(-6.017f, Mathf.Lerp(5.019f, 0.24f, elapsed / duration), -29.279f);
               //ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -21.163f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;

         case 4:

            From = ObjectMovement[0].transform.localRotation.x;
            To = -63.549f;

            while (elapsed < duration) {
               //ObjectMovement[2].transform.localPosition = new Vector3(Mathf.Lerp(-.39f, 0.17f, elapsed / duration), 0.04f, Mathf.Lerp(-6.853f, -6.45f, elapsed / duration));
               ObjectMovement[4].transform.localEulerAngles = new Vector3(Mathf.Lerp(From, To, elapsed / duration), -180, 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;

         case 5:

            From = 0;
            To = 90f;

            while (elapsed < duration) {
               //ObjectMovement[2].transform.localPosition = new Vector3(Mathf.Lerp(-.39f, 0.17f, elapsed / duration), 0.04f, Mathf.Lerp(-6.853f, -6.45f, elapsed / duration));
               ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(From, To, elapsed / duration), 0);
               ObjectMovement[3].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(From, To, elapsed / duration), 0);
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
            while (elapsed < duration) {
               ObjectMovement[0].transform.localPosition = new Vector3(2.925f, 1.362f, Mathf.Lerp(To, From, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;
         case 1:

            while (elapsed < duration) {
               ObjectMovement[1].transform.localPosition = new Vector3(-3.5f, 1.289f, Mathf.Lerp(-30.46f, -27.775f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 2:

            duration = .15f;

            while (ObjectMovement[2].transform.localPosition.y < 5.019f) {
               ObjectMovement[2].transform.localPosition = new Vector3(-0.1682601f, Mathf.Lerp(0.24f, 5.019f, elapsed / duration), -29.279f);
               //ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -21.163f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 3:

            duration = .15f;

            while (ObjectMovement[3].transform.localPosition.y < 5.019f) {
               ObjectMovement[3].transform.localPosition = new Vector3(-6.017f, Mathf.Lerp(0.24f, 5.019f, elapsed / duration), -29.279f);
               //ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -21.163f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;

         case 4:

            while (elapsed < duration) {
               //ObjectMovement[2].transform.localPosition = new Vector3(Mathf.Lerp(-.39f, 0.17f, elapsed / duration), 0.04f, Mathf.Lerp(-6.853f, -6.45f, elapsed / duration));
               ObjectMovement[4].transform.localEulerAngles = new Vector3(Mathf.Lerp(To, From, elapsed / duration), -180, 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;

         case 5:

            while (elapsed < duration) {
               //ObjectMovement[2].transform.localPosition = new Vector3(Mathf.Lerp(-.39f, 0.17f, elapsed / duration), 0.04f, Mathf.Lerp(-6.853f, -6.45f, elapsed / duration));
               ObjectMovement[2].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(To, From, elapsed / duration), 0);
               ObjectMovement[3].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(To, From, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

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
      Mod.BrokenCam = 1;
      if (Mod.CameraPos == 1) {
         Mod.RightButton.OnInteract();
      }
   }

   public void FixCamera () {
      Mod.BrokenCam = -1;
   }

   #endregion

   #region Abyss

   public void AbyssInit () {//21.29362, 0.01043047, 21.29363
      Abyss.SetActive(true);
      AbyssCor = StartCoroutine(AbyssGrow(Abyss.transform.localScale));
   }

   public void FixAbyss () {
      if (AbyssCor != null) {
         StopCoroutine(AbyssCor);
      }
      AbyssCor = StartCoroutine(AbyssShrink(Abyss.transform.localScale));
   }

   public IEnumerator AbyssGrow (Vector3 From) {
      //Debug.Log(From);
      var duration = 20f;
      var elapsed = 0f;
      Vector3 To = new Vector3(16f, 0.002270002f, 16f);
      while (Abyss.transform.localScale.x < 16f) {
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
      Vector3 To = new Vector3(0.1095824f, 0.002272631f, 0.1095824f);
      while (Abyss.transform.localScale.x > 0.1095824f) {
         Abyss.transform.localScale = Vector3.Lerp(From, To, elapsed / duration);
         //Debug.Log(Abyss.transform.localScale);
         yield return null;
         elapsed += Time.deltaTime;
      }
      Abyss.SetActive(false);
   }

   #endregion
}
