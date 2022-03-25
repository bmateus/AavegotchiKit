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


with open('getItems/aavegotchi_db_main.json', 'r') as fp:
	db = json.load(fp)

	#export the one-offs

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ db["shadow"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_shadow.png', output_width=256, output_height=256)

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ db["shadow"][1]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_shadow_side.png', output_width=256, output_height=256)

	style = ("<style>"
		+ ".gotchi-primary{fill:#181818;}"
		+ "</style>")

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ style
	+ db["mouth_neutral"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_mouth_neutral.png', output_width=256, output_height=256)

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ style
	+ db["mouth_happy"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_mouth_happy.png', output_width=256, output_height=256)

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ style
	+ db["eyes_happy"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_eyes_happy.png', output_width=256, output_height=256)

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ style
	+ db["eyes_mad"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_eyes_mad.png', output_width=256, output_height=256)

	svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
	+ style
	+ db["eyes_sleepy"][0]
	+ '</svg>')

	svg2png(bytestring=svg,write_to=f'export/base/gotchi_eyes_sleepy.png', output_width=256, output_height=256)


	for facing in range(4):

		#base - we need to export hands as a separate layer because they need to go above the body layer

		style = ("<style>"
		+ ".gotchi-primary{fill:#181818;}"
		+ ".gotchi-secondary{fill:#282828;}"
		+ ".gotchi-cheek{fill:#383838;}"
		+ ".gotchi-eyeColor{fill:#484848;display:none;}"
		+ ".gotchi-primary-mouth{fill:#585858;display:none;}"
		+ ".gotchi-sleeves-up{display:none;}"
		+ ".gotchi-handsUp{display:none;}"
		+ ".gotchi-handsDownOpen{display:none;}"
		+ ".gotchi-handsDownClosed{display:none;}"
		+ ".gotchi-shadow{display:none;}"
		+ "</style>")

		svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
		+ style
		+ db["body"][facing]
		+ db["hands"][facing]
		+ '</svg>')

		if facing == 0:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_base.png', output_width=256, output_height=256)
		else:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_base_{dirs[facing]}.png', output_width=256, output_height=256)

		#hands down closed

		style = ("<style>"
		+ ".gotchi-primary{fill:#181818;}"
		+ ".gotchi-secondary{fill:#282828;}"
		+ ".gotchi-cheek{fill:#383838;}"
		+ ".gotchi-eyeColor{fill:#484848;display:none;}"
		+ ".gotchi-primary-mouth{fill:#585858;display:none;}"
		+ ".gotchi-sleeves-up{display:none;}"
		+ ".gotchi-handsUp{display:none;}"
		+ ".gotchi-handsDownOpen{display:none;}"
		+ ".gotchi-handsDownClosed{display:block;}"
		+ ".gotchi-shadow{display:none;}"
		+ "</style>")

		svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
		+ style
		+ db["hands"][facing]
		+ '</svg>')

		if facing == 0:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_down_closed.png', output_width=256, output_height=256)
		else:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_down_closed_{dirs[facing]}.png', output_width=256, output_height=256)




		#hands down open

		style = ("<style>"
		+ ".gotchi-primary{fill:#181818;}"
		+ ".gotchi-secondary{fill:#282828;}"
		+ ".gotchi-cheek{fill:#383838;}"
		+ ".gotchi-eyeColor{fill:#484848;display:none;}"
		+ ".gotchi-primary-mouth{fill:#585858;display:none;}"
		+ ".gotchi-sleeves-up{display:none;}"
		+ ".gotchi-handsUp{display:none;}"
		+ ".gotchi-handsDownOpen{display:block;}"
		+ ".gotchi-handsDownClosed{display:none;}"
		+ ".gotchi-shadow{display:none;}"
		+ "</style>")

		svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
		+ style
		+ db["hands"][facing]
		+ '</svg>')

		if facing == 0:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_down_open.png', output_width=256, output_height=256)
		else:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_down_open_{dirs[facing]}.png', output_width=256, output_height=256)


		#hands up

		style = ("<style>"
		+ ".gotchi-primary{fill:#181818;}"
		+ ".gotchi-secondary{fill:#282828;}"
		+ ".gotchi-cheek{fill:#383838;}"
		+ ".gotchi-eyeColor{fill:#484848;display:none;}"
		+ ".gotchi-primary-mouth{fill:#585858;display:none;}"
		+ ".gotchi-sleeves-up{display:none;}"
		+ ".gotchi-handsUp{display:block;}"
		+ ".gotchi-handsDownOpen{display:none;}"
		+ ".gotchi-handsDownClosed{display:none;}"
		+ ".gotchi-shadow{display:none;}"
		+ "</style>")

		svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
		+ style
		+ db["hands"][facing]
		+ '</svg>')

		if facing == 0:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_up.png', output_width=256, output_height=256)
		else:
			svg2png(bytestring=svg,write_to=f'export/base/gotchi_hands_up_{dirs[facing]}.png', output_width=256, output_height=256)


