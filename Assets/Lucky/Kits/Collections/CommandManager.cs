using System;
using System.Collections.Generic;
using Lucky.Kits.Managers;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Lucky.Kits.Collections
{

    public interface ICommand
    {
        public void Do();
        public void Undo();
    }

    public class CommandManager : Singleton<CommandManager>
    {

        [ShowInInspector] private List<List<ICommand>> commands = new();
        [SerializeField] private int idx = -1; // 最后一个可撤销的操作序列的索引


        public void CreateNewCommandSequence() // 表示一个操作下的首个command
        {
            while (commands.Count > idx + 1)
                commands.RemoveAt(commands.Count - 1);

            commands.Add(new());
            idx += 1;
        }

        public void AddCommand(ICommand command) => commands[idx].Add(command);

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (Input.GetKeyDown(KeyCode.Z))
                Undo();
            else if (Input.GetKeyDown(KeyCode.X))
                Do(); // 返回撤销
        }

        private void Do()
        {
            Debug.Log("Do");
            if (idx == commands.Count - 1)
            {
                Debug.Log("已经回溯到底啦！");
                return;
            }

            commands[++idx].ForEach(command => command.Do());
        }

        private void Undo()
        {
            Debug.Log("Undo");
            if (idx == -1)
            {
                Debug.Log("已经撤销到底啦！");
                return;
            }

            commands[idx--].ForEach(command => command.Undo());
        }
    }
}