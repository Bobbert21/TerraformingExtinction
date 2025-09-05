using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RelationshipTreeDebugUI : MonoBehaviour
{
    public static RelationshipTreeDebugUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject debugPanel; // Root panel for tree
    [SerializeField] private GameObject rowPrefab; // Prefab with Horizontal Layout: Text + Button
    [SerializeField] private Transform treeContentParent; // Vertical layout container
    [SerializeField] private GameObject subIdentifierDetailPanel; // Panel to show SubIdentifier details
    [SerializeField] private Text detailPanelTitle;
    [SerializeField] private Text detailPanelContent;

    [Header("Trees to Debug")]
    public RelationshipPersonalTree agentTree;
    public RelationshipPersonalTree environmentTree;

    private RelationshipPersonalTree currentTree;
    private bool isVisible;
    private SubIdentifierNode currentlyShownSub = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (debugPanel != null)
            debugPanel.SetActive(false);
        if (subIdentifierDetailPanel != null)
            subIdentifierDetailPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) ToggleTree(agentTree);
        if (Input.GetKeyDown(KeyCode.F3)) ToggleTree(environmentTree);
        if (Input.GetKeyDown(KeyCode.F4)) ShowBothTrees();
    }

    private void ToggleTree(RelationshipPersonalTree tree)
    {
        if (currentTree == tree) isVisible = !isVisible;
        else { currentTree = tree; isVisible = true; }

        if (debugPanel != null)
            debugPanel.SetActive(isVisible);

        if (isVisible && currentTree != null)
            RefreshDisplay();
    }

    private void ShowBothTrees()
    {
        isVisible = true;
        debugPanel.SetActive(true);
        ClearTreeContent();

        if (agentTree != null)
        {
            AddHeader("Agent Tree");
            foreach (var root in agentTree.RootIdentifiers)
                AppendIdentifierNode(root, 0);
        }

        if (environmentTree != null)
        {
            AddHeader("Environment Tree");
            foreach (var root in environmentTree.RootIdentifiers)
                AppendIdentifierNode(root, 0);
        }
    }

    private void RefreshDisplay()
    {
        ClearTreeContent();
        if (currentTree == null) return;

        AddHeader($"Tree for: {currentTree.SelfIdentifier}");
        foreach (var root in currentTree.RootIdentifiers)
            AppendIdentifierNode(root, 0);
    }

    private void ClearTreeContent()
    {
        foreach (Transform child in treeContentParent)
            Destroy(child.gameObject);
    }

    private void AddHeader(string title)
    {
        // Use the rowPrefab for consistency
        GameObject headerGO = Instantiate(rowPrefab, treeContentParent);
        headerGO.transform.localScale = Vector3.one;

        Text txt = headerGO.transform.Find("Label")?.GetComponent<Text>();
        if (txt != null)
        {
            txt.text = $"<b>{title}</b>";
            txt.color = Color.black;
        }

        // Hide the button for header
        Button btn = headerGO.transform.Find("MoreInfoButton")?.GetComponent<Button>();
        if (btn != null)
            btn.gameObject.SetActive(false);

        Button relBtn = headerGO.transform.Find("RelationshipNodesMoreInfoButton")?.GetComponent<Button>();
        if (relBtn != null)
            relBtn.gameObject.SetActive(false);

        // Set a smaller preferredHeight to reduce extra space
        LayoutElement le = headerGO.GetComponent<LayoutElement>();
        if (le == null)
            le = headerGO.AddComponent<LayoutElement>();
        le.preferredHeight = 20; // smaller than default rows
    }

    private void AppendIdentifierNode(IdentifierNode node, int depth)
    {
        AddTextRow($"> {node.Identifier}", depth, Color.black);

        if (node.Children != null)
        {
            foreach (var child in node.Children)
                AppendIdentifierNode(child, depth + 1);
        }

        if (node.SubIdentifiers != null)
        {
            foreach (var sub in node.SubIdentifiers)
                AppendSubIdentifierNode(sub, depth + 1);
        }
    }

    private void AddTextRow(string text, int depth, Color color)
    {
        GameObject rowGO = Instantiate(rowPrefab, treeContentParent);
        rowGO.transform.localScale = Vector3.one;

        Text txt = rowGO.transform.Find("Label")?.GetComponent<Text>();
        if (txt != null)
        {
            // Increase indentation multiplier (e.g., 4 spaces per depth)
            txt.text = new string(' ', depth * 4) + text;
            txt.color = color;
        }

        Button btn = rowGO.transform.Find("MoreInfoButton")?.GetComponent<Button>();
        if (btn != null)
            btn.gameObject.SetActive(false);

        Button relBtn = rowGO.transform.Find("RelationshipNodesMoreInfoButton")?.GetComponent<Button>();
        if (relBtn != null)
            relBtn.gameObject.SetActive(false);

        LayoutElement le = rowGO.GetComponent<LayoutElement>();
        if (le == null)
            le = rowGO.AddComponent<LayoutElement>();
        le.preferredHeight = 24;
    }

    private void AppendSubIdentifierNode(SubIdentifierNode sub, int depth)
    {
        GameObject row = Instantiate(rowPrefab, treeContentParent);
        row.transform.localScale = Vector3.one;

        Text txt = row.transform.Find("Label")?.GetComponent<Text>();
        if (txt != null)
        {
            txt.text = new string(' ', depth * 4) + "- " + sub.SubIdentifierName;
            txt.color = Color.blue;
        }

        // Sub details button
        Button btn = row.transform.Find("MoreInfoButton")?.GetComponent<Button>();
        if (btn != null)
        {
            btn.gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowSubIdentifierDetail(sub));
        }

        // Relationship nodes button
        Button relBtn = row.transform.Find("RelationshipNodesMoreInfoButton")?.GetComponent<Button>();
        if (relBtn != null)
        {
            bool hasRelNodes = sub.RelationshipNodes != null && sub.RelationshipNodes.Count > 0;
            relBtn.gameObject.SetActive(hasRelNodes);

            if (hasRelNodes)
            {
                relBtn.onClick.RemoveAllListeners();
                relBtn.onClick.AddListener(() => ShowRelationshipNodesDetail(sub));
            }
        }


        LayoutElement le = row.GetComponent<LayoutElement>();
        if (le == null)
            le = row.AddComponent<LayoutElement>();
        le.preferredHeight = 24;

        if (sub.Specifics != null)
        {
            foreach (var specific in sub.Specifics)
                AppendSubIdentifierNode(specific, depth + 1);
        }
    }

    private void ShowRelationshipNodesDetail(SubIdentifierNode sub)
    {
        if (subIdentifierDetailPanel == null) return;

        // Toggle off if already showing this sub’s relationships
        if (currentlyShownSub == sub && subIdentifierDetailPanel.activeSelf)
        {
            subIdentifierDetailPanel.SetActive(false);
            currentlyShownSub = null;
            return;
        }

        subIdentifierDetailPanel.SetActive(true);
        currentlyShownSub = sub;

        detailPanelTitle.text = sub.SubIdentifierName + " - Relationships";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var rel in sub.RelationshipNodes)
        {
            sb.AppendLine($"<b>{rel.Name}</b>");
            sb.AppendLine($"  Livelihood: {rel.ModRValues.LivelihoodValue}");
            sb.AppendLine($"  DefensiveBelonging: {rel.ModRValues.DefensiveBelongingValue}");
            sb.AppendLine($"  NurtureBelonging: {rel.ModRValues.NurtureBelongingValue}");


            if (rel.ResponseNodes != null && rel.ResponseNodes.Count > 0)
            {
                sb.AppendLine("<color=blue>  Responses:</color>");
                foreach (var response in rel.ResponseNodes)
                {
                    sb.AppendLine($"<color=blue>    - {response.Decision.Name}</color>");
                    sb.AppendLine($"<color=blue>       Livelihood: {response.ModRValues.LivelihoodValue}</color>");
                    sb.AppendLine($"<color=blue>       DefensiveBelonging: {response.ModRValues.DefensiveBelongingValue}</color>");
                    sb.AppendLine($"<color=blue>       NurtureBelonging: {response.ModRValues.NurtureBelongingValue}</color>");
                }
            }


            sb.AppendLine(); // spacing between relationship nodes
        }

        detailPanelContent.text = sb.ToString();
    }


    private void ShowSubIdentifierDetail(SubIdentifierNode sub)
    {
        if (subIdentifierDetailPanel == null) return;

        // If same sub is already shown, toggle off
        if (currentlyShownSub == sub && subIdentifierDetailPanel.activeSelf)
        {
            subIdentifierDetailPanel.SetActive(false);
            currentlyShownSub = null;
            return;
        }

        // Show new sub
        subIdentifierDetailPanel.SetActive(true);
        currentlyShownSub = sub;

        detailPanelTitle.text = sub.SubIdentifierName;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>Action Characteristics:</b>");
        foreach (var ac in sub.ActionCharacteristicsWithValue)
            sb.AppendLine($"{ac.CharacteristicType}: {ac.Value}");

        sb.AppendLine("\n<b>Appearance Characteristics:</b>");
        foreach (var ap in sub.AppearanceCharacteristicsWithValue)
            sb.AppendLine($"{ap.CharacteristicType}: {ap.Value}");

        detailPanelContent.text = sb.ToString();
    }
}
