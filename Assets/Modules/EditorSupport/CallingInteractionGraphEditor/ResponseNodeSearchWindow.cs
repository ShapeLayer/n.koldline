#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EditorSupport.CallingInteractionGraphEditor
{
  public class ResponseNodeSearchWindow : ScriptableObject, ISearchWindowProvider
  {
    private ResponseGraphAuthoringWindow window;
    private ResponseGraphView graphView;

    public void Initialize(ResponseGraphAuthoringWindow window, ResponseGraphView graphView)
    {
      this.window = window;
      this.graphView = graphView;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
      return new List<SearchTreeEntry>
      {
        new SearchTreeGroupEntry(new GUIContent("ARS Response"), 0),
        new SearchTreeEntry(new GUIContent("Response Node")) { level = 1, userData = "response" }
      };
    }

    public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
    {
      if (entry.userData is string key && key == "response")
      {
        window.CreateNodeFromScreenPosition(context.screenMousePosition);
        return true;
      }

      return false;
    }
  }
}
#endif
