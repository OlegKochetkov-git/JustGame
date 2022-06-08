using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode;
        [NonSerialized] DialogueNode deletingNode;
        [NonSerialized] DialogueNode linkingParentNode;
        Vector2 scrollPosition;
        [NonSerialized] bool draggingCanvas;
        [NonSerialized] Vector2 draggingCanvasOffset;

        const float canvasSize = 4000f;
        const float backgroundSize = 50f;

        [MenuItem("Window/Dialog Editor")]
        public static void ShowEditorWindow() // �������� ���� Dialogue Editor
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]  // OnOpenAsset - ��� ������� ������ ���������, ����� �� ������� ������� �����. ����� - ������� ������. ����� ������� ������� ������, ������ �� ������ �������
        public static bool OnOpenAsset(int instanceID, int line) // ���� ������ ��������� ���� DialogueEditor, ������ � ����� Asset. instanceID - Id ������ �� �������� ������
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue; // ���������, ���� �� Dialogue �� ������
                                                                                          // EditorUtility.InstanceIDToObject(instanceID) - ����������� �������������
                                                                                          // ���������� � ������ �� ������. ���� ������ �� �������� � �����, ��������� ��� � �����.
                                                                                          // �.�. ������ ���� ������ �� ������
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        // OnEnable ����������, ����� ��������� ��� � ���� ������ EditorWindow ��������� �������.
        // ��� ���� ������������ ����� ��� ������ EditorWindow ��� ��������, ��� OnEnable ���������� ��� �������� Window.
        // (�� �����������, ������ ����� �� ��������� ����).
        void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged; // Selection.selectionChanged - ������ � ��������� �������. ������������� �� �������

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D; // node1 - ��������� ��������. ��� ���� node0 � node2. ������ ��� ���? 
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20); // ������������� �������
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        // ���������� ������, ����� � ��� ������ ����� � ����������, �� ������ ��������� �� ������� ������� ������
        void OnSelectionChanged() 
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue; // ��������, �������� �� ��������� ������ Dialogue
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue; // ���������� ����� ��������� Dialogue � ������� ���������
                Repaint(); // �������������� GUI
            }
        }

        void OnGUI() // ���������� �� ������, �������� ����� ������ ���� � �� �� ������, ����� ������������ ����� ������. ���������� � ���� Dialogue Editor
        {
            //Event.current.type = EventType.MouseDown;  // current -  ��� ���� ��� ������� �������, ������� ������� OnGUI. ��� ��� �������


            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected"); // EditorGUILayout - ��������� ����������� ������, ����� ��� ���� �� ���� �� ������������
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // ������ �������
                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize); // ���������� ������ �� ���������
                Texture2D backgroundTex = Resources.Load("background") as Texture2D; // ��������� �������� �� ����� Resources
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize); // ������� �������� ����� �������, ������ ��� � GUI.DrawTexture ����� ������ ����������� ��������
                //GUI.DrawTexture(canvas, backgroundTex);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);


                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // ������������ �� ��� ������ 
                {
                    DrawConnection(node); // ������ �����

                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // ������������ �� ��� ������ 
                {
                    DrawNode(node); // ������ ����
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    
                    selectedDialogue.CreateNode(creatingNode); // ������� �������� ����. ��������� ��� � ������������ ����
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }

            }
        }

        

        void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null) 
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition); // � draggingNode ���������� ���� �� ������ ������ ������
                                                                                             // scrollPosition - ��������, ����� ��� ��� ������ ����������� ��������� ���� 21 ������
                if (draggingNode != null)
                {
                    //Debug.Log("Node psition " + draggingNode.rect.position);
                    //Debug.Log("Mouse psition " + Event.current.mousePosition);
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition; // ����� ����� ������� ����, ��� ��� ������. ��������, �������� ������������� �����
                    //Debug.Log("OFFSET " + draggingOffset);
                    Selection.activeObject = draggingNode; // ����� ���������� � ���������� ��� ������ ����
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue; // ������ �� ������ - ���������� ���� ����� ��� � ���� Project ������ �� ��������� ���� ������
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                
                //Debug.Log("Node psition drag " + draggingNode.rect.position);
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset); // ������ ������� ��������� ����. �.�. �� ������� ������� ���� ����� ������
                                                                                           // �������� � ������������ ������� ����. ��������, ���������� ���� (0, 0) 
                                                                                           // ���� (5, 5). draggingOffset ����� (-5, -5). ������ ���� ���� ������, ��� 
                                                                                           // � ������� ��� ��� ����
                //Debug.Log("Node psition after calculate " + draggingNode.rect.position);

                GUI.changed = true; // ����� ������ �������, ��� Repaint()
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition; // ������� ������ � ������� ����

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null) 
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        

        void DrawNode(DialogueNode node)
        {
            //node.text = EditorGUILayout.TextField(node.text);  // ��������� ������ ����� ����� � ����. EditorGUILayout.TextField(node.text) - ������ �����
            // ������� �� ������� � SO
            // node.text - ������, � ������� �� ��� ����� ������ ����� � ����

            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }

            GUILayout.BeginArea(node.GetRect(), style); // node.position - ��������� �������, nodeStyle - ������ �����

            //EditorGUI.BeginChangeCheck(); // �������� ��������� �� ��������� GUI

            //EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel); // �������� � �������, ����� ������������� �������. EditorStyles.whiteLabel - ��������� ���� ���������

            node.SetText(EditorGUILayout.TextField(node.GetText())); // ���� ����� ������ ����� ����� � �������
            //string newUniqueID = EditorGUILayout.TextField(node.uniqueID);


            //if (newText != node.text) // ��������� �� ������� ���������, ����� ��������� ��.
            //if (EditorGUI.EndChangeCheck()) // ��������� �� ������� ���������. ���� �����-�� �������� ���� ����� EditorGUI.BeginChangeCheck() � EndChangeCheck - ������ true
            //{
                //EditorUtility.SetDirty(selectedDialogue);  // ����� �� �������� ���� � ����, �� �� ��������� SO ����� �������� �����. ����� ����� ��������-�������� �� ��
                // ������ ����� ���������. ������� ��� SO ��� Dirty

                //����������� � DialogueNode
                //Undo.RecordObject(node, "Update Dialogue Text");             // ������� ������ SO ��� �����������, � �� ����. ������ �������� - ��� ��� ����� 
                                                                             // ���������� ������� � ���� Edit � �����. �� � ���������� ������� �� ���������
                                                                             // ������ �� ����������� ������� Dirty, ������ ��� RecordObject ������ ��� �� ���
                                                                             //node.uniqueID = newUniqueID;
                //newText;
            //}

            GUILayout.BeginHorizontal(); // ������ ������ �������������

            if (GUILayout.Button("Add")) // ������� ������ "+" �� ����
            {
                creatingNode = node; // �� �� ����� ����� ������� ����, ������ ��� ��� ����� ���� �������� ���. ����������� ���� ���������� � ��������� ����������
            }

            DrawLinkButton(node);

            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;
            }

            GUILayout.EndHorizontal(); // ����������� �������� ������ �������������

            GUILayout.EndArea(); // ����������� �������� �����
        }

        void DrawLinkButton(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    
                    linkingParentNode.RemoveChild(node.name); 
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.AddChild(node.name); // ����� �������� Link => Child - ������������� ����� ����� ����� ���������
                    linkingParentNode = null;
                }
            }
        }

        void DrawConnection(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y); // ������ ������ �� ������ ������������ ����

            foreach (DialogueNode child in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(child.GetRect().xMin, child.GetRect().center.y); // ����� �������� ����
                //Vector3 controlPointOffet = new Vector2(100, 0); // ������� ������ �� �����
                Vector3 controlPointOffet = endPosition - startPosition; // ������� ������ �� �����
                controlPointOffet.y = 0;
                controlPointOffet.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffet, endPosition - controlPointOffet, Color.white, null, 4f);
            }
        }

        DialogueNode GetNodeAtPoint(Vector2 point) // point - ���������� �����
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // ���������� ��� ����
            {
                if (node.GetRect().Contains(point)) // ���� ����� ��������� � ����������� ����(� � ��������) - ����������
                {
                    foundNode = node; // ���� ��� ���� ���� �� ��� ������ - ���������� ���������. ��� ���� ������ � ���� => ��� "���������" ��������� ����.
                                      // ���� � �������� 0 ������� �� ����� "���������" ���� � �������� 1 => ���������� ���� � �������� 1, ������ ��� ��� ��������� �� ���� ����
                }        
            }
            return foundNode; // ��������� ���� �� ���������
        }
    }
}
