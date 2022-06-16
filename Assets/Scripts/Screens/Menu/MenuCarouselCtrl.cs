using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCarouselCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    int currentIndex = 0;
    float itemsMargin =  20;
    float pad = 0;
    
    int itemsCount;

    public GameObject itemsContainer;

    void Start()
    {
        itemsCount = itemsContainer.transform.childCount;
        if (itemsCount > 0)
        {
           pad = itemsContainer.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
           pad += itemsMargin;
        }

        Debug.Log(pad);
    }
  

  public void Navigate(int direction)
  {

      int index = currentIndex + direction;
      GetToIndex(index);

      
  }
  
  public void GetToIndex(int index)
  {
     
      if (index >=0 && index < itemsCount)
      {
          //@Are we going to left or right
          //@And by how many steps
          int indexGap = index - currentIndex;
 
           float padSize = pad * (float)indexGap;
    
            Vector3 dest = itemsContainer.transform.position;
            dest.x -= padSize ;
            itemsContainer.transform.position = Vector3.Lerp(itemsContainer.transform.position , dest , 1f);
            
            itemsContainer.transform.GetChild(index).GetComponent<Animation>().Play("active");
            // itemsContainer.transform.GetChild(currentIndex).transform.localScale = new Vector3(0.843f, 0.843f , 0.843f);

            currentIndex = index;

      }

  }

  public void LoadModule()
  {
      ModuleItem module_item = itemsContainer.transform.GetChild(currentIndex).GetComponent<ModuleItem>();
      string module_name = module_item.GetComponent<ModuleItem>().Mname;
      int level_index = module_item.GetComponent<ModuleItem>().index;
      StageManager.instance.LoadModule(module_name, level_index);
  }
    // Update is called once per frame
    void Update()
    {
        
    }



}
