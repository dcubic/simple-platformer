//using UnityEngine;

//public class PlayerCollector : MonoBehaviour {

//    private string collectibleTag = "Collectible";
//    private void onTriggerEnter(Collider2D collider) {
//        if (collider.CompareTag(collectibleTag)) {
//            collectItem(collider.gameObject);
//        }
//    }

//    private void collectItem(GameObject item) {
//        Inventory.Instance.AddItem(item.GetComponent<Item>());
//        Destroy(item);
//    }

//}
