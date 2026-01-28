#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using GamePlay.CallingInteraction;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorSupport.CallingInteractionGraphEditor
{
  public class ResponseNodeView : Node
  {
    public ResponseNode Data { get; private set; }
    public Port InputPort { get; private set; }
    private const float DefaultWidth = 260f;
    private const float BaseHeight = 120f;
    private const float LineHeight = 18f;
    private const int CharsPerLine = 28;

    private readonly ResponseGraphAuthoringWindow window;
    private readonly ResponseGraphView graphView;
    private readonly List<Port> transferPorts = new List<Port>();
    private readonly TextField contentField;
    private readonly TextField identifierField;
    private bool suppressContentChange;
    private bool suppressIdentifierChange;
    private bool showingContentPlaceholder;

    public ResponseNodeView(ResponseGraphAuthoringWindow window, ResponseGraphView graphView, ResponseNode data)
    {
      this.window = window;
      this.graphView = graphView;
      Data = NormalizeData(data);

      title = string.Empty;
      viewDataKey = Data.Id;

      identifierField = new TextField
      {
        isDelayed = true,
        value = Data.Id
      };
      identifierField.style.flexGrow = 1f;
      identifierField.style.minWidth = 120f;
      identifierField.RegisterValueChangedCallback(evt =>
      {
        if (suppressIdentifierChange) return;
        if (!window.TryRenameNode(this, evt.newValue))
        {
          suppressIdentifierChange = true;
          identifierField.value = Data.Id;
          suppressIdentifierChange = false;
        }
      });
      titleContainer.Add(identifierField);

      contentField = new TextField
      {
        multiline = true,
        value = Data.Content ?? string.Empty
      };
      contentField.label = string.Empty;
      contentField.style.minHeight = 80f;
      contentField.style.flexGrow = 1f;
      contentField.style.width = Length.Percent(100);
      contentField.style.whiteSpace = WhiteSpace.Normal;
      contentField.RegisterValueChangedCallback(evt =>
      {
        if (suppressContentChange) return;
        if (showingContentPlaceholder) return;
        var updated = Data;
        updated.Content = evt.newValue;
        window.UpdateNodeData(updated, false);
      });
      contentField.RegisterCallback<FocusInEvent>(_ =>
      {
        if (!showingContentPlaceholder) return;
        suppressContentChange = true;
        showingContentPlaceholder = false;
        contentField.value = string.Empty;
        contentField.style.color = new StyleColor(Color.white);
        suppressContentChange = false;
      });
      contentField.RegisterCallback<FocusOutEvent>(_ =>
      {
        ApplyContentPlaceholderIfNeeded(Data.Content);
      });
      mainContainer.Add(contentField);

      ApplyContentPlaceholderIfNeeded(Data.Content);

      SetPosition(new Rect(Vector2.zero, GetEstimatedSize()));

      CreateInputPort();
      RefreshTransferPorts();
    }

    public override void OnSelected()
    {
      base.OnSelected();
      window.NotifyNodeSelected(this);
    }

    public override void SetPosition(Rect newPos)
    {
      base.SetPosition(newPos);
      window.UpdateNodePosition(Data.Id, newPos.position);
    }

    public void ApplyData(ResponseNode updated)
    {
      Data = NormalizeData(updated);
      title = string.Empty;
      ApplyContentPlaceholderIfNeeded(Data.Content);
      suppressIdentifierChange = true;
      identifierField.value = Data.Id;
      suppressIdentifierChange = false;
      UpdateNodeSize();
      RefreshTransferPorts();
    }

    private void UpdateNodeSize()
    {
      var position = GetPosition();
      var size = GetEstimatedSize();
      SetPosition(new Rect(position.position, size));
    }

    public Vector2 GetEstimatedSize()
    {
      var transfers = Data.Transfers ?? Array.Empty<NodeTransfer>();
      var lineCount = EstimateLineCount(Data.Content);
      var height = BaseHeight + (lineCount * LineHeight) + (transfers.Length * 22f);
      return new Vector2(DefaultWidth, Mathf.Max(140f, height));
    }

    private static int EstimateLineCount(string content)
    {
      if (string.IsNullOrWhiteSpace(content)) return 1;
      var lines = content.Split('\n');
      var count = 0;
      foreach (var line in lines)
      {
        var length = string.IsNullOrEmpty(line) ? 1 : line.Length;
        count += Mathf.Max(1, Mathf.CeilToInt(length / (float)CharsPerLine));
      }
      return Mathf.Max(1, count);
    }

    private void ApplyContentPlaceholderIfNeeded(string content)
    {
      suppressContentChange = true;
      if (string.IsNullOrWhiteSpace(content))
      {
        showingContentPlaceholder = true;
        contentField.value = "Content";
        contentField.style.color = new StyleColor(new Color(0.7f, 0.7f, 0.7f));
      }
      else
      {
        showingContentPlaceholder = false;
        contentField.value = content;
        contentField.style.color = new StyleColor(Color.white);
      }
      suppressContentChange = false;
    }


    private static ResponseNode NormalizeData(ResponseNode node)
    {
      if (node.Transfers == null)
      {
        node.Transfers = Array.Empty<NodeTransfer>();
      }
      if (string.IsNullOrEmpty(node.Id))
      {
        node.Id = Guid.NewGuid().ToString();
      }
      return node;
    }

    private void CreateInputPort()
    {
      InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
      InputPort.portName = "In";
      inputContainer.Add(InputPort);
      RefreshPorts();
    }

    private void RefreshTransferPorts()
    {
      foreach (var port in transferPorts)
      {
        outputContainer.Remove(port);
      }
      transferPorts.Clear();

      var transfers = Data.Transfers ?? Array.Empty<NodeTransfer>();
      for (var i = 0; i < transfers.Length; i++)
      {
        var transfer = transfers[i];
        var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        port.userData = i;
        port.portName = GetTransferLabel(transfer, i);
        transferPorts.Add(port);
        outputContainer.Add(port);
      }

      RefreshPorts();
      RefreshExpandedState();
    }

    private static string GetTransferLabel(NodeTransfer transfer, int index)
    {
      var condition = transfer.Condition.Condition;
      var target = string.IsNullOrEmpty(transfer.ToNodeId) ? "(unassigned)" : transfer.ToNodeId;
      return $"{index + 1}: {condition} → {target}";
    }

    public bool TryGetTransferIndex(Port port, out int index)
    {
      if (port != null && port.userData is int i)
      {
        index = i;
        return true;
      }

      index = -1;
      return false;
    }

    public Port GetPortForTransfer(int index)
    {
      if (index < 0 || index >= transferPorts.Count) return null;
      return transferPorts[index];
    }

    public void HandlePortConnection(int transferIndex, ResponseNodeView target)
    {
      if (transferIndex < 0) return;
      var data = Data;
      var transfers = data.Transfers ?? Array.Empty<NodeTransfer>();
      if (transferIndex >= transfers.Length) return;

      transfers[transferIndex].ToNodeId = target?.Data.Id;
      data.Transfers = transfers;
      window.UpdateNodeData(data, true);
    }

    public void HandlePortDisconnection(int transferIndex)
    {
      if (transferIndex < 0) return;
      var data = Data;
      var transfers = data.Transfers ?? Array.Empty<NodeTransfer>();
      if (transferIndex >= transfers.Length) return;

      transfers[transferIndex].ToNodeId = null;
      data.Transfers = transfers;
      window.UpdateNodeData(data, true);
    }
  }
}
#endif
