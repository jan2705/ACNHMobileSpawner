using NHSE.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_SetControlFiller : MonoBehaviour
{
    private int DELETEALLTAPSNEEDED = 4;
    private float DELETEALLSECONDSALIVE = 1f;

	public UI_ACItemGrid ItemGrid;
	public UI_SearchWindow SearchWindow;

	public Button SetCurrent;
	public Button SetFillRow;
	public Button SetFillAll;
	public Button SetFillVariations;
    public Button DeleteItem;

    public Text DeleteAllTapsText;

	private int lastItemIndex = -1;
	private Item lastItem;

    private float deleteAllIntervalTimer = -1;
    private int deleteAllTapCount = 0;

	private void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		SetCurrent.onClick.AddListener(delegate
		{
			FillSelected(lastItemIndex);
        });
		SetFillRow.onClick.AddListener(delegate
		{
			FillRow(lastItemIndex / 10);
		});
        SetFillAll.onClick.AddListener(delegate
        {
            FillAll();
        });
        DeleteItem.onClick.AddListener(delegate
        {
            DeleteItemAt(lastItemIndex);
        });
    }

	private void Update()
	{
        if (deleteAllIntervalTimer > 0)
        {
            deleteAllIntervalTimer -= Time.deltaTime;
            DeleteAllTapsText.gameObject.SetActive(true);
            DeleteAllTapsText.text = string.Format("Delete all: {0} taps", DELETEALLTAPSNEEDED - deleteAllTapCount);
            if (deleteAllTapCount >= DELETEALLTAPSNEEDED)
            {
                DeleteAll();
                deleteAllIntervalTimer = -1;
                deleteAllTapCount = 0;
            }
        }
        else
        {
            DeleteAllTapsText.gameObject.SetActive(false);
            deleteAllIntervalTimer = -1;
            deleteAllTapCount = 0;
        }
	}

	public void UpdateSelected(int itemIndex, Item item)
	{
		lastItemIndex = itemIndex;
		lastItem = item;
        SetCurrent.GetComponentInChildren<Text>().text = "Set to current \n(" + itemIndex + ")";
        SetFillRow.GetComponentInChildren<Text>().text = "Fill row \n(" + itemIndex / 10 + ")";
        DeleteItem.GetComponentInChildren<Text>().text = "Delete item \n(" + itemIndex + ")";
    }

	public void FillSelected(int index)
	{
        if (SearchWindow.IsNoItemMode)
            return;

        Item asItem = SearchWindow.GetAsItem(lastItem);
		ItemGrid.SetItemAt(asItem, index, setFocus: true);
	}

	public void FillRow(int row)
	{
        if (SearchWindow.IsNoItemMode)
            return;

        int start = row * 10;
        for (int i = start; i < start + 10; ++i)
        {
            lastItem = ItemGrid.GetItemAt(i);
            FillSelected(i);
        }
	}

    public void FillAll()
    {
        if (SearchWindow.IsNoItemMode)
            return;

        for (int i = 0; i < 40; ++i)
        {
            lastItem = ItemGrid.GetItemAt(i);
            FillSelected(i);
        }
    }

    public void DeleteItemAt(int index)
    {
        deleteAllIntervalTimer = DELETEALLSECONDSALIVE;
        deleteAllTapCount++;
        lastItem = ItemGrid.GetItemAt(index);
        lastItem.Delete();
        ItemGrid.SetItemAt(lastItem, index, true);
    }

    public void DeleteAll()
    {
        for (int i = 0; i < 40; ++i)
        {
            lastItem = ItemGrid.GetItemAt(i);
            lastItem.Delete();
            ItemGrid.SetItemAt(lastItem, i, true);
        }
    }

    void doAndroidDebug()
    {
#if UNITY_ANDROID
        AndroidUSBUtils.CurrentInstance.ConnectUSB();
        AndroidUSBUtils.CurrentInstance.WriteToEndpoint(NHSE.Injection.SwitchCommand.Click(NHSE.Injection.SwitchButton.A));
#endif
    }
}
