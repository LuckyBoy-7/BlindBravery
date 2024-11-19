using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using Lucky.Kits.Collections;
using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Framework.Inputs_
{
    /// <summary>
    /// 对VirtualButton的管理
    /// </summary>
    public static class Inputs
    {

        public static void Initialize()
        {
            Esc = new VirtualButton(Settings.Esc, 0.1f);
            Pause = new VirtualButton(Settings.Pause, 0.1f);

            Left = new VirtualButton(Settings.Left, 0);
            Right = new VirtualButton(Settings.Right, 0);
            Up = new VirtualButton(Settings.Up, 0);
            Down = new VirtualButton(Settings.Down, 0);
            Jump = new VirtualButton(Settings.Jump, 0.08f);
            Grab = new VirtualButton(Settings.Grab, 0);
            Dash = new VirtualButton(Settings.Dash, 0.08f);

            MenuLeft = new VirtualButton(Settings.MenuLeft, 0).SetRepeat(0.4f, 0.1f);
            MenuRight = new VirtualButton(Settings.MenuRight, 0).SetRepeat(0.4f, 0.1f);
            MenuUp = new VirtualButton(Settings.MenuUp, 0).SetRepeat(0.4f, 0.1f);
            MenuDown = new VirtualButton(Settings.MenuDown, 0).SetRepeat(0.4f, 0.1f);
            MenuConfirm = new VirtualButton(Settings.MenuConfirm, 0);
            MenuCancel = new VirtualButton(Settings.MenuCancel, 0);
            MenuJournal = new VirtualButton(Settings.MenuJournal, 0);
            MoveX = new VirtualIntegerAxis(Settings.Left, Settings.Right);
            MoveY = new VirtualIntegerAxis(Settings.Down, Settings.Up);
            
            Player1Left = new VirtualButton(Settings.Player1Left, 0);
            Player1Right = new VirtualButton(Settings.Player1Right, 0);
            Player1Up = new VirtualButton(Settings.Player1Up, 0);
            Player1Down = new VirtualButton(Settings.Player1Down, 0);
            Player1Shoot = new VirtualButton(Settings.Player1Shoot, 0);
            Player1Rotate = new VirtualIntegerAxis(Settings.Player1Right, Settings.Player1Left);

            Player2Left = new VirtualButton(Settings.Player2Left, 0);
            Player2Right = new VirtualButton(Settings.Player2Right, 0);
            Player2Up = new VirtualButton(Settings.Player2Up, 0);
            Player2Down = new VirtualButton(Settings.Player2Down, 0);
            Player2Shoot = new VirtualButton(Settings.Player2Shoot, 0);
            Player2Rotate = new VirtualIntegerAxis(Settings.Player2Right, Settings.Player2Left);
        }

        public static void Update()
        {
            foreach (var button in inputs)
            {
                button.Update();
            }
        }

        public static void Register(VirtualInput input) => inputs.Add(input);
        public static void DeRegister(VirtualInput input) => inputs.Remove(input);

        private static List<VirtualInput> inputs = new();
        public static VirtualButton Esc;
        public static VirtualButton Pause;
        public static VirtualButton Left;
        public static VirtualButton Right;
        public static VirtualButton Up;
        public static VirtualButton Down;
        public static VirtualButton Jump;
        public static VirtualButton Grab;
        public static VirtualButton Dash;
        public static VirtualButton MenuLeft;
        public static VirtualButton MenuRight;
        public static VirtualButton MenuUp;
        public static VirtualButton MenuDown;
        public static VirtualButton MenuConfirm;
        public static VirtualButton MenuCancel;
        public static VirtualButton MenuJournal;
        public static VirtualIntegerAxis MoveX;
        public static VirtualIntegerAxis MoveY;
        
        public static VirtualButton Player1Left;
        public static VirtualButton Player1Right;
        public static VirtualButton Player1Up;
        public static VirtualButton Player1Down;
        public static VirtualIntegerAxis Player1Rotate;
        public static VirtualButton Player1Shoot;

        public static VirtualButton Player2Left;
        public static VirtualButton Player2Right;
        public static VirtualButton Player2Up;
        public static VirtualButton Player2Down;
        public static VirtualIntegerAxis Player2Rotate;
        public static VirtualButton Player2Shoot;

        public static KeyCode GetCurrentPressedKey()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(key))
                    return key;
            }

            return KeyCode.None;
        }
    }
}