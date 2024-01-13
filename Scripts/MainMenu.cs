using Godot;
using System;

namespace GJNYS.Scripts;

public partial class MainMenu : Control
{
	private void StartButton()
	{
		GetTree().ChangeSceneToFile("res://main.scn");
	}
	
	private void GuideButton()
     {
     	//add guide window 
     }
     
     
     private void QuitButton()
     {
     	GetTree().Quit();
     }
}





