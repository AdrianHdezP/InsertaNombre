using TMPro;
using UnityEngine;

public class Notifier : MonoBehaviour
{
    [SerializeField] Notice notice;
    [SerializeField] Transform holder;

    public static Notifier instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddNotice(string noticeT, Color color)
    {
        Notice notice = Instantiate(this.notice, holder);
        notice.text = noticeT;
        notice.timeToDespawn = 8;
        notice.color = color;
    }
}
