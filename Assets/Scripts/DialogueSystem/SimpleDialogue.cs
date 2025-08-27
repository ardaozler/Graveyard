using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class SimpleDialogue : MonoBehaviour
{
    [Header("XML Source")] public TextAsset xmlAsset;

    [Header("Callbacks (connect to UI)")] public UnityEvent<string, string> OnLine; // (speaker, text)
    public UnityEvent<string, string[]> OnChoice; // (prompt, options)
    public UnityEvent OnEnd; // ()

    [Header("Debug")] public bool autoAdvanceLines = true; // if false, call AdvanceLine() manually
    public bool logBlackboardOnEnd = true;

    public enum VarType
    {
        Bool,
        Int,
        String
    }

    [Serializable]
    public class Var
    {
        public string Name;
        public VarType Type;
        public object Value;
    }

    // Global dictionary of variables. All conversations share this.
    private readonly Dictionary<string, Var> _dataDictionary = new Dictionary<string, Var>(StringComparer.Ordinal);

    // Declares a var if not already present
    private void Declare(string name, VarType type, string defaultStr)
    {
        if (_dataDictionary.ContainsKey(name)) return;
        object val = type == VarType.Bool ? (defaultStr?.Trim().ToLower() == "true")
            : type == VarType.Int ? (int.TryParse(defaultStr, out var i) ? i : 0)
            : (object)(defaultStr ?? "");
        _dataDictionary[name] = new Var { Name = name, Type = type, Value = val };
    }

    // Sets a var to a value
    private void SetVar(string name, string value)
    {
        if (!_dataDictionary.TryGetValue(name, out var v))
            throw new Exception($"[Dialogue] Variable '{name}' not declared.");
        switch (v.Type)
        {
            case VarType.Bool:
                v.Value = value.Trim().ToLower() == "true";
                break;
            case VarType.Int:
                v.Value = int.Parse(value);
                break;
            case VarType.String:
                v.Value = value ?? string.Empty;
                break;
        }
    }

    // Adds, subtracts from the int var
    private void AddVar(string name, int delta)
    {
        if (!_dataDictionary.TryGetValue(name, out var v))
            throw new Exception($"[Dialogue] Variable '{name}' not declared.");
        if (v.Type != VarType.Int)
            throw new Exception($"[Dialogue] 'add' only works on int variables. '{name}' is {v.Type}.");
        v.Value = (int)v.Value + delta;
    }

    // Supported forms ONLY:
    //   var == 'Text'    (string)
    //   var == true      (bool)
    //   var == false     (bool)
    //   var >= 5         (int)
    //   var <= 10        (int)
    //   Empty require => true.
    private bool EvalCondition(string require)
    {
        if (string.IsNullOrWhiteSpace(require)) return true; // no gate

        string s = require.Trim();

        // Equality: var == 'Text' | true | false | number
        if (s.Contains("=="))
        {
            var parts = s.Split(new[] { "==" }, StringSplitOptions.None);
            if (parts.Length != 2) throw new Exception($"[Dialogue] Bad condition: {require}");
            string left = parts[0].Trim();
            string right = parts[1].Trim();

            if (!_dataDictionary.TryGetValue(left, out var v))
                throw new Exception($"[Dialogue] Unknown variable '{left}' in condition: {require}");

            switch (v.Type)
            {
                case VarType.Bool:
                    bool rb = right.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                              right.Equals("false", StringComparison.OrdinalIgnoreCase)
                        ? right.ToLower() == "true"
                        : throw new Exception($"[Dialogue] Bool compare expects true/false: {require}");
                    return (bool)v.Value == rb;
                case VarType.Int:
                    if (!int.TryParse(right, out var ri))
                        throw new Exception($"[Dialogue] Int compare expects number: {require}");
                    return (int)v.Value == ri;
                case VarType.String:
                    // Expect quotes 'Text'. If missing, we still compare raw text.
                    if (right.Length >= 2 && right.StartsWith("'") && right.EndsWith("'"))
                        right = right.Substring(1, right.Length - 2);
                    return string.Equals((string)v.Value, right, StringComparison.Ordinal);
            }
        }

        // Greater/Equal: var >= N
        if (s.Contains(">="))
        {
            var parts = s.Split(new[] { ">=" }, StringSplitOptions.None);
            if (parts.Length != 2) throw new Exception($"[Dialogue] Bad condition: {require}");
            string left = parts[0].Trim();
            string right = parts[1].Trim();
            if (!_dataDictionary.TryGetValue(left, out var v))
                throw new Exception($"[Dialogue] Unknown variable '{left}' in condition: {require}");
            if (v.Type != VarType.Int) throw new Exception($"[Dialogue] '>=' requires int variable: {left}");
            return (int)v.Value >= int.Parse(right);
        }

        // Less/Equal: var <= N
        if (s.Contains("<="))
        {
            var parts = s.Split(new[] { "<=" }, StringSplitOptions.None);
            if (parts.Length != 2) throw new Exception($"[Dialogue] Bad condition: {require}");
            string left = parts[0].Trim();
            string right = parts[1].Trim();
            if (!_dataDictionary.TryGetValue(left, out var v))
                throw new Exception($"[Dialogue] Unknown variable '{left}' in condition: {require}");
            if (v.Type != VarType.Int) throw new Exception($"[Dialogue] '<=' requires int variable: {left}");
            return (int)v.Value <= int.Parse(right);
        }

        throw new Exception($"[Dialogue] Unsupported condition (MVP supports ==, >=, <= only): {require}");
    }

    // -----------------------------
    // XML DATA STRUCTURES
    // -----------------------------
    [XmlRoot("Dialogue")]
    public class Dialogue
    {
        [XmlAttribute("id")] public string id;
        [XmlAttribute("entry")] public string entry;

        [XmlElement("Variables")] public Variables vars;

        [XmlArray("Nodes")]
        [XmlArrayItem("Line", typeof(Line))]
        [XmlArrayItem("Choice", typeof(Choice))]
        [XmlArrayItem("End", typeof(End))]
        public List<Node> nodes = new List<Node>();
    }

    public class Variables
    {
        [XmlElement("Var")] public List<VarDecl> list = new List<VarDecl>();
    }

    public class VarDecl
    {
        [XmlAttribute("name")] public string name;
        [XmlAttribute("type")] public string type;
        [XmlAttribute("default")] public string def;
    }

    public abstract class Node
    {
        [XmlAttribute("id")] public string id;
    }

    // LINE: shows a single line, then goes to next
    public class Line : Node
    {
        [XmlAttribute("speaker")] public string speaker;
        [XmlAttribute("text")] public string text;
        [XmlAttribute("next")] public string next;
    }

    // CHOICE: shows prompt + N options. Each option may have a gate and some effects.
    public class Choice : Node
    {
        [XmlAttribute("text")] public string prompt;
        [XmlElement("Option")] public List<Option> options = new List<Option>();
    }

    public class Option
    {
        [XmlAttribute("text")] public string text;

        [XmlAttribute("require")]
        public string require; // optional, e.g., rep >= 5, hasDiscount == true, quest == 'Accepted'

        [XmlAttribute("next")] public string next; // where to go after pick
        [XmlElement("Effects")] public Effects effects; // optional
    }

    public class Effects
    {
        [XmlElement("Effect")] public List<Effect> list = new List<Effect>();
    }

    public class Effect
    {
        [XmlAttribute("set")] public string set; // var name
        [XmlAttribute("add")] public string add; // var name
        [XmlAttribute("value")] public string value; // bool/int/string for set; int for add
    }

    public class End : Node
    {
    }

    // -----------------------------
    // RUNTIME STATE
    // -----------------------------
    private Dialogue _data; // parsed XML
    private readonly Dictionary<string, Node> _nodeMap = new Dictionary<string, Node>(); // id -> node
    private string _currentId; // which node we are on
    private List<Option> _visibleOptions = null; // cached choices for PickOption


    private void Start()
    {
        StartConversation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AdvanceLine();
        }


        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PickOption(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PickOption(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PickOption(2);
        }
    }

    // -----------------------------
    // PUBLIC API
    // -----------------------------
    public void StartConversation()
    {
        if (xmlAsset == null)
        {
            Debug.LogError("Assign an XML TextAsset.");
            return;
        }

        // 1) Parse XML
        var serializer = new XmlSerializer(typeof(Dialogue));
        using (var reader = new StringReader(xmlAsset.text))
            _data = (Dialogue)serializer.Deserialize(reader);

        // 2) Initialize variables from <Variables>
        _dataDictionary.Clear();
        if (_data.vars != null)
        {
            foreach (var vd in _data.vars.list)
            {
                var t = vd.type?.Trim().ToLower();
                var type = t == "bool" ? VarType.Bool : t == "int" ? VarType.Int : VarType.String;
                Declare(vd.name, type, vd.def);
            }
        }

        // 3) Build node lookup and sanity check minimal links
        _nodeMap.Clear();
        foreach (var n in _data.nodes)
        {
            if (string.IsNullOrEmpty(n.id)) throw new Exception("Node without id.");
            if (_nodeMap.ContainsKey(n.id)) throw new Exception($"Duplicate node id '{n.id}'.");
            _nodeMap[n.id] = n;
        }

        if (string.IsNullOrEmpty(_data.entry)) throw new Exception("Dialogue missing entry id.");
        if (!_nodeMap.ContainsKey(_data.entry)) throw new Exception($"Entry node '{_data.entry}' not found.");

        // 4) Begin
        _currentId = _data.entry;
        _visibleOptions = null;
        Step();
    }

    // If autoAdvanceLines = false, call this after you printed a line to progress.
    public void AdvanceLine()
    {
        Step();
    }

    // When UI button i is clicked, call this.
    public void PickOption(int index)
    {
        if (_visibleOptions == null)
        {
            Debug.LogError("Not waiting for a choice.");
            return;
        }

        if (index < 0 || index >= _visibleOptions.Count)
        {
            Debug.LogError("Bad option index.");
            return;
        }

        var opt = _visibleOptions[index];

        // Apply effects in order (if any)
        if (opt.effects != null)
        {
            foreach (var ef in opt.effects.list)
            {
                bool hasSet = !string.IsNullOrEmpty(ef.set);
                bool hasAdd = !string.IsNullOrEmpty(ef.add);
                if (hasSet == hasAdd) throw new Exception("Each <Effect> must have exactly one of 'set' or 'add'.");

                if (hasSet) SetVar(ef.set, ef.value);
                else AddVar(ef.add, int.Parse(ef.value));
            }
        }

        // Jump to next
        _currentId = opt.next;
        _visibleOptions = null;
        Step();
    }

    // -----------------------------
    // CORE LOOP (VERY SMALL)
    // -----------------------------
    private void Step()
    {
        while (true)
        {
            var node = _nodeMap[_currentId];

            if (node is Line ln)
            {
                // 1) Show the line
                OnLine?.Invoke(ln.speaker, ln.text);

                Debug.Log(ln.speaker + ":" + ln.text);

                // 2) Move to next node or wait if autoAdvance is off
                _currentId = ln.next;
                if (!autoAdvanceLines) return; // caller will call AdvanceLine()
                continue; // auto-advance to whatever is next
            }

            if (node is Choice ch)
            {
                // 1) Build visible options (gate with simple require rules)
                _visibleOptions = new List<Option>();
                foreach (var opt in ch.options)
                {
                    if (EvalCondition(opt.require)) _visibleOptions.Add(opt);
                }

                // 2) Send to UI and WAIT for PickOption
                var texts = new string[_visibleOptions.Count];
                for (int i = 0; i < texts.Length; i++) texts[i] = _visibleOptions[i].text;
                OnChoice?.Invoke(ch.prompt, texts);
                return; // stop here until user picks
            }

            if (node is End)
            {
                if (logBlackboardOnEnd)
                {
                    foreach (var kv in _dataDictionary)
                        Debug.Log($"[BB] {kv.Key} = {kv.Value.Value} ({kv.Value.Type})");
                }

                OnEnd?.Invoke();
                return; // conversation finished
            }

            throw new Exception($"Unknown node type at '{_currentId}'.");
        }
    }
}