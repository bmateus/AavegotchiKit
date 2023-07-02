const ethers = require("ethers");
const fs = require('fs');
const { XMLParser, XMLBuilder, XMLValidator} = require('fast-xml-parser');
const xml2js = require('xml2js')
const { getAllSimpleStorage, getMappingStorage } = require('./storage');

const DIAMOND = "0x86935F11C86623deC8a25696E1C19a8659CbF95d";

const RPC_ENDPOINT = "https://polygon-mainnet.g.alchemy.com/v2/Jz18Ku6PkyORcdlLik87ABkFN8DVRCgM";

const provider = new ethers.providers.JsonRpcProvider(RPC_ENDPOINT);

const abi = require('./diamond.json');

let contract = undefined

	// //aavegotchi facet
	// "function getAavegotchi(uint256 _tokenId) external view returns (AavegotchiInfo memory aavegotchiInfo_)",

	// //svg facet
	// "function getItemSvg(uint256 _itemId)",

	// uint256 internal constant CLOSED_PORTAL_SVG_ID = 0;
    // uint256 internal constant OPEN_PORTAL_SVG_ID = 1;
    // uint256 internal constant AAVEGOTCHI_BODY_SVG_ID = 2;
    // uint256 internal constant HANDS_SVG_ID = 3;
    // uint256 internal constant BACKGROUND_SVG_ID = 4;
	// "function getSvg(bytes32 name, uint256 _itemId)"
	
	// // svg views facet
	// getAavegotchiSideSvgs(uint256)
	// getItemSvgs(uint256)
	// getItemsSvgs(uint256[])
	// previewSideAavegotchi(uint256,address,int16[6],uint16[16])
	// setSideViewDimensions((uint256,string,(uint8,uint8,uint8,uint8))[])
	
	// //items facet	
	// getItemType(uint256)

	// uri(uint256)

    //Wearables
    // uint8 internal constant WEARABLE_SLOT_BODY = 0;
    // uint8 internal constant WEARABLE_SLOT_FACE = 1;
    // uint8 internal constant WEARABLE_SLOT_EYES = 2;
    // uint8 internal constant WEARABLE_SLOT_HEAD = 3;
    // uint8 internal constant WEARABLE_SLOT_HAND_LEFT = 4;
    // uint8 internal constant WEARABLE_SLOT_HAND_RIGHT = 5;
    // uint8 internal constant WEARABLE_SLOT_PET = 6;
    // uint8 internal constant WEARABLE_SLOT_BG = 7;

    // uint256 internal constant ITEM_CATEGORY_WEARABLE = 0;
    // uint256 internal constant ITEM_CATEGORY_BADGE = 1;
    // uint256 internal constant ITEM_CATEGORY_CONSUMABLE = 2;

    // uint8 internal constant WEARABLE_SLOTS_TOTAL = 11;


	//uint256 constant EQUIPPED_WEARABLE_SLOTS = 16; //for expansion?


const db = {}

db.collateralInfo = []

const getCollateralSvgs = async (collateralSvgId) =>
{
	console.log("getCollateralSvgs");
	return [
		await contract.getSvg(ethers.utils.formatBytes32String("collaterals"), collateralSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("collaterals-left"), collateralSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("collaterals-right"), collateralSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("collaterals-back"), collateralSvgId)
	];
}

const getCollateralEyeShapeSvgs = async (collateralEyeShapeSvgId, hauntId) =>
{
	console.log("getCollateralEyeShapeSvgs");
	if ( hauntId === 1 )
	{
		return [
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes"), collateralEyeShapeSvgId),
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes-left"), collateralEyeShapeSvgId),
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes-right"), collateralEyeShapeSvgId),
		]
	}
	else
	{
		return [
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH" + hauntId), collateralEyeShapeSvgId),
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH" + hauntId + "-left"), collateralEyeShapeSvgId),
			await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH" + hauntId + "-right"), collateralEyeShapeSvgId),
		]
	}
}

const addCollateralInfo = async (info, hauntId) => {

	console.log("addCollateralInfo:", info.collateralType, hauntId);

	const svgs = await getCollateralSvgs(info.collateralTypeInfo.svgId);
	const eyeShapeSvgs = await getCollateralEyeShapeSvgs(info.collateralTypeInfo.eyeShapeSvgId, hauntId);

	db.collateralInfo.push({
		collateralType: info.collateralType,
		modifiers: info.collateralTypeInfo.modifiers,
		primaryColor: info.collateralTypeInfo.primaryColor,
		secondaryColor: info.collateralTypeInfo.secondaryColor,
		cheekColor: info.collateralTypeInfo.cheekColor,
		svgId: info.collateralTypeInfo.svgId,
		svgs: svgs,
		eyeShapeSvgId: info.collateralTypeInfo.eyeShapeSvgId,
		eyeShapeSvgs: eyeShapeSvgs,
		haunt: hauntId
	})
}

const getCollateralInfo = async () =>
{
	const collateralInfoHaunt1 = await contract.getCollateralInfo(1);
	//console.log(collateralInfoHaunt1);
	for( let i = 0; i < collateralInfoHaunt1.length; ++i)
	{
		const info = collateralInfoHaunt1[i];
		await addCollateralInfo(info, 1)
	}
	
	const collateralInfoHaunt2 = await contract.getCollateralInfo(2);
	//console.log(collateralInfoHaunt2);
	for( let i = 0; i < collateralInfoHaunt2.length; ++i)
	{
		const info = collateralInfoHaunt2[i];
		await addCollateralInfo(info, 2)
	}
	//console.log(collateralInfo)
}

db.wearables = []

const getWearables = async (startingId = 1) => {
	let done = false;
	let wearableId = startingId;
	while (!done)
	{
		try
		{
			console.log("getWearable", wearableId);
			const info = await contract.getItemType(wearableId);
			//console.log(info)
			await addWearable(wearableId, info);
			wearableId++;
		}
		catch(ex)
		{
			//eventually it will crap out
			console.log(ex);
			done = true;
		}
	}
}

const getRarity = rarityScoreModifier =>
{
	switch (rarityScoreModifier) {

		case 1:
			return "common";

		case 2:
			return "uncommon";

		case 5:
			return "rare";

		case 10:
			return "legendary";

		case 20:
			return "mythical";

		case 50:
			return "godlike";

		default:
			return 'unknown?';
	}
}

const addWearable = async (id, info) => {

	const rarity = getRarity(info.rarityScoreModifier)
	const svgs = await getWearableSvgs(info.svgId);
	const offsets = await getWearableOffsets(id);

	let sleeves = null;
	const isBodyWearable = info.slotPositions[0] != 0;
	if (isBodyWearable)
	{
		//try to get sleeves id
		const sleevesId = await getSleeves(id);
		console.log("got sleeves for id", id, " = ", sleevesId );
		if ( sleevesId !== 0 || id === 8)
		{
			console.log("adding sleeves...")
			sleeves = [
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves"), sleevesId),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-left"), sleevesId),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-right"), sleevesId),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-back"), sleevesId)
			]
		}
	}

	db.wearables.push({
		id: id,
		name: info.name,
	 	description: info.description,
	 	author: info.author,
	 	traitModifiers: info.traitModifiers,
	 	slotPositions: info.slotPositions,
	 	allowedCollaterals: info.allowedCollaterals,
	 	dimensions: info.dimensions,
		offsets: offsets,
	 	svgId: info.svgId,
		svgs: svgs,
	 	rarityScoreModifier: info.rarityScoreModifier,
	 	minLevel: info.minLevel,
	 	category: info.category,
		rarity: rarity,
		sleeves: sleeves
	})

	//console.log("info:", db.wearables[info.name]);
}

const getWearableSvgs = async (wearableSvgId) =>
{
	//getItemSvgs isint actually called by anything?
	//return await contract.getItemSvgs(wearableId);
	return [
		await contract.getSvg(ethers.utils.formatBytes32String("wearables"), wearableSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("wearables-left"), wearableSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("wearables-right"), wearableSvgId),
		await contract.getSvg(ethers.utils.formatBytes32String("wearables-back"), wearableSvgId),
	]
}

db.wearableSets = []

const addWearableSet = (info) => {
	db.wearableSets.push({
		name: info.name,
		allowedCollaterals: info.allowedCollaterals,
		wearableIds: info.wearableIds,
		traitsBonuses: info.traitsBonuses,
	})
}

const getWearableSets = async () =>
{
	const result = await contract.getWearableSets()
	result.forEach(info => {
		addWearableSet(info)
	});
	//console.log(wearableSets);
}

db.eyeShapes = []

const getEyeShapes = async () => {

	const eyeShapeTraitRange = [0, 1, 2, 5, 7, 10, 15, 20, 25, 42, 58, 75, 80, 85, 90, 93, 95, 98];

	for (let i = 0; i < eyeShapeTraitRange.length-1; ++i) {
		console.log("Getting H1 eyeShapes", i);
		db.eyeShapes.push({
			haunt: 1,
			id: i,
			rangeMin: eyeShapeTraitRange[i],
			rangeMax: eyeShapeTraitRange[i+1],
			svgs: [
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes-left"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapes-right"), i)
			]
		})
	}

	for (let i = 0; i < eyeShapeTraitRange.length-1; ++i) {
		console.log("Getting H2 eyeShapes", i);
		db.eyeShapes.push({
			haunt: 2,
			id: i,
			rangeMin: eyeShapeTraitRange[i],
			rangeMax: eyeShapeTraitRange[i+1],
			svgs: [
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH2"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH2-left"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("eyeShapesH2-right"), i)
			]
		})
	}
}





const getWearableOffsets = async (wearableId) => 
{
	//this is pretty crazy, but I think it's the only way to get the side view offsets from the contract

	//first get the previewSideAavegotchi of some default gotchi
	//i don't think it matters what slot it's in, it should return the same offset;
	//there is some magic though if the item is in the WEARABLE_SLOT_HAND_LEFT slot, 
	//it gets flipped and shifted:
	//'<g transform="scale(-1, 1) translate(-', 64 - (dimensions.x * 2)),', 0)">' <[wearable]> </g>
	const result = await contract.previewSideAavegotchi(
		1, 
		"0xE0b22E0037B130A9F56bBb537684E6fA18192341",
		[50, 50, 50, 50, 50, 50],
		[wearableId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,]
		)

	//next, find the "gotchi-wearable" group and get the x, y offsets from that

	const options = { 
		ignoreAttributes: false,
		//attributeNamePrefix : "@_",
	}
	
	let offsets = []

	const parser = new XMLParser(options);

	for ( let side = 0; side < 4; ++side)
	{
		const parsed = parser.parse(result[side]);

		for (let i = 0; i < parsed.svg.g.length; ++i)
		{
			if (parsed.svg.g[i]['@_class'].startsWith('gotchi-wearable'))
			{
				const x = parsed.svg.g[i]['svg']['@_x'];
				const y = parsed.svg.g[i]['svg']['@_y'];
				offsets.push({x:x, y:y})
				break; // side views have multiple wearable layers for some reason?
			}
		}
	}

	return offsets;
}

db.gotchi = {}

const getGotchi = async () =>
{
    const AAVEGOTCHI_BODY_SVG_ID = 2;
    const HANDS_SVG_ID = 3;

	db.gotchi.body = [
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi"), AAVEGOTCHI_BODY_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-left"), AAVEGOTCHI_BODY_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-right"), AAVEGOTCHI_BODY_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-back"), AAVEGOTCHI_BODY_SVG_ID),
	]

	db.gotchi.hands = [
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi"), HANDS_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-left"), HANDS_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-right"), HANDS_SVG_ID),
		await contract.getSvg(ethers.utils.formatBytes32String("aavegotchi-back"), HANDS_SVG_ID),
	]

	db.gotchi.mouth_neutral = [
`
	<g class="gotchi-primary">
		<path d="M33 34h-4v2h6v-2h-1z" />
	</g>
`
	]

	db.gotchi.mouth_happy = [
`		
	<g class="gotchi-primary">
		<path d="M29 32h-2v2h2v-1z"/>
		<path d="M33 34h-4v2h6v-2h-1z"/>
		<path d="M36 32h-1v2h2v-2z"/>
	</g>
`
	]

	db.gotchi.eyes_mad = [
`
	<g class="gotchi-primary">
		<path d="M29 27V26H28H27V27V28H28H29V27Z"></path>
		<path d="M27 24H26H25V25V26H26H27V25V24Z"></path>
		<path d="M25 22H24H23V23V24H24H25V23V22Z"></path>
		<path d="M37 27V26H36H35V27V28H36H37V27Z"></path>
		<path d="M39 26V25V24H38H37V25V26H38H39Z"></path>
		<path d="M41 24V23V22H40H39V23V24H40H41Z"></path>
	</g>
`
	]

	db.gotchi.eyes_happy = [
`
	<g class="gotchi-primary">
		<path d="M23 26V25V24H22H21V25V26H22H23Z"></path>
		<path d="M25 24H26H27V23V22H26H25H24H23V23V24H24H25Z"></path>
		<path d="M27 26H28H29V25V24H28H27V25V26Z"></path>
		<path d="M41 26H42H43V25V24H42H41V25V26Z"></path>
		<path d="M39 24H40H41V23V22H40H39H38H37V23V24H38H39Z"></path>
		<path d="M35 24V25V26H36H37V25V24H36H35Z"></path>
	</g>
`
	]

	db.gotchi.eyes_sleepy = [
`
	<g class="gotchi-primary">
		<path d="M23 26H22H21V27V28H22H23V27V26Z"></path>
		<path d="M29 27V26H28H27V27V28H28H29V27Z"></path>
		<path d="M25 28H24H23V29V30H24H25H26H27V29V28H26H25Z"></path>
		<path d="M43 28V27V26H42H41V27V28H42H43Z"></path>
		<path d="M37 27V26H36H35V27V28H36H37V27Z"></path>
		<path d="M37 30H38H39H40H41V29V28H40H39H38H37V29V30Z"></path>
	</g>
`
	]

	db.gotchi.shadow = [
`
	<g class="gotchi-shadow">
		<path opacity=".25" d="M25 58H19v1h1v1h24V59h1V58h-1z" fill="#000"/>
	</g>
`,
`
	<g class="gotchi-shadow">
		<path d="M23 58v1h1v1h16v-1h1v-1z" opacity=".25"/>
	</g>
`,
	]
	
}

//need to get sleeves layer for body wearables :( !!!
const getSleevesSvgs = async () => 
{
	const data = {}
	data.sleeves = []
	for (i = 0; i < 43; ++i) 
	{
		console.log("getting sleeves:", i);
		data.sleeves.push({
			id: i,
			svgs: [
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-left"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-right"), i),
				await contract.getSvg(ethers.utils.formatBytes32String("sleeves-back"), i),
			]
		})
	}
	fs.writeFile('./aavegotchi_db_sleeves.json', JSON.stringify(data), 'utf8', (err)=> console.log(err));
}


//reads from sideViewDimensions in LibAppStorage
const getSideViewDimensions = async (wearableId, view) =>
{
	//view is "left", "right" or "back"
	// mapping(uint256 => mapping(bytes => Dimensions)) sideViewDimensions;
	const SIDE_VIEW_DIMENSIONS_SLOT = 62;
	const key = ethers.utils.hexZeroPad(wearableId, 32);
	console.log("wearable:", key);
	const key2 = ethers.utils.formatBytes32String(view);
	console.log("facing:", key2);
	const slot = ethers.utils.hexZeroPad(ethers.BigNumber.from(SIDE_VIEW_DIMENSIONS_SLOT).toHexString(), 32);
	console.log("slot:", slot);
	const position = ethers.utils.keccak256(ethers.utils.concat([key, slot]));
	//deref again 
	const position2 = ethers.utils.keccak256(ethers.utils.concat([key2, position]));
	const result = await provider.getStorageAt(contract.address, position2);
	console.log( "storage value:", result);
	const data = ethers.utils.defaultAbiCoder.decode(['tuple(address,uint16,uint16)'], result)
	console.log(data)
}


//ItemType struct for reference
/*
struct ItemType {
    string name; //The name of the item
    string description;
    string author;
    // treated as int8s array
    // [Experience, Rarity Score, Kinship, Eye Color, Eye Shape, Brain Size, Spookiness, Aggressiveness, Energy]
    int8[NUMERIC_TRAITS_NUM] traitModifiers; //[WEARABLE ONLY] How much the wearable modifies each trait. Should not be more than +-5 total
    //[WEARABLE ONLY] The slots that this wearable can be added to.
    bool[EQUIPPED_WEARABLE_SLOTS] slotPositions;
    // this is an array of uint indexes into the collateralTypes array
    uint8[] allowedCollaterals; //[WEARABLE ONLY] The collaterals this wearable can be equipped to. An empty array is "any"
    // SVG x,y,width,height
    Dimensions dimensions;
    uint256 ghstPrice; //How much GHST this item costs
    uint256 maxQuantity; //Total number that can be minted of this item.
    uint256 totalQuantity; //The total quantity of this item minted so far
    uint32 svgId; //The svgId of the item
    uint8 rarityScoreModifier; //Number from 1-50.
    // Each bit is a slot position. 1 is true, 0 is false
    bool canPurchaseWithGhst;
    uint16 minLevel; //The minimum Aavegotchi level required to use this item. Default is 1.
    bool canBeTransferred;
    uint8 category; // 0 is wearable, 1 is badge, 2 is consumable
    int16 kinshipBonus; //[CONSUMABLE ONLY] How much this consumable boosts (or reduces) kinship score
    uint32 experienceBonus; //[CONSUMABLE ONLY]
}
*/

/*

0x43616d6f20486174000000000000000000000000000000000000000000000010 // string name: "Camo Hat"
0x0000000000000000000000000000000000000000000000000000000000000000 // string description
0x5869626f7400000000000000000000000000000000000000000000000000000a // string author: "Xibot"
0x0000000000000000000000000000000000000000000000000000000000000100 // int8[NUMERIC_TRAITS_NUM] traitModifiers;
0x0000000000000000000000000000000000000000000000000000000001000000 // bool[EQUIPPED_WEARABLE_SLOTS] slotPositions;
0x0000000000000000000000000000000000000000000000000000000000000000 // uint8[] allowedCollaterals;
0x000000000000000000000000000000000000000000000000000000001422020f // Dimensions dimensions; ? 20 34 2 15  <-- reverse
0x0000000000000000000000000000000000000000000000004563918244f40000 // uint256 ghstPrice;
0x00000000000000000000000000000000000000000000000000000000000003e8 // uint256 maxQuantity; 
0x00000000000000000000000000000000000000000000000000000000000003e8 // uint256 totalQuantity;
0x00000000000000000000000000000000 00000000 0000 00 01 0001 00 01 00000001 //  the rest

*/

//reads from itemTypes in LibAppStorage
const getWearableType = async (wearableId) => 
{
	const ITEM_TYPES_SLOT = 6;
	const NUM_SLOTS_PER = 11;
	const slot = ethers.utils.hexZeroPad(ethers.BigNumber.from(ITEM_TYPES_SLOT).toHexString(), 32);
	const position = ethers.utils.keccak256(slot);	
	for (let index = 0; index < NUM_SLOTS_PER; ++index)
	{
		const finalPosition = ethers.BigNumber.from(position).add(wearableId * NUM_SLOTS_PER + index);
		const result = await provider.getStorageAt(contract.address, finalPosition);
		console.log(result);
	}
}


/*
struct SvgLayer {
    address svgLayersContract;
    uint16 offset;
    uint16 size;
}*/

//reads from svgLayers in LibAppStorage
const getSvgLayer = async () =>
{
	const SVG_LAYERS_SLOT = 2;
	//get svgLayers("sleeves")[2] from mapping(bytes32 => SvgLayer[]) svgLayers;
	const key = ethers.utils.formatBytes32String("sleeves");
	const slot = ethers.utils.hexZeroPad(ethers.BigNumber.from(SVG_LAYERS_SLOT).toHexString(), 32);
	//deref mapping
	const position = ethers.utils.keccak256(ethers.utils.concat([key, slot]));
	//deref array
	const position2 = ethers.utils.keccak256(position);
	const arrayIdx = 2;
	//offset into array
	const finalPosition = ethers.BigNumber.from(position2).add(arrayIdx);
	const result = await provider.getStorageAt(contract.address, finalPosition);
	console.log( "storage value:", result);
	//decode result (doesn't work? gotta find a way to parse it)
	//const svgLayer = ethers.utils.defaultAbiCoder.decode(['address, uint16, uint16'], ethers.utils.arrayify(result).slice(4));
	//console.log("svgLayer:", svgLayer);
}


const getAllSleeves = async () =>
{
	for ( let i=0; i < 265; i++ )
	{
		console.log(i, ethers.BigNumber.from(await getSleeves(i)).toString());
	}
}

// reads from sleeves in LibAppStorage
//  mapping(uint256 => uint256) sleeves;
const getSleeves = async (wearableId) => 
{
	const SLEEVES_SLOT = 58;
	const key = ethers.utils.hexZeroPad(wearableId, 32);
	const slot = ethers.utils.hexZeroPad(ethers.BigNumber.from(SLEEVES_SLOT).toHexString(), 32);
	const position = ethers.utils.keccak256(ethers.utils.concat([key, slot]));
	const result = await provider.getStorageAt(contract.address, position);
	return ethers.BigNumber.from(result).toNumber();

}

provider.ready.then(async () => {

	contract = new ethers.Contract(DIAMOND, abi, provider);
	 
	// 1) get sleeves
	//await getSleevesSvgs();

	// 2) get everything else
	// await getCollateralInfo();
	// fs.writeFile('./aavegotchi_db_collaterals.json', JSON.stringify(db.collateralInfo), 'utf8', (err)=> console.log(err));

	// await getWearableSets();
	// fs.writeFile('./aavegotchi_db_wearable_sets.json', JSON.stringify(db.wearableSets), 'utf8', (err)=> console.log(err));

	// await getEyeShapes();
	// fs.writeFile('./aavegotchi_db_eye_shapes.json', JSON.stringify(db.eyeShapes), 'utf8', (err)=> console.log(err));

	// await getWearables();
	// fs.writeFile('./aavegotchi_db_wearables.json', JSON.stringify(db.wearables), 'utf8', (err)=> console.log(err));
	
	// await getGotchi();
	// fs.writeFile('./aavegotchi_db_main.json', JSON.stringify(db.gotchi), 'utf8', (err)=> console.log(err));

	// 3) test getting an object with sleeves
	// const wearableId = 8  // 8 = Marine Jacket (body slot with sleeves)
	// const info = await contract.getItemType(wearableId)
	// console.log(info)
	// await addWearable(wearableId, info)
	// console.log(db.wearables[0])

	// 4) Get new items
	await getWearables(315);
	fs.writeFile('./aavegotchi_db_wearables_3.json', JSON.stringify(db.wearables), 'utf8', (err)=> console.log(err));
	


});


const printStorageSlots = async () => {

	//print out Aavegotchi Diamond storage
	for (let index = 0; index < 100; ++index)
	{
		const storage = await provider.getStorageAt(contract.address, index);
		console.log(storage);
	}

}


//For Reference:

//get portal closed svg
//const result = await contract.getSvg(ethers.utils.formatBytes32String("portal-closed"), hauntId)
//console.log(result);

//get portal open svg
//const result = await contract.getSvg(ethers.utils.formatBytes32String("portal-open"), hauntId)
//console.log(result);
