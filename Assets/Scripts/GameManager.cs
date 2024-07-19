using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager {
    private static GameManager instance;
    
    public static GameManager Instance() {
        instance ??= new();
        return instance;
    }

    private GameManager() {}

    public void ResetGame() {
        Inventory.Instance.ResetInventory();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
