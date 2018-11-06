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
            return "Damage Taken: " + damageTaken +" Number of kills: " + numberOfKills + " Time in level: " + timeInLevel;
        }
    }
    private LevelStats currentLevelStats;

    public List<LevelStats> levelsStats;

	void Awake () {
        DontDestroyOnLoad(this.gameObject);
        levelsStats = new List<LevelStats>();
	}
	
    public void StartNewLevel(Level level) {
        currentLevelStats = new LevelStats(level);
    }

    public void FinishLevel() {
        currentLevelStats.StopTime();
        levelsStats.Add(currentLevelStats);
        print(currentLevelStats);
        currentLevelStats = null;
    }

    public void PlayerTakesDamage() {
        currentLevelStats.damageTaken++;
    }

    public void PlayerKillsEnemy() {
        currentLevelStats.numberOfKills++;
    }
}
