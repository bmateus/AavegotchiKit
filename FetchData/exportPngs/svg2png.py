#!/usr/bin/env python3

#pipe files in to process them

import fileinput
from cairosvg import svg2png

target = None

try:
	with fileinput.input() as f_input:
		for line in f_input:
			target = line.strip()
			with open(target, 'r') as file:
				svg_code = file.read()
			outfile = target.replace('.svg', '.png')
			svg2png(bytestring=svg_code,write_to=outfile)
except:
	print("error converting", target)

