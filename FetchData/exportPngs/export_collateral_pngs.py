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

eyeStyle = ("<style>"
+ ".gotchi-primary{fill:#181818;}"
+ ".gotchi-eyeColor{fill:#484848;}"
+ "</style>")


with open('getItems/aavegotchi_db_collaterals.json', 'r') as fp:
	db = json.load(fp)
	for collateral in db:		

		for facing in range(3):

			svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64" >' 
			+ collateral["svgs"][facing]
			+ '</svg>')

			if facing == 0:
				svg2png(bytestring=svg,write_to=f'export/collaterals/collateral_{collateral["svgId"]}.png', output_width=256, output_height=256)
			else:
				svg2png(bytestring=svg,write_to=f'export/collaterals/collateral_{collateral["svgId"]}_{dirs[facing]}.png', output_width=256, output_height=256)

		for facing in range(3):

			eyeSvg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
			+ eyeStyle
			+ collateral["eyeShapeSvgs"][facing]
			+ '</svg>')

			if facing == 0:
				svg2png(bytestring=eyeSvg,write_to=f'export/collaterals/eyeShapes-{collateral["eyeShapeSvgId"]}.png', output_width=256, output_height=256)
			else:
				svg2png(bytestring=eyeSvg,write_to=f'export/collaterals/eyeShapes-{collateral["eyeShapeSvgId"]}_{dirs[facing]}.png', output_width=256, output_height=256)


