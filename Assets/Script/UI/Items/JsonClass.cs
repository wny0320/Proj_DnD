using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonClass
{
    public List<JsonSlotLine> invenSlotLines = new List<JsonSlotLine>();
    public List<JsonSlotLine> stashSlotLines = new List<JsonSlotLine>();
    public Dictionary<string, JsonSlot> equipSlotDict = new Dictionary<string, JsonSlot>();
    public int gold = new int();
}
