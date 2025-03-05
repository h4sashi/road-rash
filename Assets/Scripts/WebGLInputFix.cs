using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class WebGLInputFix : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.onSelect.AddListener(delegate { ForceFocus(); });
        }
    }

    void ForceFocus()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        inputField.DeactivateInputField();
        inputField.ActivateInputField();
#endif
    }
}
