extends Control

func restart():
    get_tree().reload_current_scene()

func menu():
    get_tree().change_scene_to_file("res://main.tscn");