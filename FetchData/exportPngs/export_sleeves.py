#!/usr/bin/env python3

import json
import fileinput
from cairosvg import svg2png

target = None

dirs = [
	"",
	"left",
	"right",
	"back"
]

style = ("<style>"
+ ".gotchi-primary{fill:#181818;}"
+ ".gotchi-eyeColor{fill:#484848;}"
+ "</style>")


with open('getItems/aavegotchi_db_sleeves.json', 'r') as fp:
	db = json.load(fp)
	for sleeve in db["sleeves"]:
		#print(sleeve)
		id = sleeve["id"]

		for facing in range(4):

			svg = ('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">' 
			#+ style
			+ sleeve["svgs"][facing]
			+ '</svg>')

			if facing == 0:
				svg2png(bytestring=svg,write_to=f'export/sleeves/sleeve-{id}.png')
			else:
				svg2png(bytestring=svg,write_to=f'export/sleeves/sleeve-{id}_{dirs[facing]}.png')
				
