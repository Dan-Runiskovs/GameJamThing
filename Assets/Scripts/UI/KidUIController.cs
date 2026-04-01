using UnityEngine;

public class KidUIController : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] private GameObject _lemonade;
    [SerializeField] private GameObject _music;
    [SerializeField] private GameObject _balloon;
    [SerializeField] private GameObject _cake;
    [SerializeField] private GameObject _pinata;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Complain(ItemScript.ItemTypes wantedItem)
    {
        switch(wantedItem)
        {
            case ItemScript.ItemTypes.Lemonade:
                _lemonade.SetActive(true);
                break;

            case ItemScript.ItemTypes.Music:
                _music.SetActive(true);
                break;

            case ItemScript.ItemTypes.Balloon:
                _balloon.SetActive(true);
                break;

            case ItemScript.ItemTypes.Cake:
                _cake.SetActive(true);
                break;

            case ItemScript.ItemTypes.Pinata:
                _pinata.SetActive(true);
                break;
        }
    }

    public void StopComplaining()
    {
        _lemonade.SetActive(false);
        _music.SetActive(false);
        _balloon.SetActive(false);
        _cake.SetActive(false);
        _pinata.SetActive(false);
    }
}
