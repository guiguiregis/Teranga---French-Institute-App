using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class BDReaderEngine : MonoBehaviour
{

    // Defines if board_clippers must be auto indexed following their order in parent hierarchy or not
    public bool auto_order = true;

    int current_board_index = 0;
    
    public float slide_speed = 4;
    bool move = false;
    Vector3 target;

    public GameObject board_clippers_container;
    public GameObject cam_overlay;
    public GameObject comic_mask;
    public GameObject splash;
    public GameObject ui_canvas;
    public GameObject puces_container;
    public GameObject puce_prefab;

    public GameObject next_board_btn;
    public GameObject play_btn;


    Camera main_camera ;

    // We get a reference to main_camera height . Height is always twice main_camera orthographic size 
    float cam_height;
    float cam_width;
    // Next orthographic size of the main_camera : Initialized to 1 . Camera default size : In use in BDZommer.cs
    public float cam_future_size = 1f;

    void Awake()
    {
        
        // FOR TESTS PURPOSE : REMOVE ALL PLAYERPREFS DATA HERE : THIS IS DONE ONCE IN THE REAL APP //
        // PlayerPrefs.DeleteAll();
        ////////////////////////////
       
        main_camera = Camera.main;
        cam_height = main_camera.orthographicSize * 2f;
        cam_width = cam_height * main_camera.aspect;
         
        InitScene(auto_order);

    }
    // Start is called before the first frame update
    void Start()
    {
        splash.transform.GetComponent<Animation>().Play("comic_fade_in");

        StartCoroutine(I_InitStart());
    }

    void Update() {
        
        if(move == true)
        {

            main_camera.transform.position = Vector3.Lerp( main_camera.transform.position , target , slide_speed * Time.deltaTime );
            
            if (TargetReached(main_camera.transform.position , target) == true)
            {
                move = false;
            }
        }

    

    }

 
    void AutoOrder()
    {
        for (int i = 0; i < board_clippers_container.transform.childCount; i++)
        {
            board_clippers_container.transform.GetChild(i).GetComponent<BoardClipper>().order = i+1;
        }
    }

    void InitScene(bool auto_order)
    {
        Color transparent  = Color.white;
        transparent.a = 0f;
        for (int i = 0; i < board_clippers_container.transform.childCount; i++)
        {   
            board_clippers_container.transform.GetChild(i).GetComponent<SpriteRenderer>().color = transparent;
       
       
            // Initialize puces

            Instantiate(puce_prefab , Vector3.zero, Quaternion.identity, puces_container.transform );


        }

        if (auto_order)
           AutoOrder();
        

    }

 
    void AnimatePuce(int index)
    {
         int prev_index = current_board_index;
         Transform prev_active_puce = puces_container.transform.GetChild(prev_index);
         Transform next_active_puce = puces_container.transform.GetChild(index);
          
         //@Deactivate ---------------------------------------------------------------
         if(prev_index != index)
         {

             Animation prev_anim = prev_active_puce.GetComponent<Animation>();
             prev_anim.GetComponent<Animation>()["puce_state"].speed = -1;
             prev_anim.GetComponent<Animation>()["puce_state"].time =  prev_anim.GetComponent<Animation>()["puce_state"].length;
             prev_anim.Play("puce_state");
         }
        
         //---------------------------------------------------------------------------
         //@Activate -----------------------------------------------------------------
         if(next_active_puce.localScale.x != 1.5)
         {
             Animation next_anim = next_active_puce.GetComponent<Animation>();
             next_anim.GetComponent<Animation>()["puce_state"].speed = 1;
             next_anim.GetComponent<Animation>()["puce_state"].time =  0;
             next_anim.Play("puce_state");

             //@Update current_board_index
             current_board_index = index;
         }
         //--------------------------------------------------------------------------
    }
    public void GoToBoardIndex(int index)
    {
        
        int child_index = index;
      
        if (child_index >=0 && child_index < board_clippers_container.transform.childCount )
        {

            // Updating next frame ui interactivity if exist and deactivate previous one (hide)
            // FindObjectOfType<BDInteractivity>().SetFrameUI(index);

            Vector3 clipper_pos = board_clippers_container.transform.GetChild(child_index).GetComponent<BoardClipper>().position;
            float clipper_width = board_clippers_container.transform.GetChild(child_index).GetComponent<BoardClipper>().width;
            float clipper_height = board_clippers_container.transform.GetChild(child_index).GetComponent<BoardClipper>().height;
            
         
            //@Center cam to clipper -----------------
            Vector3 cam_future_pos = clipper_pos; 
            cam_future_pos.x += (clipper_width/2); 
            cam_future_pos.y -= (clipper_height/2); 
            //-----------------------------------------
          
            //@Getting clipper scale for comic mask and main_camera size scale
            Vector3 clipper_scale = board_clippers_container.transform.GetChild(child_index).transform.localScale;

            //@Mask--------------------------------------
            comic_mask.transform.position = cam_future_pos;
            // clipper_scale.x -= 0.08f; 
            clipper_scale.y += 0.12f;
            Vector3 comic_mask_scale = comic_mask.transform.localScale;
            comic_mask.transform.localScale = clipper_scale;
            //---------------------------------------------
            //@Camera sizing-------------------------------
            //@Scale following the largest side of the board
            float size_ratio = clipper_height;
            float cam_ratio = cam_height;
            if (clipper_width > clipper_height)
            {
                size_ratio = clipper_width;
                cam_ratio = cam_width;

            }

            cam_future_size = size_ratio / cam_ratio;
            main_camera.orthographicSize = cam_future_size;
            //---------------------------------------------
            //Cam overlay scaling

            cam_overlay.transform.localScale *= cam_future_size;


            move = true;
            //@Reset to main_camera z index:Preventing main_camera to go behind the scene-------
            cam_future_pos.z = main_camera.transform.position.z;
            //---------------------------------------------
           
            //@Animate puce
            AnimatePuce(child_index);
           // current_board_index = child_index; : DONE INSIDE ANIMATE()

            target = cam_future_pos;


            if(child_index == board_clippers_container.transform.childCount-1)
            {

                play_btn.SetActive(true);
                next_board_btn.SetActive(false);

            }
            else
            {
                play_btn.SetActive(false);
                next_board_btn.SetActive(true); 
            }
 

        }
 
    }
    public void NextBoard()
    {
        GoToBoardIndex(current_board_index + 1);
    }

   public void PrevBoard()
    {
        GoToBoardIndex( current_board_index - 1);
    }

   public void SkipComic()
   {

     // Save last changes on fields
    //  FindObjectOfType<BDInteractivity>().SaveName();
    //  FindObjectOfType<BDInteractivity>().SaveCompany();
    //  FindObjectOfType<BDInteractivity>().SavePlayerData();
     // Load menu screen
     SceneManager.LoadScene("Menu_screen");

   } 

    public bool TargetReached(Vector3 source_position , Vector3  target_position)
    {
        if (source_position == target_position) return true;
          return false;
       
    }

    void InitStart(){

         GoToBoardIndex(0);
         AnimatePuce(0);

  }

    IEnumerator I_InitStart()
    {
        
         yield return new WaitForSeconds(0.1f);
        //  ui_canvas.SetActive(true);
         InitStart();

    }

}
