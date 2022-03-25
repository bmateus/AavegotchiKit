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


with open('getItems/aavegotchi_db_wearables.json', 'r') as fp:
	db = json.load(fp)
	for wearable in db:		

		#rarity badges
		if wearable["rarity"] == "unknown?":
			continue

		for facing in range(4):

			x = wearable["offsets"][facing]["x"]
			y = wearable["offsets"][facing]["y"]

			style = ("<style>"
			+ ".gotchi-sleeves-up{display:none;}"
			+ "</style>")

			svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
			+ style
			+ (f'<g class="gotchi-wearable"> <svg x="{x}" y="{y}">')
			+ wearable["svgs"][facing]
			+ '</svg></g></svg>')

			try:
				if facing == 0:
					svg2png(bytestring=svg,write_to=f'export/wearables/wearable_{wearable["id"]}.png', output_width=256, output_height=256)
				else:
					svg2png(bytestring=svg,write_to=f'export/wearables/wearable_{wearable["id"]}_{dirs[facing]}.png', output_width=256, output_height=256)
			except:
				print("found a bad svg... skipping")

			if wearable["sleeves"]:

				#turn sleeves-up on for body

				style = ("<style>"
				+ ".gotchi-sleeves-up{display:block;}"
				+ "</style>")

				svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
				+ style
				+ (f'<g class="gotchi-wearable"> <svg x="{x}" y="{y}">')
				+ wearable["svgs"][facing]
				+ '</svg></g></svg>')

				try:
					if facing == 0:
						svg2png(bytestring=svg,write_to=f'export/wearables/wearable_{wearable["id"]}_sleeves_up.png', output_width=256, output_height=256)
					else:
						svg2png(bytestring=svg,write_to=f'export/wearables/wearable_{wearable["id"]}_sleeves_up_{dirs[facing]}.png', output_width=256, output_height=256)
				except:
					print("found a bad svg... skipping")

				#export sleeves

				style = ("<style>"
				+ ".gotchi-sleeves-up{display:none;}"
				+ "</style>")

				svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
				+ style
				+ (f'<g class="gotchi-wearable"> <svg x="{x}" y="{y}">')
				+ wearable["sleeves"][facing]
				+ '</svg></g></svg>')

				try:
					if facing == 0:
						svg2png(bytestring=svg,write_to=f'export/wearables/sleeves_{wearable["id"]}.png', output_width=256, output_height=256)
					else:
						svg2png(bytestring=svg,write_to=f'export/wearables/sleeves_{wearable["id"]}_{dirs[facing]}.png', output_width=256, output_height=256)
				except:
					print("found a bad svg... skipping")

				#turn sleeves-up on for sleeves

				style = ("<style>"
				+ ".gotchi-sleeves-up{display:block;}"
				+ "</style>")

				svg = ('<svg xmlns="http://www.w3.org/2000/svg" shape-rendering="crispEdges" width="64" height="64">' 
				+ style
				+ (f'<g class="gotchi-wearable"> <svg x="{x}" y="{y}">')
				+ wearable["sleeves"][facing]
				+ '</svg></g></svg>')

				try:
					if facing == 0:
						svg2png(bytestring=svg,write_to=f'export/wearables/sleeves_{wearable["id"]}_sleeves_up.png', output_width=256, output_height=256)
					else:
						svg2png(bytestring=svg,write_to=f'export/wearables/sleeves_{wearable["id"]}_sleeves_up_{dirs[facing]}.png', output_width=256, output_height=256)
				except:
					print("found a bad svg... skipping")