#if UNITY_EDITOR
using System;
using System.Linq;
using GamePlay.CallingInteraction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorSupport.CallingInteractionGraphEditor
{
  public class ResponseInspectorView : VisualElement
  {
    private ResponseNodeView targetNode;
    private readonly IMGUIContainer container;
    private readonly ResponseGraphAuthoringWindow window;

    public ResponseInspectorView(ResponseGraphAuthoringWindow window)
    {
      this.window = window;
      style.flexGrow = 1f;
      style.paddingLeft = 4f;
      style.paddingRight = 4f;

      container = new IMGUIContainer(DrawInspector)
      {
        style = { flexGrow = 1f }
      };

      Add(container);
    }

    public void RefreshInspector()
    {
      container.MarkDirtyRepaint();
    }

    public void SetTarget(ResponseNodeView nodeView)
    {
      targetNode = nodeView;
      RefreshInspector();
    }

    private void DrawInspector()
    {
      if (targetNode == null)
      {
        EditorGUILayout.LabelField("Select a node to edit.");
        return;
      }

      var data = window.GetNodeData(targetNode.Data.Id);

      EditorGUI.BeginChangeCheck();
      var newId = EditorGUILayout.TextField("Identifier", data.Id ?? string.Empty);
      if (EditorGUI.EndChangeCheck())
      {
        if (!window.TryRenameNode(targetNode, newId))
        {
          RefreshInspector();
        }
      }

      EditorGUI.BeginChangeCheck();
      EditorGUILayout.PrefixLabel("Content");
      var content = EditorGUILayout.TextArea(data.Content ?? string.Empty, GUILayout.MinHeight(60f));
      if (EditorGUI.EndChangeCheck())
      {
        data.Content = content;
        window.UpdateNodeData(data, false);
      }

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Transfers", EditorStyles.boldLabel);

      if (data.Transfers == null || data.Transfers.Length == 0)
      {
        EditorGUILayout.HelpBox("No transfers. Add one to connect responses.", MessageType.Info);
      }
      else
      {
        for (var i = 0; i < data.Transfers.Length; i++)
        {
          DrawTransfer(ref data, i);
        }
      }

      if (GUILayout.Button("Add Transfer", GUILayout.Height(24f)))
      {
        var transfers = data.Transfers ?? Array.Empty<NodeTransfer>();
        var list = transfers.ToList();
        list.Add(new NodeTransfer
        {
          Condition = new NodeTransferCondition
          {
            Condition = NodeTransferConditionType.None,
            Value = Array.Empty<int>()
          },
          ToNodeId = null
        });

        data.Transfers = list.ToArray();
        window.UpdateNodeData(data, true);
      }
    }

    private void DrawTransfer(ref ResponseNode data, int index)
    {
      if (data.Transfers == null || index < 0 || index >= data.Transfers.Length) return;

      var transfer = data.Transfers[index];

      EditorGUILayout.BeginVertical(EditorStyles.helpBox);
      EditorGUILayout.LabelField($"Transfer {index + 1}", EditorStyles.boldLabel);

      var changed = false;

      EditorGUI.BeginChangeCheck();
      var conditionType = (NodeTransferConditionType)EditorGUILayout.EnumPopup("Condition", transfer.Condition.Condition);
      if (EditorGUI.EndChangeCheck())
      {
        transfer.Condition.Condition = conditionType;
        changed = true;
      }

      var values = transfer.Condition.Value ?? Array.Empty<int>();
      var valueList = values.ToList();
      EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);

      var removeIndex = -1;
      for (var i = 0; i < valueList.Count; i++)
      {
        EditorGUILayout.BeginHorizontal();
        var newValue = EditorGUILayout.IntField($"[{i}]", valueList[i]);
        if (newValue != valueList[i])
        {
          valueList[i] = newValue;
          changed = true;
        }

        if (GUILayout.Button("-", GUILayout.Width(24f)))
        {
          removeIndex = i;
        }
        EditorGUILayout.EndHorizontal();
      }

      if (removeIndex >= 0)
      {
        valueList.RemoveAt(removeIndex);
        changed = true;
      }

      if (GUILayout.Button("Add Value"))
      {
        valueList.Add(0);
        changed = true;
      }

      if (changed)
      {
        transfer.Condition.Value = valueList.ToArray();
        data.Transfers[index] = transfer;
        window.UpdateNodeData(data, true);
      }

      var targetLabel = string.IsNullOrEmpty(transfer.ToNodeId) ? "(not connected)" : transfer.ToNodeId;
      EditorGUILayout.LabelField("To Node", targetLabel);

      if (GUILayout.Button("Clear Connection"))
      {
        transfer.ToNodeId = null;
        data.Transfers[index] = transfer;
        window.UpdateNodeData(data, true);
      }

      if (GUILayout.Button("Remove Transfer"))
      {
        var list = data.Transfers.ToList();
        list.RemoveAt(index);
        data.Transfers = list.ToArray();
        window.UpdateNodeData(data, true);
      }

      EditorGUILayout.EndVertical();
    }

  }
}
#endif
