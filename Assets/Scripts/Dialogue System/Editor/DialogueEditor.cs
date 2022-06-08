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
        public static void ShowEditorWindow() // получаем окно Dialogue Editor
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]  // OnOpenAsset - это функция должна вызваться, когда мы пытаемя открыть ассет. Цифра - порядок вызова. Можно создать цепочку вызова, только по одному нажатию
        public static bool OnOpenAsset(int instanceID, int line) // дабл кликом открываем окно DialogueEditor, именно с типом Asset. instanceID - Id ассета по которому нажали
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue; // проверяет, есль ли Dialogue на ассете
                                                                                          // EditorUtility.InstanceIDToObject(instanceID) - Преобразует идентификатор
                                                                                          // экземпляра в ссылку на объект. Если объект не загружен с диска, загружает его с диска.
                                                                                          // т.е. просто даем ссылку на объект
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        // OnEnable вызывается, когда компонент или в этом случае EditorWindow физически включен.
        // Для всех практических целей для нашего EditorWindow это означает, что OnEnable вызывается при открытии Window.
        // (Он отключается, только когда мы закрываем окно).
        void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged; // Selection.selectionChanged - мышкой в инпекторе выбираю. Подписывается на событие

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D; // node1 - дефолтные текстуры. Ещё есть node0 и node2. Только где они? 
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20); // устанавливаем паддинг
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        // Вызывается всегда, когда я ГДЕ УГОДНО тыкаю в инспекторе, но дальше проверяет на наличие нужного ассета
        void OnSelectionChanged() 
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue; // проверят, является ли выбранный объект Dialogue
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue; // запихимаем НОВЫЙ вырбанный Dialogue в ТЕКУЩИЙ выбранный
                Repaint(); // перерисовывает GUI
            }
        }

        void OnGUI() // вызывается не всегда, например когда мышкой вожу и то не всегда, когда переключаюсь между окнами. ВЫЗЫВАЕТСЯ В ОКНЕ Dialogue Editor
        {
            //Event.current.type = EventType.MouseDown;  // current -  это дает вам текущее событие, которое вызвало OnGUI. Это для примера


            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected"); // EditorGUILayout - автоматом расставляет строки, чтобы они друг на дрга не наслаивались
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // начала скролла
                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize); // устанавили канвас по умолчанию
                Texture2D backgroundTex = Resources.Load("background") as Texture2D; // загружаем текстуру из папки Resources
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize); // скейлим текстуру через костыль, потому что в GUI.DrawTexture скейл только растягивает текстуру
                //GUI.DrawTexture(canvas, backgroundTex);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);


                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // перепибираем на что нажали 
                {
                    DrawConnection(node); // рисуем линии

                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // перепибираем на что нажали 
                {
                    DrawNode(node); // рисуем ноды
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    
                    selectedDialogue.CreateNode(creatingNode); // создаем дочернюю ноду. Добавляем уже в существующий лист
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
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition); // в draggingNode запихиваем ноду на котору мышкой нажали
                                                                                             // scrollPosition - добавили, потму что при скроле перестовали двигаться ноды 21 лекция
                if (draggingNode != null)
                {
                    //Debug.Log("Node psition " + draggingNode.rect.position);
                    //Debug.Log("Mouse psition " + Event.current.mousePosition);
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition; // мышка будет двигать ноду, там где нажата. Отнимаем, получаем отрицательное число
                    //Debug.Log("OFFSET " + draggingOffset);
                    Selection.activeObject = draggingNode; // будет показывать в инспекторе что внутри ноды
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue; // тыкаем на канвас - показывает тоже самое что и меню Project тыкнем на созданный нами диалог
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                
                //Debug.Log("Node psition drag " + draggingNode.rect.position);
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset); // меняем позицию выбранное ноде. т.к. по дефолту позиция ноды слева вверху
                                                                                           // заменяем её координатами нажатия мыши. Например, изначально нода (0, 0) 
                                                                                           // мышь (5, 5). draggingOffset будет (-5, -5). Теперь типо нода думает, что 
                                                                                           // её позиция это тык мыши
                //Debug.Log("Node psition after calculate " + draggingNode.rect.position);

                GUI.changed = true; // более точный вариант, чем Repaint()
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition; // двигаем канвас с помощью мыши

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
            //node.text = EditorGUILayout.TextField(node.text);  // позволяет менять текст прямо в окне. EditorGUILayout.TextField(node.text) - просто текст
            // который мы указали в SO
            // node.text - строка, в которой мы уже можем менять текст в окне

            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }

            GUILayout.BeginArea(node.GetRect(), style); // node.position - указываем позицию, nodeStyle - рисуем рамку

            //EditorGUI.BeginChangeCheck(); // Начинает проверять на изменения GUI

            //EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel); // название в эдиторе, чтобы разграничитть строчки. EditorStyles.whiteLabel - установил цвет заголовка

            node.SetText(EditorGUILayout.TextField(node.GetText())); // сюда пишел строки какие хотит в эдиторе
            //string newUniqueID = EditorGUILayout.TextField(node.uniqueID);


            //if (newText != node.text) // проверяет на наличие изменений, чтобы сохранить их.
            //if (EditorGUI.EndChangeCheck()) // проверяет на наличие изменений. Если какие-то зменения били между EditorGUI.BeginChangeCheck() и EndChangeCheck - вернет true
            //{
                //EditorUtility.SetDirty(selectedDialogue);  // когда мы поменяем тект в окне, то он сохраняет SO после закрытия юнити. Ичане после закрытия-открытия мы не
                // увидим наших изменения. Помечам это SO как Dirty

                //переместили в DialogueNode
                //Undo.RecordObject(node, "Update Dialogue Text");             // помещам ИМЕННО SO или монобехавер, а не ноду. Второй параметр - это как будет 
                                                                             // называться строчка в меню Edit в юнити. Ну и записываем конечно ДО изменений
                                                                             // Теперь не обязательно поменят Dirty, потому что RecordObject делает это за нас
                                                                             //node.uniqueID = newUniqueID;
                //newText;
            //}

            GUILayout.BeginHorizontal(); // рисуем кнопки горизонтально

            if (GUILayout.Button("Add")) // Создали кнопку "+" на ноде
            {
                creatingNode = node; // мы не можем сразу создать ноду, потому что все время идет итерация нод. Создаваемую ноду запихиваем в отдельную переменную
            }

            DrawLinkButton(node);

            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;
            }

            GUILayout.EndHorizontal(); // заканчиваем рисовать кнопки горизонтально

            GUILayout.EndArea(); // заканчиваем рисовать рамку
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
                    linkingParentNode.AddChild(node.name); // когда нажимаем Link => Child - устанавливаем связь между двумя нажатиями
                    linkingParentNode = null;
                }
            }
        }

        void DrawConnection(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y); // начала кривой на центре родительской ноды

            foreach (DialogueNode child in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(child.GetRect().xMin, child.GetRect().center.y); // конец дочерней ноды
                //Vector3 controlPointOffet = new Vector2(100, 0); // создает изгибы на линии
                Vector3 controlPointOffet = endPosition - startPosition; // создает изгибы на линии
                controlPointOffet.y = 0;
                controlPointOffet.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffet, endPosition - controlPointOffet, Color.white, null, 4f);
            }
        }

        DialogueNode GetNodeAtPoint(Vector2 point) // point - координаты мышки
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes()) // перебираем все ноды
            {
                if (node.GetRect().Contains(point)) // если мышка находится в координатах ноды(в её границах) - возвращаем
                {
                    foundNode = node; // если две ноды друг на над другом - возвращаем ПОСЛЕДНЮЮ. Чем выше индекс у ноды => она "накрывает" остальные ноды.
                                      // нода с индексом 0 никогда не будет "накрывать" ноду с индексом 1 => возвращаем ноду с индексом 1, потому что она последняя из этих двух
                }        
            }
            return foundNode; // последняя нода из выбранных
        }
    }
}
