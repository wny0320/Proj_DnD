using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotJsonClass
{
    public List<JsonSlotLine> invenSlotLines = new List<JsonSlotLine>();
    public List<JsonSlotLine> stashSlotLines = new List<JsonSlotLine>();
    public Dictionary<string, JsonSlot> equipSlotDict = new Dictionary<string, JsonSlot>();
}
