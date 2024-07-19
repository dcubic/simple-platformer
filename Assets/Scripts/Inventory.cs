using System.Collections.Generic;
using UnityEngine;

public sealed class Inventory {
    private static readonly Inventory instance = new();
    public bool hasKey = false;
    public int gemCount = 0;
    public void AddGem() {
        gemCount += 1;
    }

    public void AddKey() {
        hasKey = true;
    }

    static Inventory() {

    }
    private Inventory() {

    }

    public static Inventory Instance {
        get {
            return instance;
        }
    }

    public void ResetInventory() {
        gemCount = 0;
        hasKey = false;
    }
}
