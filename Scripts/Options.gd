extends Control

var can_change_key = false
var action_string
enum ACTIONS {Forward, Left, Right, Backward, Jump, Run, Interact}

func _ready():
	_set_keys()  
  
func _set_keys():
	for j in ACTIONS:
		get_node("TabContainer/Input/ScrollContainer/VBoxContainer/HBoxContainer_" + str(j) + "/Button").set_pressed(false)
		if !InputMap.get_action_list(j).empty():
			get_node("TabContainer/Input/ScrollContainer/VBoxContainer/HBoxContainer_" + str(j) + "/Button").set_text(InputMap.get_action_list(j)[0].as_text())
		else:
			get_node("TabContainer/Input/ScrollContainer/VBoxContainer/HBoxContainer_" + str(j) + "/Button").set_text("Not set")
			
func _mark_button(string):
	can_change_key = true
	action_string = string
	
	for j in ACTIONS:
		if j != string:
			get_node("TabContainer/Input/ScrollContainer/VBoxContainer/HBoxContainer_" + str(j) + "/Button").set_pressed(false)
			
func _input(event):
	if event is InputEventKey: 
		if can_change_key:
			_change_key(event)
			can_change_key = false
			
func _change_key(new_key):
	#Delete key of pressed button
	if !InputMap.get_action_list(action_string).empty():
		InputMap.action_erase_event(action_string, InputMap.get_action_list(action_string)[0])
	
	#Check if new key was assigned somewhere
	for i in ACTIONS:
		if InputMap.action_has_event(i, new_key):
			InputMap.action_erase_event(i, new_key)
			
	#Add new Key
	InputMap.action_add_event(action_string, new_key)
	
	_set_keys()
	
	
func b_change_key_Forward():
	_mark_button("Forward")
func b_change_key_Left():
	_mark_button("Left")
func b_change_key_Right():
	_mark_button("Right")
func b_change_key_Backward():
	_mark_button("Backward")
func b_change_key_Jump():
	_mark_button("Jump")
func b_change_key_Run():
	_mark_button("Run")
func b_change_key_Interact():
	_mark_button("Interact")
func b_change_key_Shoot():
	_mark_button("Shoot")