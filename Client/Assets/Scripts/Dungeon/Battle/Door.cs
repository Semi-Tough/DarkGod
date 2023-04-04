using UnityEngine;
public class Door : MonoBehaviour {
	public int waveIndex;
	public MapMgr mapMgr;

	private void OnTriggerExit(Collider other) {
		if(!other.CompareTag("Player")) return;
		if(mapMgr != null) {
			mapMgr.LoadMonsterByWaveId(waveIndex);
		}
		GetComponent<BoxCollider>().enabled = false;
	}
}