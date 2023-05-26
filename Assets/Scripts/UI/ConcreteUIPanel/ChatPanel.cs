using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPanel : BasePanel
{
    private static readonly string path = "UI/Panel/ChatPanel";
    
    public ChatPanel():base(new UIType(path)){}
}
