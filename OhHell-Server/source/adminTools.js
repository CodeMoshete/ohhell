const debug = require('debug')('ohhell-server');
const path = require('path');
const fs = require('fs');
const gameManager = require('./gameManager.js');

module.exports.adjustPlayerBid = async function adjustPlayerBid(gameName, playerId, handNum, bidNum) {
  const gameStateName = `gameState-${gameName}`;
  const gameStateBasePath = path.join(global.appRoot, 'gamestates');
  const gameStateFilePath = path.join(gameStateBasePath, gameStateName);
  let gameStateContent = false;
  if (fs.existsSync(gameStateFilePath)) {
    gameStateContent = JSON.parse(fs.readFileSync(gameStateFilePath));
  }
  debug(`GET GAME STATE: ${gameStateFilePath}`);

  if (gameStateContent !== false) {
    gameStateContent.Players[playerId].Bids[handNum] = bidNum;
    const serializedContent = JSON.stringify(gameStateContent, null, 2);
    fs.writeFileSync(gameStateFilePath, serializedContent);

    const testAction = {
      GameName: gameName,
      ActionType: 'AdjustBidAction',
      ActionData: JSON.stringify({ PlayerId: playerId, HandNum: handNum, BidNum: bidNum }),
      EnforceIndex: -1
    };
    await gameManager.logGameAction(testAction);

    const playerName = gameStateContent.Players[playerId].PlayerName;
    debug(`Updated player ${playerName} with a bid of ${bidNum} for hand #${handNum}`);
  }
  debug('AdjustPlayerBid: DONE');
};
