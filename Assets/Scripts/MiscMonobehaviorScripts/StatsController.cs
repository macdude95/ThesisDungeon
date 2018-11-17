using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour {

    public class LevelStats {
        public Level level;
        public int damageTaken;
        public int numberOfKills;
        public float timeInLevel;
        private float levelStartTime;

        public LevelStats(Level level) {
            this.level = level;
            StartTime();
        }

        private void StartTime() {
            levelStartTime = Time.time;
        }

        public void StopTime() {
            timeInLevel = Time.time - levelStartTime;
        }

        public override string ToString() {
            return "Damage Taken: " + damageTaken + " Number of kills: " + numberOfKills + " Time in level: " + timeInLevel;
        }
    }
    private LevelStats currentLevelStats;

    public List<LevelStats> levelsStats;

    void Awake() {
        levelsStats = new List<LevelStats>();
    }

    private void UpdateStartNewLevel(Level level) {
        currentLevelStats = new LevelStats(level);
    }

    private void UpdateFinishLevel() {
        currentLevelStats.StopTime();
        levelsStats.Add(currentLevelStats);
        print(currentLevelStats);
        currentLevelStats = null;
    }

    private void UpdatePlayerTakesDamage() {
        currentLevelStats.damageTaken++;
    }

    private void UpdatePlayerKillsEnemy() {
        currentLevelStats.numberOfKills++;
    }

    public static void PlayerKillsEnemy() {
        StatsController statsController = FindObjectOfType<StatsController>();
        if (statsController != null) {
            statsController.UpdatePlayerKillsEnemy();
        }
    }

    public static void PlayerTakesDamage() {
        StatsController statsController = FindObjectOfType<StatsController>();
        if (statsController != null) {
            statsController.UpdatePlayerTakesDamage();
        }
    }

    public static void StartNewLevel(Level level) {
        StatsController statsController = FindObjectOfType<StatsController>();
        if (statsController != null) {
            statsController.UpdateStartNewLevel(level);
        }
    }

    public static void FinishLevel() {
        StatsController statsController = FindObjectOfType<StatsController>();
        if (statsController != null) {
            statsController.UpdateFinishLevel();
        }
    }

}
