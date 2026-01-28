#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay.CallingInteraction;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorSupport.CallingInteractionGraphEditor
{
  public class ResponseGraphView : GraphView
  {
    private readonly ResponseGraphAuthoringWindow window;
    public Action<ResponseNodeView> onNodeSelected;

    public ResponseGraphView(ResponseGraphAuthoringWindow window)
    {
      this.window = window;
      style.flexGrow = 1f;
      focusable = true;

      SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

      this.AddManipulator(new ContentZoomer());
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());
      this.AddManipulator(new ContextualMenuManipulator(BuildContextMenu));

      var grid = new GridBackground();
      Insert(0, grid);
      grid.StretchToParentSize();

      var miniMap = new MiniMap { anchored = true };
      miniMap.SetPosition(new Rect(10, 10, 180, 140));
      Add(miniMap);

      graphViewChanged = OnGraphViewChanged;
    }

    private void BuildContextMenu(ContextualMenuPopulateEvent evt)
    {
      evt.menu.AppendAction("Add Node", _ =>
      {
        var screenPosition = GUIUtility.GUIToScreenPoint(evt.mousePosition);
        window.CreateNodeFromScreenPosition(screenPosition);
      });

      var selectedNode = selection.OfType<ResponseNodeView>().FirstOrDefault();
      if (selectedNode != null)
      {
        evt.menu.AppendAction("Remove Node", _ =>
        {
          DeleteElements(new List<GraphElement> { selectedNode });
        });
      }
      else
      {
        evt.menu.AppendAction("Remove Node", _ => { }, DropdownMenuAction.Status.Disabled);
      }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
      var compatible = new List<Port>();

      ports.ForEach(port =>
      {
        if (port == startPort) return;
        if (port.node == startPort.node) return;
        if (port.direction == startPort.direction) return;
        compatible.Add(port);
      });

      return compatible;
    }

    public void ClearGraph()
    {
      graphElements.ForEach(RemoveElement);
    }

    public ResponseNodeView AddNodeView(ResponseNode data, Vector2 position)
    {
      var nodeView = new ResponseNodeView(window, this, data);
      AddElement(nodeView);
      var rect = new Rect(position, nodeView.GetEstimatedSize());
      nodeView.SetPosition(rect);
      return nodeView;
    }

    public void RestoreEdges()
    {
      if (window.GraphData?.Nodes == null) return;

      foreach (var editorNode in window.GraphData.Nodes)
      {
        var sourceView = window.GetNodeView(editorNode.Node.Id);
        if (sourceView == null) continue;

        var transfers = editorNode.Node.Transfers ?? Array.Empty<NodeTransfer>();
        for (var i = 0; i < transfers.Length; i++)
        {
          var transfer = transfers[i];
          if (string.IsNullOrEmpty(transfer.ToNodeId)) continue;
          var targetView = window.GetNodeView(transfer.ToNodeId);
          if (targetView == null) continue;

          var output = sourceView.GetPortForTransfer(i);
          var input = targetView.InputPort;
          if (output == null || input == null) continue;

          CreateEdge(output, input);
        }
      }
    }

    public void RebuildEdges()
    {
      var edges = graphElements.OfType<Edge>().ToList();
      foreach (var edge in edges)
      {
        RemoveElement(edge);
      }

      RestoreEdges();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
      if (change.edgesToCreate != null)
      {
        foreach (var edge in change.edgesToCreate)
        {
          if (edge.output?.node is ResponseNodeView outputNode &&
              edge.input?.node is ResponseNodeView inputNode)
          {
            if (outputNode.TryGetTransferIndex(edge.output, out var index))
            {
              outputNode.HandlePortConnection(index, inputNode);
            }
          }
        }
      }

      if (change.elementsToRemove != null)
      {
        foreach (var element in change.elementsToRemove)
        {
          if (element is Edge edge)
          {
            if (edge.output?.node is ResponseNodeView outputNode)
            {
              if (outputNode.TryGetTransferIndex(edge.output, out var index))
              {
                outputNode.HandlePortDisconnection(index);
              }
            }
          }
          else if (element is ResponseNodeView nodeView)
          {
            window.RemoveNode(nodeView);
          }
        }
      }

      return change;
    }

    public void CreateEdge(Port output, Port input)
    {
      if (output == null || input == null) return;

      var edge = new Edge
      {
        output = output,
        input = input
      };
      edge.output.Connect(edge);
      edge.input.Connect(edge);
      AddElement(edge);
    }

    public Vector2 ScreenToGraphPosition(Vector2 screenPosition)
    {
      var windowPosition = window.position.position;
      var local = screenPosition - windowPosition;
      var world = contentViewContainer.WorldToLocal(local);
      return world;
    }
  }
}
#endif
