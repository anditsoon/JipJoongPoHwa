using TMPro;
using UnityEngine;

public class AllyLvUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lvUpText;
    private string initialLv = "1";

    private void Start()
    {
        lvUpText.text = initialLv;

       H_PlayerManager.instance.OnLevelChanged += SetUpAllyLvText;
    }

    public void SetUpAllyLvText(int lvInt)
    {
        lvUpText.text = lvInt.ToString();
    }
}
