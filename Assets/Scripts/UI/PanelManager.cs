using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板管理器，用栈来存储UI
/// </summary>
public class PanelManager
{
   //存储UI面板的栈
   private Stack<BasePanel> stackPanel;
   //UI管理器
   private UIManager uIManager;
   //方便接UI面板
   private BasePanel panel;

   public PanelManager()
   {
      stackPanel = new Stack<BasePanel>();
      uIManager = new UIManager();
   }

   /// <summary>
   /// UI的入栈操作，此操作会显示一个面板
   /// </summary>
   /// <param name="nextPanel">要显示的面板</param>
   public void Push(BasePanel nextPanel)
   {
      if (stackPanel.Count > 0)
      {
         panel = stackPanel.Peek();//获取栈顶的UI面板
         panel.OnPause();//暂停面板
      }

      stackPanel.Push(nextPanel);//新UI面板入栈
      GameObject panelGo = uIManager.GetSingleUI(nextPanel.UIType);//新UI面板显示
      nextPanel.Initialize(new UITool(panelGo));//初始化UITool
      nextPanel.Initialize(this);//初始化面板管理器
      nextPanel.Initialize(uIManager);//初始化UI管理器
      nextPanel.OnEnter();
   }

   /// <summary>
   /// 执行面板的出栈操作,此操作会执行面板的OnExit方法
   /// </summary>
   public void Pop()
   {
      if (stackPanel.Count > 0)
         stackPanel.Pop().OnExit();//退出当前UI面板并将当前UI面板清除出栈
      if (stackPanel.Count > 0)
         stackPanel.Peek().OnResume();//继续栈顶端的UI面板
   }

   /// <summary>
   /// 执行所有面板的OnExit()
   /// </summary>
   public void PopAll()
   {
      while (stackPanel.Count > 0)
      {
         stackPanel.Pop().OnExit();
      }
   }

   public BasePanel CurrentPanel()
   {
      if (stackPanel.Count == 0)
         return null;
      else
         return stackPanel.Peek();
   }

   public MainPanel MainPanel()
   {
      if (CurrentPanel().GetType() == typeof(MainPanel))
         return CurrentPanel() as MainPanel;
      return null;
   }
}
