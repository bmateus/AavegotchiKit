#!/usr/bin/env python3

import json
import fileinput
from cairosvg import svg2png

target = None

dirs = [
	"",
	"left",
	"right"
]

style = ("<style>"
+ ".gotchi-primary{fill:#181818;}"
+ ".gotchi-eyeColor{fill:#484848;}"
+ "</style>")


with open('getItems/aavegotchi_db_eye_shapes.json', 'r') as fp:
	db = json.load(fp)
	for eyeShape in db:		

		for facing in range(3):

			svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
			+ style
			+ eyeShape["svgs"][facing]
			+ '</svg>')

			if facing == 0:
				if eyeShape["haunt"] == 1:
					svg2png(bytestring=svg,write_to=f'export/eyeShapes/eyeShapes-{eyeShape["id"]}.png', output_width=256, output_height=256)
				else:
					svg2png(bytestring=svg,write_to=f'export/eyeShapes/eyeShapesH{eyeShape["haunt"]}-{eyeShape["id"]}.png', output_width=256, output_height=256)
			else:
				if eyeShape["haunt"] == 1:
					svg2png(bytestring=svg,write_to=f'export/eyeShapes/eyeShapes-{eyeShape["id"]}_{dirs[facing]}.png', output_width=256, output_height=256)
				else:
					svg2png(bytestring=svg,write_to=f'export/eyeShapes/eyeShapesH{eyeShape["haunt"]}-{eyeShape["id"]}_{dirs[facing]}.png', output_width=256, output_height=256)
