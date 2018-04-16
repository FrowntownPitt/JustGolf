using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorDirector : MonoBehaviour {
	bool started = false;
	GameObject[] spiders;
	GameObject[] spiderSkins;
	int k = 0;

	void OnTriggerEnter(Collider other){
		if(!started){
			
			started = true;

			for (int i = 0; i < spiderSkins.Length; i++) {
				spiderSkins [i].GetComponent<SkinnedMeshRenderer> ().enabled = true;
			}

			for (int i = 0; i < spiders.Length; i++) {
				spiders [i].GetComponent<Animation> ().PlayQueued("appear");
			}
		
		}
	}

	void Start(){
		spiders = GameObject.FindGameObjectsWithTag ("SpiderMain");
		spiderSkins = GameObject.FindGameObjectsWithTag ("SpiderSkin");
	}

	void Update(){
		k++;
		if (k > 100) {
			if (started) {
				for (int i = 0; i < spiders.Length; i++) {
					if(Random.Range(0,7) == 5)
						spiders [i].GetComponent<Animation> ().PlayQueued("spit");
					if(Random.Range(0,7) == 5)
						spiders [i].GetComponent<Animation> ().PlayQueued("hit1");
					if(Random.Range(0,7) == 5)
						spiders [i].GetComponent<Animation> ().PlayQueued("hit2");
					spiders [i].GetComponent<Animation> ().PlayQueued("idle");
				}
			}
		}
	}

}
