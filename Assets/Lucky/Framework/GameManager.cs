using System.Threading;
using Lucky.Kits.Managers;
using UnityEngine;
using Timer = Lucky.Kits.Utilities.Timer;

namespace Lucky.Framework
{
    public class GameManager : Engine
    {

        #region Singleton

        private static GameManager instance;

        public static GameManager Instance => instance;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        #endregion

        #region GameState

        public enum GameStateType
        {
            Play,
            Pause,
        }

        public GameStateType GameState { get; set; } = GameStateType.Play;

        #endregion

        private float freezeTimer;

        protected override void Update()
        {
            base.Update();
            freezeTimer -= Timer.DeltaTime(true);
            if (freezeTimer < 0 && Time.timeScale == 0)
                Time.timeScale = 1;
        }

        public void Freeze(float time)
        {
            freezeTimer = time;
            Time.timeScale = 0;
        }
    }
}