using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    // CreateAssetMenu - делает две вещи: создает SO и ассет.
    // В то время как DialogueNode newNode = CreateInstance<DialogueNode>() создает только SO
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")] 
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(250f, 0f);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

//#if UNITY_EDITOR // вызывает только в Editor
        //void Awake() // когда создаю в инспекторе на ScriptableObject, то вызывается Awake
        //{
            // переместили в OnBeforeSerialize
            //if (nodes.Count == 0)
            //{
            //    //DialogueNode rootNode = new DialogueNode();
            //    //rootNode.uniqueID = Guid.NewGuid().ToString(); // создает никальный номер ID. Guid - globally unique identifier. Гарантирует уникальность
            //    //nodes.Add(rootNode); // просто по дефолту создает хоя бы одну ноду
            //    CreateNode(null);
            //}
        //}
//#endif

        void OnValidate() // при создании скрипка и при изменении данных в инспекторе. При сборке игры ОБЯЗАТЕЛЬНО вызывать это в Awake
        {
            
            nodeLookup.Clear();

            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node; // запихивает в словарь. Каждая ID привязывается к своей ноде
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes() // IEnumerable вместо List'a потому что нам мы может в foreach его юзать.
                                                       // Дальше мы можем свободно менять nodes в другой тип. Зачем? хз
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            //List<DialogueNode> result = new List<DialogueNode>();  // использовалось вместе в return

            foreach (var childID in parentNode.GetChildren()) // смотрим в чаилды с поступающей ноды (parentNode). 
            {
                if (nodeLookup.ContainsKey(childID)) // проверяет, если ли вообще такой ключ
                {
                    //result.Add(nodeLookup[childID]); // записыват в result НОДЫ, берет их из словаря, которые имеют child
                    yield return nodeLookup[childID];
                }
            }

            //return result; // возвращает ноды, которые в списке чайлдов именно у это ноды
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete); // удаляем ноду
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>(); // создаем пустую ноду CreateInstance - спец класс
            newNode.name = Guid.NewGuid().ToString(); // присваиваем её уникальный ID

            if (parent != null)
            {
                parent.AddChild(newNode.name); // добавляем ЭТУ ноду в дочерние РОДИТЕЛЬСКОЙ parent
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode); // запихиваем в общий список нод

            OnValidate(); // обновляет словарь, где распределяется ID и дочерние элементы
        }

        void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name); // удаляем ID
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == string.Empty)
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
