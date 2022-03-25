const fs = require('fs');
const svg2img = require('svg2img');

var svgString = [
'<svg width="64" height="64">',
'<g fill="#2664ba">',
'	<path d="M30 15v2h3v1h-3v1h1.5v1h1v-1H34v-3h-3v-1h3v-1h-1.5v-1h-1v1H30zm4 5h1v1h-1z" />',
'	<path d="M35 19h1v1h-1z" />',
'	<path d="M36 16v3h1v-5h-1v1zm-2-4h1v1h-1z" />',
'	<path d="M35 13h1v1h-1zm-7 5v-4h-1v5h1zm1 2h1v1h-1z" />',
'	<path d="M28 19h1v1h-1zm1-7h1v1h-1z" />',
'	<path d="M28 13h1v1h-1z" />',
'</g>',
'</svg>'
	].join('');


	//1. convert from svg string
svg2img(svgString, function(error, buffer) {
    //returns a Buffer
    fs.writeFileSync('foo1.png', buffer);
});