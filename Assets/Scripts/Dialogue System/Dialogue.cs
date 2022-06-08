using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    // CreateAssetMenu - ������ ��� ����: ������� SO � �����.
    // � �� ����� ��� DialogueNode newNode = CreateInstance<DialogueNode>() ������� ������ SO
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")] 
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(250f, 0f);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

//#if UNITY_EDITOR // �������� ������ � Editor
        //void Awake() // ����� ������ � ���������� �� ScriptableObject, �� ���������� Awake
        //{
            // ����������� � OnBeforeSerialize
            //if (nodes.Count == 0)
            //{
            //    //DialogueNode rootNode = new DialogueNode();
            //    //rootNode.uniqueID = Guid.NewGuid().ToString(); // ������� ��������� ����� ID. Guid - globally unique identifier. ����������� ������������
            //    //nodes.Add(rootNode); // ������ �� ������� ������� ��� �� ���� ����
            //    CreateNode(null);
            //}
        //}
//#endif

        void OnValidate() // ��� �������� ������� � ��� ��������� ������ � ����������. ��� ������ ���� ����������� �������� ��� � Awake
        {
            
            nodeLookup.Clear();

            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node; // ���������� � �������. ������ ID ������������� � ����� ����
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes() // IEnumerable ������ List'a ������ ��� ��� �� ����� � foreach ��� �����.
                                                       // ������ �� ����� �������� ������ nodes � ������ ���. �����? ��
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            //List<DialogueNode> result = new List<DialogueNode>();  // �������������� ������ � return

            foreach (var childID in parentNode.GetChildren()) // ������� � ������ � ����������� ���� (parentNode). 
            {
                if (nodeLookup.ContainsKey(childID)) // ���������, ���� �� ������ ����� ����
                {
                    //result.Add(nodeLookup[childID]); // ��������� � result ����, ����� �� �� �������, ������� ����� child
                    yield return nodeLookup[childID];
                }
            }

            //return result; // ���������� ����, ������� � ������ ������� ������ � ��� ����
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
            nodes.Remove(nodeToDelete); // ������� ����
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>(); // ������� ������ ���� CreateInstance - ���� �����
            newNode.name = Guid.NewGuid().ToString(); // ����������� � ���������� ID

            if (parent != null)
            {
                parent.AddChild(newNode.name); // ��������� ��� ���� � �������� ������������ parent
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode); // ���������� � ����� ������ ���

            OnValidate(); // ��������� �������, ��� �������������� ID � �������� ��������
        }

        void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name); // ������� ID
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
