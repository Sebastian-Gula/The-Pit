using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Slot {

    public enum State { Empty, Taken };

    public State state = State.Empty;
    public Image Backgorund;
    public Image Image;
    public Text mainText;
    public Text subText;

    public Slot(Image Image, Text mainText, Text subText)
    {
        this.Image = Image;
        this.mainText = mainText;
        this.subText = subText;
    }

    public Slot(Image Image, Image Backgorund)
    {
        this.Image = Image;
        this.Backgorund = Backgorund;
    }

    public Slot(Image Image)
    {
        this.Image = Image;
    }


    public void ClearSlot()
    {
        state = State.Empty;
        if (Image != null) Image.sprite = null;
        if (mainText != null) mainText.text = "";
        if (subText != null) subText.text = "";
        if (Backgorund != null) Backgorund.color = Color.white;
    }
}
