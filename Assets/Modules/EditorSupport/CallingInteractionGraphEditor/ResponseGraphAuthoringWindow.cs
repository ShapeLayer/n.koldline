#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamePlay.CallingInteraction;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorSupport.CallingInteractionGraphEditor
{
  public class ResponseGraphAuthoringWindow : EditorWindow
  {
    private ResponseGraphView graphView;
    private ResponseInspectorView inspectorView;
    private ResponseNodeSearchWindow searchWindow;

    private ResponseGraphData graphData = new ResponseGraphData();
    private readonly Dictionary<string, ResponseNodeView> nodeViews = new Dictionary<string, ResponseNodeView>();

    private Vector2 cachedMouseScreenPosition;
    private string currentFilePath;

    [MenuItem("Koldline/ARS Response Graph Editor")]
    public static void Open()
    {
      var window = GetWindow<ResponseGraphAuthoringWindow>();
      window.titleContent = new GUIContent("ARS Response Graph");
      window.minSize = new Vector2(900f, 500f);
      window.Show();
    }

    private void OnEnable()
    {
      ConstructUI();
    }

    private void OnDisable()
    {
      if (searchWindow != null)
      {
        DestroyImmediate(searchWindow);
      }
    }

    private void ConstructUI()
    {
      rootVisualElement.Clear();
      rootVisualElement.style.flexDirection = FlexDirection.Column;
      rootVisualElement.style.flexGrow = 1f;
      rootVisualElement.style.overflow = Overflow.Hidden;
      rootVisualElement.style.width = Length.Percent(100);

      var toolbar = BuildToolbar();
      rootVisualElement.Add(toolbar);

      var mainContainer = new VisualElement { name = "ResponseMainContainer" };
      mainContainer.style.flexGrow = 1f;
      mainContainer.style.flexDirection = FlexDirection.Row;

      var graphHost = new VisualElement { name = "ResponseGraphHost" };
      graphHost.style.flexGrow = 1f;
      graphHost.style.flexShrink = 1f;
      graphHost.style.backgroundColor = new StyleColor(new Color(0.12f, 0.12f, 0.12f, 1f));

      graphView = new ResponseGraphView(this) { name = "ResponseGraphView" };
      graphView.style.flexGrow = 1f;
      graphView.style.flexShrink = 1f;
      graphHost.Add(graphView);

      inspectorView = new ResponseInspectorView(this) { name = "ResponseInspectorView" };
      inspectorView.style.flexGrow = 1f;

      var inspectorHeader = new Label("Response Inspector")
      {
        style =
        {
          unityFontStyleAndWeight = FontStyle.Bold,
          marginBottom = 6,
          marginTop = 4,
          fontSize = 13,
          color = Color.white
        }
      };

      var inspectorScroll = new ScrollView { name = "ResponseInspectorScroll" };
      inspectorScroll.style.flexGrow = 1f;
      inspectorScroll.Add(inspectorView);

      var inspectorPanel = new VisualElement { name = "ResponseInspectorPanel" };
      inspectorPanel.style.width = 220;
      inspectorPanel.style.minWidth = 180;
      inspectorPanel.style.flexShrink = 0f;
      inspectorPanel.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f, 1f));
      inspectorPanel.style.paddingLeft = 8;
      inspectorPanel.style.paddingRight = 8;
      inspectorPanel.style.paddingTop = 8;
      inspectorPanel.style.paddingBottom = 8;
      inspectorPanel.style.flexDirection = FlexDirection.Column;

      inspectorPanel.Add(inspectorHeader);
      inspectorPanel.Add(inspectorScroll);

      var splitView = new TwoPaneSplitView(1, 220f, TwoPaneSplitViewOrientation.Horizontal)
      {
        style = { flexGrow = 1f }
      };

      splitView.Add(graphHost);
      splitView.Add(inspectorPanel);

      mainContainer.Add(splitView);

      rootVisualElement.Add(mainContainer);

      CreateSearchWindow();
    }

    private Toolbar BuildToolbar()
    {
      var toolbar = new Toolbar();

      var newButton = new ToolbarButton(LoadBlankGraph) { text = "New" };
      var openButton = new ToolbarButton(OpenGraphFromJson) { text = "Open" };
      var saveButton = new ToolbarButton(SaveGraphToJson) { text = "Save" };
      var saveAsButton = new ToolbarButton(SaveGraphToJsonAs) { text = "Save As" };
      var addNodeButton = new ToolbarButton(OpenCreateNodeMenu) { text = "Add Node" };

      toolbar.Add(newButton);
      toolbar.Add(openButton);
      toolbar.Add(saveButton);
      toolbar.Add(saveAsButton);
      toolbar.Add(addNodeButton);

      return toolbar;
    }

    private void CreateSearchWindow()
    {
      searchWindow = ScriptableObject.CreateInstance<ResponseNodeSearchWindow>();
      searchWindow.Initialize(this, graphView);
      graphView.nodeCreationRequest = ctx =>
      {
        cachedMouseScreenPosition = ctx.screenMousePosition;
        SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), searchWindow);
      };
    }

    private void OpenCreateNodeMenu()
    {
      var center = position.position + new Vector2(position.width * 0.5f, position.height * 0.5f);
      cachedMouseScreenPosition = center;
      SearchWindow.Open(new SearchWindowContext(center), searchWindow);
    }

    private void CreateNodeAtCursor()
    {
      var screenPosition = cachedMouseScreenPosition;
      if (screenPosition == Vector2.zero)
      {
        screenPosition = position.position + new Vector2(position.width * 0.5f, position.height * 0.5f);
      }
      CreateNodeFromScreenPosition(screenPosition);
    }

    public ResponseNodeView CreateNodeFromScreenPosition(Vector2 screenPosition)
    {
      var graphPosition = graphView.ScreenToGraphPosition(screenPosition);
      if (float.IsNaN(graphPosition.x) || float.IsNaN(graphPosition.y))
      {
        graphPosition = Vector2.zero;
      }
      return CreateNode(graphPosition);
    }

    public ResponseNodeView CreateNode(Vector2 graphPosition)
    {
      EnsureGraphData();

      var node = new ResponseNode
      {
        Id = Guid.NewGuid().ToString(),
        Content = "New Response",
        Transfers = Array.Empty<NodeTransfer>()
      };

      var editorNode = new EditorResponseNode
      {
        Node = node,
        PositionX = graphPosition.x,
        PositionY = graphPosition.y
      };

      graphData.Nodes.Add(editorNode);
      return AddNodeView(editorNode);
    }

    private ResponseNodeView AddNodeView(EditorResponseNode data)
    {
      var position = new Vector2(data.PositionX, data.PositionY);
      var nodeView = graphView.AddNodeView(data.Node, position);
      nodeViews[data.Node.Id] = nodeView;
      return nodeView;
    }

    public ResponseNodeView GetNodeView(string id)
    {
      return nodeViews.TryGetValue(id, out var view) ? view : null;
    }

    public void NotifyNodeSelected(ResponseNodeView nodeView)
    {
      inspectorView.SetTarget(nodeView);
    }

    public void UpdateNodeData(ResponseNode updated, bool rebuildEdges)
    {
      if (graphData?.Nodes == null) return;

      var index = graphData.Nodes.FindIndex(n => n.Node.Id == updated.Id);
      if (index < 0) return;

      var editorNode = graphData.Nodes[index];
      editorNode.Node = updated;
      graphData.Nodes[index] = editorNode;

      if (nodeViews.TryGetValue(updated.Id, out var view))
      {
        view.ApplyData(updated);
      }

      if (rebuildEdges)
      {
        graphView.RebuildEdges();
      }
    }

    public bool TryRenameNode(ResponseNodeView nodeView, string newId)
    {
      if (nodeView == null) return false;
      if (string.IsNullOrWhiteSpace(newId)) return false;

      var trimmed = newId.Trim();
      var oldId = nodeView.Data.Id;
      if (trimmed == oldId) return true;

      if (nodeViews.ContainsKey(trimmed))
      {
        return false;
      }

      if (graphData?.Nodes == null) return false;
      var index = graphData.Nodes.FindIndex(n => n.Node.Id == oldId);
      if (index < 0) return false;

      var editorNode = graphData.Nodes[index];
      var node = editorNode.Node;
      node.Id = trimmed;
      editorNode.Node = node;
      graphData.Nodes[index] = editorNode;

      for (var i = 0; i < graphData.Nodes.Count; i++)
      {
        var other = graphData.Nodes[i];
        var otherNode = other.Node;
        var modified = false;
        if (otherNode.Transfers != null)
        {
          for (var t = 0; t < otherNode.Transfers.Length; t++)
          {
            if (otherNode.Transfers[t].ToNodeId == oldId)
            {
              otherNode.Transfers[t].ToNodeId = trimmed;
              modified = true;
            }
          }
        }

        if (modified)
        {
          other.Node = otherNode;
          graphData.Nodes[i] = other;
        }
      }

      nodeViews.Remove(oldId);
      nodeViews[trimmed] = nodeView;
      nodeView.ApplyData(node);
      graphView.RebuildEdges();
      return true;
    }

    public ResponseNode GetNodeData(string id)
    {
      if (graphData?.Nodes == null) return default;
      var node = graphData.Nodes.FirstOrDefault(n => n.Node.Id == id).Node;
      if (string.IsNullOrEmpty(node.Id))
      {
        node.Id = id;
      }
      return node;
    }

    public void UpdateNodePosition(string id, Vector2 position)
    {
      if (graphData?.Nodes == null) return;
      var index = graphData.Nodes.FindIndex(n => n.Node.Id == id);
      if (index < 0) return;

      var editorNode = graphData.Nodes[index];
      editorNode.PositionX = position.x;
      editorNode.PositionY = position.y;
      graphData.Nodes[index] = editorNode;
    }

    public void RemoveNode(ResponseNodeView nodeView)
    {
      if (nodeView == null) return;
      var id = nodeView.Data.Id;

      if (graphData?.Nodes != null)
      {
        graphData.Nodes.RemoveAll(n => n.Node.Id == id);

        for (var i = 0; i < graphData.Nodes.Count; i++)
        {
          var editorNode = graphData.Nodes[i];
          var node = editorNode.Node;
          if (node.Transfers == null) continue;

          var modified = false;
          for (var t = 0; t < node.Transfers.Length; t++)
          {
            if (node.Transfers[t].ToNodeId == id)
            {
              node.Transfers[t].ToNodeId = null;
              modified = true;
            }
          }

          if (modified)
          {
            editorNode.Node = node;
            graphData.Nodes[i] = editorNode;
          }
        }
      }

      nodeViews.Remove(id);
      graphView.RebuildEdges();
      inspectorView.SetTarget(null);
    }

    public ResponseGraphData GraphData => graphData;

    private void LoadBlankGraph()
    {
      graphData = new ResponseGraphData();
      nodeViews.Clear();
      graphView.ClearGraph();
      inspectorView.SetTarget(null);
      currentFilePath = null;
    }

    private void OpenGraphFromJson()
    {
      var path = EditorUtility.OpenFilePanel("Open Response Graph JSON", Application.dataPath, "json");
      if (string.IsNullOrEmpty(path)) return;

      try
      {
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<ResponseGraphData>(json);
        if (data == null || data.Nodes == null)
        {
          EditorUtility.DisplayDialog("Open Graph", "Failed to parse graph data.", "OK");
          return;
        }

        graphData = data;
        currentFilePath = path;
        ReloadGraphFromData();
      }
      catch (Exception ex)
      {
        Debug.LogError(ex);
        EditorUtility.DisplayDialog("Open Graph", "Failed to open graph: " + ex.Message, "OK");
      }
    }

    private void ReloadGraphFromData()
    {
      nodeViews.Clear();
      graphView.ClearGraph();
      inspectorView.SetTarget(null);

      EnsureGraphData();

      foreach (var editorNode in graphData.Nodes)
      {
        if (string.IsNullOrEmpty(editorNode.Node.Id)) continue;
        AddNodeView(editorNode);
      }

      graphView.RebuildEdges();
    }

    private void SaveGraphToJson()
    {
      EnsureGraphData();
      if (graphData.Nodes.Count == 0)
      {
        EditorUtility.DisplayDialog("Save Graph", "No nodes to save.", "OK");
        return;
      }

      if (string.IsNullOrEmpty(currentFilePath))
      {
        SaveGraphToJsonAs();
        return;
      }

      SaveGraphToPath(currentFilePath);
    }

    private void SaveGraphToJsonAs()
    {
      EnsureGraphData();
      if (graphData.Nodes.Count == 0)
      {
        EditorUtility.DisplayDialog("Save Graph", "No nodes to save.", "OK");
        return;
      }

      var path = EditorUtility.SaveFilePanel("Save Response Graph", Application.dataPath, "response_graph", "json");
      if (string.IsNullOrEmpty(path)) return;

      SaveGraphToPath(path);
      currentFilePath = path;
    }

    private void SaveGraphToPath(string path)
    {
      try
      {
        var json = JsonUtility.ToJson(graphData, true);
        File.WriteAllText(path, json);
        EditorUtility.RevealInFinder(path);
      }
      catch (Exception ex)
      {
        Debug.LogError(ex);
        EditorUtility.DisplayDialog("Save Graph", "Failed to save graph: " + ex.Message, "OK");
      }
    }

    private void EnsureGraphData()
    {
      if (graphData == null)
      {
        graphData = new ResponseGraphData();
      }

      if (graphData.Nodes == null)
      {
        graphData.Nodes = new List<EditorResponseNode>();
      }
    }

    public ResponseNodeView ChangeNodeContent(ResponseNodeView nodeView, string content)
    {
      if (nodeView == null) return null;
      var data = nodeView.Data;
      data.Content = content;
      UpdateNodeData(data, false);
      return nodeView;
    }

    public void RequestNodeCreation(Vector2 screenMousePosition)
    {
      cachedMouseScreenPosition = screenMousePosition;
      CreateNodeAtCursor();
    }
  }
}
#endif
