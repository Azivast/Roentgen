extends Spatial

func _ready():
	yield(get_tree(), 'idle_frame')
	for c in get_children():
		if c is GridMap:
			# rebuilds gridmap and fixes lighting
			c.cell_octant_size = c.cell_octant_size
