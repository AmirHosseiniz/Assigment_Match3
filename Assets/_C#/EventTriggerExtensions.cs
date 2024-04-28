using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class EventTriggerExtensions
{
    public static void AddListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
        UnityAction<BaseEventData> call)
    {
        if (eventTrigger == null)
            throw new ArgumentNullException(nameof(eventTrigger));
        if (call == null)
            throw new ArgumentNullException(nameof(call));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        if (entry == null)
        {
            entry = new EventTrigger.Entry {eventID = triggerType};
            eventTrigger.triggers.Add(entry);
        }

        entry.callback.AddListener(call);
    }

    public static void RemoveListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
        UnityAction<BaseEventData> call)
    {
        if (eventTrigger == null)
            throw new ArgumentNullException(nameof(eventTrigger));
        if (call == null)
            throw new ArgumentNullException(nameof(call));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveListener(call);
    }

    public static void RemoveAllListeners(this EventTrigger eventTrigger, EventTriggerType triggerType)
    {
        if (eventTrigger == null)
            throw new ArgumentNullException(nameof(eventTrigger));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveAllListeners();
    }
}