using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Public Variables

    public static MenuManager Instance;

    public InputField hostNameInputField;
    public InputField hostRoomInputField;
    public InputField joinNameInputField;
    public InputField joinRoomInputField;

    #endregion

    #region Private Variables

    [SerializeField] private List<GameObject> panels;
    [SerializeField] private List<string> panelNames;

    [SerializeField] private Text connectionText;

    private float dotTime = 0f;
    private float dotSpeed = 0.3f;
    private int dots = 1;


    #endregion

    #region MonoBehavior Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetPanel(0);
    }

    private void Update()
    {
        ConnectionDots(Time.deltaTime);
    }

    #endregion

    #region Private Methods

    private void ConnectionDots(float dt)
    {
        int prevDots = dots;

        dotTime += dt;

        while (dotTime > dotSpeed)
        {
            dots = dots == 3 ? 1 : dots + 1;
            dotTime -= dotSpeed;
        }

        if (dots != prevDots)
        {
            string s = "Connecting";
            for (int i = 0; i < dots; i++)
            {
                s += ".";
            }
            connectionText.text = s;
        }
    }

    #endregion

    #region Public Methods

    public void ClearInputFields()
    {
        hostNameInputField.text = "";
        hostRoomInputField.text = "";
        joinNameInputField.text = "";
        joinRoomInputField.text = "";
    }

    public void SetPanel(int index)
    {
        ClearInputFields();

        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        if(index >= 0 && index < panels.Count)
        {
            panels[index].SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid index for SetPanel().");
        }
    }

    public void SetPanel(string name)
    {
        ClearInputFields();

        if (panels.Count != panelNames.Count)
        {
            Debug.LogError("The number of panels does not equal the number of panel names.");
            return;
        }

        bool foundPanel = false;
        for (int i = 0; i < panels.Count; i++)
        {
            bool match = panelNames[i].Equals(name);
            panels[i].SetActive(match);
            foundPanel = match || foundPanel;
        }

        if (!foundPanel)
        {
            Debug.LogError("Invalid string for SetPanel().");
        }
    }

    #endregion
}
