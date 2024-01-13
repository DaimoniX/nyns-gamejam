extends TextureRect

@export var full: Texture2D
@export var empty: Texture2D

func _ready():
	heal()

func heal():
	texture = full

func damage():
	texture = empty
