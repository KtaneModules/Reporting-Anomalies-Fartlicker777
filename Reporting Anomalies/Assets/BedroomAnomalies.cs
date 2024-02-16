using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class BedroomAnomalies : MonoBehaviour {

   public Camera Cam;
   public DigitalGlitch GlitchEffect;
   public SpriteRenderer BlackSquare;

   public GameObject[] Marlboros;

   Coroutine Blackness;

   public AudioSource[] Music;

   public ReportingAnomalies Mod;

   public GameObject[] Intruder;
   int RandIntruder;

   public GameObject[] ExtraObjects;
   int ExtraObj;

   public GameObject[] ObjectDisappearance;
   int DisObj;
   bool PCDis;
   bool BedDis;

   public GameObject[] LightAnomaly;
   public GameObject Door;

   public GameObject[] ObjectMovement;
   bool PCMove;
   int MovedObject;
   bool BedMove;
   //public GameObject[] FakeMovements;
   //public Vector3[] StartingMovementVals = new Vector3[3];

   public GameObject[] PosterAnomaly;
   int TypeOfPainting;

   public GameObject Abyss;
   //0.1095824, 0.002272631, 0.1095824
   //23.41538, 0.002272631, 23.41538

   Coroutine IntruderCor;
   Coroutine MusicFazCor;
   Coroutine AbyssCor;

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
      Mod.LogFixes(ATypes[Mod.AnomalyType], "bedroom");
      Mod.RenderCameraMaterials();
   }

   public bool DoesItSoftlock () {

      for (int i = 0; i < 9; i++) { //Goes through the anomalies in the order { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence" };
         if (i == 5 && Mod.BrokenCam != -1) { //Checks if there is a broken camera anywhere
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

      ActiveAnomalies[RandomAnomaly] = true;

      string[] ATypes = { "Intruder", "Extra Object", "Object Disappearance", "Light", "Door Opening", "Camera Malfunction", "Object Movement", "Painting", "Abyss Presence" };

      Mod.LogAnomalies(ATypes[RandomAnomaly], "Bedroom");

      switch (RandomAnomaly) {
         case 0:
            IntruderInit();
            Mod.LogAnomalies(new string[] { "Fredy fabore", "Phantom Marionette" }[RandIntruder]);
            break;
         case 1:
            ExtraInit();
            Mod.LogAnomalies(new string[] { "Trash Bin", "Newton's Cradle", "Thing crammed in desk", "Water bottle left of computer", "Paper stack", "Water bottle on computer", "Marlboro" }[ExtraObj]);
            break;
         case 2:
            DisappearInit();
            Mod.LogAnomalies(new string[] { "Alarm Clock", "Camera", "Book", "Headphones", "Pillow", "Left Speaker", "Right Speaker", "Monitor", "Keyboard", "Computer", "Mouse" }[DisObj]);
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
            Mod.LogAnomalies(new string[] { "Chair", "Bed", "Paper box", "Tissue box" }[MovedObject]);
            break;
         case 7:
            PaintingInit();
            Mod.LogAnomalies(new string[] { "tsirotoM thgindiM", "KILL", "Green Day", "gaaaaaaaaaaay" }[TypeOfPainting]);
            break;
         case 8:
            AbyssInit();
            break;
      }

      Mod.RenderCameraMaterials();
   }

   #region Intruder

   public void IntruderInit () {
      RandIntruder = Rnd.Range(1, Intruder.Length); //REMEMBER TO MAKE THIS 0 TO LENGTH UPON FULL RELEASE
      Intruder[RandIntruder].SetActive(true);
      switch (RandIntruder) {
         case 0:
            IntruderCor = StartCoroutine(Fabore());
            break;
         case 1:
            IntruderCor = StartCoroutine(Puppet());
            BlackSquare.gameObject.SetActive(true);
            Blackness = StartCoroutine(FadingConsciousness());
            break;
         default:
            break;
      }
      Music[RandIntruder].Play();
   }

   public void FixIntruder () {
      for (int i = 0; i < Intruder.Length; i++) {
         Intruder[i].SetActive(false);
         Music[i].Stop();
      }
      StopCoroutine(IntruderCor);
      StopCoroutine(Blackness);
      GlitchEffect.intensity = 0f;
      Cam.transform.localEulerAngles = new Vector3(0, 150f, 0);
      BlackSquare.gameObject.SetActive(false);
      BlackSquare.color = new Color32(0, 0, 0, 0);
   }

   IEnumerator Fabore () {
      //Intruder.SetActive(true);
      while (true) {
         var duration = .25f;
         var elapsed = 0f;
         while (elapsed < duration) {
            if (Mod.CameraPos != 0) {
               Music[0].volume = 0;
            }
            else {
               Music[0].volume = 1;
            }
            Intruder[0].transform.localEulerAngles = new Vector3(270, Mathf.Lerp(0, 359, elapsed / duration), 0);
            yield return null;
            elapsed += Time.deltaTime;
         }
      }
   }

   IEnumerator FadingConsciousness () {
      var duration = .5f;
      var elapsed = 0f;
      while (true) {
         while (elapsed < duration) {
            //Debug.Log("Fading");
            BlackSquare.color = new Color32(0, 0, 0, (byte) (elapsed * 2 * 255));
            yield return null;
            elapsed += Time.deltaTime;
         }

         elapsed = 0f;
         yield return new WaitForSeconds(.5f);
         while (elapsed < duration) {
            //Debug.Log("Lighting");
            //Debug.Log(elapsed);
            BlackSquare.color = new Color32(0, 0, 0, (byte) (255 - (elapsed * 2 * 255)));
            yield return null;
            elapsed += Time.deltaTime;
         }
         yield return new WaitForSeconds(.5f);
         elapsed = 0f;
      }
   }

   IEnumerator Puppet () {
      GlitchEffect.intensity = Rnd.Range(0.3f, 0.45f);
      var duration = 2f;
      var elapsed = 0f;
      while (elapsed < duration) {
         if (Mod.CameraPos != 0) {
            Music[1].volume = 0;
         }
         else {
            Music[1].volume = 1;
         }
         Cam.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(150, 180, elapsed / duration), 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
      while (true) {
         duration = 4f;
         elapsed = 0f;
         while (elapsed < duration) {
            if (Mod.CameraPos != 0) {
               Music[1].volume = 0;
            }
            else {
               Music[1].volume = 1;
            }
            Cam.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(180, 120, elapsed / duration), 0);
            yield return null;
            elapsed += Time.deltaTime;
         }
         duration = 4f;
         elapsed = 0f;
         while (elapsed < duration) {
            if (Mod.CameraPos != 0) {
               Music[1].volume = 0;
            }
            else {
               Music[1].volume = 1;
            }
            Cam.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(120, 180, elapsed / duration), 0);
            yield return null;
            elapsed += Time.deltaTime;
         }
      }
   }

   #endregion

   #region Extra Object

   public void ExtraInit () {
      ExtraObj = Rnd.Range(0, ExtraObjects.Length);
      //ExtraObj = 6;
      ExtraObjects[ExtraObj].SetActive(true);
      if (ExtraObj == 6) {
         BRINGFORTHTHEMARLBORO();
      }
   }

   void BRINGFORTHTHEMARLBORO () { //BRINGFORTHTHEMARLBORO
      foreach (var Marlboro in Marlboros) {
         Marlboro.transform.Rotate(new Vector3(Rnd.Range(0, 360f), Rnd.Range(0, 360f), Rnd.Range(0, 360f)));
      }
   }

   public void FixExtra () {
      ExtraObjects[ExtraObj].SetActive(false);
      ExtraObj = -1;
   }

   #endregion

   #region Disappearing object

   public void DisappearInit () {
      do {
         DisObj = Rnd.Range(0, ObjectDisappearance.Length);
      } while (PCMove && DisObj > 4);
      ObjectDisappearance[DisObj].SetActive(false);
      if (DisObj > 4) {
         PCDis = true;
      }
      else {
         BedDis = true;
      }
   }

   public void FixDisappear () {
      ObjectDisappearance[DisObj].SetActive(true);
      DisObj = -1;
      PCDis = false;
      BedDis = false;
   }

   #endregion

   #region Light Anomaly

   public void LightInit () {
      LightAnomaly[0].SetActive(false);
      LightAnomaly[1].SetActive(false);
   }

   public void FixLight () {
      LightAnomaly[0].SetActive(true);
      LightAnomaly[1].SetActive(true);
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
      while (Door.transform.localEulerAngles.y < 130f) {
         Door.transform.localEulerAngles = new Vector3(270, Mathf.Lerp(90, 130, elapsed / duration), 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   IEnumerator ResetDoor () {
      var duration = .25f;
      var elapsed = 0f;
      while (Door.transform.localEulerAngles.y > 90f) {
         Door.transform.localEulerAngles = new Vector3(270, Mathf.Lerp(130, 90, elapsed / duration), 0);
         yield return null;
         elapsed += Time.deltaTime;
      }
   }

   #endregion

   #region Painting

   public void PaintingInit () {
      //PosterAnomaly[0].SetActive(false);
      TypeOfPainting = Rnd.Range(0, 3);
      //TypeOfPainting = 0;
      Debug.Log(TypeOfPainting);
      if (TypeOfPainting == 0) {
         PosterAnomaly[0].GetComponent<SpriteRenderer>().flipX = false;
      }
      else {
         PosterAnomaly[0].SetActive(false);
         PosterAnomaly[TypeOfPainting].SetActive(true);
      }
   }

   public void FixPainting () {
      PosterAnomaly[0].GetComponent<SpriteRenderer>().flipX = true;
      if (TypeOfPainting != 0) {
         PosterAnomaly[0].SetActive(true);
         PosterAnomaly[TypeOfPainting].SetActive(false);
      }
      TypeOfPainting = -1;
   }

   #endregion

   #region Object Movement

   public void MoveInit () {
      MovedObject = Rnd.Range(0, ObjectMovement.Length);

      StartCoroutine(ShowMove());
      //ObjectMovement[MovedObject].SetActive(false);
      //FakeMovements[MovedObject].SetActive(true);
   }

   public IEnumerator ShowMove () {
      var duration = .25f;
      var elapsed = 0f;
      switch (MovedObject) {
         case 0:
            while (elapsed < duration) {
               ObjectMovement[0].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(47.814f, 90f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }

            break;
         case 1:
            while (elapsed < duration) {
               ObjectMovement[1].transform.localPosition = new Vector3(Mathf.Lerp(-.39f, 0.17f, elapsed / duration), 0.04f, Mathf.Lerp(-6.853f, -6.45f, elapsed / duration));
               ObjectMovement[1].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -21.163f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 2:
            while (elapsed < duration) {
               ObjectMovement[2].transform.localPosition = new Vector3(-0.1517326f, .37f, Mathf.Lerp(0.915f, 0.65f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 3:
            while (elapsed < duration) {
               ObjectMovement[3].transform.localPosition = new Vector3(Mathf.Lerp(0.35f, -0.07f, elapsed / duration), 1.51f, -0.14f);
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
               ObjectMovement[0].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(90f, 47.814f, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }



            break;
         case 1:
            while (elapsed < duration) {
               ObjectMovement[1].transform.localPosition = new Vector3(Mathf.Lerp(0.17f, -.39f, elapsed / duration), 0.04f, Mathf.Lerp(-6.45f, -6.853f, elapsed / duration));
               ObjectMovement[1].transform.localEulerAngles = new Vector3(0, Mathf.Lerp(-21.163f, 0, elapsed / duration), 0);
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 2:
            while (elapsed < duration) {
               ObjectMovement[2].transform.localPosition = new Vector3(-0.1517326f, .37f, Mathf.Lerp(0.65f, 0.915f, elapsed / duration));
               yield return null;
               elapsed += Time.deltaTime;
            }
            break;
         case 3:
            while (elapsed < duration) {
               ObjectMovement[3].transform.localPosition = new Vector3(Mathf.Lerp(-0.07f, 0.35f, elapsed / duration), 1.51f, -0.14f);
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
      BedMove = false;
      PCMove = false;
   }

   #endregion

   #region Camera Malfunction

   public void CameraInit () {
      Mod.BrokenCam = 0;
      if (Mod.CameraPos == 0) {
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
      AbyssCor = StartCoroutine(AbyssGrow(Abyss.transform.localScale));
   }

   public void FixAbyss () {
      if (AbyssCor != null) {
         StopCoroutine(AbyssCor);
      }
      AbyssCor = StartCoroutine(AbyssShrink(Abyss.transform.localScale));
   }

   public IEnumerator AbyssGrow (Vector3 From) {
      Debug.Log(From);
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
      Vector3 To = new Vector3(0.1095824f, 0.002272631f, 0.1095824f);
      int Dis = 0;
      while (elapsed < duration && Dis < 101) {
         Abyss.transform.localScale = Vector3.Lerp(From, To, elapsed / duration);
         //Debug.Log(Abyss.transform.localScale);
         yield return null;
         elapsed += Time.deltaTime;
         Dis++;
      }
      Abyss.SetActive(false);
      Abyss.transform.localScale = new Vector3(0.1095824f, 0.002272631f, 0.1095824f);
   }

   #endregion
}
