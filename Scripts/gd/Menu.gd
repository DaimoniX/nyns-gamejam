extends Control

func play():
    get_tree().change_scene_to_file("res://Scenes/game.scn");

func guide():
    pass

func exit():
    get_tree().quit()
