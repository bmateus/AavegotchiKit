const { defaultAbiCoder } = require("@ethersproject/abi");
const ethers = require("ethers");
const leftPad = require('left-pad');

const RPC_ENDPOINT =
  "https://polygon-mainnet.g.alchemy.com/v2/Jz18Ku6PkyORcdlLik87ABkFN8DVRCgM";

const provider = new ethers.providers.JsonRpcProvider(RPC_ENDPOINT);

const getAllSimpleStorage = async (addr) => {
  let slot = 0;
  let zeroCounter = 0;
  const simpleStorage = [];
  // eslint-disable-next-line no-constant-condition
  while (true) {
    const data = await provider.getStorageAt(addr, slot);
    if (ethers.BigNumber.from(data) == 0) {
      zeroCounter++;
    }

    simpleStorage.push({ slot, data });
    slot++;

    if (zeroCounter > 10) {
      break;
    }
  }

  return simpleStorage;
};

const standardizeInput = (input) =>
  leftPad(web3.toHex(input).replace("0x", ""), 64, "0");

const getMappingSlot = (mappingSlot, key) => {
//   const mappingSlotPadded = standardizeInput(mappingSlot);
//   const keyPadded = standardizeInput(key);
//   const slot = ethers.keccak256(keyPadded.concat(mappingSlotPadded), {
//     encoding: "hex",
//   });
  return slot;
};

const getMappingStorage = async (address, mappingSlot, key) => {
	const mappingKeySlot = getMappingSlot(mappingSlot.toString(), key)
	const complexStorage = await provider.getStorageAt(address, mappingKeySlot)
	return complexStorage
  }

module.exports = {
  getAllSimpleStorage,
  getMappingSlot,
  getMappingStorage
};
