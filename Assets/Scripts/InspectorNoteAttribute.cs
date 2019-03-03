using UnityEngine;

/// <summary>
/// Credit to https://twitter.com/mwegner/status/355147544818495488
/// </summary>
public class InspectorNoteAttribute : PropertyAttribute
{
    public readonly string header;
    public readonly string message;

    public InspectorNoteAttribute(string header, string message = "")
    {
        this.header = header;
        this.message = message;
    }
}